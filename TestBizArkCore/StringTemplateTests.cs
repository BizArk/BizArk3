using System;
using System.Diagnostics;
using NUnit.Framework;
using BizArk.Core.Util;

namespace BizArk.Core.Tests
{

	[TestFixture]
    public class StringTemplateTests
    {

        [Test]
        public void EvalTemplateTest()
        {
            var template = new StringTemplate("Hello {name}");
            template["name"] = "World";
            Assert.AreEqual("Hello World", template.ToString());

            template = new StringTemplate("{greeting} {name}");
            template["greeting"] = "Hello";
            template["name"] = "World";
            Assert.AreEqual("Hello World", template.ToString());
        }

		//[Test]
		////[DeploymentItem("BizArk.Core.dll")]
		//public void GetArgNameTest()
		//{
		//    var template = "Hello {name}".ToCharArray();
		//    int position = 6;
		//    var name = StringTemplate_Accessor.GetArgName(template, ref position);
		//    var format = StringTemplate_Accessor.GetArgFormat(template, ref position);
		//    Assert.AreEqual(12, position);
		//    Assert.AreEqual("name", name);
		//    Assert.AreEqual("", format);

		//    template = "Hello {name:test}!".ToCharArray();
		//    position = 6;
		//    name = StringTemplate_Accessor.GetArgName(template, ref position);
		//    format = StringTemplate_Accessor.GetArgFormat(template, ref position);
		//    Assert.AreEqual(17, position);
		//    Assert.AreEqual("name", name);
		//    Assert.AreEqual(":test", format);
		//    var literal = StringTemplate_Accessor.GetLiteral(template, ref position);
		//    Assert.AreEqual("!", literal);

		//    try
		//    {
		//        template = "Hello {name".ToCharArray();
		//        position = 6;
		//        name = StringTemplate_Accessor.GetArgName(template, ref position);
		//        Assert.Fail("Expected FormatException");
		//    }
		//    catch (FormatException)
		//    {
		//        // expected
		//    }
		//}

		//[Test]
		////[DeploymentItem("BizArk.Core.dll")]
		//public void GetLiteralSegmentTest()
		//{
		//    var template = "Hello {name}".ToCharArray();
		//    int position = 0;
		//    var literal = StringTemplate_Accessor.GetLiteral(template, ref position);
		//    Assert.AreEqual(6, position);
		//    Assert.AreEqual("Hello ", literal);

		//    template = @"Hello \{name}".ToCharArray();
		//    position = 0;
		//    literal = StringTemplate_Accessor.GetLiteral(template, ref position);
		//    Assert.AreEqual(13, position);
		//    Assert.AreEqual("Hello {name}", literal);

		//    template = @"Hello \{name}\".ToCharArray();
		//    position = 0;
		//    literal = StringTemplate_Accessor.GetLiteral(template, ref position);
		//    Assert.AreEqual(14, position);
		//    Assert.AreEqual(@"Hello {name}\", literal);
		//}

        [Test]
        public void DateFormatTest()
        {
            var template = new StringTemplate("Test date: {date:d}");
            var dt = new DateTime(2000, 1, 1, 12, 0, 0);
            template["date"] = dt;
            var actual = template.ToString();
            Assert.AreEqual(string.Format("Test date: {0:d}", dt), actual);
        }

        //[Test] // This can take a little bit to run so take it out
        public void PerfTest()
        {
            var count = 1000000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < count; i++)
                string.Format("Hello {0}: {1,2}", "World", i);
            sw.Stop();
            var stringFormatTime = sw.ElapsedMilliseconds; // (double)sw.ElapsedMilliseconds / (double)count;

            sw.Reset();
            sw.Start();
            var template = new StringTemplate("Hello {name}: {num,2}");
            for (int i = 0; i < count; i++)
            {
                template["name"] = "World";
                template["num"] = i;
                template.ToString();
            }
            sw.Stop();
            var textTemplateTime = sw.ElapsedMilliseconds; // (double)sw.ElapsedMilliseconds / (double)count;

            sw.Reset();
            sw.Start();
            var str = "Hello {name}: {num}";
            for (int i = 0; i < count; i++)
            {
                var str2 = str.Replace("{name}", "World");
                str2 = str.Replace("{num}", i.ToString());
            }
            sw.Stop();
            var stringReplaceTime = sw.ElapsedMilliseconds; // (double)sw.ElapsedMilliseconds / (double)count;

            Debug.WriteLine(string.Format("String.Format: {0}", stringFormatTime));
            Debug.WriteLine(string.Format("TextTemplate.ToString: {0}", textTemplateTime));
            Debug.WriteLine(string.Format("String.Replace: {0}", stringReplaceTime));
            Debug.WriteLine(string.Format("Diff: {0}", (double)textTemplateTime / (double)stringFormatTime));
        }

        public class Letter
        {

            public Letter()
            {
                Recipient = new Person();
            }

            public string Greeting { get; set; }
            public int Revision { get; set; }
            public DateTime Date { get; set; }
            public Person Recipient { get; private set; }

        }

        public class Person
        {
            public string Name { get; set; }
        }

[Test]
        public void StringReplaceExample()
        {
            var str = "{greeting} {name}!";
            str = str.Replace("{greeting}", "Hello");
            str = str.Replace("{name}", "World");
            Debug.WriteLine(str);
        }

[Test]
        public void StringTemplateExample()
        {
            var template = new StringTemplate("{greeting} {name} on {date:dddd, MMMM dd, yyyy}!");
            template["greeting"] = "Hello";
            template["name"] = "World";
            template["date"] = DateTime.Now;
            Debug.WriteLine(template.ToString());
        }

    }
}
