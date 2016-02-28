using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.StringExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace BizArk.ConsoleApp.Tests
{
	static class AssertEx
	{

		public static void Throws<T>(Action action, string message = null) where T : Exception
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

		public static void Contains(string contains, string actual, string message = null)
		{
			if (actual.Contains(contains)) return;
			Assert.Fail(message.IfEmpty("AssertEx.Contains failed. Actual:<{0}> does not contain <{1}>.".Fmt(actual, contains)));
		}
	}
}
