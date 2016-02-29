using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using BizArk.Core.Convert;
using BizArk.Core.Extensions.TypeExt;

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
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static bool ToBool(object value)
        {
            if (value == null) return default(bool);
            return ConvertEx.To<bool>(value);
        }

        /// <summary>
        /// Converts the value to a Char. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static char ToChar(object value)
        {
            if (value == null) return default(Char);
            return ConvertEx.To<Char>(value);
        }

        /// <summary>
        /// Converts the value to a SByte. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static sbyte ToSByte(object value)
        {
            if (value == null) return default(SByte);
            return ConvertEx.To<SByte>(value);
        }

        /// <summary>
        /// Converts the value to a Byte. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static byte ToByte(object value)
        {
            if (value == null) return default(Byte);
            return ConvertEx.To<Byte>(value);
        }

        /// <summary>
        /// Converts the value to a Int16. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static short ToShort(object value)
        {
			if (value == null) return default(Int16);
			return ConvertEx.To<Int16>(value);
		}

		/// <summary>
		/// Converts the value to a UInt16. 
		/// Checks for a TypeConverter, conversion methods, 
		/// and the IConvertible interface.
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static ushort ToUShort(object value)
        {
            if (value == null) return default(ushort);
            return ConvertEx.To<ushort>(value);
        }
		
        /// <summary>
        /// Converts the value to a Int32. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static int ToInt(object value)
        {
			if (value == null) return default(int);
			return ConvertEx.To<int>(value);
		}

		/// <summary>
		/// Converts the value to a UInt32. 
		/// Checks for a TypeConverter, conversion methods, 
		/// and the IConvertible interface.
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static uint ToUInt(object value)
        {
            if (value == null) return default(uint);
            return ConvertEx.To<uint>(value);
        }

        /// <summary>
        /// Converts the value to a Int64. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static long ToLong(object value)
        {
			if (value == null) return default(Int64);
			return ConvertEx.To<Int64>(value);
		}

		/// <summary>
		/// Converts the value to a UInt64. 
		/// Checks for a TypeConverter, conversion methods, 
		/// and the IConvertible interface.
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		public static ulong ToULong(object value)
        {
            if (value == null) return default(UInt64);
            return ConvertEx.To<ulong>(value);
        }

        /// <summary>
        /// Converts the value to a Single. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static float ToSingle(object value)
        {
            if (value == null) return default(Single);
            return ConvertEx.To<Single>(value);
        }

        /// <summary>
        /// Converts the value to a Single. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static float ToFloat(object value)
        {
            return ToSingle(value);
        }

        /// <summary>
        /// Converts the value to a Double. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static double ToDouble(object value)
        {
            if (value == null) return default(Double);
            return ConvertEx.To<Double>(value);
        }

        /// <summary>
        /// Converts the value to a Decimal. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static decimal ToDecimal(object value)
        {
            if (value == null) return default(Decimal);
            return ConvertEx.To<Decimal>(value);
        }

        /// <summary>
        /// Converts the value to a DateTime. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static DateTime ToDateTime(object value)
        {
            if (value == null) return default(DateTime);
            return ConvertEx.To<DateTime>(value);
        }

        /// <summary>
        /// Converts the value to a String. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
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
					sConverter = new BaConverter();
				return sConverter;
			}
		}

		/// <summary>
		/// Converts the value to the specified type. 
		/// Checks for a TypeConverter, conversion methods, 
		/// and the IConvertible interface. This is an alias 
		/// to ChangeType.
		/// </summary>
		/// <typeparam name="T">The type to convert to.</typeparam>
		/// <param name="value">The value to convert from.</param>
		/// <returns></returns>
		/// <remarks>
		/// <para>The ChangeType method converts a value to another type.</para>
		/// <para>It can use a number of different conversion techniques depending on
		/// what is most appropriate based on the type of the value and the type it
		/// is converting to. The following lists explains the order that the checks 
		/// are made in.
		/// <list type="">
		/// <item>String to Boolean - Used when we are converting from a string to a boolean. Valid values for true are "true", "t", "yes", "1", and "-1", everything else is false.</item>
		/// <item>TypeConverter - Used when a TypeConverter exists for either the type we are converting to or from that can convert to the other type.</item>
		/// <item>Parse method - Used when the type we are converting from is a string and the type we are converting to defines a static, parameterless Parse method that returns the type we are converting to.</item>
		/// <item>Convert method - Used when the type we are converting from defines an instance method called ToXXX where XXX is the name of the type we are converting to with some common aliases allowed (example ToBool or ToInt instead of ToBoolean and ToInt32). The method must return the type we are converting to</item>
		/// <item>IConvertible - Used when the type we are converting from implements the IConvertible interface.</item>
		/// </list>
		/// </para>
		/// <para>This method makes use of the strategy pattern for determining how to 
		/// convert values. To define a custom strategy to convert from one type to 
		/// another, define a class that implements the IConvertStrategy interface and
		/// register it with the ConvertStrategyMgr class.</para>
		/// </remarks>
		/// <exception cref="System.InvalidCastException">This conversion is not supported. -or-value is null and conversionType is a value type.</exception>
		/// <exception cref="System.ArgumentNullException">conversionType is null.</exception>
		public static T To<T>(object value)
        {			
			return (T)To(value, typeof(T), null);
		}

		/// <summary>
		/// Converts the value to the specified type. 
		/// Checks for a TypeConverter, conversion methods, 
		/// and the IConvertible interface.
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		/// <param name="to">The type to convert to.</param>
		/// <param name="provider">The IFormatProvider to use for the conversion.</param>
		/// <returns></returns>
		/// <remarks>
		/// <para>The ChangeType method converts a value to another type.</para>
		/// <para>It can use a number of different conversion techniques depending on
		/// what is most appropriate based on the type of the value and the type it
		/// is converting to. The following lists explains the order that the checks 
		/// are made in.
		/// <list type="">
		/// <item>String to Boolean - Used when we are converting from a string to a boolean. Valid values for true are "true", "t", "yes", "1", and "-1", everything else is false.</item>
		/// <item>TypeConverter - Used when a TypeConverter exists for either the type we are converting to or from that can convert to the other type.</item>
		/// <item>Parse method - Used when the type we are converting from is a string and the type we are converting to defines a static, parameterless Parse method that returns the type we are converting to.</item>
		/// <item>Convert method - Used when the type we are converting from defines an instance method called ToXXX where XXX is the name of the type we are converting to with some common aliases allowed (example ToBool or ToInt instead of ToBoolean and ToInt32). The method must return the type we are converting to</item>
		/// <item>IConvertible - Used when the type we are converting from implements the IConvertible interface.</item>
		/// </list>
		/// </para>
		/// <para>This method makes use of the strategy pattern for determining how to 
		/// convert values. To define a custom strategy to convert from one type to 
		/// another, define a class that implements the IConvertStrategy interface and
		/// register it with the ConvertStrategyMgr class.</para>
		/// </remarks>
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

        #endregion

        #region IsEmpty

        /// <summary>
        /// Checks to see if the value is empty. The value is empty if it is null, DBNull, or matches the MinValue, MaxValue, or Empty fields of the values type.
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
        /// <returns></returns>
        public static T GetDefaultEmptyValue<T>()
        {
			return (T)Converter.GetDefaultEmptyValue(typeof(T));
		}

		/// <summary>
		/// Gets the default value that represents empty for the given type.
		/// </summary>
		/// <returns></returns>
		public static object GetDefaultEmptyValue(Type type)
        {
			return Converter.GetDefaultEmptyValue(type);
        }

        #endregion

    }
}
