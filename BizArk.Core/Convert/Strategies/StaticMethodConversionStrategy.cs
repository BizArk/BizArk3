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
		/// <param name="value">The object to convert.</param>
		/// <param name="to">The type to convert the value to.</param>
		/// <param name="convertedValue">Return the value if converted.</param>
		/// <returns>True if able to convert the value.</returns>
		public bool TryConvert(object value, Type to, out object convertedValue)
		{
			convertedValue = null;
			if (value == null) return false;

			var from = value.GetType();
			var mi = GetStaticConvertMethod(from, to);
			if (mi == null) return false;

			try
			{
				convertedValue = mi.Invoke(null, new object[] { value });
				return true;
			}
			catch (Exception) { } // Ignore exceptions

			return false;
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
