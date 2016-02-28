using System;
using BizArk.Core.Extensions.TypeExt;

namespace BizArk.Core.Convert.Strategies
{
    /// <summary>
    /// Uses the IConvertible interface to convert the value.
    /// </summary>
    public class ConvertibleConversionStrategy
        : IConvertStrategy
    {

        //private const int cEmptyIndex = 0;
        private const int cObjectIndex = 1;
        private const int cDBNullIndex = 2;
        private const int cBoolIndex = 3;
        private const int cCharIndex = 4;
        private const int cSbyteIndex = 5;
        private const int cByteIndex = 6;
        private const int cShortIndex = 7;
        private const int cUshortIndex = 8;
        private const int cIntIndex = 9;
        private const int cUintIndex = 10;
        private const int cLongIndex = 11;
        private const int cUlongIndex = 12;
        private const int cFloatIndex = 13;
        private const int cDoubleIndex = 14;
        private const int cDecimalIndex = 15;
        private const int cDateTimeIndex = 16;
        //private const int cObject2Index = 17;
        private const int cStringIndex = 18;

        private static Type[] ConvertTypes = new Type[] 
        { 
            typeof(object), // not used but needed as a place holder in the array (was Empty).
            typeof(object), 
            typeof(DBNull), 
            typeof(bool), 
            typeof(char), 
            typeof(sbyte), 
            typeof(byte), 
            typeof(short), 
            typeof(ushort), 
            typeof(int), 
            typeof(uint), 
            typeof(int), 
            typeof(uint), 
            typeof(float), 
            typeof(double), 
            typeof(decimal), 
            typeof(DateTime), 
            typeof(object), // not used but needed as a place holder in the array.
            typeof(string)
        };

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(Type from, Type to, object value, IFormatProvider provider)
        {
            var val = GetTrueValue(value);
            IConvertible convertible = val as IConvertible;
            if (convertible == null)
                throw new InvalidCastException(String.Format("Invalid cast. {0} does not implement the IConvertible interface.", value));

            var cto = GetTrueType(to);
            if (cto == ConvertTypes[cBoolIndex])
                return convertible.ToBoolean(provider);
            if (cto == ConvertTypes[cCharIndex])
                return convertible.ToChar(provider);
            if (cto == ConvertTypes[cSbyteIndex])
                return convertible.ToSByte(provider);
            if (cto == ConvertTypes[cByteIndex])
                return convertible.ToByte(provider);
            if (cto == ConvertTypes[cShortIndex])
                return convertible.ToInt16(provider);
            if (cto == ConvertTypes[cUshortIndex])
                return convertible.ToUInt16(provider);
            if (cto == ConvertTypes[cIntIndex])
                return convertible.ToInt32(provider);
            if (cto == ConvertTypes[cUintIndex])
                return convertible.ToUInt32(provider);
            if (cto == ConvertTypes[cLongIndex])
                return convertible.ToInt64(provider);
            if (cto == ConvertTypes[cUlongIndex])
                return convertible.ToUInt64(provider);
            if (cto == ConvertTypes[cFloatIndex])
                return convertible.ToSingle(provider);
            if (cto == ConvertTypes[cDoubleIndex])
                return convertible.ToDouble(provider);
            if (cto == ConvertTypes[cDecimalIndex])
                return convertible.ToDecimal(provider);
            if (cto == ConvertTypes[cDateTimeIndex])
                return convertible.ToDateTime(provider);
            if (cto == ConvertTypes[cStringIndex])
                return convertible.ToString(provider);
            if (cto == ConvertTypes[cObjectIndex])
                return value;
            return convertible.ToType(cto, provider);
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            var cfrom = GetTrueType(from);
            if (!cfrom.Implements(typeof(IConvertible))) return false;
            var cto = GetTrueType(to); 
            return CanConvertTo(cto);
        }

        /// <summary>
        /// Handles nullable types.
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        private static Type GetTrueType(Type to)
        {
            // check for Nullable enums. 
            // Null values should be handled by DefaultValueConversionStrategy, but we need to be able
            // to get the actual type of the enum here.
            if (to.IsDerivedFromGenericType(typeof(Nullable<>)))
                return to.GetGenericArguments()[0];
            else
                return to;
        }

        private static object GetTrueValue(object val)
        {
            // I'm not sure if this method is necessary. It seems that nullable values
            // are sent in with the actual value or null, not as Nullable<?>.

            if (val == null) return null;
            if (!val.GetType().IsDerivedFromGenericType(typeof(Nullable<>))) return val;

            var prop = val.GetType().GetProperty("Value");
            return prop.GetValue(val, null);
        }

        /// <summary>
        /// Determines if IConvertible can convert to the given type.
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        private static bool CanConvertTo(Type to)
        {
            foreach (var cType in ConvertTypes)
                if (to == cType) return true;
            return false;
        }

    }
}
