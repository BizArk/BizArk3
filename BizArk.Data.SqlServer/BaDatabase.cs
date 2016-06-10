using BizArk.Core;
using BizArk.Core.Extensions.DataExt;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;

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
			if (connStr.IsEmpty()) throw new ArgumentNullException("connStr");
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

		/// <summary>
		/// Creates the BaDatabase from the connection string named in the config file.
		/// </summary>
		/// <param name="name">The name or key of the connection string in the config file.</param>
		/// <returns></returns>
		public static BaDatabase Create(string name)
		{
			var connStrSetting = ConfigurationManager.ConnectionStrings[name];
			if (connStrSetting == null)
				throw new InvalidOperationException($"The connection string setting for '{name}' was not found.");
            return ClassFactory.CreateObject<BaDatabase>(connStrSetting.ConnectionString);
        }

        #endregion

        #region Fields and Properties

        // Internal so it can be viewed in the unit tests.
        internal string mConnectionString;

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
		public BaTransaction Transaction { get; internal set; } // Internal so it can be called from BaTransaction.

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
				var conn = Connection;
				if (conn.State == ConnectionState.Closed)
					conn.Open();
				cmd.Connection = conn;				
				if (Transaction != null)
					cmd.Transaction = Transaction.Transaction;
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
		/// Executes a Transact-SQL statement against the connection and returns the number
		/// of rows affected.
		/// </summary>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <returns></returns>
		public int ExecuteNonQuery(string sprocName, object parameters = null)
		{
			var cmd = PrepareSprocCmd(sprocName, parameters);
			return ExecuteNonQuery(cmd);
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
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public object ExecuteScalar(string sprocName, object parameters = null, object dflt = null)
		{
			var cmd = PrepareSprocCmd(sprocName, parameters);
			return ExecuteScalar(cmd, dflt);
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
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public T ExecuteScalar<T>(string sprocName, object parameters = null, T dflt = default(T))
		{
			var cmd = PrepareSprocCmd(sprocName, parameters);
			return ExecuteScalar<T>(cmd, dflt);
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

		/// <summary>
		/// Sends the System.Data.SqlClient.SqlCommand.CommandText to the System.Data.SqlClient.SqlCommand.Connection
		/// and builds a System.Data.SqlClient.SqlDataReader. The reader is only valid during execution of the method. 
		/// Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public void ExecuteReader(string sprocName, Func<SqlDataReader, bool> processRow)
		{
			var cmd = PrepareSprocCmd(sprocName, null);
			ExecuteReader(cmd, processRow);
		}

		/// <summary>
		/// Sends the System.Data.SqlClient.SqlCommand.CommandText to the System.Data.SqlClient.SqlCommand.Connection
		/// and builds a System.Data.SqlClient.SqlDataReader. The reader is only valid during execution of the method. 
		/// Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public void ExecuteReader(string sprocName, object parameters, Func<SqlDataReader, bool> processRow)
		{
			var cmd = PrepareSprocCmd(sprocName, parameters);
			ExecuteReader(cmd, processRow);
		}

		#endregion

		#region Transaction Methods

		/// <summary>
		/// Starts a transaction. Must call Dispose on the transaction.
		/// </summary>
		/// <returns></returns>
		public BaTransaction BeginTransaction()
		{
			var conn = Connection;
			if (conn.State == ConnectionState.Closed)
				conn.Open();

			return Transaction = new BaTransaction(this);
		}

		#endregion

		#region Object Insert/Update/Delete Methods

		/// <summary>
		/// Inserts a new record into the table.
		/// </summary>
		/// <param name="tableName">Name of the table to insert into.</param>
		/// <param name="values">The values that will be added to the table. Can be anything that can be converted to a property bag.</param>
		/// <returns>The newly inserted row.</returns>
		public dynamic Insert(string tableName, object values)
		{
			var cmd = PrepareInsertCmd(tableName, values);
			return GetDynamic(cmd);
		}

		/// <summary>
		/// Creates the insert command.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="values"></param>
		/// <remarks>Internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		internal SqlCommand PrepareInsertCmd(string tableName, object values)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder();

			var fields = new List<string>();
			var propBag = ObjectUtil.ToPropertyBag(values);

			foreach (var val in propBag)
			{
				fields.Add(val.Key);
				cmd.Parameters.AddWithValue(val.Key, val.Value, true);
			}

			sb.AppendLine($"INSERT INTO {tableName} ({string.Join(", ", fields)})");
			sb.AppendLine("\tOUTPUT INSERTED.*");
			sb.AppendLine($"\tVALUES (@{string.Join(", @", fields)});");

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		/// <summary>
		/// Updates the table with the given values.
		/// </summary>
		/// <param name="tableName">Name of the table to update.</param>
		/// <param name="key">Key of the record to update. Can be anything that can be converted to a property bag.</param>
		/// <param name="values">The values that will be updated. Can be anything that can be converted to a property bag.</param>
		/// <returns></returns>
		public int Update(string tableName, object key, object values)
		{
			var cmd = PrepareUpdateCmd(tableName, key, values);
			return ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// Creates the update command.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="key"></param>
		/// <param name="values"></param>
		/// <remarks>Internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		internal SqlCommand PrepareUpdateCmd(string tableName, object key, object values)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder();

			sb.AppendLine($"UPDATE {tableName} SET");

			var propBag = ObjectUtil.ToPropertyBag(values);
			var keys = propBag.Keys.ToArray();
			for (var i = 0; i < keys.Length; i++)
			{
				var val = propBag[keys[i]];
				sb.AppendLine($"\t\t{keys[i]} = @{keys[i]}{(i < keys.Length - 1 ? "," : "")}");
				cmd.Parameters.AddWithValue(keys[i], val, true);
			}

			// Get the criteria for the UPDATE.
			if (key != null)
			{
				var criteria = new List<string>();
				propBag = ObjectUtil.ToPropertyBag(key);
				foreach (var val in propBag)
				{
					criteria.Add($"{val.Key} = @{val.Key}");
					cmd.Parameters.AddWithValue(val.Key, val.Value, true);
				}
				sb.AppendLine($"\tWHERE {string.Join("\n\t\tAND ", criteria) }");
			}

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		/// <summary>
		/// Deletes the records with the given key.
		/// </summary>
		/// <param name="tableName">Name of the table to remove records from.</param>
		/// <param name="key">Key of the record to delete. Can be anything that can be converted to a property bag.</param>
		/// <returns></returns>
		public int Delete(string tableName, object key)
		{
			var cmd = PrepareDeleteCmd(tableName, key);
			return ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// Creates the delete command.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="key"></param>
		/// <remarks>Internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		internal SqlCommand PrepareDeleteCmd(string tableName, object key)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder();

			sb.AppendLine($"DELETE FROM {tableName}");

			// Get the criteria for the UPDATE.
			if (key != null)
			{
				var criteria = new List<string>();
				var propBag = ObjectUtil.ToPropertyBag(key);
				foreach (var val in propBag)
				{
					criteria.Add($"{val.Key} = @{val.Key}");
					cmd.Parameters.AddWithValue(val.Key, val.Value, true);
				}
				sb.AppendLine($"\tWHERE {string.Join("\n\t\tAND ", criteria) }");
			}

			cmd.CommandText = sb.ToString();

			return cmd;
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
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor).</param>
		/// <returns></returns>
		public T GetObject<T>(string sprocName, object parameters = null, Func<IDataReader, T> load = null) where T : class
		{
			var cmd = PrepareSprocCmd(sprocName, parameters);
			return GetObject<T>(cmd, load);
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
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor). If this returns null, it will not be added to the results.</param>
		/// <returns></returns>
		public T[] GetObjects<T>(string sprocName, object parameters = null, Func<SqlDataReader, T> load = null) where T : class
		{
			var cmd = PrepareSprocCmd(sprocName, parameters);
			return GetObjects<T>(cmd, load);
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
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <returns></returns>
		public dynamic[] GetDynamic(string sprocName, object parameters = null)
		{
			var cmd = PrepareSprocCmd(sprocName, parameters);
			return GetDynamic(cmd);
		}

		/// <summary>
		/// Returns the results of the SQL command as a list of dynamic objects.
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

		/// <summary>
		/// Returns the results of the SQL command as a list of dynamic objects.
		/// </summary>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <returns></returns>
		public dynamic[] GetDynamics(string sprocName, object parameters = null)
		{
			var cmd = PrepareSprocCmd(sprocName, parameters);
			return GetDynamics(cmd);
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

		#region Utility Methods

		/// <summary>
		/// Creates a SqlCommand to execute a stored procedure.
		/// </summary>
		/// <param name="sprocName"></param>
		/// <param name="parameters"></param>
		/// <remarks>This is internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		internal SqlCommand PrepareSprocCmd(string sprocName, object parameters)
		{
			var cmd = new SqlCommand(sprocName);
			cmd.CommandType = CommandType.StoredProcedure;

			if (parameters != null)
				cmd.AddParameters(parameters);

			return cmd;
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

