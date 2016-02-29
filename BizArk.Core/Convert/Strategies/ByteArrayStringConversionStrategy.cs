using System;
using System.Text;

namespace BizArk.Core.Convert.Strategies
{

	/// <summary>
	/// Converts a string to a byte[] (and vice-versa).
	/// </summary>
	public class ByteArrayStringConversionStrategy
		: IConvertStrategy
	{

		/// <summary>
		/// Gets the encoding to use for converting the value.
		/// </summary>
		public static Encoding Encoding { get; set; } = Encoding.Default;

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

			if (from == typeof(byte[]))
			{
				var strBytes = value as byte[];
				if (strBytes == null) return false;

				convertedValue = Encoding.GetString(strBytes);
				return true;
			}

			var str = value as string;
			if (str == null) return false;

			convertedValue = Encoding.GetBytes(str);
			return true;
		}

	}
}
