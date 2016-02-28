using System.Diagnostics;
using System.Xml;
using BizArk.Core.Extensions.XmlExt;
using NUnit.Framework;

namespace BizArk.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for XmlExtTest and is intended
    ///to contain all XmlExtTest Unit Tests
    ///</summary>
	[TestFixture]
    public class XmlExtTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for SetAttributeValue
        ///</summary>
        [Test]
        public void SetAttributeValueTest()
        {
            var xml = new XmlDocument();
            xml.LoadXml("<Test />");

            var root = xml.DocumentElement;
            root.SetAttributeValue("Hello", "John");
            var val = root.GetString("Hello");
            Assert.AreEqual("John", val);

            root.SetAttributeValue("Goodbye", "Sally<br>Smith");
            val = root.GetString("Goodbye");
            Assert.AreEqual("Sally<br>Smith", val);
            Debug.WriteLine(root.OuterXml);
        }

        /// <summary>
        ///A test for SetElementValue
        ///</summary>
        [Test]
        public void SetElementValueTest()
        {
            var xml = new XmlDocument();
            xml.LoadXml("<Test />");

            var root = xml.DocumentElement;
            root.SetElementValue("Hello", "John");
            var val = root.GetString("Hello");
            Assert.AreEqual("John", val);

            root.SetElementValue("Goodbye","Sally<br>Smith");
            val = root.GetString("Goodbye");
            Assert.AreEqual("Sally<br>Smith", val);
            Debug.WriteLine(root.OuterXml);
        }
    }
}
