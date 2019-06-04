using BizArk.ConsoleApp.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BizArk.ConsoleApp.Tests
{
    [TestClass]
    public class ConsoleAppConfigAttributeTest
    {
        [TestMethod]
        public void EvaluateNonConfigAttributeTest()
        {
            //Arrange
            var config = new Mock<IConfigurationProvider>();
            //Act
            var app = new TestConsoleApp(config.Object);
            //Assert
            Assert.IsNull(app.Test);
            Assert.IsNull(app.SpecificValue);
            Assert.IsNull(app.NonConfigProperty);
        }
        [TestMethod]
        public void EvaluateConfigAttributeTestStatic()
        {
            //Arrange
            var config = new Mock<IConfigurationProvider>();
            config.Setup(c => c.GetSetting(It.IsAny<string>())).Returns("static");
            //Act
            var app = new TestConsoleApp(config.Object);
            //Assert
            Assert.AreEqual("static", app.Test);
            Assert.AreEqual("static", app.SpecificValue);
            Assert.IsNull(app.NonConfigProperty);
        }
        [TestMethod]
        public void EvaluateConfigAttributeTestDynamic()
        {
            var config = new Mock<IConfigurationProvider>();
            config.Setup(c => c.GetSetting(It.IsAny<string>())).Returns((string input) => String.Concat(input, "value"));
           
            //act
            var app = new TestConsoleApp(config.Object);

            //assert
            Assert.AreEqual("Testvalue", app.Test);
            Assert.AreEqual("SpecificValuevalue", app.SpecificValue);
            Assert.IsNull(app.NonConfigProperty);
        }
        [TestMethod]
        public void EvaluateConfigAttributeTestMultiple()
        {
            var config = new Mock<IConfigurationProvider>();
            config.Setup(c => c.GetSetting(It.IsAny<string>())).Returns((string input) => String.Concat(input, "value"));
            config.Setup(c => c.GetSetting(It.Is<String>(s => s == "SpecificValue"))).Returns("SpecificResult");
            //act
            var app = new TestConsoleApp(config.Object);

            //assert
            Assert.AreEqual("Testvalue", app.Test);
            Assert.AreEqual("SpecificResult", app.SpecificValue);
            Assert.IsNull(app.NonConfigProperty);
        }
    }
    public class TestConsoleApp : BaseConsoleApp
    {
        public TestConsoleApp(IConfigurationProvider config) : base(config)
        {
        }

        public override int Start()
        {
            throw new NotImplementedException();
        }

        [Config]
        public string Test { get; set; }

        [Config]
        public string SpecificValue { get; set; }

        public string NonConfigProperty { get; set; }

    }

}
