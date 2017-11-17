using System;
using System.Data;

namespace BizArk.Core.Extensions.DataExt
{
	/// <summary>
	/// Provides extension methods for string arrays.
	/// </summary>
	public static class DataExt
	{

		#region DataRow

		/// <summary>
		/// Returns the field value as a bool. Uses ConvertEx to convert the value.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static bool? GetBool(this DataRow row, string fieldName)
		{
			return GetValue<bool?>(row, fieldName);
		}

		/// <summary>
		/// Returns the field value as a byte array.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static byte[] GetBytes(this DataRow row, string fieldName)
		{
			return GetValue<byte[]>(row, fieldName);
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
		/// Returns the field value as a decimal. Uses ConvertEx to convert the value.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static decimal? GetDecimal(this DataRow row, string fieldName)
		{
			return GetValue<decimal?>(row, fieldName);
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
		/// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <param name="dfltVal">The value to return if the value is DBNull</param>
		/// <returns></returns>
		public static T GetValue<T>(this DataRow row, string fieldName, T dfltVal = default(T))
		{
			if (row.IsNull(fieldName)) return dfltVal;
			return ConvertEx.To<T>(row[fieldName]);
		}

		#endregion

		#region DataRowView

		/// <summary>
		/// Returns the field value as a bool. Uses ConvertEx to convert the value.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static bool? GetBool(this DataRowView row, string fieldName)
		{
			return GetValue<bool?>(row, fieldName);
		}

		/// <summary>
		/// Returns the field value as a byte array.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static byte[] GetBytes(this DataRowView row, string fieldName)
		{
			return GetValue<byte[]>(row, fieldName);
		}

		/// <summary>
		/// Returns the field value as a DateTime. Uses ConvertEx to convert the value.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static DateTime? GetDateTime(this DataRowView row, string fieldName)
		{
			return GetValue<DateTime?>(row, fieldName);
		}

		/// <summary>
		/// Returns the field value as a decimal. Uses ConvertEx to convert the value.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static decimal? GetDecimal(this DataRowView row, string fieldName)
		{
			return GetValue<decimal?>(row, fieldName);
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
		/// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <param name="dfltVal">The value to return if the value is DBNull</param>
		/// <returns></returns>
		public static T GetValue<T>(this DataRowView row, string fieldName, T dfltVal = default(T))
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

		/// <summary>
		/// Returns the field value as a bool. Uses ConvertEx to convert the value.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static bool? GetBool(this IDataReader row, string fieldName)
		{
			return GetValue<bool?>(row, fieldName);
		}

		/// <summary>
		/// Returns the field value as a byte array.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static byte[] GetBytes(this IDataReader row, string fieldName)
		{
			return GetValue<byte[]>(row, fieldName);
		}

		/// <summary>
		/// Returns the field value as a DateTime. Uses ConvertEx to convert the value.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static DateTime? GetDateTime(this IDataReader row, string fieldName)
		{
			return GetValue<DateTime?>(row, fieldName);
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
		/// Returns the field value as the specified type. Uses ConvertEx to convert the value to the correct type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="row"></param>
		/// <param name="fieldName"></param>
		/// <param name="dfltVal">The value to return if the value is DBNull</param>
		/// <returns></returns>
		public static T GetValue<T>(this IDataReader row, string fieldName, T dfltVal = default(T))
		{
			var i = row.GetOrdinal(fieldName);
			if (row.IsDBNull(i)) return dfltVal;
			return ConvertEx.To<T>(row[i]);
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

		#endregion

	}
}
