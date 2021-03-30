using BizArk.Core;
using BizArk.Core.Extensions.StringExt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BizArk.Data
{

	/// <summary>
	/// Abstract base class for working with databases. Encapsulates the `DbConnection` object and 
	/// manages the lifetime of it, including closing it and recreating it as necessary. Provides 
	/// some basic methods for accessing the database that are similar to what is found directly on 
	/// DbCommand, such as ExecuteNonQuery, ExecuteScalar, and ExecuteReader. There are async 
	/// versions of these methods as well.
	/// </summary>
	public abstract class BaDatabase : IDisposable, IBaRepository
	{

		#region Initialization and Destruction

		/// <summary>
		/// Disposes the BaDatabase.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Called when disposing the BaDatabase.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (mConnection != null)
			{
				mConnection.Close();
				mConnection.Dispose();
				mConnection = null;
			}
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Instantiate the Connection object. The connection should not be opened.
		/// </summary>
		/// <returns></returns>
		protected abstract DbConnection InstantiateConnection();

		/// <summary>
		/// Gets a DataTable with no rows that represents the table. Recommended: SELECT * FROM {tableName} WHERE 0 = 1.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		protected abstract DataTable GetSchema(DbConnection conn, string tableName);

		/// <summary>
		/// Determines if this exception represents something we should retry. Typically for 
		/// deadlocks, but can be used for other types of exceptions if the derived class wants to 
		/// support other types of errors to retry.
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		protected abstract bool ShouldRetry(Exception ex);

		#endregion


		#region Fields and Properties

		private DbConnection mConnection;

		/// <summary>
		/// Gets or sets the default number of times to retry a command if an exception is thrown 
		/// that should be retried.
		/// </summary>
		public static short DefaultRetries { get; set; } = 1;

		/// <summary>
		/// Gets or sets the number of times to retry a command if an exception is thrown that 
		/// should be retried. By default, only non-transactional commands will be retried. Use 
		/// BaRepository.TryTransaction() to retry entire transactions.
		/// </summary>
		public short Retries { get; set; } = DefaultRetries;

		/// <summary>
		/// Gets the connection to use for this database. If not set, calls `InstantiateConnection` 
		/// and opens it. For async, use `GetConnectionAsync` instead.
		/// </summary>
		public DbConnection Connection
		{
			get
			{
				if (mConnection == null)
				{
					mConnection = InstantiateConnection();
					mConnection.Open();
				}
				return mConnection;
			}
		}

		/// <summary>
		/// Gets the currently executing transaction for this database instance.
		/// </summary>
		public BaTransaction Transaction { get; internal set; } // Internal so it can be called from BaTransaction.

		#endregion

		#region Basic Database Methods

		/// <summary>
		/// All database calls should go through this method. It ensures the connection is properly 
		/// established and the command is setup correctly. Also handles deadlock detection for 
		/// non-transactional commands.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="execute"></param>
		protected virtual void ExecuteCommand(DbCommand cmd, Action<DbCommand> execute)
		{
			// Nothing to do, just exit.
			if (cmd == null) return;

			PrepareCommand(cmd);

			var attempt = 1;
			while (true)
			{
				try
				{
					cmd.Connection = cmd.Connection ?? Connection;
					cmd.Transaction = cmd.Transaction ?? Transaction?.Transaction;
					execute(cmd);
					return;
				}
				catch (DbException ex) when (cmd.Transaction == null && attempt <= Retries && ShouldRetry(ex))
				{
					Debug.WriteLine($"Deadlock identified on attempt {attempt}. Retrying.");
					attempt++;
				}
				finally
				{
					// We don't want to leave the connection and transaction on the DbCommand
					// in case it is reused and the connection/transaction are no longer valid.
					if (cmd.Connection != Connection)
						cmd.Connection = null;
					if (cmd.Transaction != Transaction?.Transaction)
						cmd.Transaction = null;
				}
			}
		}

		/// <summary>
		/// All database calls should go through this method.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="execute"></param>
		protected async virtual Task ExecuteCommandAsync(DbCommand cmd, Func<DbCommand, Task> execute)
		{
			// Nothing to do, just exit.
			if (cmd == null) return;

			PrepareCommand(cmd);

			var attempt = 1;
			while (true)
			{
				try
				{
					cmd.Connection = cmd.Connection ?? await GetConnectionAsync().ConfigureAwait(false);
					cmd.Transaction = cmd.Transaction ?? Transaction?.Transaction;
					await execute(cmd).ConfigureAwait(false);
					return;
				}
				catch (DbException ex) when (cmd.Transaction == null && attempt <= Retries && ShouldRetry(ex))
				{
					Debug.WriteLine($"Deadlock identified on attempt {attempt}. Retrying.");
					attempt++;
				}
				finally
				{
					// We don't want to leave the connection and transaction on the DbCommand
					// in case it is reused and the connection/transaction are no longer valid.
					if (cmd.Connection != Connection)
						cmd.Connection = null;
					if (cmd.Transaction != Transaction?.Transaction)
						cmd.Transaction = null;
				}
			}
		}

		/// <summary>
		/// Executes the command and returns the number of rows that were changed.
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public int ExecuteNonQuery(DbCommand cmd)
		{
			var count = 0;
			ExecuteCommand(cmd, (exeCmd) =>
			{
				count = exeCmd.ExecuteNonQuery();
			});
			return count;
		}

		/// <summary>
		/// Executes the command and returns the number of rows that were changed.
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public async Task<int> ExecuteNonQueryAsync(DbCommand cmd)
		{
			var count = 0;
			await ExecuteCommandAsync(cmd, async (exeCmd) =>
			{
				count = await exeCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
			}).ConfigureAwait(false);
			return count;
		}

		/// <summary>
		/// Executes the command, and returns the first column of the first row in the result
		/// set. Additional columns or rows are ignored.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public object ExecuteScalar(DbCommand cmd, object dflt = null)
		{
			var result = dflt;
			ExecuteCommand(cmd, (exeCmd) =>
			{
				result = exeCmd.ExecuteScalar();
			});
			return result;
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public async Task<object> ExecuteScalarAsync(DbCommand cmd, object dflt = null)
		{
			var result = dflt;
			await ExecuteCommandAsync(cmd, async (exeCmd) =>
			{
				result = await exeCmd.ExecuteScalarAsync().ConfigureAwait(false);
			}).ConfigureAwait(false);
			return result;
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmd"></param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public T ExecuteScalar<T>(DbCommand cmd, T dflt = default(T))
		{
			var result = ExecuteScalar(cmd);
			if (result == null) return dflt;
			if (result == DBNull.Value) return dflt;
			return ConvertEx.To<T>(result);
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmd"></param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public async Task<T> ExecuteScalarAsync<T>(DbCommand cmd, T dflt = default(T))
		{
			var result = await ExecuteScalarAsync(cmd).ConfigureAwait(false);
			if (result == null) return dflt;
			if (result == DBNull.Value) return dflt;
			return ConvertEx.To<T>(result);
		}

		/// <summary>
		/// Executes the command and processes the returned DbDataReader. The reader is only valid 
		/// during execution of the method. Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public void ExecuteReader(DbCommand cmd, Func<DbDataReader, bool> processRow)
		{
			ExecuteCommand(cmd, (exeCmd) =>
			{
				using (var rdr = cmd.ExecuteReader())
				{
					while (rdr.Read())
					{
						// Once processRow returns false, exit.
						if (!processRow(rdr))
							return;
					}
				}
			});
		}

		/// <summary>
		/// Executes the command and processes the returned DbDataReader. The reader is only valid 
		/// during execution of the method. Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public async Task ExecuteReaderAsync(DbCommand cmd, Func<DbDataReader, bool> processRow)
		{
			await ExecuteCommandAsync(cmd, async (exeCmd) =>
			{
				using (var rdr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
				{
					while (await rdr.ReadAsync().ConfigureAwait(false))
					{
						// Once processRow returns false, exit.
						if (!processRow(rdr))
							return;
					}
				}
			}).ConfigureAwait(false);
		}

		/// <summary>
		/// Disposes of the connection and allows it to be recreated.
		/// </summary>
		public void ResetConnection()
		{
			if (Transaction != null)
				throw new InvalidOperationException("Cannot call BaDatabase.ResetConnection while a transaction is pending.");

			if (mConnection != null)
			{
				mConnection.Close();
				mConnection.Dispose();
				mConnection = null;
			}
		}

		#endregion

		#region Transaction Methods

		/// <summary>
		/// Starts a transaction. Must call Dispose on the transaction.
		/// </summary>
		/// <returns></returns>
		public BaTransaction BeginTransaction()
		{
			return Transaction = BaTransaction.Begin(this);
		}

		/// <summary>
		/// Starts a transaction. Must call Dispose on the transaction.
		/// </summary>
		/// <returns></returns>
		public async Task<BaTransaction> BeginTransactionAsync()
		{
			return Transaction = await BaTransaction.BeginAsync(this);
		}

		/// <summary>
		/// Creates a transaction and executes the batch within it. If a deadlock is detected, rolls the transaction back and tries again.
		/// </summary>
		/// <param name="batch">The code to execute within a transaction.</param>
		public void TryTransaction(Action batch)
		{
			var attempt = 1;
			while (true)
			{
				try
				{
					using (var trans = BeginTransaction())
					{
						batch();
						trans.Commit();
						return;
					}
				}
				catch (DbException ex) when (attempt <= Retries && ShouldRetry(ex))
				{
					Debug.WriteLine($"Deadlock identified on attempt {attempt}. Retrying.");
					attempt++;

					// If the transaction fails it can leave the connection in a poor state. 
					// Re-establish the connection to be sure we are working from a good connection.
					ResetConnection();
				}
			}
		}

		/// <summary>
		/// Creates a transaction and executes the batch within it. If a deadlock is detected, rolls the transaction back and tries again.
		/// </summary>
		/// <param name="batch">The code to execute within a transaction.</param>
		public async Task TryTransactionAsync(Func<Task> batch)
		{
			var attempt = 1;
			while (true)
			{
				try
				{
					using (var trans = await BeginTransactionAsync().ConfigureAwait(false))
					{
						await batch().ConfigureAwait(false);
						trans.Commit();
						return;
					}
				}
				catch (DbException ex) when (attempt <= Retries && ShouldRetry(ex))
				{
					Debug.WriteLine($"Deadlock identified on attempt {attempt}. Retrying.");
					attempt++;

					// If the transaction fails it can leave the connection in a poor state. 
					// Re-establish the connection to be sure we are working from a good connection.
					ResetConnection();
				}
			}
		}

		/// <summary>
		/// Creates a transaction and executes the batch within it. If a deadlock is detected, rolls the transaction back and tries again. Returns the value returned from the batch.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="txn"></param>
		/// <returns></returns>
		public T TryTransaction<T>(Func<T> txn)
		{
			var ret = default(T);
			TryTransaction(() =>
			{
				ret = txn();
			});
			return ret;
		}

		/// <summary>
		/// Creates a transaction and executes the batch within it. If a deadlock is detected, rolls the transaction back and tries again. Returns the value returned from the batch.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="txn"></param>
		/// <returns></returns>
		public async Task<T> TryTransactionAsync<T>(Func<Task<T>> txn)
		{
			var ret = default(T);
			await TryTransactionAsync(async () =>
			{
				ret = await txn().ConfigureAwait(false);
			}).ConfigureAwait(false);
			return ret;
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Called from `ExecuteCommand` before executing it. Override in order to do something 
		/// with the command before it is executed.
		/// </summary>
		/// <param name="cmd"></param>
		/// <remarks>This method is called before the connection and transaction have been set on it.</remarks>
		protected virtual void PrepareCommand(DbCommand cmd)
		{
			// This is just so derived classes have an opportunity to do something with the command before it is executed.
		}

		/// <summary>
		/// Gets the schema for a table from the database.
		/// </summary>
		/// <param name="tableName">Gets just the schema for this table.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public DataTable GetSchema(string tableName)
		{
			return GetSchema(Connection, tableName);
		}

		/// <summary>
		/// Gets the schema for a table from the database.
		/// </summary>
		/// <param name="tableName">Gets just the schema for this table.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public async Task<DataTable> GetSchemaAsync(string tableName)
		{
			var conn = await GetConnectionAsync().ConfigureAwait(false);
			return GetSchema(conn, tableName);
		}

		/// <summary>
		/// Gets the connection instance. If there isn't one, instantiates it and opens it asynchronously.
		/// </summary>
		/// <returns></returns>
		public async Task<DbConnection> GetConnectionAsync()
		{
			if (mConnection != null) return mConnection;

			mConnection = InstantiateConnection();
			await mConnection.OpenAsync().ConfigureAwait(false);
			return mConnection;
		}

		#endregion

		#region ISupportBaDatabase

		/// <summary>
		/// Implementing this interface makes it simpler to pass this instance around.
		/// </summary>
		BaDatabase ISupportBaDatabase.DB
		{
			get
			{
				return this;
			}
		}

		#endregion

		#region Factory Methods

		private static Dictionary<string, Func<BaDatabase>> sDbFactories;

		/// <summary>
		/// Gets the registered factories.
		/// </summary>
		public static IEnumerable<KeyValuePair<string, Func<BaDatabase>>> DbFactories
		{
			get
			{
				// Return an empty array so that the property always returns a valid value.
				if (sDbFactories == null) return new KeyValuePair<string, Func<BaDatabase>>[] { };
				
				// We don't want to return a reference to the dictionary in order to prevent others from updating it directly.
				return sDbFactories.ToArray(); 
			}
		}

		/// <summary>
		/// Register a factory method to instantiate the appropriate BaDatabase.
		/// </summary>
		/// <param name="name">The name of the factory.</param>
		/// <param name="create">Creates a new instance.</param>
		public static void Register(string name, Func<BaDatabase> create)
		{
			// Only create the dictionary if we are actually using factories.
			if (sDbFactories == null)
				sDbFactories = new Dictionary<string, Func<BaDatabase>>();

			if (sDbFactories.ContainsKey(name))
				sDbFactories[name] = create;
			else
				sDbFactories.Add(name, create);
		}

		/// <summary>
		/// Unregisters a factory method.
		/// </summary>
		/// <param name="name"></param>
		public static void Unregister(string name)
		{
			if (sDbFactories.ContainsKey(name))
				sDbFactories.Remove(name);
		}

		/// <summary>
		/// Creates the BaDatabase from the connection string named in the config file.
		/// </summary>
		/// <param name="name">The name or key of the connection string in the config file.</param>
		/// <returns></returns>
		public static BaDatabase Create(string name)
		{
			if (name.IsEmpty())
				throw new ArgumentNullException("name");

			if (sDbFactories == null) throw new InvalidOperationException($"No factory methods have been registered. Call {nameof(BaDatabase)}.{nameof(Register)} to register new {nameof(BaDatabase)} factory methods.");

			if (!sDbFactories.ContainsKey(name))
				throw new ArgumentException($"The factory method for '{name}' does not exist. Call {nameof(BaDatabase)}.{nameof(Register)} to register new {nameof(BaDatabase)} factory methods.", nameof(name));

			return sDbFactories[name]();
		}

		#endregion

	}

	/// <summary>
	/// Provides a way to get a database instance from the object. Useful for passing a single BaDatabase instance around.
	/// </summary>
	public interface ISupportBaDatabase
	{

		/// <summary>
		/// The database instance that is exposed from the object.
		/// </summary>
		BaDatabase DB { get; }

	}

	/// <summary>
	/// Provides a convenient mechanism to hook extension methods up to the BaDatabase 
	/// class or any class that implements this interface. This encourages the use of
	/// stateless repository methods, which is considered best practice.
	/// </summary>
	public interface IBaRepository : ISupportBaDatabase
	{

	}

}

