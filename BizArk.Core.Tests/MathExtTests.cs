using NUnit.Framework;
using BizArk.Core.Extensions.MathExt;

namespace BizArk.Core.Tests
{


	/// <summary>
	///This is a test class for FormatExtTest and is intended
	///to contain all FormatExtTest Unit Tests
	///</summary>
	[TestFixture]
	public class MathExtTests
	{

		[Test]
		public void BetweenTest()
		{
			var val = 5.Between(1, 10);
			Assert.AreEqual(5, val);

			val = 5.Between(1, 5);
			Assert.AreEqual(5, val);

			val = 5.Between(5, 10);
			Assert.AreEqual(5, val);

			val = 10.Between(1, 5);
			Assert.AreEqual(5, val);

			val = 1.Between(5, 10);
			Assert.AreEqual(5, val);
		}

		[Test]
		public void IsBetweenTest()
		{
			Assert.IsTrue(5.IsBetween(1, 10));

			Assert.IsTrue(5.IsBetween(1, 5));

			Assert.IsTrue(5.IsBetween(5, 10));

			Assert.IsFalse(10.IsBetween(1, 5));

			Assert.IsFalse(1.IsBetween(5, 10));
		}

	}
}
