using System;
using BizArk.Core.Extensions.DateExt;
using BizArk.Core.Extensions.FormatExt;
using NUnit.Framework;
using System.Diagnostics;

namespace BizArk.Core.Tests
{
    /// <summary>
    /// Additional assertions beyond what is provided by Assert.
    /// </summary>
    public static class AssertEx
    {
        public static void AreEqual(Array expected, Array actual)
        {
            AreEqual(expected, actual, "");
        }

        public static void AreEqual(Array expected, Array actual, string message)
        {
            if (expected == actual) return;
            if (expected == null) Assert.Fail(message == "" ? string.Format("Expected <null>. Actual array contained {0} elements.", actual.Length) : message);
            if (actual == null) Assert.Fail(message == "" ? string.Format("Expected {0} elements. Actual array was null.", expected.Length) : message);
            if (expected.GetType().GetElementType() != actual.GetType().GetElementType()) Assert.Fail(message == "" ? string.Format("Array element types differ. Expected elements of type {0}, actual array conains elements of type {1}.", expected.GetType().GetElementType().FullName, actual.GetType().GetElementType().FullName) : message);
            if (expected.Length != actual.Length) Assert.Fail(message == "" ? string.Format("Expected {0} elements. Actual array conains {1} elements.", expected.Length, actual.Length) : message);

            for (int i = 0; i < actual.Length; i++)
            {
                if (!expected.GetValue(i).Equals(actual.GetValue(i)))
                    Assert.Fail(message == "" ? string.Format("Arrays differ at element {0}. Expected '{1}', actual '{2}'.", i, expected.GetValue(i), actual.GetValue(i)) : message);
            }
        }

        /// <summary>
        /// Catches the given exception type and ignores it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public static void Throws<T>(Action action) where T : Exception
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
        public static void Throws(Type exceptionType, Action action)
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

        public static void AreClose(DateTime expected, DateTime actual, TimeSpan giveOrTake)
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
