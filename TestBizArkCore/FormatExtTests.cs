using BizArk.Core.Extensions.FormatExt;
using NUnit.Framework;

namespace BizArk.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for FormatExtTest and is intended
    ///to contain all FormatExtTest Unit Tests
    ///</summary>
[TestFixture]
    public class FormatExtTests
    {
        
        /// <summary>
        ///A test for Fmt
        ///</summary>
        [Test]
        public void FmtIntTest()
        {
            Assert.AreEqual("1", 1.Fmt());
            Assert.AreEqual("1,000", 1000.Fmt());
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [Test]
        public void FmtNullIntTest()
        {
            int? i = null;
            Assert.AreEqual("", i.Fmt());
            i = 1;
            Assert.AreEqual("1", i.Fmt());
            i = 1000;
            Assert.AreEqual("1,000", i.Fmt());
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [Test]
        public void FmtDecimalTest()
        {
            Assert.AreEqual("1.00", 1M.Fmt());
            Assert.AreEqual("1,000.00", 1000M.Fmt());
            Assert.AreEqual("1,000", 1000M.Fmt(0));
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [Test]
        public void FmtNullDecimalTest()
        {
            decimal? i = null;
            Assert.AreEqual("", i.Fmt());
            i = 1;
            Assert.AreEqual("1.00", i.Fmt());
            i = 1000;
            Assert.AreEqual("1,000.00", i.Fmt());
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [Test]
        public void FmtCurrencyTest()
        {
            Assert.AreEqual("$1.00", 1M.FmtCurrency());
            Assert.AreEqual("$1,000.00", 1000M.FmtCurrency());
            Assert.AreEqual("$1,000", 1000M.FmtCurrency(0));
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [Test]
        public void FmtNullCurrencyTest()
        {
            decimal? i = null;
            Assert.AreEqual("", i.FmtCurrency());
            i = 1;
            Assert.AreEqual("$1.00", i.FmtCurrency());
            i = 1000;
            Assert.AreEqual("$1,000.00", i.FmtCurrency());
            Assert.AreEqual("$1,000", i.FmtCurrency(0));
        }
        
        /// <summary>
        ///A test for Tmpl
        ///</summary>
        [Test]
        public void TmplTest()
        {
            var tmpl = "Hello {name}! You are {age:N0} years old.";
            Assert.AreEqual("Hello Brian! You are 39 years old.", tmpl.Tmpl(new { name = "Brian", age = 39 }));
            Assert.AreEqual("Hello King Tut! You are 3,345 years old.", tmpl.Tmpl(new { name = "King Tut", age = 3345 }));
        }

    }
}
