using BizArk.Data.DataExt;
using BizArk.Data.ExtractExt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace BizArk.Data.SprocExt
{

	/// <summary>
	/// Extension methods for `BaDatabase` to support calling stored procedures.
	/// </summary>
	public static class BaDatabaseSprocExt
	{

		#region Basic Database Methods

		/// <summary>
		/// Executes a Transact-SQL statement against the connection and returns the number
		/// of rows affected.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <returns></returns>
		public static int ExecuteNonQuery(this BaDatabase db, string sprocName, object parameters = null)
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return db.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// Executes a Transact-SQL statement against the connection and returns the number
		/// of rows affected.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <returns></returns>
		public static async Task<int> ExecuteNonQueryAsync(this BaDatabase db, string sprocName, object parameters = null)
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return await db.ExecuteNonQueryAsync(cmd).ConfigureAwait(false);
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public static object ExecuteScalar(this BaDatabase db, string sprocName, object parameters = null, object dflt = null)
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return db.ExecuteScalar(cmd, dflt);
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public static async Task<object> ExecuteScalarAsync(this BaDatabase db, string sprocName, object parameters = null, object dflt = null)
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return await db.ExecuteScalarAsync(cmd, dflt).ConfigureAwait(false);
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public static T ExecuteScalar<T>(this BaDatabase db, string sprocName, object parameters = null, T dflt = default(T))
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return db.ExecuteScalar(cmd, dflt);
		}

		/// <summary>
		/// Executes the query, and returns the first column of the first row in the result
		/// set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public static async Task<T> ExecuteScalarAsync<T>(this BaDatabase db, string sprocName, object parameters = null, T dflt = default(T))
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return await db.ExecuteScalarAsync(cmd, dflt).ConfigureAwait(false);
		}

		/// <summary>
		/// Sends the System.Data.SqlClient.DbCommand.CommandText to the System.Data.SqlClient.DbCommand.Connection
		/// and builds a System.Data.SqlClient.DbDataReader. The reader is only valid during execution of the method. 
		/// Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public static void ExecuteReader(this BaDatabase db, string sprocName, Func<DbDataReader, bool> processRow)
		{
			var cmd = db.PrepareSprocCmd(sprocName, null);
			db.ExecuteReader(cmd, processRow);
		}

		/// <summary>
		/// Sends the System.Data.SqlClient.DbCommand.CommandText to the System.Data.SqlClient.DbCommand.Connection
		/// and builds a System.Data.SqlClient.DbDataReader. The reader is only valid during execution of the method. 
		/// Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public static async Task ExecuteReaderAsync(this BaDatabase db, string sprocName, Func<DbDataReader, bool> processRow)
		{
			var cmd = db.PrepareSprocCmd(sprocName, null);
			await db.ExecuteReaderAsync(cmd, processRow).ConfigureAwait(false);
		}

		/// <summary>
		/// Sends the System.Data.SqlClient.DbCommand.CommandText to the System.Data.SqlClient.DbCommand.Connection
		/// and builds a System.Data.SqlClient.DbDataReader. The reader is only valid during execution of the method. 
		/// Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public static void ExecuteReader(this BaDatabase db, string sprocName, object parameters, Func<DbDataReader, bool> processRow)
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			db.ExecuteReader(cmd, processRow);
		}

		/// <summary>
		/// Sends the System.Data.SqlClient.DbCommand.CommandText to the System.Data.SqlClient.DbCommand.Connection
		/// and builds a System.Data.SqlClient.DbDataReader. The reader is only valid during execution of the method. 
		/// Use processRow to process each row in the reader.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="processRow">Called for each row in the data reader. Return true to continue processing more rows.</param>
		public static async Task ExecuteReaderAsync(this BaDatabase db, string sprocName, object parameters, Func<DbDataReader, bool> processRow)
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			await db.ExecuteReaderAsync(cmd, processRow).ConfigureAwait(false);
		}

		#endregion

		#region Typed Object Methods

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor).</param>
		/// <returns></returns>
		public static T GetObject<T>(this BaDatabase db, string sprocName, object parameters = null, Func<IDataReader, T> load = null) where T : class
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return db.GetObject(cmd, load);
		}

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor).</param>
		/// <returns></returns>
		public async static Task<T> GetObjectAsync<T>(this BaDatabase db, string sprocName, object parameters = null, Func<IDataReader, T> load = null) where T : class
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return await db.GetObjectAsync(cmd, load).ConfigureAwait(false);
		}

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor). If this returns null, it will not be added to the results.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetObjects<T>(this BaDatabase db, string sprocName, object parameters = null, Func<DbDataReader, T> load = null) where T : class
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return db.GetObjects(cmd, load);
		}

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor). If this returns null, it will not be added to the results.</param>
		/// <returns></returns>
		public async static Task<IEnumerable<T>> GetObjectsAsync<T>(this BaDatabase db, string sprocName, object parameters = null, Func<DbDataReader, T> load = null) where T : class
		{
			var cmd = await db.PrepareSprocCmdAsync(sprocName, parameters).ConfigureAwait(false);
			return await db.GetObjectsAsync(cmd, load).ConfigureAwait(false);
		}

		#endregion

		#region Dynamic Methods

		/// <summary>
		/// Gets the first row as a dynamic object.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <returns></returns>
		public static dynamic GetDynamic(this BaDatabase db, string sprocName, object parameters = null)
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return db.GetDynamic(cmd);
		}

		/// <summary>
		/// Gets the first row as a dynamic object.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <returns></returns>
		public async static Task<dynamic> GetDynamicAsync(this BaDatabase db, string sprocName, object parameters = null)
		{
			var cmd = await db.PrepareSprocCmdAsync(sprocName, parameters).ConfigureAwait(false);
			return await db.GetDynamicAsync(cmd).ConfigureAwait(false);
		}

		/// <summary>
		/// Returns the results of the SQL command as a list of dynamic objects.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <returns></returns>
		public static IEnumerable<dynamic> GetDynamics(this BaDatabase db, string sprocName, object parameters = null)
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return db.GetDynamics(cmd);
		}

		/// <summary>
		/// Returns the results of the SQL command as a list of dynamic objects.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <returns></returns>
		public async static Task<IEnumerable<dynamic>> GetDynamicsAsync(this BaDatabase db, string sprocName, object parameters = null)
		{
			var cmd = await db.PrepareSprocCmdAsync(sprocName, parameters).ConfigureAwait(false);
			return await db.GetDynamicsAsync(cmd).ConfigureAwait(false);
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Creates a DbCommand to execute a stored procedure.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName"></param>
		/// <param name="parameters"></param>
		/// <remarks>This is internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		internal static DbCommand PrepareSprocCmd(this BaDatabase db, string sprocName, object parameters)
		{
			var cmd = db.Connection.CreateCommand();
			cmd.CommandText = sprocName;
			cmd.CommandType = CommandType.StoredProcedure;

			if (parameters != null)
				cmd.AddParameters(parameters);

			return cmd;
		}

		/// <summary>
		/// Creates a DbCommand to execute a stored procedure.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="sprocName"></param>
		/// <param name="parameters"></param>
		/// <remarks>This is internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		internal static async Task<DbCommand> PrepareSprocCmdAsync(this BaDatabase db, string sprocName, object parameters)
		{
			var conn = await db.GetConnectionAsync().ConfigureAwait(false);
			var cmd = conn.CreateCommand();
			cmd.CommandText = sprocName;
			cmd.CommandType = CommandType.StoredProcedure;

			if (parameters != null)
				cmd.AddParameters(parameters);

			return cmd;
		}

		#endregion

	}

}

