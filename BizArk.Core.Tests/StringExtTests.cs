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
			string str;
			int maxLineLength;
			string expected;
			string actual;

			str = "This is my line.";
			maxLineLength = 5;
			expected = "This\r\nis\r\nmy\r\nline\r\n.";
			actual = StringExt.Wrap(str, maxLineLength);
			Assert.AreEqual(expected, actual);

			str = "ThisIsMyLine.";
			maxLineLength = 5;
			expected = "ThisI\r\nsMyLi\r\nne.";
			actual = StringExt.Wrap(str, maxLineLength);
			Assert.AreEqual(expected, actual);

			str = "";
			maxLineLength = 5;
			expected = "";
			actual = StringExt.Wrap(str, maxLineLength);
			Assert.AreEqual(expected, actual);

			str = null;
			maxLineLength = 5;
			expected = "";
			actual = StringExt.Wrap(str, maxLineLength);
			Assert.AreEqual(expected, actual);

			str = "hi";
			maxLineLength = 5;
			expected = "hi";
			actual = StringExt.Wrap(str, maxLineLength);
			Assert.AreEqual(expected, actual);

			str = "hellohello";
			maxLineLength = 5;
			expected = "hello\r\nhello";
			actual = StringExt.Wrap(str, maxLineLength);
			Assert.AreEqual(expected, actual);

			str = " is not a valid project path. The file was not found.";
			maxLineLength = 80;
			expected = "hello" + Environment.NewLine + "hello";
			actual = StringExt.Wrap(str, maxLineLength, "    ");
			//Assert.AreEqual(expected, actual);
		}
	}
}
