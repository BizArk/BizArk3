using System;
using System.Globalization;
using BizArk.Core.Extensions.TypeExt;

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
			if (!(value is IConvertible)) return false;

			var cto = GetTrueType(to);
			var provider = CultureInfo.CurrentCulture;

			try
			{
				if (cto.Implements(typeof(IConvertible)))
				{
					convertedValue = System.Convert.ChangeType(value, cto, provider);
					return true;
				}
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
