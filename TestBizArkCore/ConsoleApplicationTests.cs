using System;
using BizArk.Core.CmdLine;
using NUnit.Framework;

namespace BizArk.Core.Tests
{
	[TestFixture]
	public class ConsoleApplicationTests
	{
		private static void ExecuteConsoleProgram(AttributeCmdLineObject args)
		{
		}

		private class AttributeCmdLineObject : CmdLineObject
		{
			[CmdLineArg(Required = true)]
			public int Count { get; set; }
		}

		[Test]
		public void RunProgram_NotValidCmdLineObject_ExitCodeMinusOne()
		{
			ConsoleApplication.RunProgram<AttributeCmdLineObject>(ExecuteConsoleProgram);

			Assert.That(Environment.ExitCode, Is.EqualTo(-1));
		}

        private class HavingNameCommandLineObject : CmdLineObject
        {
            [CmdLineArg]
            public string Name { get; set; }
        }


        [Test]
        public void RunProgram_ExceptionThrown_ExitCodeMinusOne()
        {
            ConsoleApplication.RunProgram<HavingNameCommandLineObject>(a => { throw new Exception(); });

            Assert.That(Environment.ExitCode, Is.EqualTo(1));
        }
	}
}