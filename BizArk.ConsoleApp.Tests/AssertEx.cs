using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.StringExt;
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizArk.ConsoleApp.Tests
{
	static class AssertEx
	{

		public static void Throws<T>(this Assert assert, Action action, string message = null) where T : Exception
		{
			try
			{
				action();
				Assert.Fail(message.IfEmpty("Expecting to throw {0}.".Fmt(typeof(T).Name)));
			}
			catch (T ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public static void Throws<T>(this Assert assert, Action action, Func<T, bool> when, string message = null) where T : Exception
		{
			try
			{
				action();
				Assert.Fail(message.IfEmpty("Expecting to throw {0}.".Fmt(typeof(T).Name)));
			}
			catch (T ex) when (when(ex))
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public static void Contains(this Assert assert, string contains, string actual, string message = null)
		{
			if (actual.Contains(contains)) return;
			Assert.Fail(message.IfEmpty("Assert.That.Contains failed. Actual:<{0}> does not contain <{1}>.".Fmt(actual, contains)));
		}
	}
}
