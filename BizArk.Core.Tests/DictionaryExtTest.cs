using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizArk.Core.Extensions.DictionaryExt;

namespace BizArk.Core.Tests
{
    [TestFixture]
    public class DictionaryExtTest
    {
        [Test]
        public void ConvertValueFromPropertyBag()
        {
            var dict = new Dictionary<string, object>();
            var val = dict.TryGetValue<int>("Test");
            Assert.AreEqual(0, val);

            dict["Test"] = "123";
            val = dict.TryGetValue<int>("Test");
            Assert.AreEqual(123, val);

            val = dict.TryGetValue<int>("INVALID", -1);
            Assert.AreEqual(-1, val);
        }

        [Test]
        public void ConvertValueFromIntStringDict()
        {
            var dict = new Dictionary<int, string>();
            var val = dict.TryGetValue<int, string, decimal>(123);
            Assert.AreEqual(0m, val);

            dict[123] = "1.23";
            val = dict.TryGetValue<int, string, decimal>(123);
            Assert.AreEqual(1.23m, val);

            val = dict.TryGetValue<int, string, decimal>(-1, decimal.MinValue);
            Assert.AreEqual(decimal.MinValue, val);
        }

        [Test]
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
