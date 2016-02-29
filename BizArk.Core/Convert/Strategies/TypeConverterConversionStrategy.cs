using System;
using System.ComponentModel;

namespace BizArk.Core.Convert.Strategies
{
	/// <summary>
	/// Uses a TypeConverter to perform a conversion.
	/// </summary>
	public class TypeConverterConversionStrategy
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

			var converter = TypeDescriptor.GetConverter(from);
			if (converter != null && converter.CanConvertTo(to))
			{
				try
				{
					convertedValue = converter.ConvertTo(value, to);
					return true;
				}
				catch (Exception) { } // Ignore exceptions. Just keep trying other strategies.
			}

			converter = TypeDescriptor.GetConverter(to);
			if (converter != null && converter.CanConvertFrom(from))
			{
				try
				{
					convertedValue = converter.ConvertFrom(value);
					return true;
				}
				catch (Exception) { } // Ignore exceptions. Just keep trying other strategies.
			}

			return false;
		}

	}
}
