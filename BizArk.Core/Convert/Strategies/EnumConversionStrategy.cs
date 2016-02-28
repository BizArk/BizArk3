using System;
using BizArk.Core.Extensions.TypeExt;

namespace BizArk.Core.Convert.Strategies
{
    /// <summary>
    /// Converts to enumeration values.
    /// </summary>
    public class EnumConversionStrategy
        : IConvertStrategy
    {

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
            var cfrom = from.GetTrueType();
            var cto = to.GetTrueType();
            var val = GetTrueValue(value);

            if (cto.IsEnum)
            {
                string s = val as string;
                if (s != null) return Enum.Parse(cto, s);
                return Enum.ToObject(cto, val);
            }
            else if (cfrom.IsEnum)
                return System.Convert.ChangeType(val, cto);
            else
                throw new InvalidCastException(string.Format("Cannot convert from {0} to {1}.", from.Name, to.Name));

        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            var cfrom = from.GetTrueType();
            var cto = to.GetTrueType();

            if (cfrom.IsEnum && (cto.IsNumericType() || cto == typeof(string))) return true;
            if (cto.IsEnum && (cfrom.IsNumericType() || cfrom == typeof(string))) return true;

            return false;
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

    }
}
