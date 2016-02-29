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
			sTypeAliases.Add(typeof(DateTime), new string[] { "Date" });
		}

		private static Dictionary<Type, string[]> sTypeAliases;

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

			try
			{
				var mi = GetInstanceConvertMethod(from, to);
				convertedValue = mi.Invoke(value, null);
				return true;
			}
			catch (Exception) { } // Ignore exceptions. Keep looking for a strategy.

			return false;
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
