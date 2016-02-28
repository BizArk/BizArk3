using System.IO;
using BizArk.Core.Web;
using NUnit.Framework;

namespace BizArk.Core.Tests
{
	
	
	/// <summary>
	///This is a test class for UploadFileTest and is intended
	///to contain all UploadFileTest Unit Tests
	///</summary>
	[TestFixture]
	public class UploadFileTests
	{

		[Test]
		public void op_ExplicitTest()
		{
			var fi = new FileInfo(@"C:\Test.txt");
			var file = (UploadFile)fi;
			Assert.IsNotNull(file);
			Assert.AreEqual(fi.FullName, file.FilePath);
		}

	}
}
