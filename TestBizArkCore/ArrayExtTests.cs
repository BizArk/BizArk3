using System;
using BizArk.Core.Extensions.ArrayExt;
using NUnit.Framework;

namespace BizArk.Core.Tests
{


    /// <summary>
    ///This is a test class for StringArrayExtTest and is intended
    ///to contain all StringArrayExtTest Unit Tests
    ///</summary>
[TestFixture]
    public class ArrayExtTests
    {

        [Test]
        public void StandardConvertTest()
        {
            string[] arr = new string[] { "1", "2", "3" };
            Type elementType = typeof(int);
            int[] expected = new int[] { 1, 2, 3 };
            int[] actual;
            actual = (int[])ArrayExt.Convert(arr, elementType);
            AssertEx.AreEqual(expected, actual);
        }

        [Test]
        public void GenericConvertTest()
        {
            string[] arr = new string[] { "1", "2", "3" };
            Type elementType = typeof(int);
            int[] expected = new int[] { 1, 2, 3 };
            int[] actual;
            actual = ArrayExt.Convert<int>(arr);
            AssertEx.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Shrink
        ///</summary>
        [Test]
        public void ShrinkTest()
        {
            string[] test;
            string[] expected;
            string[] actual;

            test = new string[] { };
            actual = ArrayExt.Shrink(test, 0, 0);
            expected = new string[] { };
            Assert.AreEqual(0, actual.Length);

            test = new string[] { "Hi", "Bye" };
            actual = ArrayExt.Shrink(test, 0, 1);
            expected = new string[] { "Hi", "Bye" };
            AssertEx.AreEqual(expected, actual);

            test = new string[] { "Hi", "Bye" };
            actual = ArrayExt.Shrink(test, 0, 0);
            expected = new string[] { "Hi" };
            AssertEx.AreEqual(expected, actual);

            test = new string[] { "Hi", "Bye" };
            actual = ArrayExt.Shrink(test, 1, 1);
            expected = new string[] { "Bye" };
            AssertEx.AreEqual(expected, actual);
        }

        [Test]
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
            AssertEx.AreEqual(expected, actual);

            test = new string[] { "Hi", "" };
            actual = test.RemoveEmpties();
            expected = new string[] { "Hi" };
            AssertEx.AreEqual(expected, actual);

            test = new string[] { null, "Bye" };
            actual = test.RemoveEmpties();
            expected = new string[] { "Bye" };
            AssertEx.AreEqual(expected, actual);

            var itest = new int[] { 1, 2, 3 };
            var iactual = itest.RemoveEmpties();
            var iexpected = new int[] { 1, 2, 3 };
            AssertEx.AreEqual(iexpected, iactual);

            itest = new int[] { 1, int.MinValue, 3 };
            iactual = itest.RemoveEmpties();
            iexpected = new int[] { 1, 3 };
            AssertEx.AreEqual(iexpected, iactual);

        }

    }
}
