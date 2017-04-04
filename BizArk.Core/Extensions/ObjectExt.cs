using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BizArk.Core.Data;
using BizArk.Core.Util;

namespace BizArk.Core.Extensions.ObjectExt
{
	/// <summary>
	/// Extends the Object class.
	/// </summary>
	public static class ObjectExt
	{
		/// <summary>
		/// Converts the value to the specified type. 
		/// Checks for a TypeConverter, conversion methods, 
		/// and the IConvertible interface. Uses <see cref="BizArk.Core.ConvertEx.To(object, Type, IFormatProvider)"/>.
		/// </summary>
		/// <typeparam name="T">The type to convert to.</typeparam>
		/// <param name="obj">The value to convert from.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">This conversion is not supported. -or-value is null and conversionType is a value type.</exception>
		/// <exception cref="System.ArgumentNullException">conversionType is null.</exception>
		public static T To<T>(this object obj)
		{
			return ConvertEx.To<T>(obj);
		}

		/// <summary>
		/// Gets the value for the given property name. Uses ObjectUtil.GetValue so you can use any propertyName that supports.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static object GetValue(this object obj, string propertyName)
		{
			return ObjectUtil.GetValue(obj, propertyName);
		}

		/// <summary>
		/// Gets the value for the given property name. Uses ObjectUtil.GetValue so you can use any propertyName that supports.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static T GetValue<T>(this object obj, string propertyName)
		{
			return (T)GetValue(obj, propertyName);
		}

		/// <summary>
		/// Gets the value for the given property name. Uses ObjectUtil.GetValue so you can use any propertyName that supports.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static int GetInt(this object obj, string propertyName)
		{
			return GetValue<int>(obj, propertyName);
		}

		/// <summary>
		/// Gets the value for the given property name. Uses ObjectUtil.GetValue so you can use any propertyName that supports.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static string GetString(this object obj, string propertyName)
		{
			return GetValue<string>(obj, propertyName);
		}

		/// <summary>
		/// Gets the value for the given property name. Uses ObjectUtil.GetValue so you can use any propertyName that supports.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static bool GetBoolean(this object obj, string propertyName)
		{
			return GetValue<bool>(obj, propertyName);
		}

		/// <summary>
		/// Gets the value for the given property name. Uses ObjectUtil.GetValue so you can use any propertyName that supports.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static decimal GetDecimal(this object obj, string propertyName)
		{
			return GetValue<decimal>(obj, propertyName);
		}

		/// <summary>
		/// Uses DataAnnotations to validate the properties of the object.
		/// </summary>
		/// <param name="obj"></param>
		public static ValidationResult[] Validate(this object obj)
		{
			var baObj = obj as BaObject;
			if (baObj != null)
				return baObj.Validate();

			var ctx = new ValidationContext(obj, null, null);
			var results = new List<ValidationResult>();
			Validator.TryValidateObject(obj, ctx, results, true);
			return results.ToArray();
		}

		/// <summary>
		/// Normalizes a number of different types into a dictionary. Works with DataRow, DataTable, IDataReader, IDictionary, or POCO.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static IDictionary<string, object> ToPropertyBag(this object obj)
		{
			return ObjectUtil.ToPropertyBag(obj);
		}

	}
}
