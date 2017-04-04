using System;
using System.Threading;
using BizArk.Core.Util;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Compatibility;
using My = BizArk.Core.Tests.Properties;
using System.Drawing;
using BizArk.Core.Extensions.ImageExt;

namespace BizArk.Core.Tests
{
	[TestFixture]
	public class UtilTests
	{
		[Test]
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

		[Test]
		[Ignore("Uses Thread.Sleep so it takes too long to run.")]
		public void RemoteTimeTest()
		{
			var remote = new RemoteDateTime(DateTime.Now);
			AssertEx.AreClose(DateTime.Now, remote.Now, TimeSpan.FromMilliseconds(10));
			Thread.Sleep(TimeSpan.FromSeconds(5));
			DateTime now = DateTime.Now;
			DateTime remoteNow = remote.Now;
			Console.WriteLine("Now: {0} Remote: {1}", now.ToString("O"), remoteNow.ToString("O"));
			AssertEx.AreClose(now, remoteNow, TimeSpan.FromMilliseconds(10));
		}

		[Test]
		public void EqualityComparerTest()
		{
			var comp = new EqualityComparer<int>((val1, val2) =>
			{
				return val1.Equals(val2);
			});
			Assert.IsTrue(comp.Equals(1, 1));
			Assert.IsFalse(comp.Equals(1, 2));
		}

		[Test]
		public void DeleteFileThatDoesntExistTest()
		{
			// This should succeed (though print a Debug message).
			FileEx.DeleteFile(@"C:\Test\For\Invalid\File.txt");
		}

		[Test]
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

		[Test]
		public void DeleteEmptyDirectoriesTest()
		{
			var path = @"C:\Garb\Test\Whatever";
			FileEx.EnsureDirectory(path);

			FileEx.DeleteEmptyDirectories(path);
		}

		[Test]
		[Ignore("For some reason the font isn't loading correctly. Can't figure it out right now.")]
		public void FontUtilTest()
		{
			FontUtil.RegisterFont(My.Resources.Spirax_Regular);
			var fontName = "Spirax";
			var fontSize = 40;

			using (var myfont = FontUtil.Create(fontName, fontSize, FontStyle.Regular, GraphicsUnit.Point))
			using (var bmp = new Bitmap(500, 500))
			using (var g = Graphics.FromImage(bmp))
			{
				Assert.AreEqual(fontSize, myfont.Size);
				Assert.AreEqual(fontName, myfont.Name); // Fails because Spirax isn't loading for some reason (though I did confirm it was registered properly).

				// Make sure we can use the font.
				g.DrawString("Hello World", myfont, Brushes.Black, 10, 10);

				// Uncomment the following line to visually confirm if the font was used or not.
				// Do not leave check it in with this open since that will cause problems for
				// automated testing.
				//bmp.Open();

			}
		}

		[Test]
		public void MemSizeTest()
		{
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
			Assert.AreEqual(1.0 / 1024, sz.TotalMebibytes);
			Assert.AreEqual(.001 / 1024, sz.TotalGibibytes);
			Assert.AreEqual(.000001 / 1024, sz.TotalTebibytes);
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
			Assert.AreEqual("1,000,000,000.0 KiB", sz.ToString("KiB"));
			Assert.AreEqual("1,000,000.0 MiB", sz.ToString("MiB"));
			Assert.AreEqual("1,000.0 GiB", sz.ToString("GiB"));
			Assert.AreEqual("1.0 TiB", sz.ToString("TiB"));
			Assert.AreEqual("1.0 TB", sz.ToString("IEC*"));
			Assert.AreEqual("1,000,000,000.0 KB", sz.ToString("KiB*"));
			Assert.AreEqual("1,000,000.0 MB", sz.ToString("MiB*"));
			Assert.AreEqual("1,000.0 GB", sz.ToString("GiB*"));
			Assert.AreEqual("1.0 TB", sz.ToString("TiB*"));

			sz = new MemSize(123456789);
			Assert.AreEqual("Size is 123,456,789 bytes!", $"Size is {sz:bytes}!");
			Assert.AreEqual("Size is 123,456.8 KB!", $"Size is {sz:KB}!");
			Assert.AreEqual("Size is 123,456.789 KB!", $"Size is {sz:KB3}!");
		}

		[Test]
		public void BitmaskTest()
		{
			var options = new MyOptions(0);

			Assert.AreEqual("[0000][0000][0000][0000][0000][0000][0000][0000]", options.ToString());

			options.Option1 = true;
			Assert.AreEqual("[0000][0000][0000][0000][0000][0000][0000][0001]", options.ToString());

			options.Option2 = true;
			Assert.AreEqual("[0000][0000][0000][0000][0000][0000][0000][0011]", options.ToString());

			options.Option3 = true;
			Assert.AreEqual("[1000][0000][0000][0000][0000][0000][0000][0011]", options.ToString());
		}

		private class MyOptions : Bitmask
		{
			public MyOptions(int value)
				: base(value)
			{
			}

			public bool Option1
			{
				// Test to make sure right-most bit is set.
				get { return GetBit(1); }
				set { SetBit(1, value); }
			}

			public bool Option2
			{
				// Test to make sure a bit in the middle is set.
				get { return GetBit(2); }
				set { SetBit(2, value); }
			}

			public bool Option3
			{
				// Test to make sure left-most bit is set.
				get { return GetBit(32); }
				set { SetBit(32, value); }
			}

		}

	}
}