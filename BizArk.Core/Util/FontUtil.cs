using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Provides helper methods for working with fonts.
	/// </summary>
	public static class FontUtil
	{
		private static PrivateFontCollection sCustomFamilies = new PrivateFontCollection();

		/// <summary>
		/// Register a font from a byte array. Useful when embedding fonts as a resource in your project.
		/// </summary>
		/// <param name="font"></param>
		public static void RegisterFont(byte[] font)
		{
			var buffer = Marshal.AllocCoTaskMem(font.Length);
			Marshal.Copy(font, 0, buffer, font.Length);
			sCustomFamilies.AddMemoryFont(buffer, font.Length);
		}

		/// <summary>
		/// Register a font from a file.
		/// </summary>
		/// <param name="fontFilePath"></param>
		public static void RegisterFont(string fontFilePath)
		{
			sCustomFamilies.AddFontFile(fontFilePath);
		}

		/// <summary>
		/// Returns all of the installed and custom font families that are available.
		/// </summary>
		/// <returns></returns>
		public static FontFamily[] GetAllFamilies()
		{
			var families = new List<FontFamily>();

			families.AddRange(FontFamily.Families);
			families.AddRange(sCustomFamilies.Families);

			families.Sort((f1, f2) => { return f1.Name.CompareTo(f2.Name); });
			return families.ToArray();
		}

		/// <summary>
		/// Gets the FontFamily based on the name.
		/// </summary>
		/// <param name="familyName"></param>
		/// <returns></returns>
		public static FontFamily GetFamily(string familyName)
		{
			try
			{
				return new FontFamily(familyName, sCustomFamilies);
			}
			catch (ArgumentException)
			{
				// Not a private font, use installed font.
				return new FontFamily(familyName);
			}
		}

		/// <summary>
		/// Creates a font. Caller is responsible for disposing of the font. Will create any installed font or custom fonts that were registered with this class.
		/// </summary>
		/// <param name="familyName"></param>
		/// <param name="emSize"></param>
		/// <param name="style"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		public static Font Create(string familyName, float emSize, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Pixel)
		{
			var family = GetFamily(familyName);
			return new Font(family, emSize, style, unit);
		}
	}
}
