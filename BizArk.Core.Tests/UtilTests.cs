using BizArk.Core.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;

namespace BizArk.Core.Tests
{

	[TestClass]
	public class UtilTests
	{
		[TestMethod]
		public void RangeIncludesTest()
		{
			var range1 = new Range<long>(0, 5);
			Assert.IsFalse(range1.Includes(-1));
			Assert.IsTrue(range1.Includes(0));
			Assert.IsTrue(range1.Includes(3));
			Assert.IsTrue(range1.Includes(5));
			Assert.IsFalse(range1.Includes(6));

			var range2 = new Range<string>("b", "f");
			Assert.IsFalse(range2.Includes("a"));
			Assert.IsTrue(range2.Includes("b"));
			Assert.IsTrue(range2.Includes("d"));
			Assert.IsTrue(range2.Includes("f"));
			Assert.IsFalse(range2.Includes("g"));
		}

		[TestMethod]
		[Ignore("Uses Thread.Sleep so it takes too long to run.")]
		public void RemoteTimeTest()
		{
			var remote = new RemoteDateTime(DateTime.Now);
			Assert.That.AreClose(DateTime.Now, remote.Now, TimeSpan.FromMilliseconds(10));
			Thread.Sleep(TimeSpan.FromSeconds(5));
			DateTime now = DateTime.Now;
			DateTime remoteNow = remote.Now;
			Console.WriteLine("Now: {0} Remote: {1}", now.ToString("O"), remoteNow.ToString("O"));
			Assert.That.AreClose(now, remoteNow, TimeSpan.FromMilliseconds(10));
		}

		[TestMethod]
		public void EqualityComparerTest()
		{
			var comp = new EqualityComparer<int>((val1, val2) =>
			{
				return val1.Equals(val2);
			});
			Assert.IsTrue(comp.Equals(1, 1));
			Assert.IsFalse(comp.Equals(1, 2));
		}

		[TestMethod]
		public void DeleteFileThatDoesntExistTest()
		{
			// This should succeed (though print a Debug message).
			FileEx.DeleteFile(@"C:\Test\For\Invalid\File.txt");
		}

		[TestMethod]
		public void EnsureDirectoryTest()
		{
			var path = @"C:\Garb\Test\Whatever";

			// If the directory exists, delete it so we can test the Ensure works correctly.
			FileEx.DeleteDirectory(path);

			Assert.IsFalse(Directory.Exists(path));

			FileEx.EnsureDirectory(path);
			Assert.IsTrue(Directory.Exists(path));

			FileEx.EnsureDirectory(path);
			Assert.IsTrue(Directory.Exists(path));
		}

		[TestMethod]
		public void DeleteEmptyDirectoriesTest()
		{
			var path = @"C:\Garb\Test\Whatever";
			FileEx.EnsureDirectory(path);

			FileEx.DeleteEmptyDirectories(path);
		}

