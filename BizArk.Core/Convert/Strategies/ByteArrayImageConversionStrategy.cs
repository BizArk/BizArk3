using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BizArk.Core.Convert.Strategies
{
	/// <summary>
	/// Uses the IConvertible interface to convert the value.
	/// </summary>
	public class ByteArrayImageConversionStrategy
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

			try
			{
				if (from == typeof(byte[]))
				{
					var imgBytes = value as byte[];
					if (imgBytes == null) return false;

					MemoryStream ms = new MemoryStream(imgBytes, 0, imgBytes.Length);
					ms.Write(imgBytes, 0, imgBytes.Length);
					convertedValue = Image.FromStream(ms, true);
					return true;
				}

				var img = value as Image;
				if (img == null) return false;

				using (MemoryStream ms = new MemoryStream())
				{
					if (img.RawFormat.Equals(ImageFormat.MemoryBmp))
						img.Save(ms, ImageFormat.Bmp);
					else
						img.Save(ms, img.RawFormat);
					convertedValue = ms.ToArray();
					return true;
				}
			}
			catch (Exception) { } // Ignore exceptions. Keep looking for strategies.
			return false;
		}

	}
}
