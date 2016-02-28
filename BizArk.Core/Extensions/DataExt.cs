using System;
using System.Data;
using System.Data.SqlClient;

namespace BizArk.Core.Extensions.DataExt
{
    /// <summary>
    /// Provides extension methods for string arrays.
    /// </summary>
    public static class DataExt
    {

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
            return ConvertEx.ChangeType<T>(row[fieldName]);
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
            return ConvertEx.ChangeType<T>(row[fieldName]);
        }

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
            return ConvertEx.ChangeType<T>(row[fieldName]);
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
            return ConvertEx.ChangeType<T>(row[fieldName]);
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
            return ConvertEx.ChangeType<T>(row[fieldName]);
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
            return ConvertEx.ChangeType<T>(row[i]);
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
            return ConvertEx.ChangeType(row[i], type);
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

    }
}
