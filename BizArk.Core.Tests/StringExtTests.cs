using System;
using BizArk.Core.Extensions.StringExt;
using NUnit.Framework;

namespace BizArk.Core.Tests
{
	/// <summary>
	///This is a test class for StringExtTest and is intended
	///to contain all StringExtTest Unit Tests
	///</summary>
	[TestFixture]
	public class StringExtTests
	{

		[Test]
		public void WrapTest()
		{
			string expected;
			string actual;

			actual = "".Wrap(5);
			Assert.AreEqual("", actual);

			actual = ((string)null).Wrap(5);
			Assert.AreEqual("", actual);

			actual = "hi".Wrap(5);
			Assert.AreEqual("hi", actual);

			expected = "This\r\nis my\r\nline.";
			actual = "This is my line.".Wrap(5);
			Assert.AreEqual(expected, actual);

			expected = "ThisI\r\nsMyLi\r\nne.";
			actual = "ThisIsMyLine.".Wrap(5);
			Assert.AreEqual(expected, actual);

			expected = "hello\r\nhello";
			actual = "hellohello".Wrap(5);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void WrapWithPrefixTest()
		{
			var options = new StringWrapOptions() { MaxWidth = 10, TabWidth = 4, Prefix = " >> " };
			var expected = "This is a\r\n >> test\r\n >> of the\r\n >> emerge\r\n >> ncy\r\n >> broadc\r\n >> ast\r\n >> system\r\n >> .";
			var actual = "This is a test of the emergency broadcast system.".Wrap(options);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void WrapWithTabTest()
		{
			var options = new StringWrapOptions() { MaxWidth = 12, TabWidth = 4 };
			var actual = "\tWhat about this?".Wrap(options);
			Assert.AreEqual("\tWhat\r\nabout this?", actual);

			options.Prefix = "\t";
			actual = "\tWhat about this?".Wrap(options);
			Assert.AreEqual("\tWhat\r\n\tabout\r\n\tthis?", actual);

			options = new StringWrapOptions() { MaxWidth = 20, TabWidth = 4 };
			actual = "Do\tsomething\twith\tinternal\ttabs.".Wrap(options);
			Assert.AreEqual("Do\tsomething\twith\r\ninternal\ttabs.", actual);

			options = new StringWrapOptions() { MaxWidth = 9, TabWidth = 8 };
			actual = "123456789\n1\t9 123456789".Wrap(options);
			Assert.AreEqual("123456789\r\n1\t9\r\n123456789", actual);
		}

		[Test]
		public void BaConErrorTest()
		{
			// This is an error we are getting in the help for the sample console app.
			// It was adding an extra, blank line to the output.

			var str = @"/Name <String> REQUIRED
	The name of the person.
	The field Name must be a string with a minimum length of 2 and a maximum length of 20.";

			var expected = Normalize(@"/Name <String> REQUIRED
	The name of the person.
	The field Name must be a string with a minimum
	length of 2 and a maximum length of 20.");

			var options = new StringWrapOptions() { MaxWidth = 59, TabWidth = 8, Prefix = "\t" };
			var actual = Normalize(str.Wrap(options));
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Makes sure all newlines are just '\n' (basically just removes '\r' from the string).
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private string Normalize(string str)
		{
			return str.Remove('\r');
		}

	}
}
