using System;

namespace BizArk.Core.Convert.Strategies
{
    /// <summary>
    /// Strategy used to do no conversion at all. Just returns the value that was sent in.
    /// </summary>
    public class NoConvertConversionStrategy
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
            return value;
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            if (to.IsAssignableFrom(from)) 
                return true;
            else
                return false;
        }

    }
}
