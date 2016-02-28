using System;

namespace BizArk.Core.Convert.Strategies
{
    /// <summary>
    /// Strategy used to return the default value for a type;
    /// </summary>
    public class DefaultValueConversionStrategy
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
            if (to == null) return null;
            return ConvertEx.GetDefaultEmptyValue(to);
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            if (to == null) return true;
            if (from == null) return true;
            return false;
        }

    }
}
