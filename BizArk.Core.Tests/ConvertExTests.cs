using System;
using System.Data;
using System.Drawing;
using NUnit.Framework;

namespace BizArk.Core.Tests
{


	/// <summary>
	///This is a test class for ConvertExTest and is intended
	///to contain all ConvertExTest Unit Tests
	///</summary>
	[TestFixture]
	public class ConvertExTests
	{

		[Test]
		public void IsEmptyTest()
		{
			Assert.IsTrue(ConvertEx.IsEmpty(null));
			Assert.IsFalse(ConvertEx.IsEmpty("test"));
			Assert.IsTrue(ConvertEx.IsEmpty(""));
			Assert.IsTrue(ConvertEx.IsEmpty(int.MinValue));
			Assert.IsTrue(ConvertEx.IsEmpty(int.MaxValue));
			Assert.IsTrue(ConvertEx.IsEmpty(0));
			Assert.IsFalse(ConvertEx.IsEmpty(123));
			Assert.IsTrue(ConvertEx.IsEmpty(new char()));
			Assert.IsTrue(ConvertEx.IsEmpty(DateTime.MinValue));
			Assert.IsFalse(ConvertEx.IsEmpty("7/4/2008"));
			Assert.IsTrue(ConvertEx.IsEmpty(String.Empty));
			Assert.IsTrue(ConvertEx.IsEmpty(ConvertTest.Empty));
			Assert.IsFalse(ConvertEx.IsEmpty(new ConvertTest() { X = 5, Y = 10 }));
			Assert.IsTrue(ConvertEx.IsEmpty(new ConvertTest() { X = 0, Y = 0 }));
		}

		[Test]
		public void ChangeTypeTest()
		{
			object value;

			value = ConvertEx.To<DateTime>("7/4/2008");
			Assert.AreEqual(DateTime.Parse("7/4/2008"), value);

			var test = ConvertEx.To<ConvertTest>("1,2");
			Assert.AreEqual(1, test.X);
			Assert.AreEqual(2, test.Y);

			var testPt = ConvertEx.To<Point>(test);
			Assert.AreEqual(1, testPt.X);
			Assert.AreEqual(2, testPt.Y);

			test = ConvertEx.To<ConvertTest>(testPt);
			Assert.AreEqual(1, test.X);
			Assert.AreEqual(2, test.Y);
		}

		[Test]
		public void TryChangeTypeTest()
		{
			DateTime dt;
			Assert.IsTrue(ConvertEx.Try("7/4/2008", out dt));

			object obj;
			Assert.IsTrue(ConvertEx.Try("7/4/2008", typeof(DateTime), out obj));
		}

		[Test]
		public void GetDefaultEmptyTest()
		{
			object emptyValue;

			emptyValue = ConvertEx.GetDefaultEmptyValue<object>();
			Assert.IsNull(emptyValue);

			emptyValue = ConvertEx.GetDefaultEmptyValue<string>();
			Assert.IsNull(emptyValue, string.Format("The value is '{0}'.", emptyValue));

			emptyValue = ConvertEx.GetDefaultEmptyValue<int>();
			Assert.AreEqual(0, emptyValue);

			emptyValue = ConvertEx.GetDefaultEmptyValue<char>();
			Assert.AreEqual('\0', emptyValue);

			emptyValue = ConvertEx.GetDefaultEmptyValue<DateTime>();
			Assert.AreEqual(DateTime.MinValue, emptyValue);

			emptyValue = ConvertEx.GetDefaultEmptyValue<ConvertTest>();
			Assert.IsNull(emptyValue);

			emptyValue = ConvertEx.GetDefaultEmptyValue<ConvertStructTest>();
			Assert.AreEqual(ConvertStructTest.Empty, emptyValue);

		}

		[Test]
		public void InheritanceConversionTest()
		{
			var test = new ConvertTest();
			var btest = ConvertEx.To<ConvertTestBase>(test);
			Assert.AreSame(test, btest);
		}

		[Test]
		public void TypeCtorTest()
		{
			var pt = new Point(5, 10);
			var test = ConvertEx.To<ConvertTest>(pt);
			Assert.AreEqual(pt.X, test.X);
			Assert.AreEqual(pt.Y, test.Y);
		}

		private class ConvertTestBase
		{
			public int X { get; set; }
			public int Y { get; set; }

			public override bool Equals(object obj)
			{
				var other = obj as ConvertTest;
				if (other == null) return false;
				if (this.X != other.X) return false;
				if (this.Y != other.Y) return false;
				return true;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		private class ConvertTest
			: ConvertTestBase
		{
			public ConvertTest()
			{
			}

			public ConvertTest(Point pt)
			{
				X = pt.X;
				Y = pt.Y;
			}

			public static ConvertTest Parse(string s)
			{
				var test = new ConvertTest();
				var vals = s.Split(',');
				test.X = ConvertEx.To<int>(vals[0]);
				test.Y = ConvertEx.To<int>(vals[1]);
				return test;
			}

			public static readonly ConvertTest Empty = new ConvertTest() { X = 0, Y = 0 };

			public static implicit operator Point(ConvertTest test)
			{
				return new Point(test.X, test.Y);
			}

			public static explicit operator ConvertTest(Point pt)
			{
				return new ConvertTest(pt);
			}
		}

		private struct ConvertStructTest
		{
			public static readonly ConvertStructTest Empty = new ConvertStructTest() { X = 0, Y = 0 };

			public int X { get; set; }
			public int Y { get; set; }

			public override bool Equals(object obj)
			{
				var other = (ConvertStructTest)obj;
				if (this.X != other.X) return false;
				if (this.Y != other.Y) return false;
				return true;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}
	}
}
