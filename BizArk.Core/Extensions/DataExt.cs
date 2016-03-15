using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BizArk.Core.Extensions.ArrayExt;
using BizArk.Core.Util;

namespace BizArk.Core.Extensions.DataExt
{
    /// <summary>
    /// Provides extension methods for string arrays.
    /// </summary>
    public static class DataExt
    {

		#region DataRow

		/// <summary>
		/// Returns the field value as a string. Uses ConvertEx to convert the value.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static string GetString(this DataRow row, string fieldName)
        {
            return GetValue<string>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a string. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static string GetString(this DataRow row, string fieldName, string dfltVal)
        {
            return GetValue<string>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int? GetInt(this DataRow row, string fieldName)
        {
            return GetValue<int?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static int GetInt(this DataRow row, string fieldName, int dfltVal)
        {
            return GetValue<int>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool GetBool(this DataRow row, string fieldName)
        {
            return GetValue<bool>(row, fieldName, false);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static bool GetBool(this DataRow row, string fieldName, bool dfltVal)
        {
            return GetValue<bool>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static DateTime? GetDateTime(this DataRow row, string fieldName)
        {
            return GetValue<DateTime?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static DateTime GetDateTime(this DataRow row, string fieldName, DateTime dfltVal)
        {
            return GetValue<DateTime>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a double. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static double? GetDouble(this DataRow row, string fieldName)
        {
            return GetValue<double?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a double. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static double GetDouble(this DataRow row, string fieldName, double dfltVal)
        {
            return GetValue<double>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a Guid. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Guid? GetGuid(this DataRow row, string fieldName)
        {
            return GetValue<Guid?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a Guid. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static Guid GetGuid(this DataRow row, string fieldName, Guid dfltVal)
        {
            return GetValue<Guid>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DataRow row, string fieldName)
        {
            return ConvertEx.To<T>(row[fieldName]);
        }

        /// <summary>
        /// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static T GetValue<T>(this DataRow row, string fieldName, T dfltVal)
        {
            if (row.IsNull(fieldName)) return dfltVal;
            return ConvertEx.To<T>(row[fieldName]);
        }

		#endregion

		#region DataRowView

		/// <summary>
		/// Returns the field value as a string. Uses ConvertEx to convert the value to a string.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static string GetString(this DataRowView row, string fieldName)
        {
            return GetValue<string>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a string. Uses ConvertEx to convert the value to a string.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static string GetString(this DataRowView row, string fieldName, string dfltVal)
        {
            return GetValue<string>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int? GetInt(this DataRowView row, string fieldName)
        {
            return GetValue<int?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static int GetInt(this DataRowView row, string fieldName, int dfltVal)
        {
            return GetValue<int>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool GetBool(this DataRowView row, string fieldName)
        {
            return GetValue<bool>(row, fieldName, false);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static bool GetBool(this DataRowView row, string fieldName, bool dfltVal)
        {
            return GetValue<bool>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static DateTime? GetDateTime(this DataRowView row, string fieldName)
        {
            return GetValue<DateTime?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static DateTime GetDateTime(this DataRowView row, string fieldName, DateTime dfltVal)
        {
            return GetValue<DateTime>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a double. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static double? GetDouble(this DataRowView row, string fieldName)
        {
            return GetValue<double?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a double. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static double GetDouble(this DataRowView row, string fieldName, double dfltVal)
        {
            return GetValue<double>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a Guid. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Guid? GetGuid(this DataRowView row, string fieldName)
        {
            return GetValue<Guid?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a Guid. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static Guid GetGuid(this DataRowView row, string fieldName, Guid dfltVal)
        {
            return GetValue<Guid>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DataRowView row, string fieldName)
        {
            return ConvertEx.To<T>(row[fieldName]);
        }

        /// <summary>
        /// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static T GetValue<T>(this DataRowView row, string fieldName, T dfltVal)
        {
            if (row.IsNull(fieldName)) return dfltVal;
            return ConvertEx.To<T>(row[fieldName]);
        }

        /// <summary>
        /// Determines if the field is null.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool IsNull(this DataRowView row, string fieldName)
        {
            return row.Row.IsNull(fieldName);
        }

		#endregion

		#region IDataReader

		/// <summary>
		/// Returns the field value as a string. Uses ConvertEx to convert the value to a string.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static string GetString(this IDataReader row, string fieldName)
        {
            return GetValue<string>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a string. Uses ConvertEx to convert the value to a string.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static string GetString(this IDataReader row, string fieldName, string dfltVal)
        {
            return GetValue<string>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int? GetInt(this IDataReader row, string fieldName)
        {
            return GetValue<int?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static int GetInt(this IDataReader row, string fieldName, int dfltVal)
        {
            return GetValue<int>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool GetBool(this IDataReader row, string fieldName)
        {
            return GetValue<bool>(row, fieldName, false);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static bool GetBool(this IDataReader row, string fieldName, bool dfltVal)
        {
            return GetValue<bool>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static DateTime? GetDateTime(this IDataReader row, string fieldName)
        {
            return GetValue<DateTime?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a int. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static DateTime GetDateTime(this IDataReader row, string fieldName, DateTime dfltVal)
        {
            return GetValue<DateTime>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a double. Uses ConvertEx to convert the value to a double.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static double? GetDouble(this IDataReader row, string fieldName)
        {
            return GetValue<double?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a double. Uses ConvertEx to convert the value to a double.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static double GetDouble(this IDataReader row, string fieldName, double dfltVal)
        {
            return GetValue<double>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a decimal. Uses ConvertEx to convert the value to a decimal.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static decimal? GetDecimal(this IDataReader row, string fieldName)
        {
            return GetValue<decimal?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a decimal. Uses ConvertEx to convert the value to a decimal.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static decimal GetDecimal(this IDataReader row, string fieldName, decimal dfltVal)
        {
            return GetValue<decimal>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a Guid. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Guid? GetGuid(this IDataReader row, string fieldName)
        {
            return GetValue<Guid?>(row, fieldName);
        }

        /// <summary>
        /// Returns the field value as a Guid. Uses ConvertEx to convert the value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static Guid GetGuid(this IDataReader row, string fieldName, Guid dfltVal)
        {
            return GetValue<Guid>(row, fieldName, dfltVal);
        }

        /// <summary>
        /// Returns the field value as a byte array.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this IDataReader row, string fieldName)
        {
            return GetValue<byte[]>(row, fieldName, null);
        }

        /// <summary>
        /// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader row, string fieldName)
        {
            return ConvertEx.To<T>(row[fieldName]);
        }

        /// <summary>
        /// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="dfltVal">The value to return if the value is DBNull</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader row, string fieldName, T dfltVal)
        {
            var i = row.GetOrdinal(fieldName);
            if (row.IsDBNull(i)) return dfltVal;
            return ConvertEx.To<T>(row[i]);
        }

        /// <summary>
        /// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetValue(this IDataReader row, string fieldName, Type type)
        {
            var i = row.GetOrdinal(fieldName);
            return ConvertEx.To(row[i], type);
        }

        /// <summary>
        /// Determines if the field is null.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool IsNull(this IDataReader row, string fieldName)
        {
            var i = row.GetOrdinal(fieldName);
            return row.IsDBNull(i);
        }

        /// <summary>
        /// Determines if the IDataReader contains the specified field.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool ContainsField(this IDataReader row, string fieldName)
        {
            for (int i = 0; i < row.FieldCount; i++)
            {
                if (row.GetName(i).Equals(fieldName, StringComparison.CurrentCultureIgnoreCase)) return true;
            }
            return false;
        }

		#endregion

		#region SqlParameter

		/// <summary>
		/// Adds a value to the end of the parameter collection.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="setNull">If true, sets the value to DBNull if it ConvertEx.IsEmpty is true.</param>
		/// <returns></returns>
		public static SqlParameter AddWithValue(this SqlParameterCollection parameters, string name, object value, bool setNull)
        {
            if (setNull && ConvertEx.IsEmpty(value))
                value = DBNull.Value;
            return parameters.AddWithValue(name, value);
        }

		/// <summary>
		/// This will add an array of parameters to a SqlCommand. This is used for an IN statement.
		/// Use the returned value for the IN part of your SQL call. (i.e. SELECT * FROM table WHERE field IN ({paramNameRoot}))
		/// </summary>
		/// <param name="cmd">The SqlCommand object to add parameters to.</param>
		/// <param name="values">The array of strings that need to be added as parameters.</param>
		/// <param name="paramNameRoot">What the parameter should be named followed by a unique value for each value. This value surrounded by {} in the CommandText will be replaced.</param>
		/// <param name="start">The beginning number to append to the end of paramNameRoot for each value.</param>
		/// <param name="separator">The string that separates the parameter names in the sql command.</param>
		public static SqlParameter[] AddArrayParameters<T>(this SqlCommand cmd, IEnumerable<T> values, string paramNameRoot, int start = 1, string separator = ", ")
		{
			/* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
			 * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
			 * IN statement in the CommandText.
			 */
			var parameters = new List<SqlParameter>();
			var parameterNames = new List<string>();
			var paramNbr = start;
			foreach (var value in values)
			{
				var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
				parameterNames.Add(paramName);
				parameters.Add(cmd.Parameters.AddWithValue(paramName, value));
			}

			cmd.CommandText = cmd.CommandText.Replace("{" + paramNameRoot + "}", string.Join(separator, parameterNames));

			return parameters.ToArray();
		}

		/// <summary>
		/// Adds the objects properties as parameters.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="parameters"></param>
		public static void AddParameters(this SqlCommand cmd, object parameters)
		{
			var propBag = ObjectUtil.ToPropertyBag(parameters);
			foreach(var prop in propBag)
				cmd.Parameters.AddWithValue(prop.Key, prop.Value, true);
		}

		#endregion

		#region Debug SQL

		/// <summary>
		/// Converts the command into TSql that can be executed in Sql Server Management Studio.
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public static string DebugText(this SqlCommand cmd)
		{
			// No parameters, no problem.
			if (cmd.Parameters.Count == 0)
				return cmd.CommandText;
			
			var sb = new StringBuilder();
			
			foreach(SqlParameter param in cmd.Parameters)
			{				
				sb.AppendFormat($"DECLARE {DebugSqlParamName(param)} AS {DebugSqlType(param)} = {DebugSqlValue(param)}\n");
			}

			sb.Append(cmd.CommandText);

			return sb.ToString();
		}

		private static object DebugSqlParamName(SqlParameter param)
		{
			if (param.ParameterName.StartsWith("@"))
				return param.ParameterName;
			else
				return "@" + param.ParameterName;
		}

		private static object DebugSqlValue(SqlParameter param)
		{
			if (param.Value == null) return "NULL";
			if (param.Value == DBNull.Value) return "NULL";

			switch(param.SqlDbType)
			{
				case SqlDbType.Char:
				case SqlDbType.Text:
				case SqlDbType.Time:
				case SqlDbType.VarChar:
				case SqlDbType.Xml:
				case SqlDbType.Date:
				case SqlDbType.DateTime:
				case SqlDbType.DateTime2:
				case SqlDbType.DateTimeOffset:
					return $"'{param.Value.ToString().Replace("'", "''")}'";

				case SqlDbType.NChar:
				case SqlDbType.NText:
				case SqlDbType.NVarChar:
					return $"N'{param.Value.ToString().Replace("'", "''")}'";

				case SqlDbType.Binary:
				case SqlDbType.VarBinary:
					var bytes = param.Value as IEnumerable<byte>;
					return $"0x{bytes.ToHex()}";

				case SqlDbType.Bit:
					return ConvertEx.ToBool(param.Value) ? "1" : "0";

				default:
					return param.Value.ToString();
			}
		}

		private static string DebugSqlType(SqlParameter param)
		{
			switch (param.SqlDbType)
			{
				case SqlDbType.Char:
				case SqlDbType.VarChar:
				case SqlDbType.NChar:
				case SqlDbType.NText:
				case SqlDbType.NVarChar:
				case SqlDbType.Binary:
				case SqlDbType.VarBinary:
					return $"{param.SqlDbType.ToString().ToUpper()}({param.Size})";
				default:
					return param.SqlDbType.ToString().ToUpper();
			}
		}

		#endregion

	}
}
