using System;

namespace BizArk.Core.Convert.Strategies
{
	/// <summary>
	/// Strategy used to do no conversion at all. Just returns the value that was sent in if it can be assigned to from the value.
	/// </summary>
	public class AssignableFromConversionStrategy
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
			if (to.IsAssignableFrom(from))
			{
				convertedValue = value;
				return true;
			}
			else
				return false;
		}

	}
}
