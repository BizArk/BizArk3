using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizArk.Core.Extensions.WebExt;

namespace BizArk.Core.Tests
{
	[TestFixture]
	public class WebExtTests
	{

		[Test]
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

		[Test]
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
