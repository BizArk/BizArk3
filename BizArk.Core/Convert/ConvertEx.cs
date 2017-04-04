using System;
using BizArk.Core.Convert;

namespace BizArk.Core
{
	/// <summary>
	/// This class provides the ability to convert types 
	/// beyond what is provided by the System.Convert
	/// class.
	/// </summary>
	public class ConvertEx
	{

		#region ToXxx

		/// <summary>
		/// Converts the value to a Boolean. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static bool ToBool(object value)
		{
			if (value == null) return default(bool);
			return ConvertEx.To<bool>(value);
		}

		/// <summary>
		/// Converts the value to a Char. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static char ToChar(object value)
		{
			if (value == null) return default(Char);
			return ConvertEx.To<Char>(value);
		}

		/// <summary>
		/// Converts the value to a SByte. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static sbyte ToSByte(object value)
		{
			if (value == null) return default(SByte);
			return ConvertEx.To<SByte>(value);
		}

		/// <summary>
		/// Converts the value to a Byte. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static byte ToByte(object value)
		{
			if (value == null) return default(Byte);
			return ConvertEx.To<Byte>(value);
		}

		/// <summary>
		/// Converts the value to a Int16. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static short ToShort(object value)
		{
			if (value == null) return default(Int16);
			return ConvertEx.To<Int16>(value);
		}

		/// <summary>
		/// Converts the value to a UInt16. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static ushort ToUShort(object value)
		{
			if (value == null) return default(ushort);
			return ConvertEx.To<ushort>(value);
		}

		/// <summary>
		/// Converts the value to a Int32. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static int ToInt(object value)
		{
			if (value == null) return default(int);
			return ConvertEx.To<int>(value);
		}

		/// <summary>
		/// Converts the value to a UInt32. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static uint ToUInt(object value)
		{
			if (value == null) return default(uint);
			return ConvertEx.To<uint>(value);
		}

		/// <summary>
		/// Converts the value to a Int64. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static long ToLong(object value)
		{
			if (value == null) return default(Int64);
			return ConvertEx.To<Int64>(value);
		}

		/// <summary>
		/// Converts the value to a UInt64. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static ulong ToULong(object value)
		{
			if (value == null) return default(UInt64);
			return ConvertEx.To<ulong>(value);
		}

		/// <summary>
		/// Converts the value to a Single. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static float ToSingle(object value)
		{
			if (value == null) return default(Single);
			return ConvertEx.To<Single>(value);
		}

		/// <summary>
		/// Converts the value to a Single. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static float ToFloat(object value)
		{
			return ToSingle(value);
		}

		/// <summary>
		/// Converts the value to a Double. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static double ToDouble(object value)
		{
			if (value == null) return default(Double);
			return ConvertEx.To<Double>(value);
		}

		/// <summary>
		/// Converts the value to a Decimal. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static decimal ToDecimal(object value)
		{
			if (value == null) return default(Decimal);
			return ConvertEx.To<Decimal>(value);
		}

		/// <summary>
		/// Converts the value to a DateTime. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static DateTime ToDateTime(object value)
		{
			if (value == null) return default(DateTime);
			return ConvertEx.To<DateTime>(value);
		}

		/// <summary>
		/// Converts the value to a String. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static string ToString(object value)
		{
			if (value == null) return default(String);
			return ConvertEx.To<String>(value);
		}

		#endregion

		#region ChangeType

		[ThreadStatic] // Each thread gets it's own version of BaConverter so we can avoid multi-threading issues.
		private static BaConverter sConverter;

		/// <summary>
		/// According to the documentation, we can't initialize 
		/// sConverter in the class (only run once), we must
		/// initialize it in a method call.
		/// </summary>
		/// https://msdn.microsoft.com/en-us/library/system.threadstaticattribute.aspx
		private static BaConverter Converter
		{
			get
			{
				if (sConverter == null)
					sConverter = ClassFactory.CreateObject<BaConverter>();
				return sConverter;
			}
		}

		/// <summary>
		/// Converts the value to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to convert to.</typeparam>
		/// <param name="value">The value to convert from.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">This conversion is not supported. -or-value is null and conversionType is a value type.</exception>
		public static T To<T>(object value)
		{
			return (T)To(value, typeof(T), null);
		}

		/// <summary>
		/// Converts the value to the specified type. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		/// <param name="to">The type to convert to.</param>
		/// <param name="provider">The IFormatProvider to use for the conversion.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">This conversion is not supported. -or-value is null and conversionType is a value type.</exception>
		/// <exception cref="System.ArgumentNullException">conversionType is null.</exception>
		public static object To(object value, Type to, IFormatProvider provider = null)
		{
			object convertedValue;
			if (Try(value, to, out convertedValue))
				return convertedValue;

			// if we get to here, we know value cannot be null (that is the first strategy).
			throw new InvalidCastException($"Unable to convert value from {value.GetType().FullName} to {to.FullName}.");
		}

		/// <summary>
		/// Attempts to convert the value to the specified type. 
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		/// <param name="to">The type to convert to.</param>
		/// <param name="convertedValue">The converted value.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">This conversion is not supported.</exception>
		/// <exception cref="System.ArgumentNullException">to is null.</exception>
		public static bool Try(object value, Type to, out object convertedValue)
		{
			return Converter.Try(value, to, out convertedValue);
		}

		/// <summary>
		/// Attempts to convert the value to the specified type. 
		/// </summary>
		/// <typeparam name="T">The type to convert the value to.</typeparam>
		/// <param name="value">The value to convert from.</param>
		/// <param name="convertedValue">The converted value.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">This conversion is not supported.</exception>
		public static bool Try<T>(object value, out T convertedValue)
		{
			convertedValue = default(T);

			object obj;
			if (!Converter.Try(value, typeof(T), out obj))
				return false;

			convertedValue = (T)obj;
			return true;
		}

		#endregion

		#region IsEmpty

		/// <summary>
		/// Checks to see if the value is considered to have no value. Null, DBNull.Value and Min/Max values, and Empty values are considered empty.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsEmpty(object value)
		{
			return Converter.IsEmpty(value);
		}

		/// <summary>
		/// Gets the default value that represents empty for the given type.
		/// </summary>
		/// <typeparam name="T">Used to identify the type to get the default empty value for.</typeparam>
		/// <returns></returns>
		public static T GetDefaultEmptyValue<T>()
		{
			return (T)Converter.GetDefaultEmptyValue(typeof(T));
		}

		/// <summary>
		/// Gets the default value that represents empty for the given type.
		/// </summary>
		/// <param name="type">The type to get the default empty value for.</param>
		/// <returns></returns>
		public static object GetDefaultEmptyValue(Type type)
		{
			return Converter.GetDefaultEmptyValue(type);
		}

		#endregion

	}
}
