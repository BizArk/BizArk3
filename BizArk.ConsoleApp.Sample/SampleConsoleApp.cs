using BizArk.Core.Extensions.FormatExt;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BizArk.ConsoleApp.Sample
{
	[CmdLineOptions("Name")]
	public class SampleConsoleApp : BaseConsoleApp
	{

		public SampleConsoleApp()
		{
			// Setup the default values.
			HasHair = true;
			Status = MaritalStatus.Unknown;
		}

		[Required]
		[StringLength(20, MinimumLength = 2)]
		[Description("The name of the person.")]
		public string Name { get; set; }

		[Required]
		[Range(0, 120)]
		[Description("The age in years of the person.")]
		public int Age { get; set; }

		[CmdLineArg("Job", ShowInUsage = true)]
		[Description("The type of job the person has.")]
		public string Occupation { get; set; }

		[CmdLineArg(ShowInUsage = true)]
		[Description("Does the person have hair.")]
		public bool HasHair { get; set; }

		[Description("Names of the person's children.")]
		public string[] Children { get; set; }

		[CmdLineArg(ShowInUsage = true)]
		[Description("Marital status of the person.")]
		public MaritalStatus Status { get; set; }

		public override int Start()
		{
			BaCon.WriteLine($"Name={Name}", ConsoleColor.White);
			BaCon.WriteLine($"Age={Age}");
			BaCon.WriteLine($"Job={Occupation}");
			BaCon.WriteLine($"HasHair={HasHair}");
			BaCon.WriteLine($"Children={string.Join(", ", Children)}");
			BaCon.WriteLine($"Marital Status={Status}");

			return 0; // 0 for success.
		}

		public enum MaritalStatus
		{
			Single,
			Married,
			ItsComplicated,
			Unknown
		}
	}
}
