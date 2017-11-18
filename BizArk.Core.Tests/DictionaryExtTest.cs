using BizArk.Core.Extensions.DictionaryExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace BizArk.Core.Tests
{
	[TestClass]
	public class DictionaryExtTest
	{
		[TestMethod]
		public void ConvertValueFromPropertyBag()
		{
			var dict = new Dictionary<string, object>();
			var val = dict.TryGetValue("Test", 0);
			Assert.AreEqual(0, val);

			dict["Test"] = "123";
			val = dict.TryGetValue("Test", 0);
			Assert.AreEqual(123, val);

			val = dict.TryGetValue("INVALID", -1);
			Assert.AreEqual(-1, val);
		}

		[TestMethod]
		public void ConvertValueFromIntStringDict()
		{
			var dict = new Dictionary<int, string>();
			var val = dict.TryGetValue(123, 0m);
			Assert.AreEqual(0m, val);

			dict[123] = "1.23";
			val = dict.TryGetValue(123, 0m);
			Assert.AreEqual(1.23m, val);

			val = dict.TryGetValue(-1, decimal.MinValue);
			Assert.AreEqual(decimal.MinValue, val);
		}

		[TestMethod]
		public void ConvertDictToDynamic()
		{
			var dict = new Dictionary<string, object>();
			dict.Add("Test1", "Hello World");
			dict.Add("Test2", 123);

			var obj = dict.ToDynamic();
			Assert.AreEqual("Hello World", obj.Test1);
			Assert.AreEqual(123, obj.Test2);
		}

	}
}
