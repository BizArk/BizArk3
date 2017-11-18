using BizArk.Core.Convert.Strategies;
using My = BizArk.Core.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace BizArk.Core.Tests
{
	[TestClass]
	public class ConversionStrategyTests
	{

		[TestMethod]
		public void DefaultValueConversionTest()
		{
			var strategy = new NullValueConversionStrategy();
			object newValue;

			Assert.IsTrue(strategy.TryConvert(null, typeof(object), out newValue));
			Assert.IsNull(newValue);

			Assert.IsTrue(strategy.TryConvert(null, typeof(int), out newValue));
			Assert.AreEqual(0, newValue);

			Assert.IsTrue(strategy.TryConvert(null, typeof(char), out newValue));
			Assert.AreEqual('\0', newValue);

			Assert.IsTrue(strategy.TryConvert(null, typeof(bool), out newValue));
			Assert.AreEqual(false, newValue);

			Assert.IsTrue(strategy.TryConvert(null, typeof(ConvertEnumTest), out newValue));
			Assert.AreEqual((ConvertEnumTest)0, newValue);

		}

		[TestMethod]
		public void AssignableFromConversionStrategyTest()
		{
			var strategy = new AssignableFromConversionStrategy();
			object newValue;
			object value;

			value = new object();
			Assert.IsTrue(strategy.TryConvert(value, typeof(object), out newValue));
			Assert.AreSame(value, newValue);

			value = new NoConvertTest();
			Assert.IsTrue(strategy.TryConvert(value, typeof(ConvertTest), out newValue));
			Assert.AreSame(value, newValue);

			Assert.IsFalse(strategy.TryConvert(new ConvertTest(0), typeof(NoConvertTest), out newValue));
			Assert.IsFalse(strategy.TryConvert(0m, typeof(NoConvertTest), out newValue));
			Assert.IsFalse(strategy.TryConvert(0m, typeof(int), out newValue));
			Assert.IsFalse(strategy.TryConvert(0, typeof(decimal), out newValue));

			// Make sure it works with enums.
			value = ConvertEnumTest.Two;
			Assert.IsTrue(strategy.TryConvert(value, typeof(ConvertEnumTest), out newValue));
			Assert.AreSame(value, newValue);

		}

		[TestMethod]
		public void ConvertibleConversionStrategyTest()
		{
			var strategy = new ConvertibleConversionStrategy();
			object newValue;

			Assert.IsFalse(strategy.TryConvert(new object(), typeof(object), out newValue));

			Assert.IsTrue(strategy.TryConvert(1, typeof(string), out newValue));
			Assert.AreEqual("1", newValue);

			Assert.IsTrue(strategy.TryConvert("2", typeof(int), out newValue));
			Assert.AreEqual(2, newValue);

			Assert.IsTrue(strategy.TryConvert(new int?(3), typeof(string), out newValue));
			Assert.AreEqual("3", newValue);

			Assert.IsTrue(strategy.TryConvert("4", typeof(int?), out newValue));
			Assert.AreEqual(4, newValue);

			Assert.IsFalse(strategy.TryConvert("Hello", typeof(int), out newValue));

		}

		[TestMethod]
		public void StringToBoolConversionStrategyTest()
		{
			var strategy = new StringToBoolConversionStrategy();
			object newValue;

			Assert.IsFalse(strategy.TryConvert(new object(), typeof(object), out newValue));
			Assert.IsFalse(strategy.TryConvert(true, typeof(string), out newValue));
			Assert.IsFalse(strategy.TryConvert(null, typeof(bool), out newValue));
			Assert.IsFalse(strategy.TryConvert(5, typeof(bool), out newValue));

			Assert.IsTrue(strategy.TryConvert("true", typeof(bool), out newValue));
			Assert.AreEqual(true, newValue);

			Assert.IsTrue(strategy.TryConvert(" true ", typeof(bool), out newValue));
			Assert.AreEqual(true, newValue);

			Assert.IsTrue(strategy.TryConvert("false", typeof(bool), out newValue));
			Assert.AreEqual(false, newValue);

			Assert.IsTrue(strategy.TryConvert("TrUe", typeof(bool), out newValue));
			Assert.AreEqual(true, newValue);

			Assert.IsTrue(strategy.TryConvert("yes", typeof(bool), out newValue));
			Assert.AreEqual(true, newValue);

			Assert.IsTrue(strategy.TryConvert("ok", typeof(bool), out newValue));
			Assert.AreEqual(true, newValue);

			Assert.IsTrue(strategy.TryConvert("hello", typeof(bool), out newValue));
			Assert.AreEqual(false, newValue);
		}

		[TestMethod]
		public void EnumConversionStrategyTest()
		{
			var strategy = new EnumConversionStrategy();
			object newValue;


			Assert.IsFalse(strategy.TryConvert(new object(), typeof(object), out newValue));


			// String to Enum

			Assert.IsFalse(strategy.TryConvert("XXX", typeof(ConvertEnumTest), out newValue));

			Assert.IsTrue(strategy.TryConvert("One", typeof(ConvertEnumTest), out newValue));
			Assert.AreEqual(ConvertEnumTest.One, newValue);

			Assert.IsTrue(strategy.TryConvert("Two", typeof(ConvertEnumTest), out newValue));
			Assert.AreEqual(ConvertEnumTest.Two, newValue);

			Assert.IsTrue(strategy.TryConvert(" THREE ", typeof(ConvertEnumTest), out newValue));
			Assert.AreEqual(ConvertEnumTest.Three, newValue);


			// Int to Enum

			Assert.IsFalse(strategy.TryConvert(5, typeof(ConvertEnumTest), out newValue));

			Assert.IsTrue(strategy.TryConvert(1, typeof(ConvertEnumTest), out newValue));
			Assert.AreEqual(ConvertEnumTest.One, newValue);

			Assert.IsTrue(strategy.TryConvert(2, typeof(ConvertEnumTest), out newValue));
			Assert.AreEqual(ConvertEnumTest.Two, newValue);

			Assert.IsTrue(strategy.TryConvert(2, typeof(ConvertEnumTest2), out newValue));
			Assert.AreEqual(ConvertEnumTest2.Two, newValue);


			// Enum to Xxx

			Assert.IsFalse(strategy.TryConvert(ConvertEnumTest.One, typeof(DateTime), out newValue));

			Assert.IsTrue(strategy.TryConvert(ConvertEnumTest.One, typeof(decimal), out newValue));
			Assert.AreEqual(1m, newValue);

			Assert.IsTrue(strategy.TryConvert(ConvertEnumTest.One, typeof(string), out newValue));
			Assert.AreEqual("One", newValue);

			Assert.IsTrue(strategy.TryConvert(ConvertEnumTest.One, typeof(int), out newValue));
			Assert.AreEqual(1, newValue);

			Assert.IsTrue(strategy.TryConvert(ConvertEnumTest.One, typeof(byte), out newValue));
			Assert.AreEqual((byte)1, newValue);

			Assert.IsTrue(strategy.TryConvert(ConvertEnumTest.One, typeof(long), out newValue));
			Assert.AreEqual((long)1, newValue);

		}

		[TestMethod]
		public void TypeConverterConversionStrategyTest()
		{
			var strategy = new TypeConverterConversionStrategy();
			object newValue;

			Assert.IsTrue(strategy.TryConvert("true", typeof(bool), out newValue));
			Assert.AreEqual(true, newValue);

			Assert.IsTrue(strategy.TryConvert("false", typeof(bool), out newValue));
			Assert.AreEqual(false, newValue);

			Assert.IsTrue(strategy.TryConvert(99, typeof(ConvertTest), out newValue));
			var ctest = newValue as ConvertTest;
			Assert.IsNotNull(ctest);
			Assert.AreEqual(99, ctest.Value);
		}

		[TestMethod]
		public void StaticMethodConversionStrategyTest()
		{
			var strategy = new StaticMethodConversionStrategy();
			object newValue;

			Assert.IsFalse(strategy.TryConvert(new ConvertTest(42), typeof(DateTime), out newValue));

			// Tests cast operator
			Assert.IsTrue(strategy.TryConvert(new ConvertTest(42), typeof(int), out newValue));
			Assert.AreEqual(42, newValue);

			// Tests cast operator
			Assert.IsTrue(strategy.TryConvert(42, typeof(ConvertTest), out newValue));
			var cval = newValue as ConvertTest;
			Assert.IsNotNull(cval);
			Assert.AreEqual(42, cval.Value);

			Assert.IsTrue(strategy.TryConvert(new ConvertTest2(42), typeof(int), out newValue));
			Assert.AreEqual(42, newValue);

			Assert.IsTrue(strategy.TryConvert(42, typeof(ConvertTest2), out newValue));
			var cval2 = newValue as ConvertTest2;
			Assert.IsNotNull(cval2);
			Assert.AreEqual(42, cval2.Value);
		}

		[TestMethod]
		public void CtorConversionStrategyTest()
		{
			var strategy = new CtorConversionStrategy();
			object newValue;

			Assert.IsFalse(strategy.TryConvert("ASDF", typeof(ConvertTest), out newValue));

			Assert.IsTrue(strategy.TryConvert(42, typeof(ConvertTest), out newValue));
			var cval = newValue as ConvertTest;
			Assert.IsNotNull(cval);
			Assert.AreEqual(42, cval.Value);
		}

		[TestMethod]
		public void ConvertMethodConversionStrategyTest()
		{
			var strategy = new ConvertMethodConversionStrategy();
			object newValue;

			Assert.IsTrue(strategy.TryConvert(new ConvertTest(42), typeof(int), out newValue));
			Assert.AreEqual(42, newValue);
		}

		[TestMethod]
		public void ByteArrayStringConversionStrategyTest()
		{
			var strategy = new ByteArrayStringConversionStrategy();
			object newValue;

			Assert.IsTrue(strategy.TryConvert("hello", typeof(byte[]), out newValue));
			var bytes = newValue as byte[];
			Assert.IsNotNull(bytes);
			Assert.AreEqual(5, bytes.Length);

			Assert.IsTrue(strategy.TryConvert(bytes, typeof(string), out newValue));
			Assert.AreEqual("hello", newValue);
		}

		[TestMethod]
		public void ByteArrayImageConversionStrategyTest()
		{
			var strategy = new ByteArrayImageConversionStrategy();
			object newValue;

			var ms = new MemoryStream(My.Resources.TestImg);
			ms.Position = 0;
			var img = Image.FromStream(ms);

			Assert.IsTrue(strategy.TryConvert(img, typeof(byte[]), out newValue));
			var bytes = newValue as byte[];
			Assert.IsNotNull(bytes);
			Assert.IsTrue(bytes.Length > 0);
			
			Assert.IsTrue(strategy.TryConvert(bytes, typeof(Image), out newValue));
			var newImg = newValue as Image;
			Assert.IsNotNull(newImg);
			Assert.AreEqual(img.Width, newImg.Width);
			Assert.AreEqual(img.Height, newImg.Height);
		}

		[TypeConverter(typeof(ConvertTestTypeConverter))]
		private class ConvertTest
		{
			public ConvertTest(int val)
			{
				Value = val;
			}

			public int Value { get; set; }

			public int ToInt()
			{
				return Value;
			}

			public static implicit operator int(ConvertTest test)
			{
				return test.Value;
			}

			public static explicit operator ConvertTest(int val)
			{
				return new ConvertTest(val);
			}
		}

		private class NoConvertTest
			: ConvertTest
		{
			public NoConvertTest()
				: base(0)
			{
			}
		}

		private class ConvertTest2
		{
			public ConvertTest2(int val)
			{
				Value = val;
			}

			public int Value { get; set; }

			public static int ToInt(ConvertTest2 val)
			{
				return val.Value;
			}

			public static ConvertTest2 ToConvert(int val)
			{
				return new ConvertTest2(val);
			}
		}

		private class ConvertTestTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(int);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value == null || value.GetType() != typeof(int))
					return base.ConvertFrom(context, culture, value);

				var ival = (int)value;
				return new ConvertTest(ival);
			}
		}

		private enum ConvertEnumTest
		{
			One = 1,
			Two = 2,
			Three = 3
		}

		private enum ConvertEnumTest2 : byte
		{
			One = 1,
			Two = 2,
			Three = 3
		}
	}
}