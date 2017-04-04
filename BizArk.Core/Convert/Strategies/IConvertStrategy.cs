using System;

namespace BizArk.Core.Convert.Strategies
{
	/// <summary>
	/// Interface for defining conversion strategies. Used in ConvertEx. Each strategy object should be used to convert from exactly one type to another.
	/// </summary>
	public interface IConvertStrategy
	{

		/// <summary>
		/// Changes the type of the value.
		/// </summary>
		/// <param name="value">The object to convert.</param>
		/// <param name="to">The type to convert the value to.</param>
		/// <param name="convertedValue">Return the value if converted.</param>
		/// <returns>True if able to convert the value.</returns>
		bool TryConvert(object value, Type to, out object convertedValue);

	}
}
