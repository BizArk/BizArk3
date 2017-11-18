using BizArk.Core.Extensions.TypeExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BizArk.Core.Tests
{

	/// <summary>
	///This is a test class for StringExtTest and is intended
	///to contain all StringExtTest Unit Tests
	///</summary>
	[TestClass]
	public class TypeExtTests
	{

		[TestMethod]
		public void IsNullableTest()
		{

			Assert.IsFalse(typeof(int).AllowNull());
			Assert.IsTrue(typeof(int?).AllowNull());
			Assert.IsTrue(typeof(string).AllowNull());
			Assert.IsTrue(typeof(TypeExtTester).AllowNull());
			Assert.IsFalse(typeof(TypeExtEnum).AllowNull());

		}

		[TestMethod]
		public void GetTrueTypeTest()
		{
			Assert.AreEqual(typeof(int), typeof(int).GetTrueType());
			Assert.AreEqual(typeof(int), typeof(int?).GetTrueType());
			Assert.AreEqual(typeof(TypeExtEnum), typeof(TypeExtEnum?).GetTrueType());
			Assert.AreEqual(typeof(TypeExtTester), typeof(TypeExtTester).GetTrueType());
		}

		[TestMethod]
		public void ImplementsTest()
		{
			Assert.IsTrue(typeof(TypeExtTester).Implements(typeof(IDisposable)));
			Assert.IsFalse(typeof(TypeExtTester).Implements(typeof(IConvertible)));
		}

		public enum TypeExtEnum
		{
			One,
			Two
		}

		public class TypeExtTester : IDisposable
		{
			public void Dispose()
			{
				throw new NotImplementedException();
			}
		}

	}
}
