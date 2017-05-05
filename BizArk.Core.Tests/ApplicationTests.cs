using System;
using BizArk.Core.Util;
using NUnit.Framework;

namespace BizArk.Core.Tests
{


	/// <summary>
	/// Tests for the Cleanup class.
	///</summary>
	[TestFixture]
	public class ApplicationTest
	{

		[Test]
		public void SetCurrentDirectoryTest()
		{
			var origDir = Environment.CurrentDirectory;
			using (Application.SetCurrentDirectory(@"C:\"))
			{
				Assert.AreEqual(@"C:\", Environment.CurrentDirectory);
			}
			Assert.AreEqual(origDir, Environment.CurrentDirectory);
		}

	}
}
