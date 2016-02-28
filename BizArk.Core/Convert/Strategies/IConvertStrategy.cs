using System;

namespace BizArk.Core.Convert.Strategies
{
    /// <summary>
    /// Interface for defining conversion strategies. Used in ConvertEx. Each strategy object should be used to convert from exactly one type to another.
    /// </summary>
    public interface IConvertStrategy
    {

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        object Convert(Type from, Type to, object value, IFormatProvider provider);

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool CanConvert(Type from, Type to);

    }
}
