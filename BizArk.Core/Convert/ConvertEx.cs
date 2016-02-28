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
    public static class ConvertEx
    {

        #region ToXxx

        /// <summary>
        /// Converts the value to a Boolean. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static bool ToBoolean(object value)
        {
            if (value == null) return default(bool);
            return ConvertEx.ChangeType<bool>(value);
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
            return ConvertEx.ChangeType<Char>(value);
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
            return ConvertEx.ChangeType<SByte>(value);
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
            return ConvertEx.ChangeType<Byte>(value);
        }

        /// <summary>
        /// Converts the value to a Int16. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static short ToInt16(object value)
        {
            if (value == null) return default(Int16);
            return ConvertEx.ChangeType<Int16>(value);
        }

        /// <summary>
        /// Converts the value to a Int16. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static short ToShort(object value)
        {
            return ToInt16(value);
        }

        /// <summary>
        /// Converts the value to a UInt16. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static ushort ToUInt16(object value)
        {
            if (value == null) return default(ushort);
            return ConvertEx.ChangeType<ushort>(value);
        }

        /// <summary>
        /// Converts the value to a Int32. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static int ToInt32(object value)
        {
            if (value == null) return default(int);
            return ConvertEx.ChangeType<int>(value);
        }

        /// <summary>
        /// Converts the value to a Int32. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static int ToInt(object value)
        {
            return ToInt32(value);
        }

        /// <summary>
        /// Converts the value to a Int32. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static int ToInteger(object value)
        {
            return ToInt32(value);
        }

        /// <summary>
        /// Converts the value to a UInt32. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static uint ToUInt32(object value)
        {
            if (value == null) return default(UInt32);
            return ConvertEx.ChangeType<UInt32>(value);
        }

        /// <summary>
        /// Converts the value to a Int64. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static long ToInt64(object value)
        {
            if (value == null) return default(Int64);
            return ConvertEx.ChangeType<Int64>(value);
        }

        /// <summary>
        /// Converts the value to a Int64. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static long ToLong(object value)
        {
            return ToInt64(value);
        }

        /// <summary>
        /// Converts the value to a UInt64. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
        /// </summary>
        /// <param name="value">The value to convert from.</param>
        public static ulong ToUInt64(object value)
        {
            if (value == null) return default(UInt64);
            return ConvertEx.ChangeType<UInt64>(value);
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
            return ConvertEx.ChangeType<Single>(value);
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
            return ConvertEx.ChangeType<Double>(value);
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
            return ConvertEx.ChangeType<Decimal>(value);
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
            return ConvertEx.ChangeType<DateTime>(value);
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
            return ConvertEx.ChangeType<String>(value);
        }

        #endregion

        #region ChangeType

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
            return (T)ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Converts the value to the specified type. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface.
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
        public static T ChangeType<T>(object value)
        {
            return (T)ChangeType(value, typeof(T));
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
        public static object ChangeType(object value, Type to, IFormatProvider provider = null)
        {
            if (to == null)
                throw new ArgumentNullException("conversionType");

            Type from;
            if (value == null)
                from = null;
            else if (value == DBNull.Value)
                from = null;
            else
                from = value.GetType();

            if (provider == null) provider = Thread.CurrentThread.CurrentCulture;

            // This method uses the strategy pattern to convert values. The ConvertStrategyMgr handles
            // the creation of strategy patterns. 

            var strategy = ConvertStrategyMgr.GetStrategy(from, to);
            if(strategy == null)
                throw new InvalidCastException(String.Format("Invalid cast. Cannot convert from {0} to {1}.", from, to));
            return strategy.Convert(from, to, value, provider);
        }

        #endregion

        #region IsEmpty

        private static object sEmptyValuesLock = new object();
        private static Dictionary<Type, List<object>> sEmptyValues = new Dictionary<Type, List<object>>();
        private static object sRegisteredDefaultEmptyValuesLock = new object();
        private static List<Type> sRegisteredDefaultEmptyValues = new List<Type>();

        /// <summary>
        /// Checks to see if the value is empty. The value is empty if it is null, DBNull, or matches the MinValue, MaxValue, or Empty fields of the values type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty(object value)
        {
            if (value == null) return true;
            if (value == DBNull.Value) return true;

            Type type = value.GetType();
            RegisterDefaultEmptyValues(type);

            lock (sEmptyValuesLock)
            {
                foreach (var emptyVal in sEmptyValues[type])
                {
                    if (value.Equals(emptyVal))
                        return true;
                }
            }
            return false;
        }

        private static object GetStaticFieldValue(Type type, string fieldName)
        {
            var prop = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (prop == null) return null;
            return prop.GetValue(type);
        }

        /// <summary>
        /// Register a value that will be interpreted as an empty value for IsEmpty.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="emptyValue"></param>
        public static void RegisterEmptyValue(Type type, object emptyValue)
        {
            if (type == null) return;

            lock (sEmptyValuesLock)
            {
                if (!sEmptyValues.ContainsKey(type))
                    sEmptyValues.Add(type, new List<object>());

                if (!sEmptyValues[type].Contains(emptyValue))
                    sEmptyValues[type].Add(emptyValue);
            }
        }

        /// <summary>
        /// Registers the default empty values for this type for use in IsEmpty.
        /// </summary>
        /// <param name="type"></param>
        private static void RegisterDefaultEmptyValues(Type type)
        {
            if (type == null) return;

            lock (sRegisteredDefaultEmptyValuesLock)
            {
                // check to see if the default values have already been registered. Can only register once.
                if (sRegisteredDefaultEmptyValues.Contains(type)) return;

                sRegisteredDefaultEmptyValues.Add(type);
            }

            lock (sEmptyValuesLock)
            {
                // some types might not have any "default" empty values (such as bool)
                if (!sEmptyValues.ContainsKey(type))
                    sEmptyValues.Add(type, new List<object>());
            }

            if (type == typeof(char))
            {
                RegisterEmptyValue(type, '\0');
            }
            else
            {
                object emptyVal;
                emptyVal = GetStaticFieldValue(type, "MinValue");
                if (emptyVal != null) RegisterEmptyValue(type, emptyVal);
                emptyVal = GetStaticFieldValue(type, "MaxValue");
                if (emptyVal != null) RegisterEmptyValue(type, emptyVal);
                emptyVal = GetStaticFieldValue(type, "Empty");
                if (emptyVal != null) RegisterEmptyValue(type, emptyVal);
            }

        }

        /// <summary>
        /// Gets the default value that represents empty for the given type.
        /// </summary>
        /// <returns></returns>
        public static T GetDefaultEmptyValue<T>()
        {
            return (T)GetDefaultEmptyValue(typeof(T));
        }

        /// <summary>
        /// Gets the default value that represents empty for the given type.
        /// </summary>
        /// <returns></returns>
        public static object GetDefaultEmptyValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            lock (sEmptyValuesLock)
            {
                // Get the first registered empty value.
                if (sEmptyValues.ContainsKey(type) && sEmptyValues[type].Count > 0)
                    return sEmptyValues[type][0];
            }

            if (type.IsNullable())
                return null;
            else if (type == typeof(char))
                return '\0';
            else if (type == typeof(bool))
                return false;
            else if (type.IsEnum)
            {
                var fields = type.GetFields();
                if (fields.Length > 1)
                    return fields[1].GetValue(type);
                else
                    return 0;
            }
            else
            {
                object emptyVal;
                emptyVal = GetStaticFieldValue(type, "Empty");
                if (emptyVal != null) return emptyVal;
                emptyVal = GetStaticFieldValue(type, "MinValue");
                if (emptyVal != null) return emptyVal;
            }

            // Should only reach here if we have an unhandled type.
            throw new ArgumentException(string.Format("Unable to determine default empty value for {0}.", type.FullName), "type");
        }

        /// <summary>
        /// Removes all the custom default empty values.
        /// </summary>
        public static void ResetEmptyValues()
        {
            sEmptyValues.Clear();
            sRegisteredDefaultEmptyValues.Clear();
        }

        #endregion

    }
}
