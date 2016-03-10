using NUnit.Framework;
using BizArk.Core.Extensions.MathExt;
using BizArk.Core.Util;
using System.Collections.Generic;

namespace BizArk.Core.Tests
{


	/// <summary>
	///This is a test class for FormatExtTest and is intended
	///to contain all FormatExtTest Unit Tests
	///</summary>
	[TestFixture]
	public class ObjectUtilTest
	{

		[Test]
		public void GetBasicValueTest()
		{
			var test = CreateTestStructure();
			var value = ObjectUtil.GetValue(test, "Name", "Bob");
			Assert.AreEqual(test.Name, value);
		}

		[Test]
		public void GetNestedValueTest()
		{
			var test = CreateTestStructure();
			var value = ObjectUtil.GetValue(test, "Test2.Test3.Greeting", "Goodbye");
			Assert.AreEqual(test.Test2.Test3.Greeting, value);
		}

		[Test]
		public void GetIndexedValueTest()
		{
			var test = CreateTestStructure();
			var numVal = ObjectUtil.GetValue(test, "Test2.Numbers[1]", 0);
			Assert.AreEqual(test.Test2.Numbers[1], numVal);

			var strVal = ObjectUtil.GetValue(test, "Test2.Greeters[Maria].Greeting", "Goodbye");
			Assert.AreEqual(test.Test2.Greeters["Maria"].Greeting, strVal);
		}

		[Test]
		public void TryBasicValueTest()
		{
			var test = CreateTestStructure();
			string value;
			Assert.IsTrue(ObjectUtil.TryGetValue(test, "Name", out value, "Bob"));
			Assert.AreEqual(test.Name, value);
		}

		[Test]
		public void TryNestedValueTest()
		{
			var test = CreateTestStructure();
			string value;
			Assert.IsTrue(ObjectUtil.TryGetValue(test, "Test2.Test3.Greeting", out value, "Goodbye"));
			Assert.AreEqual(test.Test2.Test3.Greeting, value);
		}

		[Test]
		public void TryIndexedValueTest()
		{
			var test = CreateTestStructure();
			int numVal;
			Assert.IsTrue(ObjectUtil.TryGetValue(test, "Test2.Numbers[1]", out numVal, 0));
			Assert.AreEqual(test.Test2.Numbers[1], numVal);

			string strVal;
			Assert.IsTrue(ObjectUtil.TryGetValue(test, "Test2.Greeters[Maria].Greeting", out strVal, "Goodbye"));
			Assert.AreEqual(test.Test2.Greeters["Maria"].Greeting, strVal);
		}

		[Test]
		public void ObjectDictionaryTest()
		{
			var test = CreateTestStructure();
			var dict = new ObjectDictionary(test);

			var value = dict["Test2.Numbers[1]"];
			Assert.AreEqual(test.Test2.Numbers[1], value);

			value = dict["Test2.Greeters[Maria].Greeting"];
			Assert.AreEqual(test.Test2.Greeters["Maria"].Greeting, value);
		}

		#region Test Classes

		private Test1 CreateTestStructure()
		{
			var test1 = new Test1();
			test1.Name = "Gunther";
			var test2 = test1.Test2 = new Test2(1, 2, 3, 5, 7, 11, 13, 17, 19);
			var greeters = test2.Greeters = new Dictionary<string, Test3>();
			greeters.Add("Maria", new Test3("Hola Mundo"));
			greeters.Add("Gunther", new Test3("Hallo Welt"));
			greeters.Add("Rachel", new Test3("שלום עולם"));
			var test3 = test2.Test3 = new Test3("Hello World");
			return test1;
		}

		private class Test1
		{
			public string Name { get; set; }
			public Test2 Test2 { get; set; }
		}

		private class Test2
		{
			public Test2(params int[] numbers)
			{
				Numbers = numbers;
			}
			public int[] Numbers { get; set; }
			public Dictionary<string, Test3> Greeters { get; set; }
			public Test3 Test3 { get; set; }
		}

		private class Test3
		{
			public Test3(string greeting)
			{
				Greeting = greeting;
			}
			public string Greeting { get; set; }
		}

		#endregion

	}

}
