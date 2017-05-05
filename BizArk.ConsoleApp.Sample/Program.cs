using System;
using BizArk.ConsoleApp.Logging;

namespace BizArk.ConsoleApp.Sample
{
	class Program
	{
static void Main(string[] args)
{
	Exception ex = null;
	try { throw new Exception("TEST"); } catch (Exception xex) { ex = xex; }
	BaCon.Trace("This is a Trace.", ex);
	BaCon.Debug("This is a Debug.", ex);
	BaCon.Info("This is a Info.", ex);
	BaCon.Warn("This is a Warn.", ex);
	BaCon.Error("This is a Error.", ex);
	BaCon.Fatal("This is a Fatal.", ex);


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

}
