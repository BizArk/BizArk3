using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.ConsoleApp
{
	/// <summary>
	/// Provides an easy to use mechanism to change the color of the console, and then put it back when you are done.
	/// </summary>
	public class BaConColor : IDisposable
	{

		#region Intialization and Destruction

		/// <summary>
		/// Creates a new instance of BaConColor.
		/// </summary>
		/// <param name="foreground"></param>
		/// <param name="background"></param>
		public BaConColor(ConsoleColor? foreground, ConsoleColor? background = null)
		{
			OrigForegroundColor = Console.ForegroundColor;
			ForegroundColor = foreground ?? Console.ForegroundColor;
			Console.ForegroundColor = ForegroundColor;

			OrigBackgroundColor = Console.BackgroundColor;
			BackgroundColor = background ?? Console.BackgroundColor;
			Console.BackgroundColor = BackgroundColor;
		}

		/// <summary>
		/// Sets the Console.ForegroundColor back to the original color.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Console.ForegroundColor = OrigForegroundColor;
			Console.BackgroundColor = OrigBackgroundColor;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the foreground color of the console before changing it.
		/// </summary>
		public ConsoleColor OrigForegroundColor { get; private set; }

		/// <summary>
		/// Gets the foreground console color being used.
		/// </summary>
		public ConsoleColor ForegroundColor { get; private set; }

		/// <summary>
		/// Gets the background color of the console before changing it.
		/// </summary>
		public ConsoleColor OrigBackgroundColor { get; private set; }

		/// <summary>
		/// Gets the background console color being used.
		/// </summary>
		public ConsoleColor BackgroundColor { get; private set; }

		#endregion

		#region Colors

		/// <summary>
		/// Changes the console text to Black.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Black()
		{
			return new BaConColor(ConsoleColor.Black);
		}

		/// <summary>
		/// Changes the console text to DarkBlue.
		/// </summary>
		/// <returns></returns>
		public static BaConColor DarkBlue()
		{
			return new BaConColor(ConsoleColor.DarkBlue);
		}

		/// <summary>
		/// Changes the console text to DarkGreen.
		/// </summary>
		/// <returns></returns>
		public static BaConColor DarkGreen()
		{
			return new BaConColor(ConsoleColor.DarkGreen);
		}

		/// <summary>
		/// Changes the console text to DarkCyan.
		/// </summary>
		/// <returns></returns>
		public static BaConColor DarkCyan()
		{
			return new BaConColor(ConsoleColor.DarkCyan);
		}

		/// <summary>
		/// Changes the console text to DarkRed.
		/// </summary>
		/// <returns></returns>
		public static BaConColor DarkRed()
		{
			return new BaConColor(ConsoleColor.DarkRed);
		}

		/// <summary>
		/// Changes the console text to DarkMagenta.
		/// </summary>
		/// <returns></returns>
		public static BaConColor DarkMagenta()
		{
			return new BaConColor(ConsoleColor.DarkMagenta);
		}

		/// <summary>
		/// Changes the console text to DarkYellow.
		/// </summary>
		/// <returns></returns>
		public static BaConColor DarkYellow()
		{
			return new BaConColor(ConsoleColor.DarkYellow);
		}

		/// <summary>
		/// Changes the console text to Gray.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Gray()
		{
			return new BaConColor(ConsoleColor.Gray);
		}

		/// <summary>
		/// Changes the console text to DarkGray.
		/// </summary>
		/// <returns></returns>
		public static BaConColor DarkGray()
		{
			return new BaConColor(ConsoleColor.DarkGray);
		}

		/// <summary>
		/// Changes the console text to Blue.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Blue()
		{
			return new BaConColor(ConsoleColor.Blue);
		}

		/// <summary>
		/// Changes the console text to Green.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Green()
		{
			return new BaConColor(ConsoleColor.Green);
		}

		/// <summary>
		/// Changes the console text to Cyan.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Cyan()
		{
			return new BaConColor(ConsoleColor.Cyan);
		}

		/// <summary>
		/// Changes the console text to Red.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Red()
		{
			return new BaConColor(ConsoleColor.Red);
		}

		/// <summary>
		/// Changes the console text to Magenta.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Magenta()
		{
			return new BaConColor(ConsoleColor.Magenta);
		}

		/// <summary>
		/// Changes the console text to Yellow.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Yellow()
		{
			return new BaConColor(ConsoleColor.Yellow);
		}

		/// <summary>
		/// Changes the console text to White.
		/// </summary>
		/// <returns></returns>
		public static BaConColor White()
		{
			return new BaConColor(ConsoleColor.White);
		}

		/// <summary>
		/// Reverses the current foreground and background colors.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Reverse()
		{
			return new BaConColor(Console.BackgroundColor, Console.ForegroundColor);
		}

		/// <summary>
		/// Changes the background color to yellow and the foreground to black.
		/// </summary>
		/// <returns></returns>
		public static BaConColor Highlight()
		{
			return new BaConColor(ConsoleColor.Black, ConsoleColor.Yellow);
		}

		#endregion

	}
}
