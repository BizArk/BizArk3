using NUnit.Framework;
using BizArk.Core.Extensions.AttributeExt;

namespace BizArk.Core.Tests
{


	/// <summary>
	///This is a test class for FormatExtTest and is intended
	///to contain all FormatExtTest Unit Tests
	///</summary>
	[TestFixture]
	public class AttributeExtTests
	{

		/// <summary>
		///A test for Fmt
		///</summary>
		[Test]
		public void GetDescriptionTest()
		{
			Assert.AreEqual("First", Test.One.GetDescription());
			Assert.AreEqual("Second", Test.Two.GetDescription());
		}

		public enum Test
		{
			[System.ComponentModel.Description("First")]
			One,
			[System.ComponentModel.Description("Second")]
			Two
		}

	}
}
