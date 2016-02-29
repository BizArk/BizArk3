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
		/// <param name="value">The object to convert.</param>
		/// <param name="to">The type to convert the value to.</param>
		/// <param name="convertedValue">Return the value if converted.</param>
		/// <returns>True if able to convert the value.</returns>
		public bool TryConvert(object value, Type to, out object convertedValue)
		{
			convertedValue = null;
			if (value == null) return false;

			if (to.IsEnum)
			{

				var str = value as string;
				if (str != null)
				{
					str = str.Trim();
					var name = GetName(to, str);
					if (name == null)
					{
						// Check to see if we can convert the string to a numeric value. Use a long 
						// since that has the best chance of catching any of the valid enum types.
						// We do this because string parsing is very common and we want to ensure we
						// can convert to the enumerated value.
						long longVal;
						if (long.TryParse(str, out longVal))
							value = longVal; // Set the value so it can be checked below.
						else
							return false;
					}
					else
					{
						convertedValue = Enum.Parse(to, name);
						return true;
					}
				}

				var enumType = Enum.GetUnderlyingType(to);
				convertedValue = ConvertEx.To(value, enumType);
				if (Enum.IsDefined(to, convertedValue))
				{
					convertedValue = Enum.ToObject(to, convertedValue);
					return true;
				}
			}

			var from = value.GetType();
			if (from.IsEnum)
			{

				if (to == typeof(string))
				{
					convertedValue = Enum.GetName(from, value);
					return true;
				}

				try
				{
					convertedValue = (value as IConvertible).ToType(to, null);
					return true;
				}
				catch (InvalidCastException) { } // Ignore exceptions, keep searching for a successful strategy.

				//if (to == typeof(int))
				//{
				//	convertedValue = (int)value;
				//	return true;
				//}
				//if (to == typeof(byte))
				//{

				//	convertedValue = (byte)value;
				//	return true;
				//}
				//if (to == typeof(long))
				//{
				//	convertedValue = (long)value;
				//	return true;
				//}
			}


			return false;
		}

		private string GetName(Type to, string value)
		{
			foreach (var name in Enum.GetNames(to))
			{
				if (name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
					return name;
			}
			return null;
		}

	}
}
