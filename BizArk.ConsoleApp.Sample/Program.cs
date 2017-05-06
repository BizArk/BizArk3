using System;
using System.ComponentModel;
using BizArk.ConsoleApp.Logging;

namespace BizArk.ConsoleApp.Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			/* Add a custom logger.
			BaCon.Loggers.Add(new MyFancyLogger());
			*/

			/* Recommended approach for building console applications. */
			// command-line: /Name "Billy Bob" /Age 23 /Occupation "Chicken Catcher" /HasHair /Children Billy Bob Sue /Status ItsComplicated /W
			BaCon.Start<SampleConsoleApp>();

			/* A simpler example for building console applications.
			// command-line: /Name "Billy Bob" /BirthDate 7/19/1999 /W
			BaCon.Start<BasicConsoleApp>();
			*/

			/* Just want a good command-line parser to populate your POCO? Here you go.
			// command-line: /Name "Billy Bob" /Age 23 /Occupation "Chicken Catcher" /HasHair /Children Billy Bob Sue /Status ItsComplicated /W
			// ParseArgs actually returns validation information and other information as well, if you want it.
			var results = BaCon.ParseArgs<BasicArgs>();
			var myargs = results.Args;
			// BaCon.WriteHelp(results); // Displays help if you want to.
			Console.WriteLine(myargs.Name);
			Console.ReadKey(true);
			*/
		}
	}

	public class MyFancyLogger : BaConLogger
	{
		public override void WriteLogMessage(BaConLogLevel level, string msg, Exception ex)
		{
			Console.WriteLine(msg);
		}
	}

	/// <summary>
	/// Exit codes. BaCon will automatically find an enum called "ExitCodes" and display it in help.
	/// If you name it something other than ExitCodes you will need to set the CmdLineOptions.ExitCodes
	/// property. BaConStartOptions.CmdLineOptions.ExitCodes = typeof(MyExitCodes);
	/// </summary>
	public enum ExitCodes
	{

		/// <summary>
		/// There should always be an exit code for success and it's value should always be 0.
		/// </summary>
		[Description("The sample ran successfully.")]
		Success = 0,

		/// <summary>
		/// BaCon will automatically find the enum value called "InvalidArgs" and 
		/// use it for BaConStartOptions.CmdLineOptions.InvalidArgsExitCode
		/// </summary>
		[Description("Invalid command-line arguments.")]
		InvalidArgs = -1234,

		/// <summary>
		/// BaCon will automatically find the enum value called "FatalError" and 
		/// use it for BaConStartOptions.CmdLineOptions.FatalErrorExitCode
		/// </summary>
		[Description("A fatal error has occurred.")]
		FatalError = -9876
	}

}
