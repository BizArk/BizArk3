using System;
using System.Collections.Generic;
using System.Reflection;

namespace BizArk.Core.Convert.Strategies
{
    /// <summary>
    /// Uses a conversion method to convert the value.
    /// </summary>
    public class ConvertMethodConversionStrategy
        : IConvertStrategy
    {

        static ConvertMethodConversionStrategy()
        {
            // Some types have commonly used aliases. These can be used for the ToXXX conversion methods.
            sTypeAliases = new Dictionary<Type, string[]>();
            sTypeAliases.Add(typeof(bool), new string[] { "Bool" });
            sTypeAliases.Add(typeof(int), new string[] { "Integer", "Int" });
            sTypeAliases.Add(typeof(short), new string[] { "Short" });
            sTypeAliases.Add(typeof(ushort), new string[] { "UShort" });
            sTypeAliases.Add(typeof(uint), new string[] { "UInt" });
            sTypeAliases.Add(typeof(long), new string[] { "Long" });
            sTypeAliases.Add(typeof(ulong), new string[] { "ULong" });
            sTypeAliases.Add(typeof(float), new string[] { "Float" });
        }

        private static Dictionary<Type, string[]> sTypeAliases;

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
            var mi = GetInstanceConvertMethod(from, to);
            return mi.Invoke(value, null);
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            return (GetInstanceConvertMethod(from, to) != null);
        }

        private static MethodInfo GetInstanceConvertMethod(Type fromType, Type toType)
        {
            var names = new List<string>();
            names.Add(toType.Name);
            if (sTypeAliases.ContainsKey(toType))
                names.AddRange(sTypeAliases[toType]);

            // Find the convert method.
            // Iterate through each of the alias' to find a method that
            // can be used to convert the value.
            foreach (string name in names)
            {
                MethodInfo mi = fromType.GetMethod("To" + name, new Type[] { });
                if (mi == null) continue;
                if (mi.ReturnType != toType) continue;
                return mi;
            }

            return null;
        }

    }
}
