using System;

namespace BizArk.Core.Convert.Strategies
{
    /// <summary>
    /// Strategy used to return the default value for a type;
    /// </summary>
    public class NullValueConversionStrategy
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
			if (value == null)
			{
				convertedValue = ConvertEx.GetDefaultEmptyValue(to);
				return true;
			}
			else
			{
				convertedValue = null;
				return false;
			}
		}

    }
}
