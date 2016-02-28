using System;
using System.Data;
using System.Drawing;
using System.Threading;
using BizArk.Core.Convert.Strategies;
using BizArk.Core.Tests.Properties;
using NUnit.Framework;

namespace BizArk.Core.Tests
{
    [TestFixture]
    public class ConversionStrategyTests
    {
        [Test]
        public void ByteArrayImageConversionStrategyTest()
        {
            var strategy = new ByteArrayImageConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (string), typeof (string)));
            Assert.IsTrue(strategy.CanConvert(typeof (byte[]), typeof (Image)));
            Assert.IsTrue(strategy.CanConvert(typeof (Image), typeof (byte[])));

            var bytes =
                strategy.Convert(typeof (Image), typeof (byte[]), Resources.TestImg, Thread.CurrentThread.CurrentCulture)
                    as byte[];
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);

            var img =
                strategy.Convert(typeof (byte[]), typeof (Image), bytes, Thread.CurrentThread.CurrentCulture) as Image;
            Assert.IsNotNull(img);
            Assert.AreEqual(Resources.TestImg.Width, img.Width);
            Assert.AreEqual(Resources.TestImg.Height, img.Height);
        }

        [Test]
        public void ByteArrayStringConversionStrategyTest()
        {
            var strategy = new ByteArrayStringConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (int), typeof (int)));
            Assert.IsTrue(strategy.CanConvert(typeof (byte[]), typeof (string)));
            Assert.IsTrue(strategy.CanConvert(typeof (string), typeof (byte[])));

            var bytes =
                strategy.Convert(typeof (string), typeof (byte[]), "hello", Thread.CurrentThread.CurrentCulture) as
                    byte[];
            Assert.IsNotNull(bytes);
            Assert.AreEqual(5, bytes.Length);

            var str = strategy.Convert(typeof (byte[]), typeof (string), bytes, Thread.CurrentThread.CurrentCulture);
            Assert.AreEqual("hello", str);
        }

        [Test]
        public void ConvertibleConversionStrategyTest()
        {
            var strategy = new ConvertibleConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsTrue(strategy.CanConvert(typeof (int), typeof (string)));
            Assert.IsTrue(strategy.CanConvert(typeof (string), typeof (int)));
            Assert.IsTrue(strategy.CanConvert(typeof (int?), typeof (string)));
            Assert.IsTrue(strategy.CanConvert(typeof (string), typeof (int?)));

            var intVal = (int) strategy.Convert(typeof (string), typeof (int), "5", Thread.CurrentThread.CurrentCulture);
            Assert.AreEqual(5, intVal);

            var strVal =
                strategy.Convert(typeof (int), typeof (string), 5, Thread.CurrentThread.CurrentCulture) as string;
            Assert.AreEqual("5", strVal);

            int? nullIntVal = null;
            nullIntVal =
                (int?) strategy.Convert(typeof (string), typeof (int?), "5", Thread.CurrentThread.CurrentCulture);
            Assert.AreEqual(5, nullIntVal);

            strVal =
                strategy.Convert(typeof (int), typeof (string), nullIntVal, Thread.CurrentThread.CurrentCulture) as
                    string;
            Assert.AreEqual("5", strVal);
        }

        [Test]
        public void ConvertMethodConversionStrategyTest()
        {
            var strategy = new ConvertMethodConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsTrue(strategy.CanConvert(typeof (ConvertTest), typeof (int)));
            Assert.IsFalse(strategy.CanConvert(typeof (int), typeof (ConvertTest)));

            var val = strategy.Convert(typeof (ConvertTest), typeof (int), new ConvertTest(42), null);
            Assert.AreEqual(42, val);
        }

        [Test]
        public void CtorConversionStrategyTest()
        {
            var strategy = new CtorConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsFalse(strategy.CanConvert(typeof (ConvertTest), typeof (int)));
            Assert.IsTrue(strategy.CanConvert(typeof (int), typeof (ConvertTest)));

            var val = strategy.Convert(typeof (int), typeof (ConvertTest), 42, null) as ConvertTest;
            Assert.IsNotNull(val);
            Assert.AreEqual(42, val.Value);
        }

        [Test]
        public void DefaultValueConversionStrategyTest()
        {
            var strategy = new DefaultValueConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsTrue(strategy.CanConvert(typeof (ConvertTest), null));
            Assert.IsTrue(strategy.CanConvert(null, typeof (ConvertTest)));
            Assert.IsTrue(strategy.CanConvert(null, typeof (int)));

            var val = strategy.Convert(typeof (ConvertTest), null, new ConvertTest(1), null);
            Assert.IsNull(val);

            val = strategy.Convert(null, null, new ConvertTest(1), null);
            Assert.IsNull(val);

            val = strategy.Convert(null, typeof (int), new ConvertTest(1), null);
            Assert.AreEqual(int.MinValue, val);

            val = strategy.Convert(null, typeof (int?), new ConvertTest(1), null);
            Assert.IsNull(val);
        }

        [Test]
        public void EnumConversionStrategyTest()
        {
            var strategy = new EnumConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsTrue(strategy.CanConvert(typeof (ConvertEnumTest), typeof (int)));
            Assert.IsTrue(strategy.CanConvert(typeof (int), typeof (ConvertEnumTest)));
            Assert.IsTrue(strategy.CanConvert(typeof (ConvertEnumTest), typeof (string)));
            Assert.IsTrue(strategy.CanConvert(typeof (string), typeof (ConvertEnumTest)));
            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (ConvertEnumTest)));

            var val = strategy.Convert(typeof (ConvertEnumTest), typeof (int), ConvertEnumTest.Two, null);
            Assert.AreEqual(2, val);

            val = strategy.Convert(typeof (ConvertEnumTest), typeof (string), ConvertEnumTest.Two, null);
            Assert.AreEqual("Two", val);

            val = strategy.Convert(typeof (int), typeof (ConvertEnumTest), 2, null);
            Assert.AreEqual(ConvertEnumTest.Two, val);

            val = strategy.Convert(typeof (string), typeof (ConvertEnumTest), "Two", null);
            Assert.AreEqual(ConvertEnumTest.Two, val);
        }

        [Test]
        public void NoConvertConversionStrategyTest()
        {
            var strategy = new NoConvertConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (ConvertTest), typeof (NoConvertTest)));
            Assert.IsTrue(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsTrue(strategy.CanConvert(typeof (NoConvertTest), typeof (ConvertTest)));

            // No further tests necessary, the Convert method always returns the value sent in.
        }

        [Test]
        public void SqlDBTypeConversionStrategyTest()
        {
            var strategy = new SqlDBTypeConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsTrue(strategy.CanConvert(typeof (SqlDbType), typeof (Type)));
            Assert.IsTrue(strategy.CanConvert(typeof (Type), typeof (SqlDbType)));

            var val = strategy.Convert(typeof (SqlDbType), typeof (Type), SqlDbType.Int, null);
            Assert.AreEqual(typeof (int), val);

            val = strategy.Convert(typeof (Type), typeof (SqlDbType), typeof (int), null);
            Assert.AreEqual(SqlDbType.Int, val);
        }

        [Test]
        public void StaticMethodConversionStrategyTest()
        {
            var strategy = new StaticMethodConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsTrue(strategy.CanConvert(typeof (ConvertTest), typeof (int)));
            Assert.IsTrue(strategy.CanConvert(typeof (int), typeof (ConvertTest)));
            Assert.IsFalse(strategy.CanConvert(typeof (ConvertTest), typeof (string)));
            Assert.IsFalse(strategy.CanConvert(typeof (string), typeof (ConvertTest)));

            var val = strategy.Convert(typeof (ConvertTest), typeof (int), new ConvertTest(42), null);
            Assert.AreEqual(42, val);

            var cval = strategy.Convert(typeof (int), typeof (ConvertTest), 42, null) as ConvertTest;
            Assert.IsNotNull(cval);
            Assert.AreEqual(42, cval.Value);

            val = strategy.Convert(typeof (ConvertTest2), typeof (int), new ConvertTest2(42), null);
            Assert.AreEqual(42, val);

            var cval2 = strategy.Convert(typeof (int), typeof (ConvertTest2), 42, null) as ConvertTest2;
            Assert.IsNotNull(cval2);
            Assert.AreEqual(42, cval2.Value);
        }

        [Test]
        public void StringToBoolConversionStrategyTest()
        {
            var strategy = new StringToBoolConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsFalse(strategy.CanConvert(typeof (bool), typeof (string)));
            Assert.IsTrue(strategy.CanConvert(typeof (string), typeof (bool)));

            var val = strategy.Convert(typeof (string), typeof (bool), "true", null);
            Assert.AreEqual(true, val);

            val = strategy.Convert(typeof (string), typeof (bool), "false", null);
            Assert.AreEqual(false, val);

            val = strategy.Convert(typeof (string), typeof (bool), "TrUe", null);
            Assert.AreEqual(true, val);

            val = strategy.Convert(typeof (string), typeof (bool), "yes", null);
            Assert.AreEqual(true, val);

            val = strategy.Convert(typeof (string), typeof (bool), "ok", null);
            Assert.AreEqual(true, val);
        }

        [Test]
        public void TypeConverterConversionStrategyTest()
        {
            var strategy = new TypeConverterConversionStrategy();

            Assert.IsFalse(strategy.CanConvert(typeof (object), typeof (object)));
            Assert.IsTrue(strategy.CanConvert(typeof (bool), typeof (string)));
            Assert.IsTrue(strategy.CanConvert(typeof (string), typeof (bool)));

            var val = strategy.Convert(typeof (string), typeof (bool), "true", null);
            Assert.AreEqual(true, val);

            val = strategy.Convert(typeof (string), typeof (bool), "false", null);
            Assert.AreEqual(false, val);
        }

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

        private enum ConvertEnumTest
        {
            One = 1,
            Two = 2,
            Three = 3
        }
    }
}