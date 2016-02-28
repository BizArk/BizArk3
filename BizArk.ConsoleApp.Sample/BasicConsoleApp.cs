using BizArk.Core.Extensions.FormatExt;
using System;

namespace BizArk.ConsoleApp.Sample
{

	/// <summary>
	/// This class demonstrates a very basic console application without using any attributes.
	/// </summary>
	public class BasicConsoleApp : BaseConsoleApp
	{

		public string Name { get; set; }

		public DateTime BirthDate { get; set; }

		public override int Start()
		{
			var today = DateTime.Today;
			if (BirthDate.Month == today.Month && BirthDate.Day == today.Day)
			{
				BaCon.WriteLine("Happy birthday {0}!!!".Fmt(Name), ConsoleColor.Red);
			}
			else
			{
				var next = BirthDate.AddYears(today.Year - BirthDate.Year);
				if (next < today)
					next = next.AddYears(1);
				int numDays = (next - today).Days;
				BaCon.WriteLine("Only {0} days until your birthday!".Fmt(numDays));
			}

			return 0; // 0 for success.
		}

	}
}
