using BizArk.Core.Extensions.WebExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizArk.Core.Tests
{

	[TestClass]
	public class WebExtTests
	{

		[TestMethod]
		public void ToQueryString()
		{
			var qs = (new
			{
				Nbr = 123,
				Str = "Hello"
			}).ToQueryString();
			Assert.AreEqual("Nbr=123&Str=Hello", qs);

			qs = (new
			{
				Nbr = 123,
				Str = "Hello World"
			}).ToQueryString();
			Assert.AreEqual("Nbr=123&Str=Hello%20World", qs);

			qs = (new
			{
				Nbr = 123,
				Str = "Hello World"
			}).ToQueryString("Nbr");
			Assert.AreEqual("Nbr=123", qs);
		}

		[TestMethod]
		public void GenerateSlug()
		{
			var slug = "".GenerateSlug();
			Assert.AreEqual("", slug);

			slug = "hello".GenerateSlug();
			Assert.AreEqual("hello", slug);

			slug = "hello world".GenerateSlug();
			Assert.AreEqual("hello-world", slug);
		}


	}
}
