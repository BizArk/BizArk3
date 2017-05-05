using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.ConsoleApp.Themes
{

	/// <summary>
	/// Provides the ability to modify the colors used by BaCon when displaying generated text to the screen.
	/// </summary>
	public class BaConTheme
	{

		#region Default Themes

		private static BaConTheme sLightTheme;
		/// <summary>
		/// Gets the light theme.
		/// </summary>
		public static BaConTheme LightTheme
		{
			get
			{
				if (sLightTheme == null)
				{
					sLightTheme = new BaConTheme()
					{
						AppTitleColor = ConsoleColor.Black,
						AppDescriptionColor = ConsoleColor.DarkGray,
						ErrorColor = ConsoleColor.Red,
						UsageColor = ConsoleColor.DarkYellow,
						RequiredArgColor = ConsoleColor.Black,
						StandardArgColor = ConsoleColor.DarkGray,
						LogTraceColor = ConsoleColor.Gray,
						LogDebugColor = ConsoleColor.DarkGray,
						LogInfoColor = ConsoleColor.Black,
						LogWarnColor = ConsoleColor.DarkMagenta,
						LogErrorColor = ConsoleColor.Magenta,
						LogFatalColor = ConsoleColor.Red
					};
				}

				return sLightTheme;
			}
		}

		private static BaConTheme sDarkTheme;
		/// <summary>
		/// Gets the dark theme.
		/// </summary>
		public static BaConTheme DarkTheme
		{
			get
			{
				if (sDarkTheme == null)
				{
					sDarkTheme = new BaConTheme()
					{
						AppTitleColor = ConsoleColor.White,
						AppDescriptionColor = ConsoleColor.Gray,
						ErrorColor = ConsoleColor.Red,
						UsageColor = ConsoleColor.Yellow,
						RequiredArgColor = ConsoleColor.White,
						StandardArgColor = ConsoleColor.Gray,
						LogTraceColor = ConsoleColor.DarkGray,
						LogDebugColor = ConsoleColor.Gray,
						LogInfoColor = ConsoleColor.White,
						LogWarnColor = ConsoleColor.Magenta,
						LogErrorColor = ConsoleColor.Yellow,
						LogFatalColor = ConsoleColor.Red
					};
				}

				return sDarkTheme;
			}
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets or sets the color used to display the app title. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? AppTitleColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display the app description. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? AppDescriptionColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display the error title text. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? ErrorTitleText { get; set; }

		/// <summary>
		/// Gets or sets the color used to display the error title background. A null will use the default console background color.
		/// </summary>
		public ConsoleColor? ErrorTitleBackground { get; set; }

		/// <summary>
		/// Gets or sets the color used to display the error message. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? ErrorColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display the usage description. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? UsageColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display a required argument. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? RequiredArgColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display a non-required argument. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? StandardArgColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display trace messages.
		/// </summary>
		public ConsoleColor? LogTraceColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display debug messages.
		/// </summary>
		public ConsoleColor? LogDebugColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display info messages.
		/// </summary>
		public ConsoleColor? LogInfoColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display warning messages.
		/// </summary>
		public ConsoleColor? LogWarnColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display error messages.
		/// </summary>
		public ConsoleColor? LogErrorColor { get; set; }

		/// <summary>
		/// Gets or sets the color used to display fatal messages.
		/// </summary>
		public ConsoleColor? LogFatalColor { get; set; }

		#endregion

	}
}
