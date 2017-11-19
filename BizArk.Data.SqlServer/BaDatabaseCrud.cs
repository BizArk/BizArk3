using BizArk.Core;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Util;
using BizArk.Data.SqlServer.DataExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace BizArk.Data.SqlServer.Crud
{

	/// <summary>
	/// Provides methods for doing CRUD using basic objects.
	/// </summary>
	public static class BaDatabaseCrud
	{

		#region Object Insert/Update/Delete Methods

		/// <summary>
		/// Inserts a new record into the table.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName">Name of the table to insert into.</param>
		/// <param name="values">The values that will be added to the table. Can be anything that can be converted to a property bag.</param>
		/// <returns>The newly inserted row.</returns>
		public static dynamic Insert(this BaDatabase db, string tableName, object values)
		{
			var cmd = PrepareInsertCmd(tableName, values);
			return db.GetDynamic(cmd);
		}

		/// <summary>
		/// Creates the insert command.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="values"></param>
		/// <remarks>Internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		internal static SqlCommand PrepareInsertCmd(string tableName, object values)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder();

			var propBag = ObjectUtil.ToPropertyBag(values);

			var inFields = new StringBuilder();
			var valFields = new StringBuilder();
			foreach (var val in propBag)
			{
				if (inFields.Length > 0) inFields.Append(", ");
				inFields.Append(val.Key);

				if (valFields.Length > 0) valFields.Append(", ");
				var literal = GetValueLiteral(val.Value as string);
				if (literal != null)
				{
					valFields.Append(literal);
				}
				else
				{
					valFields.Append($"@{val.Key}");
					cmd.Parameters.AddWithValue(val.Key, val.Value, true);
				}
			}

			sb.AppendLine($"INSERT INTO {tableName} ({inFields})");
			sb.AppendLine("\tOUTPUT INSERTED.*");
			sb.AppendLine($"\tVALUES ({valFields});");

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		/// <summary>
		/// Updates the table with the given values.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName">Name of the table to update.</param>
		/// <param name="key">Key of the record to update. Can be anything that can be converted to a property bag.</param>
		/// <param name="values">The values that will be updated. Can be anything that can be converted to a property bag.</param>
		/// <returns></returns>
		public static int Update(this BaDatabase db, string tableName, object key, object values)
		{
			var cmd = PrepareUpdateCmd(tableName, key, values);
			return db.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// Creates the update command.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="key"></param>
		/// <param name="values"></param>
		/// <remarks>Internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		internal static SqlCommand PrepareUpdateCmd(string tableName, object key, object values)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder();

			sb.AppendLine($"UPDATE {tableName} SET");

			var propBag = ObjectUtil.ToPropertyBag(values);
			var keys = propBag.Keys.ToArray();
			for (var i = 0; i < keys.Length; i++)
			{
				var val = propBag[keys[i]];

				sb.Append($"\t\t{keys[i]} = ");

				var literal = GetValueLiteral(val as string);
				if (literal != null)
				{
					sb.Append(literal);
				}
				else
				{
					sb.Append($"@{keys[i]}");
					cmd.Parameters.AddWithValue(keys[i], val, true);
				}

				if (i < keys.Length - 1)
					sb.Append(",");
				sb.AppendLine();
			}

			var criteria = PrepareCriteria(cmd, key);
			if (criteria != null)
				sb.AppendLine(criteria);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		/// <summary>
		/// Deletes the records with the given key.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName">Name of the table to remove records from.</param>
		/// <param name="key">Key of the record to delete. Can be anything that can be converted to a property bag.</param>
		/// <returns></returns>
		public static int Delete(this BaDatabase db, string tableName, object key)
		{
			var cmd = PrepareDeleteCmd(tableName, key);
			return db.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// Creates the delete command.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="key"></param>
		/// <remarks>Internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		internal static SqlCommand PrepareDeleteCmd(string tableName, object key)
		{
			var cmd = new SqlCommand();
			var sb = new StringBuilder();

			sb.AppendLine($"DELETE FROM {tableName}");

			var criteria = PrepareCriteria(cmd, key);
			if (criteria != null)
				sb.AppendLine(criteria);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private static string PrepareCriteria(SqlCommand cmd, object key)
		{
			if (key == null) return null;

			var criteria = new StringBuilder();
			var propBag = ObjectUtil.ToPropertyBag(key);
			foreach (var val in propBag)
			{
				if (criteria.Length > 0)
					criteria.Append("\n\t\tAND ");
				criteria.Append($"{val.Key} = ");

				var literal = GetValueLiteral(val.Value as string);
				if (literal != null)
				{
					criteria.Append(literal);
				}
				else
				{
					criteria.Append($"@{val.Key}");
					cmd.Parameters.AddWithValue(val.Key, val.Value, true);
				}
			}
			return $"\tWHERE {criteria}";
		}

		private static string GetValueLiteral(string str)
		{
			if (str == null) return null;
			if (!str.StartsWith("[[")) return null;
			if (!str.EndsWith("]]")) return null;
			return str.Substring(2, str.Length - 4);
		}

		#endregion

		#region Typed Object Methods

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor).</param>
		/// <returns></returns>
		public static T GetObject<T>(this BaDatabase db, SqlCommand cmd, Func<IDataReader, T> load = null) where T : class
		{
			T obj = null;

			db.ExecuteReader(cmd, (row) =>
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
		/// <param name="db"></param>
		/// <param name="sprocName">Name of the stored procedure to call.</param>
		/// <param name="parameters">An object that contains the properties to add as SQL parameters to the SQL command.</param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor).</param>
		/// <returns></returns>
		public static T GetObject<T>(this BaDatabase db, string sprocName, object parameters = null, Func<IDataReader, T> load = null) where T : class
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return db.GetObject<T>(cmd, load);
		}

		/// <summary>
		/// Instantiates the object and sets properties based on the field name. Only returns the first row.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <param name="load">A method that will create an object and fill it. If null, the object will be instantiated based on its type using the ClassFactory (must have a default ctor). If this returns null, it will not be added to the results.</param>
		/// <returns></returns>
		public static IEnumerable<T> GetObjects<T>(this BaDatabase db, SqlCommand cmd, Func<SqlDataReader, T> load = null) where T : class
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

			db.ExecuteReader(cmd, (row) =>
			{
				var result = load(row);
				if (result != null)
					results.Add(result);

				return true;
			});

			return results;
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
		public static IEnumerable<T> GetObjects<T>(this BaDatabase db, string sprocName, object parameters = null, Func<SqlDataReader, T> load = null) where T : class
		{
			var cmd = db.PrepareSprocCmd(sprocName, parameters);
			return db.GetObjects<T>(cmd, load);
		}

		/// <summary>
		/// Fills the object and sets properties based on the field name. Assumes that the DataReader is on the correct row.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="obj"></param>
		/// <param name="props"></param>
		/// <returns>True if the object was filled, false if the data reader didn't contain any data.</returns>
		private static void FillObject(SqlDataReader row, object obj, PropertyDescriptorCollection props)
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
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public static dynamic GetDynamic(this BaDatabase db, SqlCommand cmd)
		{
			dynamic result = null;

			db.ExecuteReader(cmd, (row) =>
			{
				result = SqlDataReaderToDynamic(row);
				return false;
			});

			return result;
		}

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
		/// Returns the results of the SQL command as a list of dynamic objects.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public static IEnumerable<dynamic> GetDynamics(this BaDatabase db, SqlCommand cmd)
		{
			var results = new List<dynamic>();

			db.ExecuteReader(cmd, (row) =>
			{
				var result = SqlDataReaderToDynamic(row);
				results.Add(result);
				return true;
			});

			return results;
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

		private static dynamic SqlDataReaderToDynamic(SqlDataReader row)
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

	}

}