		[TestMethod]
		public void MemSizeTest()
		{

			Assert.AreEqual(1024, MemSize.cNumBytesInKibibyte);
			Assert.AreEqual(Math.Pow(2, 20), MemSize.cNumBytesInMebibyte);
			Assert.AreEqual(Math.Pow(2, 30), MemSize.cNumBytesInGibibyte);
			Assert.AreEqual(Math.Pow(2, 40), MemSize.cNumBytesInTebibyte);

			Assert.AreEqual(1_000, MemSize.cNumBytesInKilobyte);
			Assert.AreEqual(1_000_000, MemSize.cNumBytesInMegabyte);
			Assert.AreEqual(1_000_000_000, MemSize.cNumBytesInGigabyte);
			Assert.AreEqual(1_000_000_000_000, MemSize.cNumBytesInTerabyte);

			var sz = new MemSize(0);
			Assert.AreEqual(0, sz.TotalBytes);
			Assert.AreEqual(0, sz.TotalKibibytes);
			Assert.AreEqual(0, sz.TotalMebibytes);
			Assert.AreEqual(0, sz.TotalGibibytes);
			Assert.AreEqual(0, sz.TotalTebibytes);
			Assert.AreEqual(0, sz.TotalKilobytes);
			Assert.AreEqual(0, sz.TotalMegabytes);
			Assert.AreEqual(0, sz.TotalGigabytes);
			Assert.AreEqual(0, sz.TotalTerabytes);
			Assert.AreEqual("0 bytes", sz.ToString("bytes"));
			Assert.AreEqual("0 bytes", sz.ToString("SI"));
			Assert.AreEqual("0.0 KB", sz.ToString("KB"));
			Assert.AreEqual("0.0 MB", sz.ToString("MB"));
			Assert.AreEqual("0.0 GB", sz.ToString("GB"));
			Assert.AreEqual("0.0 TB", sz.ToString("TB"));
			Assert.AreEqual("0 bytes", sz.ToString("IEC"));
			Assert.AreEqual("0.0 KiB", sz.ToString("KiB"));
			Assert.AreEqual("0.0 MiB", sz.ToString("MiB"));
			Assert.AreEqual("0.0 GiB", sz.ToString("GiB"));
			Assert.AreEqual("0.0 TiB", sz.ToString("TiB"));

			sz = new MemSize(1000);
			Assert.AreEqual(1000, sz.TotalBytes);
			Assert.AreEqual(1000.0 / 1024, sz.TotalKibibytes);
			Assert.AreEqual(1000.0 / Math.Pow(2, 20), sz.TotalMebibytes);
			Assert.AreEqual(1000.0 / Math.Pow(2, 30), sz.TotalGibibytes);
			Assert.AreEqual(1000.0 / Math.Pow(2, 40), sz.TotalTebibytes);
			Assert.AreEqual(1, sz.TotalKilobytes);
			Assert.AreEqual(.001, sz.TotalMegabytes);
			Assert.AreEqual(.000001, sz.TotalGigabytes);
			Assert.AreEqual(.000000001, sz.TotalTerabytes);
			Assert.AreEqual("1,000 bytes", sz.ToString("bytes"));
			Assert.AreEqual("1.0 KB", sz.ToString("SI"));
			Assert.AreEqual("1.0 KB", sz.ToString("KB"));
			Assert.AreEqual("0.0 MB", sz.ToString("MB"));
			Assert.AreEqual("0.0 GB", sz.ToString("GB"));
			Assert.AreEqual("0.0 TB", sz.ToString("TB"));
			Assert.AreEqual("1.0 KiB", sz.ToString("IEC"));
			Assert.AreEqual("1.0 KiB", sz.ToString("KiB"));
			Assert.AreEqual("0.0 MiB", sz.ToString("MiB"));
			Assert.AreEqual("0.0 GiB", sz.ToString("GiB"));
			Assert.AreEqual("0.0 TiB", sz.ToString("TiB"));

			sz = new MemSize(MemSize.cNumBytesInTebibyte);
			Assert.AreEqual(1.0, sz.TotalTebibytes);
			Assert.AreEqual($"{MemSize.cNumBytesInTebibyte:N0} bytes", sz.ToString("bytes"));
			Assert.AreEqual("1.0 TiB", sz.ToString());
			Assert.AreEqual("1.0 TiB", sz.ToString("IEC"));
			Assert.AreEqual($"{(MemSize.cNumBytesInTebibyte / MemSize.cNumBytesInKibibyte):N1} KiB", sz.ToString("KiB"));
			Assert.AreEqual($"{(MemSize.cNumBytesInTebibyte / MemSize.cNumBytesInMebibyte):N1} MiB", sz.ToString("MiB"));
			Assert.AreEqual($"{(MemSize.cNumBytesInTebibyte / MemSize.cNumBytesInGibibyte):N1} GiB", sz.ToString("GiB"));
			Assert.AreEqual("1.0 TiB", sz.ToString("TiB"));
			Assert.AreEqual("1.0 TB", sz.ToString("IEC*"));
			Assert.AreEqual($"{(MemSize.cNumBytesInTebibyte / MemSize.cNumBytesInKibibyte):N1} KB", sz.ToString("KiB*"));
			Assert.AreEqual($"{(MemSize.cNumBytesInTebibyte / MemSize.cNumBytesInMebibyte):N1} MB", sz.ToString("MiB*"));
			Assert.AreEqual($"{(MemSize.cNumBytesInTebibyte / MemSize.cNumBytesInGibibyte):N1} GB", sz.ToString("GiB*"));
			Assert.AreEqual("1.0 TB", sz.ToString("TiB*"));

			sz = new MemSize(123456789);
			Assert.AreEqual("Size is 123,456,789 bytes!", $"Size is {sz:bytes}!");
			Assert.AreEqual("Size is 123,456.8 KB!", $"Size is {sz:KB}!");
			Assert.AreEqual("Size is 123,456.789 KB!", $"Size is {sz:KB3}!");

			sz = MemSize.Parse("1MiB");
			Assert.AreEqual(MemSize.cNumBytesInMebibyte, sz.TotalBytes);
			Assert.AreEqual("1.0 MiB", sz.ToString("MiB"));

			sz = MemSize.Parse("1024MiB");
			Assert.AreEqual(MemSize.cNumBytesInGibibyte, sz.TotalBytes);
			Assert.AreEqual("1,024.0 MiB", sz.ToString("MiB"));

			sz = MemSize.Parse("1024");
			Assert.AreEqual(MemSize.cNumBytesInKibibyte, sz.TotalBytes);

			sz = MemSize.Parse("1MB");
			Assert.AreEqual(MemSize.cNumBytesInMegabyte, sz.TotalBytes);

			sz = MemSize.Parse("1MB", true);
			Assert.AreEqual(MemSize.cNumBytesInMebibyte, sz.TotalBytes);

			Assert.IsTrue(MemSize.TryParse("1MB", out sz));
			Assert.IsFalse(MemSize.TryParse("ASDF", out sz));
			Assert.IsFalse(MemSize.TryParse("NOT", out sz));
			Assert.IsFalse(MemSize.TryParse("1NOT", out sz));

			sz = ConvertEx.To<MemSize>("1MB");
			Assert.AreEqual(MemSize.cNumBytesInMegabyte, sz.TotalBytes);
		}

