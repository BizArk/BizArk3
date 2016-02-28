using System;
using System.Reflection;

namespace BizArk.Core.Convert.Strategies
{

    /// <summary>
    /// Uses a typed constructor to convert the value.
    /// </summary>
    public class StaticMethodConversionStrategy
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
            var mi = GetStaticConvertMethod(from, to);
            return mi.Invoke(null, new object[] { value });
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            return (GetStaticConvertMethod(from, to) != null);
        }

        private static MethodInfo GetStaticConvertMethod(Type from, Type to)
        {
            var mi = GetStaticConvertMethod(from, from, to);
            if (mi != null) return mi;
            return GetStaticConvertMethod(to, from, to);
        }

        private static MethodInfo GetStaticConvertMethod(Type typeToSearch, Type from, Type to)
        {
            // essentially looks for overloaded operators (or methods that looks like an operator).

            foreach (var method in typeToSearch.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                if (method.ReturnType != to) continue;

                var parameters = method.GetParameters();
                if (parameters.Length != 1) continue;
                if (parameters[0].ParameterType != from) continue;

                return method;
            }

            return null;
        }

    }
}
