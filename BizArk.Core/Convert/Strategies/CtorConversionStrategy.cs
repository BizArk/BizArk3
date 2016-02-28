using System;
using System.Reflection;

namespace BizArk.Core.Convert.Strategies
{

    /// <summary>
    /// Uses a typed constructor to convert the value.
    /// </summary>
    public class CtorConversionStrategy
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
            var ctor = GetCtor(from, to);
            return ctor.Invoke(new object[] { value });
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            return (GetCtor(from, to) != null);
        }

        private static ConstructorInfo GetCtor(Type from, Type to)
        {
            return to.GetConstructor(new Type[] { from });
        }

    }
}
