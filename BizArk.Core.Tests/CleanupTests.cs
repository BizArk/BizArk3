using BizArk.Core.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BizArk.Core.Tests
{
	
	/// <summary>
	/// Tests for the Cleanup class.
	///</summary>
	[TestClass]
	public class CleanupTest
	{

		[TestMethod]
		public void BasicCleanupTest()
		{
			Assert.That.Throws<ArgumentNullException>(() =>
			{
				using (new Cleanup(null)) { }
			});

			var called = false;
			using (new Cleanup(() => { called = true; })) { }
			Assert.IsTrue(called);
		}

	}
}
