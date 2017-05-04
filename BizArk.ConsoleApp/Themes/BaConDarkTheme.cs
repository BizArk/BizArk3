using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.ConsoleApp.Themes
{

	/// <summary>
	/// Provides colors for the console that has a dark background.
	/// </summary>
	public class BaConDarkTheme : IBaConTheme
	{

		/// <summary>
		/// Gets or sets the color used to display the app title. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? AppTitleColor { get; set; } = ConsoleColor.White;

		/// <summary>
		/// Gets or sets the color used to display the app description. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? AppDescriptionColor { get; set; } = ConsoleColor.Gray;

		/// <summary>
		/// Gets or sets the color used to display the error message. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? ErrorColor { get; set; } = ConsoleColor.Red;

		/// <summary>
		/// Gets or sets the color used to display the usage description. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? UsageColor { get; set; } = ConsoleColor.Yellow;

		/// <summary>
		/// Gets or sets the color used to display a required argument. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? RequiredArgColor { get; set; } = ConsoleColor.White;

		/// <summary>
		/// Gets or sets the color used to display a non-required argument. A null will use the default console text color.
		/// </summary>
		public ConsoleColor? StandardArgColor { get; set; } = ConsoleColor.Gray;

		/// <summary>
		/// Gets or sets the color used to display trace messages.
		/// </summary>
		public ConsoleColor? LogTraceColor { get; set; } = ConsoleColor.DarkGray;

		/// <summary>
		/// Gets or sets the color used to display debug messages.
		/// </summary>
		public ConsoleColor? LogDebugColor { get; set; } = ConsoleColor.Gray;

		/// <summary>
		/// Gets or sets the color used to display info messages.
		/// </summary>
		public ConsoleColor? LogInfoColor { get; set; } = ConsoleColor.White;

		/// <summary>
		/// Gets or sets the color used to display warning messages.
		/// </summary>
		public ConsoleColor? LogWarnColor { get; set; } = ConsoleColor.Magenta;

		/// <summary>
		/// Gets or sets the color used to display error messages.
		/// </summary>
		public ConsoleColor? LogErrorColor { get; set; } = ConsoleColor.Yellow;

		/// <summary>
		/// Gets or sets the color used to display fatal messages.
		/// </summary>
		public ConsoleColor? LogFatalColor { get; set; } = ConsoleColor.Red;

	}
}