		[TestMethod]
		public void BitmaskTest()
		{
			var options = new MyOptions(0);

			Assert.AreEqual("[0000][0000]", options.ToString());

			options.Option1 = true;
			Assert.AreEqual("[0000][0001]", options.ToString());

			options.Option2 = true;
			Assert.AreEqual("[0000][0011]", options.ToString());

			options.Option3 = true;
			Assert.AreEqual("[1000][0011]", options.ToString());
		}

		[TestMethod]
		public void BitmaskEqualsTest()
		{
			var option1 = new MyOptions(0);
			var option2 = new MyOptions(0);

			Assert.IsTrue(option1.Equals(option2));

			option1.Option1 = true;
			Assert.IsFalse(option1.Equals(option2));

			Assert.IsTrue(option1.Equals(1));

			Assert.IsTrue(option1 == 1);
		}

		private class MyOptions : Bitmask
		{
			public MyOptions(int value)
				: base(value, 8)
			{
			}

			public bool Option1
			{
				// Test to make sure right-most bit is set.
				get { return IsSet(1); }
				set { SetBit(1, value); }
			}

			public bool Option2
			{
				// Test to make sure a bit in the middle is set.
				get { return IsSet(2); }
				set { SetBit(2, value); }
			}

			public bool Option3
			{
				// Test to make sure left-most bit is set.
				get { return IsSet(8); }
				set { SetBit(8, value); }
			}

			public static implicit operator MyOptions(int val)
			{
				return new MyOptions(val);
			}

			public static implicit operator int(MyOptions val)
			{
				return (int)val.Value;
			}

		}

	}
}