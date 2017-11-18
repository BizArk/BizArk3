using BizArk.Core.Extensions.DateExt;
using BizArk.Core.Extensions.FormatExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BizArk.Core.Tests
{
	/// <summary>
	/// Additional assertions beyond what is provided by Assert.
	/// </summary>
	public static class AssertEx
	{
		public static void EnumerablesAreEqual<T>(this Assert assert, IEnumerable<T> expected, IEnumerable<T> actual, string message = null)
		{
			if (expected == actual) return;
			if (expected == null) Assert.Fail(message ?? $"Expected <null>. Actual array contained {actual.Count()} elements.");
			if (actual == null) Assert.Fail(message ?? $"Expected {expected.Count()} elements. Actual array was null.");
			if (expected.Count() != actual.Count()) Assert.Fail(message ?? $"Expected {expected.Count()} elements. Actual array conains {actual.Count()} elements.");

			for (int i = 0; i < actual.Count(); i++)
			{
				if (!expected.ElementAt(i).Equals(actual.ElementAt(i)))
					Assert.Fail(message ?? $"Arrays differ at element {i}. Expected '{expected.ElementAt(i)}', actual '{actual.ElementAt(i)}'.");
			}
		}

		public static void EnumerablesAreNotEqual<T>(this Assert assert, IEnumerable<T> expected, IEnumerable<T> actual, string message = null)
		{
			if (expected == actual) Assert.Fail(message ?? "The arrays are the same instance.");
			if (expected == null) return;
			if (actual == null) return;
			if (expected.Count() != actual.Count()) return;
			if (expected.GetType().GetElementType() != actual.GetType().GetElementType()) return;

			for (int i = 0; i < actual.Count(); i++)
			{
				if (!expected.ElementAt(i).Equals(actual.ElementAt(i)))
					return;

			}
			Assert.Fail(message ?? $"Arrays have identical values");
		}

		/// <summary>
		/// Catches the given exception type and ignores it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		public static void Throws<T>(this Assert assert, Action action) where T : Exception
		{
			try
			{
				action();
				Assert.Fail("Expected exception '{0}' not thrown.", typeof(T).Name);
			}
			catch (T ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// Catches all excpetions and makes sure the exception thrown is exactly the type sent in.
		/// </summary>
		/// <param name="exceptionType"></param>
		/// <param name="action"></param>
		public static void Throws(this Assert assert, Type exceptionType, Action action)
		{
			try
			{
				action();
				Assert.Fail("Expected exception '{0}' not thrown.", exceptionType.Name);
			}
			catch (Exception ex)
			{
				if (ex.GetType() == exceptionType)
					return;
				else
					throw;
			}
		}

		public static void AreClose(this Assert assert, DateTime expected, DateTime actual, TimeSpan giveOrTake)
		{
			if (!expected.IsClose(actual, giveOrTake))
				Assert.Fail("Expected {0} (~{1}). Actual was {2}.".Fmt(expected, giveOrTake, actual));
		}

		//public static void NoError(FieldValidationResults res)
		//{
		//    if (res.Error == null) return;
		//    Assert.Fail("Validation resulted in error: ({0}) {1}", res.Error.MessageID, res.Error.Message);
		//}

		//public static void HasError(FieldValidationResults res, Enum msgID)
		//{
		//    if (res.Error == null)
		//        Assert.Fail("Validation succeeded. Expected error {0}", msgID);

		//    if (res.Error.MessageID.Equals(msgID))
		//        return;

		//    Assert.Fail("Expected error {0}. Validation resulted in unexpected error: ({1}) {2}", msgID, res.Error.MessageID, res.Error.Message);
		//}

	}
}
