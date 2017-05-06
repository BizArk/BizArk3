using BizArk.ConsoleApp.CmdLineHelpGenerator;
using BizArk.ConsoleApp.Parser;
using BizArk.Core;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using cm = System.ComponentModel;

namespace BizArk.ConsoleApp.Tests
{
	[TestFixture]
	public class CmdLineHelpTest
	{

		[Test]
		public void GenerateUsageTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var results = parser.Parse(new string[] { });
			results.ApplicationFileName = "test.exe";
			var generator = new HelpGenerator(results);

			var usage = generator.GetUsage();
			Assert.AreEqual("test.exe <Name|String> [/Type <Father|Mother|Child>]", usage);
		}

		[Test]
		public void GeneratePropHelpTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var results = parser.Parse(new string[] { });
			var generator = new HelpGenerator(results);

			var help = generator.GetPropertyHelp(results.Properties["Name"]);
			AssertEx.Contains("/Name <String> REQUIRED", help);
			AssertEx.Contains("\tTEST DESC", help);

			// Type doesn't have a description, but the flag is set to show it.
			help = generator.GetPropertyHelp(results.Properties["Type"]);
			AssertEx.Contains("/PersonType (/Type) <Father|Mother|Child>", help);
			AssertEx.Contains("\tDefault: Father", help);

			help = generator.GetPropertyHelp(results.Properties["Children"]);
			AssertEx.Contains("/Children", help);
			AssertEx.Contains("\tDefault: [\"One\", \"Two\", \"Three\"]", help);
		}

		[Test]
		public void GenerateExitCodesTest()
		{
			// No exit codes have been defined.
			var parser = new CmdLineParser<TestCmdLineObj>();
			var results = parser.Parse(new string[] { });
			parser.Options.ExitCodes = null;
			var generator = new HelpGenerator(results);
			var help = generator.GetExitCodesDisplay();
			Assert.AreEqual("", help);


			// Verify the parser can find the ExitCodes enum and InvalidArgs value.
			parser = new CmdLineParser<TestCmdLineObj>();
			results = parser.Parse(new string[] { });
			generator = new HelpGenerator(results);

			help = generator.GetExitCodesDisplay();
			Assert.AreEqual("   0 = Success!!!\r\n1234 = Failed :'(\r\n", help);
			Assert.AreEqual((int)ExitCodes.InvalidArgs, parser.Options.InvalidArgsExitCode);
			Assert.IsNull(parser.Options.FatalErrorExitCode);
		}

		#region TestCmdLineObj

		[CmdLineOptions(DefaultArgNames = new string[] { "Name", "Job" })]
		private class TestCmdLineObj
		{

			public TestCmdLineObj()
			{
				PersonType = PersonType.Father;
				Children = new string[] { "One", "Two", "Three" };
			}

			[Required(ErrorMessage = "TEST ERR")]
			[cm.Description("TEST DESC")]
			public string Name { get; set; }

			public int Age { get; set; }

			[CmdLineArg("Job")]
			[StringLength(10, ErrorMessage = "TEST ERR")]
			public string Occupation { get; set; }

			public bool HasHair { get; set; }

			public string[] Children { get; set; }

			[CmdLineArg("Type", ShowInUsage = true)]
			public PersonType PersonType { get; set; }
		}

		private enum PersonType
		{
			Father,
			Mother,
			Child
		}

		private enum ExitCodes
		{
			[System.ComponentModel.Description("Success!!!")]
			Success = 0,
			[System.ComponentModel.Description("Failed :'(")]
			InvalidArgs = 1234
		}

		#endregion

	}
}
