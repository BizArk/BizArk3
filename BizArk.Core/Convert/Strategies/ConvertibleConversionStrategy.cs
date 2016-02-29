using System;
using BizArk.Core.Extensions.TypeExt;
using System.Globalization;

namespace BizArk.Core.Convert.Strategies
{
	/// <summary>
	/// Uses the IConvertible interface to convert the value.
	/// </summary>
	public class ConvertibleConversionStrategy
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
			IConvertible convertible = value as IConvertible;
			if (convertible == null) return false;

			var cto = GetTrueType(to);
			var provider = CultureInfo.CurrentCulture;

			try
			{
				if (cto == typeof(bool))
				{
					convertedValue = convertible.ToBoolean(provider);
					return true;
				}

				if (cto == typeof(char))
				{
					convertedValue = convertible.ToChar(provider);
					return true;
				}

				if (cto == typeof(sbyte))
				{
					convertedValue = convertible.ToSByte(provider);
					return true;
				}

				if (cto == typeof(byte))
				{
					convertedValue = convertible.ToByte(provider);
					return true;
				}

				if (cto == typeof(short))
				{
					convertedValue = convertible.ToInt16(provider);
					return true;
				}

				if (cto == typeof(ushort))
				{
					convertedValue = convertible.ToUInt16(provider);
					return true;
				}

				if (cto == typeof(int))
				{
					convertedValue = convertible.ToInt32(provider);
					return true;
				}

				if (cto == typeof(uint))
				{
					convertedValue = convertible.ToUInt32(provider);
					return true;
				}

				if (cto == typeof(long))
				{
					convertedValue = convertible.ToInt64(provider);
					return true;
				}

				if (cto == typeof(ulong))
				{
					convertedValue = convertible.ToUInt64(provider);
					return true;
				}

				if (cto == typeof(float))
				{
					convertedValue = convertible.ToSingle(provider);
					return true;
				}

				if (cto == typeof(double))
				{
					convertedValue = convertible.ToDouble(provider);
					return true;
				}

				if (cto == typeof(decimal))
				{
					convertedValue = convertible.ToDecimal(provider);
					return true;
				}

				if (cto == typeof(DateTime))
				{
					convertedValue = convertible.ToDateTime(provider);
					return true;
				}

				if (cto == typeof(string))
				{
					convertedValue = convertible.ToString(provider);
					return true;
				}

				convertedValue = convertible.ToType(cto, provider);
				return true;
			}
			catch (Exception) { } // Just keep trying other strategies.

			return false;
		}

		/// <summary>
		/// Handles nullable types.
		/// </summary>
		/// <param name="to"></param>
		/// <returns></returns>
		private static Type GetTrueType(Type to)
		{
			// check for Nullable enums. 
			// Null values should be handled by DefaultValueConversionStrategy, but we need to be able
			// to get the actual type of the enum here.
			if (to.IsDerivedFromGenericType(typeof(Nullable<>)))
				return to.GetGenericArguments()[0];
			else
				return to;
		}

	}
}
