using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwerb.BizArk.Core.Biz.FieldValidationRules;
using Redwerb.BizArk.Core.Biz;

namespace TestBizArkCore
{
    /// <summary>
    /// Summary description for BizTest
    /// </summary>
    [TestClass]
    public class BizTest
    {
        public BizTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        //[TestMethod]
        //public void SimpleBizRecordTest()
        //{
        //    var bizObj = new TestBizObject();
        //    var record = (TestRecord)bizObj.CreateNew();

        //    record["Test1"].Value = "Test";
        //    Assert.AreEqual("Test", record["Test1"].Value);

        //    record["Test1"].ReadOnly = true;
        //    try
        //    {
        //        record["Test1"].Value = "Another Test";
        //        Assert.Fail("Expected Redwerb.BizArk.Core.Biz.FieldReadOnlyException.");
        //    }
        //    catch (Redwerb.BizArk.Core.Biz.FieldReadOnlyException)
        //    {
        //        // Expected.
        //    }
        //    Assert.AreEqual("Test", record["Test1"].Value);
        //}

        [TestMethod]
        public void BasicValueValidationRuleTest()
        {
            var bizObj = new TestBizRules();
            var record = new TestRecord();
            FieldValidationResults res;

            res = bizObj.ValidateValue(record, "Test1", "Hello");
            AssertEx.NoError(res);

            res = bizObj.ValidateValue(record, "Test1", "");
            AssertEx.NoError(res);
            Assert.IsNull(res.ProposedValue);

            try
            {
                res = bizObj.ValidateValue(record, "Test4", "Hello");
                Assert.Fail("Expected FieldReadOnlyException to be thrown.");
            }
            catch (FieldReadOnlyException)
            {
                // Expected.
            }
        }

        [TestMethod]
        public void ValueInSetValidationRuleTest()
        {
            var bizObj = new TestBizRules();

            var rule = new ValueInSetValidationRule<string>();
            rule.ValidValues = new List<string>() { "TestA", "TestB" };
            bizObj["Test1"].Rules.Add(rule);
            var record = new TestRecord();

            var res = bizObj.ValidateValue(record, "Test1", "TestA");
            AssertEx.NoError(res);

            res = bizObj.ValidateValue(record, "Test1", "TestB");
            AssertEx.NoError(res);

            res = bizObj.ValidateValue(record, "Test1", "TestC");
            AssertEx.HasError(res, ValueInSetValidationRule<string>.MSG.ValueNotInSet);
        }

        [TestMethod]
        public void ValueInRangeValidationRuleTest()
        {
            var bizObj = new TestBizRules();
            var record = new TestRecord();
            FieldValidationResults res;

            res = bizObj.ValidateValue(record, "Test2", 5);
            AssertEx.NoError(res);

            res = bizObj.ValidateValue(record, "Test2", 0);
            AssertEx.NoError(res);

            res = bizObj.ValidateValue(record, "Test2", 10);
            AssertEx.NoError(res);

            res = bizObj.ValidateValue(record, "Test2", -1);
            AssertEx.HasError(res, ValueInRangeValidationRule<int>.MSG.ValueTooSmall);

            res = bizObj.ValidateValue(record, "Test2", 11);
            AssertEx.HasError(res, ValueInRangeValidationRule<int>.MSG.ValueTooGreat);

            res = bizObj.ValidateValue(record, "Test3", new DateTime(2000, 6, 1));
            AssertEx.NoError(res);

            res = bizObj.ValidateValue(record, "Test3", new DateTime(2000, 1, 1));
            AssertEx.NoError(res);

            res = bizObj.ValidateValue(record, "Test3", new DateTime(2000, 12, 31));
            AssertEx.NoError(res);

            res = bizObj.ValidateValue(record, "Test3", new DateTime(1999, 12, 31));
            AssertEx.HasError(res, ValueInRangeValidationRule<DateTime>.MSG.ValueTooSmall);

            res = bizObj.ValidateValue(record, "Test3", new DateTime(2001, 1, 1));
            AssertEx.HasError(res, ValueInRangeValidationRule<DateTime>.MSG.ValueTooGreat);

        }
    }

    internal class TestBizRules
        : BizRules
    {
        public TestBizRules()
        {
            RegisterFields(new BizRulesField("Test1", typeof(string)), new BizRulesField("Test2", typeof(string)), new BizRulesField("Test3", typeof(string)), new BizRulesField("Test4", typeof(string)));

            var rules = this["Test2"].Rules;
            var intRngRule = new ValueInRangeValidationRule<int>();
            intRngRule.MinValue = 0;
            intRngRule.MaxValue = 10;
            rules.Add(intRngRule);

            rules = this["Test3"].Rules;
            var dtRngRule = new ValueInRangeValidationRule<DateTime>();
            dtRngRule.MinValue = new DateTime(2000, 1, 1);
            dtRngRule.MaxValue = new DateTime(2000, 12, 31);
            rules.Add(dtRngRule);
        }
    }

    internal class TestRecord
    {
        public string Test1 { get; set; }
        public int Test2 { get; set; }
        public DateTime Test3 { get; set; }
    }
}
