using System;
using System.Diagnostics;
using NUnit.Framework;
using BizArk.Core.Util;
using BizArk.Core.Extensions.FormatExt;
using My = BizArk.Core.Tests.Properties;
using System.Collections.Generic;

namespace BizArk.Core.Tests
{

	[TestFixture]
	public class StringTemplateTests
	{

		[Test]
		public void EvalTemplateTest()
		{
			var obj = new { Name = "Jane" };

			var template = new StringTemplate("Hello {name}");
			Assert.AreEqual("Hello World", template.Format(new { name = "World" }));

			template = new StringTemplate("{greeting} {name}");
			Assert.AreEqual("Hello World", template.Format(new { greeting = "Hello", name = "World" }));

			var value = "Hello {name}".Tmpl(new { name = "World" });
			Assert.AreEqual("Hello World", value);
		}

		[Test]
		public void FormLetterTest()
		{
			var letter = new Letter();
			letter.Greeting = "Hello";
			letter.Recipient.Name = "Johnny";
			letter.Sent = DateTime.Parse("3/8/2016");
			letter.Revision = 12345;

			var formLetter = new StringTemplate(My.Resources.FormLetter);
			var value = formLetter.Format(letter);
			Debug.WriteLine(value);
			Assert.IsTrue(value.Contains("Hello Johnny 5.0,"));
		}

		[Test]
		public void DateFormatTest()
		{
			var template = new StringTemplate("Test date: {date:d}");
			var dt = new DateTime(2000, 1, 1, 12, 0, 0);
			var actual = template.Format(new { date = dt });
			Assert.AreEqual(string.Format("Test date: {0:d}", dt), actual);
		}

		[Test]
		[Ignore("Not an actual test (no asserts). Can also take a long time to run.")]
		public void PerfTest()
		{
			var count = 1000000;
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < count; i++)
				string.Format("Hello {0}: {1,2}", "World", i);
			sw.Stop();
			var stringFormatTime = sw.Elapsed;

			sw.Reset();
			sw.Start();
			var template = new StringTemplate("Hello {name}: {num,2}");
			var values = new { name = "World", num = 0 };
			for (int i = 0; i < count; i++)
			{
				template.Format(values);
			}
			sw.Stop();
			var textTemplateTime = sw.Elapsed;

			sw.Reset();
			sw.Start();
			var str = "Hello {name}: {num}";
			for (int i = 0; i < count; i++)
			{
				var str2 = str.Replace("{name}", "World");
				str2 = str.Replace("{num}", i.ToString());
			}
			sw.Stop();
			var stringReplaceTime = sw.Elapsed;

			Console.WriteLine($"String.Format: {stringFormatTime.TotalMilliseconds:N0}");
			Console.WriteLine($"TextTemplate.ToString: {textTemplateTime.TotalMilliseconds:N0}");
			Console.WriteLine($"String.Replace: {stringReplaceTime.TotalMilliseconds:N0}");
			Console.WriteLine($"Diff: {textTemplateTime.TotalMilliseconds / stringFormatTime.TotalMilliseconds}");
		}

		[Test]
		public void DictionaryTest()
		{
			var dict = new Dictionary<string, object>();
			dict.Add("Name", "World");
			dict.Add("Greeting", "Hello");

			var template = new StringTemplate("{Greeting} {Name}");
			var actual = template.Format(dict);
			Assert.AreEqual("Hello World", actual);

			template = new StringTemplate("{Test.Greeting} {Test.Name}");
			var args = new { Test = dict };
			actual = template.Format(args);
			Assert.AreEqual("Hello World", actual);
		}

		public class Letter
		{

			public Letter()
			{
				Recipient = new Person();
			}

			public string Greeting { get; set; }
			public int Revision { get; set; }
			public DateTime Sent { get; set; }
			public Person Recipient { get; private set; }
			public int[] Numbers { get; set; }

		}

		public class Person
		{
			public string Name { get; set; }
			public int[] Numbers { get; } = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };

		}

	}
}
