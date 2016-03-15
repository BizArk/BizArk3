using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizArk.Core.Extensions.DataExt;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core;
using BizArk.Core.Util;
using BizArk.Core.Data;
using System.ComponentModel;
using System.Data;
using System.Dynamic;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// All database calls should be marshalled through this object.
	/// </summary>
	public class BaDatabase : IDisposable, ISupportBaDatabase
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaDatabase.
		/// </summary>
		/// <param name="connStr">The connection string to use for the database.</param>
		public BaDatabase(string connStr)
		{
			mConnectionString = connStr;
		}

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

			// Once the database has been disposed, we shouldn't need this anymore.
			// Null it out so it will fail if anybody attempts to access it after 
			// it's been disposed.
			mConnectionString = null;
		}

		#endregion

		#region Fields and Properties

		private string mConnectionString;

		private SqlConnection mConnection;
		/// <summary>
		/// Gets the connection to use for this database. Only a single instance per BaDatabase instance is supported.
		/// </summary>
		public virtual SqlConnection Connection
		{
			get
			{
				if (mConnection == null)
					mConnection = new SqlConnection(mConnectionString);
				return mConnection;
			}
		}

		/// <summary>
		/// Gets the currently executing transaction for this database instance.
		/// </summary>
		public SqlTransaction Transaction { get; private set; }

		#endregion

		#region Basic Database Methods

		/// <summary>
		/// All database calls should go through this method.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="execute"></param>
		protected virtual void ExecuteCommand(SqlCommand cmd, Action<SqlCommand> execute)
		{
			// Nothing to do, just exit.
			if (cmd == null) return;

			Debug.WriteLine(cmd.DebugText());

			// The connection is already set. Don't do anything with it.
			if (cmd.Connection != null)
			{
				execute(cmd);
				return;
			}

			try
			{
				cmd.Connection = Connection;
				cmd.Transaction = Transaction;
				execute(cmd);
			}
			finally
			{
				// We don't want to leave the connection and transaction on the SqlCommand
				// in case it is reused and the connection/transaction are no longer valid.
				cmd.Connection = null;
				cmd.Transaction = null;
			}
		}

		/// <summary>
		/// Executes a Transact-SQL statement against the connection and returns the number
		/// of rows affected.
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public int ExecuteNonQuery(SqlCommand cmd)
		{
			var count = 0;
			ExecuteCommand(cmd, (exeCmd) =>
			{
				count = exeCmd.ExecuteNonQuery();
			});
			return count;
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public object ExecuteScalar(SqlCommand cmd, object dflt = null)
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
		/// <typeparam name="T"></typeparam>
		/// <param name="cmd"></param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public T ExecuteScalar<T>(SqlCommand cmd, T dflt = default(T))
		{
			var result = ExecuteScalar(cmd);
			if (result == null) return dflt;
			if (result == DBNull.Value) return dflt;
			return ConvertEx.To<T>(result);
		}

		/// <summary>
		/// Sends the System.Data.SqlClient.SqlCommand.CommandText to the System.Data.SqlClient.SqlCommand.Connection
		/// and builds a System.Data.SqlClient.SqlDataReader. The reader is only valid during execution of the method. 
		/// Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public void ExecuteReader(SqlCommand cmd, Func<SqlDataReader, bool> processRow)
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

		#endregion

		#region Typed Object Methods

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmd"></param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor).</param>
		/// <returns></returns>
		public T GetObject<T>(SqlCommand cmd, Func<IDataReader, T> load = null) where T : class
		{
			T obj = null;

			ExecuteReader(cmd, (row) =>
			{
				if (load != null)
				{
					obj = load(row);
					return false;
				}

				// Load doesn't have a value, so use the default loader.
				obj = ClassFactory.CreateObject<T>();
				var props = TypeDescriptor.GetProperties(typeof(T));
				FillObject(row, obj, props);

				return false;
			});

			return obj;
		}

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmd"></param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor). If this returns null, it will not be added to the results.</param>
		/// <returns></returns>
		public T[] GetObjects<T>(SqlCommand cmd, Func<SqlDataReader, T> load = null) where T : class
		{
			var results = new List<T>();

			// If load doesn't have a value, use the default loader.
			if (load == null)
			{
				var props = TypeDescriptor.GetProperties(typeof(T));
				load = (row) =>
				{
					var obj = ClassFactory.CreateObject<T>();
					FillObject(row, obj, props);
					return obj;
				};
			}

			ExecuteReader(cmd, (row) =>
			{
				var result = load(row);
				if (result != null)
					results.Add(result);

				return true;
			});

			return results.ToArray();
		}

		/// <summary>
		/// Fills the object and sets properties based on the field name. Assumes that the DataReader is on the correct row.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="obj"></param>
		/// <param name="props"></param>
		/// <returns>True if the object was filled, false if the data reader didn't contain any data.</returns>
		private void FillObject(SqlDataReader row, object obj, PropertyDescriptorCollection props)
		{
			for (var i = 0; i < row.FieldCount; i++)
			{
				var name = row.GetName(i);
				if (name.IsEmpty()) continue;
				var prop = props.Find(name, false);
				if (prop == null) continue;
				var value = ConvertEx.To(row[i], prop.PropertyType);
				prop.SetValue(obj, value);
			}
		}

		#endregion

		#region Dynamic Methods

		/// <summary>
		/// Gets the first row as a dynamic object.
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public dynamic GetDynamic(SqlCommand cmd)
		{
			dynamic result = null;

			ExecuteReader(cmd, (row) =>
			{
				result = SqlDataReaderToDynamic(row);
				return false;
			});

			return result;
		}

		/// <summary>
		/// Gets the first row as a dynamic object.
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public dynamic[] GetDynamics(SqlCommand cmd)
		{
			var results = new List<dynamic>();

			ExecuteReader(cmd, (row) =>
			{
				var result = SqlDataReaderToDynamic(row);
				results.Add(result);
				return true;
			});

			return results.ToArray();
		}

		private dynamic SqlDataReaderToDynamic(SqlDataReader row)
		{
			var result = new ExpandoObject() as IDictionary<string, object>;

			for (var i = 0; i < row.FieldCount; i++)
			{
				var value = row[i];
				if (value == DBNull.Value) value = null;
				var name = row.GetName(i);
				if (result.ContainsKey(name))
					result[name] = value;
				else
					result.Add(name, value);
			}

			return result;
		}

		#endregion

		#region ISupportBaDatabase

		/// <summary>
		/// Implementing this interface makes it simpler to pass this instance around.
		/// </summary>
		BaDatabase ISupportBaDatabase.Database
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
		BaDatabase Database { get; }
	}

}

