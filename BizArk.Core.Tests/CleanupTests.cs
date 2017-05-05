using System;
using BizArk.Core.Util;
using NUnit.Framework;

namespace BizArk.Core.Tests
{


	/// <summary>
	/// Tests for the Cleanup class.
	///</summary>
	[TestFixture]
	public class CleanupTest
	{

		[Test]
		public void BasicCleanupTest()
		{
			AssertEx.Throws<ArgumentNullException>(() =>
			{
				using (new Cleanup(null)) { }
			});

			var called = false;
			using (new Cleanup(() => { called = true; })) { }
			Assert.IsTrue(called);
		}

	}
}
