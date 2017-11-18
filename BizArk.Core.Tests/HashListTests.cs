using BizArk.Core.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizArk.Core.Tests
{

	/// <summary>
	///This is a test class for HashListTest and is intended
	///to contain all HashListTest Unit Tests
	///</summary>
	[TestClass]
	public class HashListTests
	{

		[TestMethod]
		public void AddTest()
		{
			var lst = GetTestList();

			// Ensure the list is in the order it was entered.
			var val = -1;
			for (int i = 0; i < lst.Count; i++)
			{
				Assert.IsTrue(lst[i].TestInt > val);
				val = lst[i].TestInt;
			}

			Assert.AreEqual(5, lst[5].TestInt);
			Assert.AreEqual(5, lst["Test5"].TestInt);
			Assert.AreSame(lst[5], lst["Test5"]);
		}

		[TestMethod]
		public void RemoveTest()
		{
			var lst = GetTestList();

			Assert.IsTrue(lst.ContainsKey("Test5"));
			Assert.AreEqual(5, lst.IndexOf("Test5"));
			Assert.IsTrue(lst.Remove("Test5"));
			Assert.IsFalse(lst.Remove("Test5"));
			Assert.IsFalse(lst.ContainsKey("Test5"));
			Assert.AreEqual(5, lst.IndexOf("Test6"));

			Assert.IsTrue(lst.ContainsKey("Test1"));
			Assert.AreEqual(1, lst.IndexOf("Test1"));
			lst.RemoveAt(1);
			Assert.IsFalse(lst.Remove("Test1"));
			Assert.IsFalse(lst.ContainsKey("Test1"));
			Assert.AreEqual(1, lst.IndexOf("Test2"));

			Assert.IsTrue(lst.ContainsKey("Test6"));
			Assert.AreEqual(4, lst.IndexOf("Test6"));
			var val = lst[4];
			lst.Remove(val);
			Assert.IsFalse(lst.Remove("Test6"));
			Assert.IsFalse(lst.ContainsKey("Test6"));
			Assert.AreEqual(4, lst.IndexOf("Test7"));

			Assert.AreEqual(7, lst.Count);
			Assert.AreEqual(0, lst.IndexOf("Test0"));
			Assert.AreEqual(1, lst.IndexOf("Test2"));
			Assert.AreEqual(2, lst.IndexOf("Test3"));
			Assert.AreEqual(3, lst.IndexOf("Test4"));
			Assert.AreEqual(4, lst.IndexOf("Test7"));
			Assert.AreEqual(5, lst.IndexOf("Test8"));
			Assert.AreEqual(6, lst.IndexOf("Test9"));

		}

		[TestMethod]
		public void InsertTest()
		{
			var lst = GetTestList();

			Assert.AreEqual(10, lst.Count);

			var val = new MyClass() { TestInt = int.MaxValue };
			lst.Insert(2, "TestXXX", val);

			Assert.AreEqual(11, lst.Count);

			Assert.AreEqual(0, lst.IndexOf("Test0"));
			Assert.AreEqual(1, lst.IndexOf("Test1"));
			Assert.AreEqual(2, lst.IndexOf("TestXXX"));
			Assert.AreEqual(3, lst.IndexOf("Test2"));
			Assert.AreEqual(4, lst.IndexOf("Test3"));
			Assert.AreEqual(5, lst.IndexOf("Test4"));
			Assert.AreEqual(6, lst.IndexOf("Test5"));
			Assert.AreEqual(7, lst.IndexOf("Test6"));
			Assert.AreEqual(8, lst.IndexOf("Test7"));
			Assert.AreEqual(9, lst.IndexOf("Test8"));
			Assert.AreEqual(10, lst.IndexOf("Test9"));
		}

		[TestMethod]
		public void ClearTest()
		{
			var lst = GetTestList();
			Assert.AreEqual(10, lst.Count);
			lst.Clear();
			Assert.AreEqual(0, lst.Count);
		}

		[TestMethod]
		public void IndexerTest()
		{
			var lst = GetTestList();
			Assert.AreEqual(10, lst.Count);

			Assert.AreEqual(lst["Test0"], lst[0]);
			Assert.AreEqual(lst["Test1"], lst[1]);
			Assert.AreEqual(lst["Test2"], lst[2]);
			Assert.AreEqual(lst["Test3"], lst[3]);
			Assert.AreEqual(lst["Test4"], lst[4]);
			Assert.AreEqual(lst["Test5"], lst[5]);
			Assert.AreEqual(lst["Test6"], lst[6]);
			Assert.AreEqual(lst["Test7"], lst[7]);
			Assert.AreEqual(lst["Test8"], lst[8]);
			Assert.AreEqual(lst["Test9"], lst[9]);

			lst.Insert(2, "TestXXX", new MyClass() { TestInt = int.MaxValue });
			Assert.AreEqual(lst["Test0"], lst[0]);
			Assert.AreEqual(lst["Test1"], lst[1]);
			Assert.AreEqual(lst["TestXXX"], lst[2]);
			Assert.AreEqual(lst["Test2"], lst[3]);
			Assert.AreEqual(lst["Test3"], lst[4]);
			Assert.AreEqual(lst["Test4"], lst[5]);
			Assert.AreEqual(lst["Test5"], lst[6]);
			Assert.AreEqual(lst["Test6"], lst[7]);
			Assert.AreEqual(lst["Test7"], lst[8]);
			Assert.AreEqual(lst["Test8"], lst[9]);
			Assert.AreEqual(lst["Test9"], lst[10]);

			lst.Remove("Test6");
			Assert.AreEqual(lst["Test0"], lst[0]);
			Assert.AreEqual(lst["Test1"], lst[1]);
			Assert.AreEqual(lst["TestXXX"], lst[2]);
			Assert.AreEqual(lst["Test2"], lst[3]);
			Assert.AreEqual(lst["Test3"], lst[4]);
			Assert.AreEqual(lst["Test4"], lst[5]);
			Assert.AreEqual(lst["Test5"], lst[6]);
			Assert.AreEqual(lst["Test7"], lst[7]);
			Assert.AreEqual(lst["Test8"], lst[8]);
			Assert.AreEqual(lst["Test9"], lst[9]);
		}

		[TestMethod]
		public void GetValueTest()
		{
			var lst = GetTestList();
			var val = lst.GetValue("Test1", null);
			Assert.IsNotNull(val);
			Assert.AreEqual(1, val.TestInt);

			val = lst.GetValue("TestXXX", null);
			Assert.IsNull(val);
		}

		[TestMethod]
		public void EnumeratorTest()
		{
			var lst = GetTestList();
			MyClass prevVal = null;
			var count = 0;
			foreach (var curVal in lst)
			{
				if (prevVal != null)
					Assert.IsTrue(prevVal.TestInt < curVal.TestInt);
				prevVal = curVal;
				count++;
			}
			Assert.AreEqual(lst.Count, count);
		}

		private HashList<string, MyClass> GetTestList()
		{
			var lst = new HashList<string, MyClass>();
			lst.Add("Test0", new MyClass() { TestInt = 0 });
			lst.Add("Test1", new MyClass() { TestInt = 1 });
			lst.Add("Test2", new MyClass() { TestInt = 2 });
			lst.Add("Test3", new MyClass() { TestInt = 3 });
			lst.Add("Test4", new MyClass() { TestInt = 4 });
			lst.Add("Test5", new MyClass() { TestInt = 5 });
			lst.Add("Test6", new MyClass() { TestInt = 6 });
			lst.Add("Test7", new MyClass() { TestInt = 7 });
			lst.Add("Test8", new MyClass() { TestInt = 8 });
			lst.Add("Test9", new MyClass() { TestInt = 9 });
			return lst;
		}

		private class MyClass
		{
			public int TestInt;
		}

	}
}
