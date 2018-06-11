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
		private static Dictionary<string, FontFamily> sFamilies = new Dictionary<string, FontFamily>(StringComparer.InvariantCultureIgnoreCase);

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
		/// <param name="family"></param>
		/// <returns></returns>
		public static FontFamily GetFamily(string family)
		{
			try
			{
				// cache the family in a dictionary for fast lookup.
				if (!sFamilies.ContainsKey(family))
					sFamilies.Add(family, new FontFamily(family, sCustomFamilies));
			}
			catch (ArgumentException)
			{
				// Not a private font, use installed font.
				sFamilies.Add(family, new FontFamily(family));
			}
			return sFamilies[family];
		}

		/// <summary>
		/// Creates a font. Caller is responsible for disposing of the font. Will create any installed font or custom fonts that were registered with this class.
		/// </summary>
		/// <param name="family"></param>
		/// <param name="emSize"></param>
		/// <param name="style"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		public static Font Create(string family, float emSize, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Pixel)
		{
			var fam = GetFamily(family);
			return new Font(fam, emSize, FontStyle.Regular, GraphicsUnit.Point);
			//return new Font(family, emSize, style, unit);
		}
	}
}
