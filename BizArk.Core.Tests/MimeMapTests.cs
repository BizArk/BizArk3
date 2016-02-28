using BizArk.Core.Util;
using NUnit.Framework;

namespace BizArk.Core.Tests
{
	
	
	/// <summary>
	///This is a test class for MimeMapTest and is intended
	///to contain all MimeMapTest Unit Tests
	///</summary>
	[TestFixture]
	public class MimeMapTests
	{

		[Test]
		public void GetMimeTypeTest()
		{
			var mimeType = MimeMap.GetMimeType(".TxT");
			Assert.AreEqual("text/plain", mimeType);
		}

		[Test]
		public void InvalidMimeTypeTest()
		{
			var mimeType = MimeMap.GetMimeType(".!@#");
			Assert.AreEqual(null, mimeType);
		}

	}
}
