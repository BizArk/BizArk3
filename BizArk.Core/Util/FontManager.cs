using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace BizArk.Core.Util
{

	/// <summary>
	/// A light wrapper around PrivateFontCollection to make it slightly easier to work with.
	/// </summary>
	public class FontManager : IDisposable
	{

		/**
		 * Custom fonts aren't supported on Linux.
		 * https://github.com/dotnet/corefx/issues/30326
		 */

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of FontManager.
		/// </summary>
		public FontManager()
		{
		}

		/// <summary>
		/// Releases font resources. In Linux, this will remove any fonts added to ~/.fonts.
		/// </summary>
		public void Dispose()
		{
			sCustomFamilies.Dispose();
		}

		#endregion

		#region Fields and Methods

		private PrivateFontCollection sCustomFamilies = new PrivateFontCollection();

		#endregion

		#region Methods

		/// <summary>
		/// Register a font from a byte array. Useful when embedding fonts as a resource in your project.
		/// </summary>
		/// <param name="font"></param>
		public void RegisterFont(byte[] font)
		{
			var buffer = Marshal.AllocCoTaskMem(font.Length);
			Marshal.Copy(font, 0, buffer, font.Length);
			sCustomFamilies.AddMemoryFont(buffer, font.Length);
		}

		/// <summary>
		/// Register a font from a file.
		/// </summary>
		/// <param name="fontFilePath"></param>
		public void RegisterFont(string fontFilePath)
		{
			sCustomFamilies.AddFontFile(fontFilePath);
		}

		/// <summary>
		/// Returns all of the installed and custom font families that are available.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<FontFamily> GetAllFamilies()
		{
			return FontFamily.Families
					.Concat(sCustomFamilies.Families)
					.OrderBy(f => f.Name);
		}

		/// <summary>
		/// Gets the FontFamily based on the name. Prioritizes custom fonts over installed fonts.
		/// </summary>
		/// <param name="familyName"></param>
		/// <returns></returns>
		public FontFamily GetFamily(string familyName)
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
		/// Creates the requested font. Will create any installed or custom fonts that were registered with this class. Prioritizes custom fonts over installed fonts. Caller is responsible for disposing of the font.
		/// </summary>
		/// <param name="familyName"></param>
		/// <param name="emSize"></param>
		/// <param name="style"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		public Font CreateFont(string familyName, float emSize, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Pixel)
		{
			var family = GetFamily(familyName);
			return new Font(family, emSize, style, unit);
		}

		#endregion

	}
}
