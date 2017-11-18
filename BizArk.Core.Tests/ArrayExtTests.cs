using BizArk.Core.Extensions.ArrayExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BizArk.Core.Tests
{
	/// <summary>
	///This is a test class for StringArrayExtTest and is intended
	///to contain all StringArrayExtTest Unit Tests
	///</summary>
	[TestClass]
	public class ArrayExtTests
	{

		[TestMethod]
		public void StandardConvertTest()
		{
			string[] arr = new string[] { "1", "2", "3" };
			Type elementType = typeof(int);
			int[] expected = new int[] { 1, 2, 3 };
			int[] actual;
			actual = (int[])ArrayExt.Convert(arr, elementType);
			Assert.That.EnumerablesAreEqual(expected, actual);
		}

		[TestMethod]
		public void GenericConvertTest()
		{
			string[] arr = new string[] { "1", "2", "3" };
			Type elementType = typeof(int);
			int[] expected = new int[] { 1, 2, 3 };
			int[] actual;
			actual = ArrayExt.Convert<int>(arr);
			Assert.That.EnumerablesAreEqual(expected, actual);
		}

		[TestMethod]
		public void RemoveEmptiesTest()
		{
			string[] test;
			string[] expected;
			string[] actual;

			test = new string[] { };
			actual = test.RemoveEmpties();
			expected = new string[] { };
			Assert.AreEqual(0, actual.Length);

			test = new string[] { "Hi", "Bye" };
			actual = test.RemoveEmpties();
			expected = new string[] { "Hi", "Bye" };
			Assert.That.EnumerablesAreEqual(expected, actual);

			test = new string[] { "Hi", "" };
			actual = test.RemoveEmpties();
			expected = new string[] { "Hi" };
			Assert.That.EnumerablesAreEqual(expected, actual);

			test = new string[] { null, "Bye" };
			actual = test.RemoveEmpties();
			expected = new string[] { "Bye" };
			Assert.That.EnumerablesAreEqual(expected, actual);

			var itest = new int[] { 1, 2, 3 };
			var iactual = itest.RemoveEmpties();
			var iexpected = new int[] { 1, 2, 3 };
			Assert.That.EnumerablesAreEqual(iexpected, iactual);

			itest = new int[] { 1, int.MinValue, 3 };
			iactual = itest.RemoveEmpties();
			iexpected = new int[] { 1, 3 };
			Assert.That.EnumerablesAreEqual(iexpected, iactual);

		}

	}
}
