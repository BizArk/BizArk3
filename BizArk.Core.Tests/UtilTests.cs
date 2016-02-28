using System;
using System.Threading;
using BizArk.Core.Util;
using NUnit.Framework;

namespace BizArk.Core.Tests
{
	[TestFixture]
	public class UtilTests
	{
		[Test]
		public void RangeIncludesTest()
		{
			var range1 = new Range<long>(0, 5);
			Assert.IsTrue(range1.Includes(3));
			Assert.IsTrue(range1.Includes(0));
			Assert.IsTrue(range1.Includes(5));
			Assert.IsFalse(range1.Includes(-1));
			Assert.IsFalse(range1.Includes(6));

			var range2 = new Range<string>("b", "f");
			Assert.IsTrue(range2.Includes("d"));
			Assert.IsTrue(range2.Includes("b"));
			Assert.IsTrue(range2.Includes("f"));
			Assert.IsFalse(range2.Includes("a"));
			Assert.IsFalse(range2.Includes("g"));
		}

		[Test, Ignore("Not sure.")]
		public void RemoteTimeTest()
		{
			var remote = new RemoteDateTime(DateTime.Now);
			AssertEx.AreClose(DateTime.Now, remote.Now, TimeSpan.FromMilliseconds(1));
			Thread.Sleep(TimeSpan.FromSeconds(5));
			DateTime now = DateTime.Now;
			DateTime remoteNow = remote.Now;
			Console.WriteLine("Now: {0} Remote: {1}", now.ToString("O"), remoteNow.ToString("O"));
			AssertEx.AreClose(now, remoteNow, TimeSpan.FromMilliseconds(100));
		}
	}
}