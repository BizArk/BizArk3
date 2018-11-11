using BizArk.Core;
using BizArk.Core.Extensions.StringExt;
using BizArk.Data.DataExt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BizArk.Data
{

	/// <summary>
	/// All database calls should be marshalled through this object.
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

		/// <summary>
		/// Creates the BaDatabase from the connection string named in the config file.
		/// </summary>
		/// <param name="name">The name or key of the connection string in the config file.</param>
		/// <returns></returns>
		public static BaDatabase Create(string name)
		{
			if (name.IsEmpty())
				throw new ArgumentNullException("name");

			if (ConnectionStrings.Count == 0)
				throw new InvalidOperationException("Add connection strings to BaDatabase.ConnectionStrings before using Create.");

			if (!ConnectionStrings.ContainsKey(name))
				throw new ArgumentException($"The connection string for '{name}' does not exist.", "name");

			var connStr = ConnectionStrings[name];
			if (connStr.IsEmpty())
				throw new ArgumentException($"The connection string for '{name}' was empty.", "name");

			return ClassFactory.CreateObject<BaDatabase>(connStr);
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Instantiate the Connection object. Should not be opened.
		/// </summary>
		/// <returns></returns>
		protected abstract DbConnection InstantiateConnection();

		#endregion


		#region Fields and Properties

		/// <summary>
		/// Error code for deadlocks in Sql Server.
		/// </summary>
		internal const int cSqlError_Deadlock = 1205;

		private DbConnection mConnection;

		/// <summary>
		/// Gets the collection of connection strings that can be used. The key should correspond to the name passed in to Create.
		/// </summary>
		public static Dictionary<string, string> ConnectionStrings { get; } = new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets the default number of times to retry a command if a deadlock is identified.
		/// </summary>
		public static short DefaultRetriesOnDeadlock { get; set; } = 1;

		/// <summary>
		/// Gets or sets the number of times to retry a command if a deadlock is identified. By default, only non-transactional commands will be retried. Use BaRepository.TryTransaction() to retry entire transactions.
		/// </summary>
		public short RetriesOnDeadlock { get; set; } = DefaultRetriesOnDeadlock;

		/// <summary>
		/// Gets the connection to use for this database. Only a single instance per BaDatabase instance is supported.
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
		/// All database calls should go through this method.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="execute"></param>
		protected virtual void ExecuteCommand(DbCommand cmd, Action<DbCommand> execute)
		{
			// Nothing to do, just exit.
			if (cmd == null) return;

			Debug.WriteLine(cmd.DebugText());

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
				catch (DbException ex) when (ex.ErrorCode == cSqlError_Deadlock && attempt <= RetriesOnDeadlock && cmd.Transaction == null)
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

			Debug.WriteLine(cmd.DebugText());

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
				catch (DbException ex) when (ex.ErrorCode == cSqlError_Deadlock && attempt <= RetriesOnDeadlock && cmd.Transaction == null)
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
		/// Executes a Transact-SQL statement against the connection and returns the number
		/// of rows affected.
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
		/// Executes a Transact-SQL statement against the connection and returns the number
		/// of rows affected.
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
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
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
		/// Sends the System.Data.SqlClient.DbCommand.CommandText to the System.Data.SqlClient.DbCommand.Connection
		/// and builds a System.Data.SqlClient.DbDataReader. The reader is only valid during execution of the method. 
		/// Use processRow to process each row in the reader.
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
		/// Sends the System.Data.SqlClient.DbCommand.CommandText to the System.Data.SqlClient.DbCommand.Connection
		/// and builds a System.Data.SqlClient.DbDataReader. The reader is only valid during execution of the method. 
		/// Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public async Task ExecuteReaderAsync(DbCommand cmd, Func<DbDataReader, Task<bool>> processRow)
		{
			await ExecuteCommandAsync(cmd, async (exeCmd) =>
			{
				using (var rdr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
				{
					while (await rdr.ReadAsync().ConfigureAwait(false))
					{
						// Once processRow returns false, exit.
						if (!await processRow(rdr).ConfigureAwait(false))
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
				catch (DbException ex) when (ex.ErrorCode == cSqlError_Deadlock && attempt <= RetriesOnDeadlock)
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
		public async void TryTransactionAsync(Func<Task> batch)
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
				catch (DbException ex) when (ex.ErrorCode == cSqlError_Deadlock && attempt <= RetriesOnDeadlock)
				{
					Debug.WriteLine($"Deadlock identified on attempt {attempt}. Retrying.");
					attempt++;

					// If the transaction fails it can leave the connection in a poor state. 
					// Re-establish the connection to be sure we are working from a good connection.
					ResetConnection();
				}
			}
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Gets the schema for a table from the database.
		/// </summary>
		/// <param name="tableName">Gets just the schema for this table.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public DataTable GetSchema(string tableName)
		{
			return Connection.GetSchema(tableName);
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
			return conn.GetSchema(tableName);
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

	}

	/// <summary>
	/// Provides a way to get a database instance from the object. Useful for keeping only a single connection open at a time and participating in transactions.
	/// </summary>
	public interface ISupportBaDatabase
	{

		/// <summary>
		/// The database that is exposed from the object.
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

