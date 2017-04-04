using System;

namespace BizArk.Core.Convert.Strategies
{
	/// <summary>
	/// Converts from a string to a bool.
	/// </summary>
	public class StringToBoolConversionStrategy
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
			convertedValue = false;
			if (to != typeof(bool)) return false;

			var str = value as string;
			if (str == null) return false;
			str = str.Trim();

			int i;
			if (int.TryParse(str, out i))
			{
				convertedValue = i != 0; // only false if i == 0.
				return true;
			}

			foreach (var trueVal in TrueValues)
			{
				if (trueVal.Equals(str, StringComparison.InvariantCultureIgnoreCase))
				{
					convertedValue = true;
					return true;
				}
			}

			convertedValue = false;
			return true;
		}

		/// <summary>
		/// Gets the list of values that will equate to True. Everything else is false.
		/// </summary>
		public string[] TrueValues { get; set; } = new string[] { "true", "t", "yes", "ok", "aye", "yep", "yea" };

	}
}
