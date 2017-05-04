using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BizArk.ConsoleApp.CmdLineHelpGenerator;
using BizArk.ConsoleApp.Logging;
using BizArk.ConsoleApp.Parser;
using BizArk.ConsoleApp.Themes;
using BizArk.Core;
using BizArk.Core.Extensions.ExceptionExt;
using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.ConsoleApp
{

	/// <summary>
	/// Static class that provides utility methods for working with the console and BaCon apps.
	/// </summary>
	public static class BaCon
	{

		#region Initialization and Destruction

		static BaCon()
		{

			// Guess which theme we should use.
			switch (Console.BackgroundColor)
			{
				case ConsoleColor.Black:
				case ConsoleColor.Blue:
				case ConsoleColor.DarkBlue:
				case ConsoleColor.DarkCyan:
				case ConsoleColor.DarkGray:
				case ConsoleColor.DarkGreen:
				case ConsoleColor.DarkMagenta:
				case ConsoleColor.DarkRed:
					Theme = new BaConDarkTheme();
					break;
				default:
					Theme = new BaConLightTheme();
					break;
			}
		}

		#endregion

		#region Command-Line Methods

		/// <summary>
		/// Creates a console object, fills it from the default command-line arguments, then calls its Start method. Also supports Click-Once deployed URL arguments.
		/// </summary>
		/// <param name="options">Options for how to handle running the console app.</param>
		public static void Start<T>(BaConStartOptions options = null) where T : IConsoleApp, new()
		{
			if (options == null) options = new BaConStartOptions();

			IConsoleApp consoleApp = null;
			try
			{
				var results = ParseArgs<T>(options.CmdLineOptions);
				consoleApp = results.Args;

				if (results.Title.HasValue())
					WriteLine(results.Title, Theme.AppTitleColor);
				if (results.Description.HasValue())
					WriteLine(results.Description, Theme.AppDescriptionColor);
				if (results.Copyright.HasValue())
					WriteLine(results.Copyright.Replace("©", "(c)"), Theme.AppDescriptionColor); // The © doesn't show up correctly in the console.

				WriteLine(); // Put a line between the title and rest of the information.

				// Validate only after checking to see if they requested help
				// in order to prevent displaying errors when they request help.
				if (consoleApp.Help || results.Errors.Length > 0)
				{
					WriteHelp(results);
					if (!consoleApp.Help && results.Errors.Length > 0)
						Environment.ExitCode = 1; // There were errors, return 1.
					else
						Environment.ExitCode = -1;
					return;
				}

				Environment.ExitCode = consoleApp.Start();
			}
			catch (Exception ex)
			{
				if (consoleApp == null || !consoleApp.Error(ex))
				{
					WriteError(ex);
					Environment.ExitCode = -1;
				}
			}
			finally
			{
				if (consoleApp != null && consoleApp.Wait)
				{
					WriteLine(options.PressAnyKey);
					Console.ReadKey();
				}

				// Dispose of the app if it is disposable.
				(consoleApp as IDisposable)?.Dispose();
			}
		}

		/// <summary>
		/// Parses the default command-line arguments, retrieved from Environment.GetCommandLineArgs() (ignores the first argument which is the path). Also supports Click-Once deployed URL arguments.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="options"></param>
		/// <returns></returns>
		public static CmdLineParseResults<T> ParseArgs<T>(CmdLineOptions options = null) where T : new()
		{
			var clickOnceUrl = Application.ClickOnceUrl;
			if (clickOnceUrl.HasValue())
			{
				return ParseArgs<T>(clickOnceUrl);
			}
			else
			{
				var args = Environment.GetCommandLineArgs();
				args = args.Skip(1).ToArray(); // MSDN: The first element in the array contains the file name of the executing program. If the file name is not available, the first element is equal to String.Empty. The remaining elements contain any additional tokens entered on the command line.
				return ParseArgs<T>(args, options);
			}
		}

		/// <summary>
		/// Parses the supplied command-line arguments.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="args"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static CmdLineParseResults<T> ParseArgs<T>(IEnumerable<string> args, CmdLineOptions options = null) where T : new()
		{
			var parser = new CmdLineParser<T>(options);
			return parser.Parse(args);
		}

		/// <summary>
		/// Parses a querystring as command-line arguments. Useful for Click-Once apps that are started from a URL.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryString"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static CmdLineParseResults<T> ParseArgs<T>(string queryString, CmdLineOptions options = null) where T : new()
		{
			var args = QueryStringToArgs(queryString, options);
			return ParseArgs<T>(args, options);
		}

		#endregion

		#region Console Colors

		/// <summary>
		/// Gets or sets the theme. If not set, a theme will be selected based on the background color of the console.
		/// </summary>
		public static IBaConTheme Theme { get; set; }

		#endregion

		#region Command-Line Help Methods

		/// <summary>
		/// Displays the help for the command-line object.
		/// </summary>
		/// <param name="results"></param>
		public static void WriteHelp(CmdLineParseResults results)
		{
			var generator = ClassFactory.CreateObject<HelpGenerator>(results);
			generator.WriteHelp(results);
		}

		/// <summary>
		/// Gets or sets the title used above the error message.
		/// </summary>
		public static string ErrorMessageTitle { get; set; } = "An error has occurred.";

		/// <summary>
		/// Displays the exception to the console.
		/// </summary>
		/// <param name="ex"></param>
		public static void WriteError(Exception ex)
		{
			var curEx = ex;
			if (curEx == null) return;

			if (ErrorMessageTitle.HasValue())
			{
				using (var clr = BaConColor.Error())
					(Out ?? Console.Out).WriteLine(ErrorMessageTitle);
			}

			while (curEx != null)
			{
				WriteLine(new string('*', ConsoleWidth));
				WriteLine("{0} - {1}".Fmt(curEx.GetType().FullName, curEx.Message), Theme.ErrorColor);
				WriteLine(curEx.StackTrace, indentN: "\t");
				WriteLine();
				curEx = curEx.InnerException;
			}
		}

		#endregion

		#region Console WriteLine Methods

		/// <summary>
		/// Gets or sets the output stream. If null, defaults to Console.Out.
		/// </summary>
		public static TextWriter Out { get; set; }

		private static int? sConsoleWidth;
		/// <summary>
		/// Gets or sets the width of the console to use when writing the line. Set to null (the default) to use Console.WindowWidth.
		/// </summary>
		public static int ConsoleWidth
		{
			get
			{
				if (sConsoleWidth == null)
				{
					try
					{
						return Console.WindowWidth - 1;
					}
					catch (System.IO.IOException ex)
					{
						// This exception is thrown if we can't get a handle to the window.
						// This happens in CSI when running from Visual Studio.
						System.Diagnostics.Debug.WriteLine(ex.Message);
						System.Diagnostics.Debug.WriteLine(ex.StackTrace);
						return int.MaxValue;
					}
				}
				else
					return sConsoleWidth.Value;
			}
			set
			{
				sConsoleWidth = value;
			}
		}

		/// <summary>
		/// Gets or sets the width of the TAB in the console. Defaults to 8. Needed for word wrap. However, it is recommended that you do not put TABs in your text since it might not look the way you want it.
		/// </summary>
		public static byte ConsoleTabWidth { get; set; } = 8;

		/***
		 * BaCon does not support Write (as opposed to WriteLine) because BaCon needs to have full control over the line
		 * that it is printing on in order for line wrapping to work correctly. 
		 */

		/// <summary>
		/// Writes the value to the console using the given options.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="color">The color to print the value in. Only works when using the Console.</param>
		/// <param name="indent1">Number of spaces to put in front of first line of text.</param>
		/// <param name="indentN">Number of spaces to put in front of following lines of text if the value must be wrapped to additional lines.</param>
		public static void WriteLine(string value = null, ConsoleColor? color = null, string indent1 = "", string indentN = "")
		{
			if (value.IsEmpty())
			{
				(Out ?? Console.Out).WriteLine();
				return;
			}

			using (var clr = new BaConColor(color ?? Console.ForegroundColor))
			{
				var displayVal = value;

				if (indent1.HasValue())
					displayVal = indent1 + displayVal;

				var options = new StringWrapOptions() { MaxWidth = ConsoleWidth, TabWidth = ConsoleTabWidth, Prefix = indentN };
				displayVal = displayVal.Wrap(options);

				(Out ?? Console.Out).WriteLine(displayVal);
			}
		}

		#endregion

		#region Command-Line utility methods

		/// <summary>
		/// Converts a query string into an argument list that the parser can understand.
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		internal static string[] QueryStringToArgs(string queryString, CmdLineOptions options)
		{
			if (queryString.IsEmpty()) return new string[] { };

			var args = new List<string>();
			var qs = queryString;

			// If there is a ? in the querystring, remove it and everything to the left.
			var idx = qs.IndexOf('?');
			if (idx >= 0)
				qs = qs.Substring(idx + 1);

			// If there is a URL fragment, remove it.
			idx = qs.LastIndexOf('#');
			if (idx >= 0)
				qs = qs.Substring(0, idx);

			var pairs = qs.Split('&');
			foreach (var vp in pairs)
			{
				string name;
				string value;
				idx = vp.IndexOf('=');
				if (idx < 0) // Flag option.
				{
					name = vp.Trim();
					value = null; // Just add the flag.
				}
				else
				{
					name = vp.Substring(0, idx).Trim();
					value = vp.Substring(idx + 1);
					value = Uri.UnescapeDataString(value);
					value = value.Replace('+', ' '); // Uri.UnescapeDataString does not unescape +.
				}

				if (name.HasValue())
				{
					args.Add(options.ArgumentPrefix + name);
					if (value != null)
						args.Add(value);
				}
			}

			return args.ToArray();
		}

		#endregion

		#region Logging Methods

		/// <summary>
		/// Gets the log adapters to use when writing log messages. Useful for writing log messages to something other than the console.
		/// </summary>
		public static List<BaConLogger> Loggers { get; } = new List<BaConLogger>();

		/// <summary>
		/// Use the BaConConsoleLogger. Sets the min level.
		/// </summary>
		/// <param name="minLevel"></param>
		/// <returns></returns>
		public static T UseLogger<T>(BaConLogLevel minLevel = BaConLogLevel.Trace) where T : BaConLogger, new()
		{
			var logger = FindLogger<T>();
			if (logger == null)
			{
				logger = new T();
				Loggers.Add(logger);
			}
			logger.MinLogLevel = minLevel;
			return logger;
		}

		/// <summary>
		/// Use the BaConConsoleLogger. Sets the min level.
		/// </summary>
		/// <param name="minLevel"></param>
		/// <returns></returns>
		public static BaConFileLogger UseFileLogger(string filePath, BaConLogLevel minLevel = BaConLogLevel.Trace)
		{
			var logger = FindFileLogger(filePath);
			if (logger == null || logger.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
			{
				logger = new BaConFileLogger(filePath);
				Loggers.Add(logger);
			}
			logger.MinLogLevel = minLevel;
			return logger;
		}

		public static BaConFileLogger FindFileLogger(string filePath)
		{
			foreach (var logger in Loggers)
			{
				var fileLogger = logger as BaConFileLogger;
				if (fileLogger?.FilePath == filePath)
					return fileLogger;
			}

			// The logger type wasn't found.
			return null;
		}

		public static T FindLogger<T>() where T : BaConLogger
		{
			foreach (var logger in Loggers)
			{
				if (logger.GetType() == typeof(T))
					return (T)logger;
			}

			// The logger type wasn't found.
			return null;
		}

		private static void WriteLogMessage(BaConLogLevel level, string msg, Exception ex)
		{
			if (Loggers.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine("No log adapters are set. Logging to Debug. Add loggers to BaCon.Loggers");
				if (msg.HasValue())
					System.Diagnostics.Debug.WriteLine(msg);
				if (ex != null)
					System.Diagnostics.Debug.WriteLine(ex.GetDetails());
				return;
			}

			foreach (var logger in Loggers)
			{
				if (logger.ShouldWriteLogMessage(level, msg, ex))
					logger.WriteLogMessage(level, msg, ex);
			}

		}

		/// <summary>
		/// Most verbose logging level.
		/// </summary>
		/// <param name="msg">Message to display.</param>
		/// <param name="ex">Exception to display.</param>
		public static void Trace(string msg, Exception ex = null)
		{
			WriteLogMessage(BaConLogLevel.Trace, msg, ex);
		}

		/// <summary>
		/// Debug messages useful for identifying basically what is happening at a somewhat verbose level.
		/// </summary>
		/// <param name="msg">Message to display.</param>
		/// <param name="ex">Exception to display.</param>
		public static void Debug(string msg, Exception ex = null)
		{
			WriteLogMessage(BaConLogLevel.Debug, msg, ex);
		}

		/// <summary>
		/// Basic information. Good for important events that should be logged.
		/// </summary>
		/// <param name="msg">Message to display.</param>
		/// <param name="ex">Exception to display.</param>
		public static void Info(string msg, Exception ex = null)
		{
			WriteLogMessage(BaConLogLevel.Info, msg, ex);
		}

		/// <summary>
		/// Just a warning to the user.
		/// </summary>
		/// <param name="msg">Message to display.</param>
		/// <param name="ex">Exception to display.</param>
		public static void Warn(string msg, Exception ex = null)
		{
			WriteLogMessage(BaConLogLevel.Warn, msg, ex);
		}

		/// <summary>
		/// An error has occurred, but the application is able to continue.
		/// </summary>
		/// <param name="msg">Message to display.</param>
		/// <param name="ex">Exception to display.</param>
		public static void Error(string msg, Exception ex = null)
		{
			WriteLogMessage(BaConLogLevel.Error, msg, ex);
		}

		/// <summary>
		/// A fatal error has occurred and execution of the application is ending.
		/// </summary>
		/// <param name="msg">Message to display.</param>
		/// <param name="ex">Exception to display.</param>
		public static void Fatal(string msg, Exception ex = null)
		{
			WriteLogMessage(BaConLogLevel.Fatal, msg, ex);
		}

		#endregion

	}

}
