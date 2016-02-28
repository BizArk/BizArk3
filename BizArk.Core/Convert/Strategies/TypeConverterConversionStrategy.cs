using System;
using System.ComponentModel;

namespace BizArk.Core.Convert.Strategies
{
    /// <summary>
    /// Uses a TypeConverter to perform a conversion.
    /// </summary>
    public class TypeConverterConversionStrategy
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
            var converter = TypeDescriptor.GetConverter(from);
            if (converter != null && converter.CanConvertTo(to))
                return converter.ConvertTo(value, to);

            converter = TypeDescriptor.GetConverter(to);
            if (converter != null && converter.CanConvertFrom(from))
                return converter.ConvertFrom(value);

            throw new InvalidOperationException(string.Format("Cannot convert from {0} to {1} using the TypeConverterConversionStrategy.", from.Name, to.Name));
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            var converter = TypeDescriptor.GetConverter(from);
            if (converter != null && converter.CanConvertTo(to)) return true;

            converter = TypeDescriptor.GetConverter(to);
            if (converter != null && converter.CanConvertFrom(from)) return true;

            return false;
        }

    }
}
