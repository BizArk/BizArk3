using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BizArk.Core.CmdLine;
using BizArk.Core.DataAnnotations;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Extensions.WebExt;
using BizArk.Core.Util;
using FluentAssertions;
using NUnit.Framework;

namespace BizArk.Core.Tests.CmdLine
{
    [TestFixture]
    public class CmdLineObjectTests
    {
        #region GetHelpText

        [Test]
        public void InitializeFromCmdLine_CmdLineObjectWithColonAssignmentDelimiter_ArgumentContainsCorrectValue()
        {
            // Act
            var cmdLine = new CmdLineObjectWithColonAssignmentDelimiter();
            cmdLine.InitializeFromCmdLine(new []{@"-path:""d:\1\"});

            // Assert
            cmdLine.Path.Should().Be(@"d:\1\");
        }

        [Test]
        public void GetHelpText_CmdLineWithEnumProperty_HelpTextContainsAllEnumValues()
        {
            // Act
            var cmdLine = new CmdLineObjectWithEnumPropertyWithManyValues();
            cmdLine.Initialize();
            var helpText = cmdLine.GetHelpText(80);

            // Assert
            helpText.Should().Be(@"Command-line options.

 [/?[-]]

/Country     Country
             Default Value: Abkhazia
             Possible Values: [Abkhazia, Albania, Afghanistan, Algeria, Andorra,
             Angola, Anguilla, Antigua, Argentina, Armenia, Aruba, Australia,
             Austria, Azerbaijan]
/?           Displays command-line usage information.
             Default Value: False
");
        }

        [Test]
        public void GetHelpText_ColonAssignmentDelimiterCmdLineObject_UsageContainsSpace()
        {
            // Arrange
            var cmdLineObj = new ColonAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] { "/Name", "John" });

            //Assert
            string helpText = cmdLineObj.GetHelpText(80);
            Assert.That(helpText, Is.EqualTo(@"Command-line options.

 [/Name:<Name>] [/?[-]]

/Name      The name of the user
/Count     Count
           Default Value: 0
/?         Displays command-line usage information.
           Default Value: False
"));
        }

        [Test]
        public void GetHelpText_DefaultAssignmentDelimiterOneArgument_UsageContainsSpace()
        {
            // Arrange
            var cmdLineObj = new DefaultAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] { "/Name", "John" });
            string helpText = cmdLineObj.GetHelpText(80);

            //Assert
            helpText.Should().Be(@"Command-line options.

 [/Name <Name>] [/?[-]]

/Name     The name of the user
/?        Displays command-line usage information.
          Default Value: False
");
        }

        [Test]
        public void GetHelpText_EnumPropertyIncmdLineObject_EnumValuesDisplayed()
        {
            var cmdLineObject = new CmdLineObjectWithEnumProperty();
            cmdLineObject.InitializeFromCmdLine(new[] { "/?" });
            string helpText = cmdLineObject.GetHelpText(80);

            helpText.Should().Be(@"Command-line options.

 [/?[-]]

/Car      Car
          Default Value: Tesla
          Possible Values: [Tesla, Ferrari, Lamborghini, Kia]
/?        Displays command-line usage information.
          Default Value: False
");
        }

        [Test]
        public void HelpTextWrappingTest()
        {
            var args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine();
            var vals = new[] { 1, 2 };
            Debug.WriteLine(args.GetHelpText(40));
            Assert.IsTrue(args.IsValid());
        }
        #endregion GetHelpText

        #region InitializeFromCmdLine
        [Test]
        public void InitializeFromCmdLine_StringArray_PropertyContainsTheWholeValue()
        {
            // full alias
            var cmdLine = new CmdLineObjectWithStringArray();
            cmdLine.InitializeFromCmdLine(new[] { @"-Names:1;2" });

            cmdLine.Names.Should().BeEquivalentTo(new[] {"1", "2"});
        }

        [Test]
        public void InitializeFromCmdLine_ValueContainsDollarSign_PropertyContainsTheWholeValue()
        {
            // full alias
            var cmdLine = new CmdLineObjectWithPassword();
            cmdLine.InitializeFromCmdLine(new[] {@"/Password=qaz$12"});

            cmdLine.Password.Should().Be("qaz$12");
        }

        [Test]
        public void InitializeFromCmdLine_Alias_Initialized()
        {
            // full alias
            var cmdLine = new MyTestCmdLineObject();
            cmdLine.InitializeFromCmdLine(new[] {"/crap", "Christine"});
            Assert.AreEqual("Christine", cmdLine.StuffILike[0]);

            // partial alias
            cmdLine = new MyTestCmdLineObject();
            cmdLine.InitializeFromCmdLine(new[] {"/c", "Christine"});
            Assert.AreEqual("Christine", cmdLine.StuffILike[0]);
        }

        [Test]
        public void InitializeFromCmdLine_ArgumentValidationTest()
        {
            var args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/NumberOfScoops", "2");
            Assert.AreEqual(2, args.NumberOfScoops);
            Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/NumberOfScoops", "chocolate");
            Assert.AreEqual(1, args.NumberOfScoops);
            Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.GetHelpText(200));

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/FavoriteNumbers", "1", "2", "3");
            Assert.AreEqual(3, args.FavoriteNumbers.Length);
            Assert.AreEqual(1, args.FavoriteNumbers[0]);
            Assert.AreEqual(2, args.FavoriteNumbers[1]);
            Assert.AreEqual(3, args.FavoriteNumbers[2]);
            Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/FavoriteNumbers", "Red", "Green", "Blue");
            Assert.AreEqual(1, args.NumberOfScoops);
            Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.GetHelpText(200));

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/FavoriteCar", "Ford");
            Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.GetHelpText(200));

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/NumberOfScoops", "5");
            Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.GetHelpText(200));
        }

        [Test]
        public void InitializeFromCmdLine_DefaultPropTest()
        {
            MyTestCmdLineObject3 target;
            string[] args;
            string[] expected;

            target = new MyTestCmdLineObject3();
            args = new[] { "Brian", "Christine", "Abrian", "Brooke" };
            target.InitializeFromCmdLine(args);
            expected = new[] { "Brian", "Christine", "Abrian", "Brooke" };
            AssertEx.AreEqual(expected, target.Family);

            target = new MyTestCmdLineObject3();
            args = new[] { "Brian", "Christine", "Abrian", "Brooke", "/D", "Brian" };
            target.InitializeFromCmdLine(args);
            expected = new[] { "Brian", "Christine", "Abrian", "Brooke" };
            AssertEx.AreEqual(expected, target.Family);
            Assert.AreEqual("Brian", target.Father);
        }

        [Test]
        public void CmdLineDescriptionTest()
        {
            string test = "This is a test\ntest  test test";
            string[] lines = test.Lines();
            foreach (string line in lines)
                Debug.WriteLine(line);

            var args = new MyTestCmdLineObject();
            args.InitializeEmpty();
            Debug.WriteLine(args.GetHelpText(50));
        }

        [Test]
        public void InitializeEmpty_DuplicateArgumentsTest()
        {
            var cmdLine = new AliasesWithDifferentCaseTestCmdLineObject();
            AssertEx.Throws(typeof(CmdLineArgumentException), () => { cmdLine.InitializeEmpty(); });
        }

        [Test]
        public void InitializeFromCmdLine_ArgsContainsArrayOfEnum_EnumValuesAreParsed()
        {
            var objectWithMultipleEnumProperty = new CmdLineObjectWithMultipleEnumProperty();

            objectWithMultipleEnumProperty.InitializeFromCmdLine(new[] {@"/Cars=Tesla,Ferrari,Lamborghini"});

            Assert.That(objectWithMultipleEnumProperty.Cars,
                Is.EquivalentTo(new[] {Car.Tesla, Car.Ferrari, Car.Lamborghini}));
        }

        [Test]
        public void
            InitializeFromCmdLine_DefaultAssignmentDelimiterCmdLineObjectValueContainsDoubleQuotes_InitializedFromArgs()
        {
            // Arrange
            var cmdLineObj = new DefaultAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] {@"/Name", @"""John Smith"""});

            //Assert
            Assert.That(cmdLineObj.Name, Is.EqualTo("John Smith"));
        }

        [Test]
        public void
            InitializeFromCmdLine_EmptyPrefixAndAssignmentDelimiterEqualitySignValueSurroundedByDoubleQuotes_FullPathWithoutDoubleQuotes
            ()
        {
            var cmdLineObjectWithEmptyPrefix = new CmdLineObjectWithEmptyPrefixAssignmentDelimiterEqualitySign();

            cmdLineObjectWithEmptyPrefix.InitializeFromCmdLine(@"FullPath=""C:\Program Files\bizark\""");

            cmdLineObjectWithEmptyPrefix.FullPath.Should().Be(@"C:\Program Files\bizark\");
        }

        [Test]
        public void InitializeFromCmdLine_EmptyPrefixAndAssignmentDelimiterEqualitySign_CmdLineArgumentParsed()
        {
            var cmdLineObjectWithEmptyPrefix = new CmdLineObjectWithEmptyPrefixAssignmentDelimiterEqualitySign();

            cmdLineObjectWithEmptyPrefix.InitializeFromCmdLine("Car=Lamborghini");

            cmdLineObjectWithEmptyPrefix.Car.Should().Be(Car.Lamborghini);
        }

        [Test]
        public void InitializeFromCmdLine_EmptyPrefixArray_CmdLineArgumentParsed()
        {
            var cmdLineObjectWithEmptyPrefix = new CmdLineObjectWithEmptyPrefix();

            cmdLineObjectWithEmptyPrefix.InitializeFromCmdLine("Cars", "Tesla,Ferrari,Lamborghini");

            cmdLineObjectWithEmptyPrefix.Cars.ShouldBeEquivalentTo(new[] {Car.Tesla, Car.Ferrari, Car.Lamborghini});
        }

        [Test]
        public void InitializeFromCmdLine_EmptyPrefix_CmdLineArgumentParsed()
        {
            var cmdLineObjectWithEmptyPrefix = new CmdLineObjectWithEmptyPrefix();

            cmdLineObjectWithEmptyPrefix.InitializeFromCmdLine("Car", "Lamborghini");

            cmdLineObjectWithEmptyPrefix.Car.Should().Be(Car.Lamborghini);
        }

        [Test]
        public void
            InitializeFromFullCmdLine_AssignmentDelimiterColonArrayArgumentsValueContainsDoubleQuotes_InitializedFromArgs
            ()
        {
            // Arrange
            var cmdLineObj = new ColonAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] {@"/Name:""John Smith"""});

            //Assert
            Assert.That(cmdLineObj.Name, Is.EqualTo("John Smith"));
        }

        [Test]
        public void InitializeFromFullCmdLine_AssignmentDelimiterColonArrayArguments_InitializedFromArgs()
        {
            // Arrange
            var cmdLineObj = new ColonAssignmentDelimiterArrayArgCmndLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] {"/Names:John", "Maria"});

            //Assert
            Assert.That(cmdLineObj.Names, Is.EquivalentTo(new[] {"John", "Maria"}));
        }

        [Test]
        public void InitializeFromFullCmdLine_AssignmentDelimiterColonOneArgument_InitializedFromArgs()
        {
            // Arrange
            var cmdLineObj = new ColonAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] {"/Name:John"});

            //Assert
            Assert.That(cmdLineObj.Name, Is.EqualTo("John"));
        }

        [Test]
        public void InitializeFromFullCmdLine_AssignmentDelimiterColonTwoArguments_InitializedFromArgs()
        {
            // Arrange
            var cmdLineObj = new ColonAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] {"/Name:John", "/Count:3"});

            //Assert
            Assert.That(cmdLineObj.Name, Is.EqualTo("John"));
            Assert.That(cmdLineObj.Count, Is.EqualTo(3));
        }

        [Test]
        public void InitializeFromFullCmdLine_AssignmentDelimiterSpace_InitializedFromArgs()
        {
            // Arrange
            var cmdLineObj = new DefaultAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] {"/Name", "John"});

            //Assert
            Assert.That(cmdLineObj.Name, Is.EqualTo("John"));
        }

        [Test]
        public void
            InitializeFromFullCmdLine_DefaultAssignmentDelimiterCmdLineObjectValueContainsDoubleQuotes_InitializedFromArgs
            ()
        {
            // Arrange
            var cmdLineObj = new DefaultAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] {@"/Name", @"John Smith"});

            //Assert
            Assert.That(cmdLineObj.Name, Is.EqualTo("John Smith"));
        }

        [Test]
        public void LongArgPrefixTest()
        {
            var args = new MyTestCmdLineObject6();
            Assert.AreEqual("--", args.Options.ArgumentPrefix);
            args.InitializeFromCmdLine("--D", "Brian");
            Assert.AreEqual("Brian", args.Father);
        }
        #endregion

        [Test]
        public void InitializeTest()
        {
            MyTestCmdLineObject target;
            string[] args;

            target = new MyTestCmdLineObject();
            args = new string[] {};
            target.InitializeFromCmdLine(args);
            Assert.IsNull(target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new[] {"/H", "Hi Brian!"};
            target.InitializeFromCmdLine(args);
            Assert.That(target.Hello, Is.EqualTo("Hi Brian!"));
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new[] {"/h", "Hi Brian!"};
            target.InitializeFromCmdLine(args);
            Assert.AreEqual("Hi Brian!", target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new[] {"/h", "Hi Brian!", "/G", "Goodbye Christine."};
            target.InitializeFromCmdLine(args);
            Assert.AreEqual("Hi Brian!", target.Hello);
            Assert.AreEqual("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new[] {"Hi Brian!", "/G", "Goodbye Christine."};
            target.InitializeFromCmdLine(args);
            Assert.AreEqual("Hi Brian!", target.Hello);
            Assert.AreEqual("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject2();
            args = new[] {"Goodbye Christine."};
            target.InitializeFromCmdLine(args);
            Assert.IsNull(target.Hello);
            Assert.AreEqual("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new[] {"TEST", "/h", "Hi Brian!", "/G", "Goodbye Christine.", "TEST"};
            target.InitializeFromCmdLine(args);
            Assert.AreEqual("Hi Brian!", target.Hello);
            Assert.AreEqual("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new[] {"/I"};
            target.InitializeFromCmdLine(args);
            Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new[] {"/I-"};
            target.InitializeFromCmdLine(args);
            Assert.IsFalse(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new[] {"/I", "Yes"};
            target.InitializeFromCmdLine(args);
            Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new[] {"/I", "No"};
            target.InitializeFromCmdLine(args);
            Assert.IsFalse(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new[] {"/S", "Cars", "Computers", "Food"};
            target.InitializeFromCmdLine(args);
            string[] expectedStuff = {"Cars", "Computers", "Food"};
            AssertEx.AreEqual(expectedStuff, target.StuffILike);
        }

        [Test]
        public void MultipleDefaultPropTest()
        {
            MyTestCmdLineObject4 target;
            string[] args;
            string[] expected;

            target = new MyTestCmdLineObject4();
            args = new[] {"Brian", "Christine", "/c", "Abrian", "Brooke"};
            target.InitializeFromCmdLine(args);
            Assert.AreEqual("Christine", target.Mother);
            Assert.AreEqual("Brian", target.Father);
            expected = new[] {"Abrian", "Brooke"};
            AssertEx.AreEqual(expected, target.Children);

            target = new MyTestCmdLineObject4();
            args = new[] {"Brian", "/c", "Abrian", "Brooke"};
            target.InitializeFromCmdLine(args);
            Assert.AreEqual(null, target.Mother);
            Assert.AreEqual("Brian", target.Father);
            expected = new[] {"Abrian", "Brooke"};
            AssertEx.AreEqual(expected, target.Children);

            Debug.WriteLine(target.GetHelpText(200));
        }

        [Test]
        public void OptionsAttTest()
        {
            var args = new MyTestCmdLineObject5();
            Assert.AreEqual("+", args.Options.ArgumentPrefix);
            args.InitializeFromCmdLine("+D", "Brian");
            Assert.AreEqual("Brian", args.Father);
        }

        [Test]
        public void OverrideValidationTest()
        {
            var args1 = new MyTestCmdLineObject7();
            args1.InitializeFromCmdLine("/v", "true");
            Assert.AreEqual(true, args1.IsValidProp);
            Assert.IsTrue(args1.IsValid());

            var args2 = new MyTestCmdLineObject7();
            args2.InitializeFromCmdLine("/v", "false");
            Assert.AreEqual(false, args2.IsValidProp);
            Assert.IsTrue(!args2.IsValid());
        }

        /// <summary>
        ///     A test for Initialize
        /// </summary>
        [Test]
        public void PartialNameTest()
        {
            MyTestCmdLineObject target;
            string[] args;

            target = new MyTestCmdLineObject();
            args = new[] {"/Hell", "Hi Brian"};
            target.InitializeFromCmdLine(args);
            Assert.AreEqual("Hi Brian", target.Hello);
        }

        [Test]
        public void QueryStringTest()
        {
            MyTestCmdLineObject target;
            string args;

            target = new MyTestCmdLineObject();
            args = "";
            target.InitializeFromQueryString(args);
            Assert.IsNull(target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = string.Format("H={0}", "Hi Brian!".UrlEncode());
            target.InitializeFromQueryString(args);
            Assert.AreEqual("Hi Brian!", target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = string.Format("h={0}", "Hi Brian!".UrlEncode());
            target.InitializeFromQueryString(args);
            Assert.AreEqual("Hi Brian!", target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = string.Format("h={0}&G={1}", "Hi Brian!".UrlEncode(), "Goodbye Christine.".UrlEncode());
            target.InitializeFromQueryString(args);
            Assert.AreEqual("Hi Brian!", target.Hello);
            Assert.AreEqual("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = "i=true";
            target.InitializeFromQueryString(args);
            Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = "I=false";
            target.InitializeFromQueryString(args);
            Assert.IsFalse(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = "i=Yes";
            target.InitializeFromQueryString(args);
            Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = "i=No";
            target.InitializeFromQueryString(args);
            Assert.IsFalse(target.DoesLikeIceCream);
        }

        [Test]
        public void SaveAndRestoreTest()
        {
            string settingsPath = @"C:\garb\Test.xml";
            if (!Directory.Exists(@"C:\garb"))
                Directory.CreateDirectory(@"C:\garb");
            var cmdLine = new MyTestCmdLineObject();
            var stuffILike = new[] {"Cookies", "Candy", "Ice Cream"};
            Assert.AreNotEqual("Hi", cmdLine.Hello);
            cmdLine.InitializeFromCmdLine("/H", "Hi", "/S", "Cookies", "Candy", "Ice Cream");
            Assert.AreEqual("Hi", cmdLine.Hello);
            AssertEx.AreEqual(stuffILike, cmdLine.StuffILike);
            cmdLine.SaveToXml(settingsPath);
            Assert.IsTrue(File.Exists(settingsPath));

            cmdLine = new MyTestCmdLineObject();
            Assert.AreNotEqual("Hi", cmdLine.Hello);
            cmdLine.RestoreFromXml(settingsPath);
            Assert.AreEqual("Hi", cmdLine.Hello);
            AssertEx.AreEqual(stuffILike, cmdLine.StuffILike);
        }

        [Test]
        public void UsageTest()
        {
            var args = new MyTestCmdLineObject();
            args.InitializeEmpty();
            Debug.WriteLine(args.Options.Usage);
            Assert.AreEqual("TESTAPP [/H <Hello>] [/FavoriteCar <FavoriteCar>] [/?[-]]", args.Options.Usage);
        }

        [Test]
        public void ValidateSetTest()
        {
            var args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "red");
            Assert.IsTrue(args.IsValid());
            Assert.AreEqual("red", args.SampleColor);

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "green");
            Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "blue");
            Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "Blue");
            Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "purple");
            Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.ErrorText);

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor2", "pink");
            args.Properties["SampleColor2"].Validators.Add(new SetValidator<string>("pink", "purple", "puce"));
            Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor2", "pink");
            // Uses a custom equality comparer (a BizArk class).
            args.Properties["SampleColor2"].Validators.Add(
                new SetValidator<string>(new EqualityComparer((a, b) => { return a == b; }), "pink"));
            Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/NumberOfScoops", "2");
            var vals = new[] {1, 2};
            args.Properties["NumberOfScoops"].Validators.Add(new SetValidator<int>(vals));
            Debug.WriteLine(args.GetHelpText(50));
            Assert.IsTrue(args.IsValid());
        }

        [Test]
        public void Wait_CmdLineWithWaitTrue_True()
        {
            var cmdLineWithWaitTrue = new CmdLineWithWaitTrue();

            cmdLineWithWaitTrue.Options.Wait.Should().BeTrue();
        }        
        
        [Test]
        public void WaitArgs_CmdLineWithWaitTrue_True()
        {
            // Act
            var cmdLineWithWaitArgName = new CmdLineWithWaitArgName();
            cmdLineWithWaitArgName.InitializeFromCmdLine("/PleaseWaitMe");

            // Assert
            cmdLineWithWaitArgName.Options.Wait.Should().BeTrue();
            cmdLineWithWaitArgName.Options.WaitArgName.Should().Be("PleaseWaitMe");
        }
    }

    [CmdLineOptions(Wait = true, WaitArgName = "PleaseWaitMe")]
    public class CmdLineWithWaitArgName : CmdLineObject
    {
        [CmdLineArg]
        public string Name { get; set; }

        [CmdLineArg]
        public bool PleaseWaitMe { get; set; }
        
    }

    [CmdLineOptions(Wait = true)]
    public class CmdLineWithWaitTrue : CmdLineObject
    {
        [CmdLineArg]
        public string[] Names { get; set; }
        
    }

    [CmdLineOptions(ArgumentPrefix = "-", AssignmentDelimiter = ':', ArraySeparator = ";")]
    public class CmdLineObjectWithStringArray : CmdLineObject
    {
        [CmdLineArg]
        public string[] Names { get; set; }
    }

    [CmdLineOptions(AssignmentDelimiter = ':', ArgumentPrefix = "-")]
    public class CmdLineObjectWithColonAssignmentDelimiter : CmdLineObject
    {
        [CmdLineArg]
        public string Path { get; set; }
    }

    [CmdLineOptions(AssignmentDelimiter = '=')]
    public class CmdLineObjectWithPassword : CmdLineObject
    {
        [CmdLineArg]
        public string Password { get; set; }
    }

    [CmdLineOptions(AssignmentDelimiter = '=')]
    internal class CmdLineObjectWithMultipleEnumProperty : CmdLineObject
    {
        [CmdLineArg]
        public Car[] Cars { get; set; }
    }

    internal class CmdLineObjectWithEnumProperty : CmdLineObject
    {
        [CmdLineArg]
        public Car Car { get; set; }
    }

    internal class CmdLineObjectWithEnumPropertyWithManyValues : CmdLineObject
    {
        [CmdLineArg]
        public Country Country { get; set; }
    }

    internal enum Country
    {
        Abkhazia,
        Albania,
        Afghanistan,
        Algeria,
        Andorra,
        Angola,
        Anguilla,
        Antigua,
        Argentina,
        Armenia,
        Aruba,
        Australia,
        Austria,
        Azerbaijan
    }

    [CmdLineOptions(ArgumentPrefix = "", AssignmentDelimiter = '=')]
    internal class CmdLineObjectWithEmptyPrefixAssignmentDelimiterEqualitySign : CmdLineObject
    {
        [CmdLineArg]
        public string FullPath { get; set; }

        [CmdLineArg]
        public Car Car { get; set; }

        [CmdLineArg]
        public Car[] Cars { get; set; }
    }

    [CmdLineOptions(ArgumentPrefix = "")]
    internal class CmdLineObjectWithEmptyPrefix : CmdLineObject
    {
        [CmdLineArg]
        public Car Car { get; set; }

        [CmdLineArg]
        public Car[] Cars { get; set; }
    }

    [CmdLineOptions(DefaultArgName = "Hello", ApplicationName = "TESTAPP")]
    internal class MyTestCmdLineObject
        : CmdLineObject
    {
        public MyTestCmdLineObject()
        {
            NumberOfScoops = 1;
            SampleColor = "blue";
        }

        [CmdLineArg(Alias = "H", ShowInUsage = DefaultBoolean.True)]
        [System.ComponentModel.Description("Says hello to user.")]
        public string Hello { get; set; }

        [CmdLineArg(Alias = "G")]
        [System.ComponentModel.Description("Says goodbye to user.")]
        public string Goodbye { get; set; }

        [CmdLineArg(Alias = "I")]
        [System.ComponentModel.Description("Determines whether user likes ice cream or not.")]
        public bool DoesLikeIceCream { get; set; }

        [CmdLineArg]
        [System.ComponentModel.DataAnnotations.Range(1, 3)]
        public int NumberOfScoops { get; set; }

        [CmdLineArg]
        public int[] FavoriteNumbers { get; set; }

        [CmdLineArg(ShowInUsage = DefaultBoolean.True)]
        public Car FavoriteCar { get; set; }

        [CmdLineArg(Aliases = new[] {"S", "Stuff", "Crap"})]
        [System.ComponentModel.Description("List of things the user likes.")]
        public string[] StuffILike { get; set; }

        // ValidateSet: http://msdn.microsoft.com/en-us/library/windows/desktop/ms714432(v=vs.85).aspx
        [CmdLineArg]
        [Set(true, "red", "green", "blue")]
        public string SampleColor { get; set; }

        [CmdLineArg]
        public string SampleColor2 { get; set; }
    }

    [CmdLineDefaultArg("g")]
    internal class MyTestCmdLineObject2
        : MyTestCmdLineObject
    {
    }

    [CmdLineDefaultArg("F")]
    internal class MyTestCmdLineObject3
        : CmdLineObject
    {
        [CmdLineArg(Alias = "F")]
        public string[] Family { get; set; }

        [CmdLineArg(Alias = "D")]
        public string Father { get; set; }
    }

    internal class MyTestCmdLineObject4
        : CmdLineObject
    {
        public MyTestCmdLineObject4()
        {
            Options.DefaultArgNames = new[] {"F", "M"};
        }

        [CmdLineArg(Alias = "M")]
        public string Mother { get; set; }

        [CmdLineArg(Alias = "F")]
        public string Father { get; set; }

        [CmdLineArg(Alias = "C")]
        public string[] Children { get; set; }
    }

    [CmdLineOptions(DefaultArgName = "F", ArgumentPrefix = "+")]
    internal class MyTestCmdLineObject5
        : CmdLineObject
    {
        [CmdLineArg(Alias = "F")]
        public string[] Family { get; set; }

        [CmdLineArg(Alias = "D")]
        public string Father { get; set; }
    }

    [CmdLineOptions(DefaultArgName = "F", ArgumentPrefix = "--")]
    internal class MyTestCmdLineObject6
        : CmdLineObject
    {
        [CmdLineArg(Alias = "F")]
        public string[] Family { get; set; }

        [CmdLineArg(Alias = "D")]
        public string Father { get; set; }
    }

    internal class MyTestCmdLineObject7
        : CmdLineObject
    {
        [CmdLineArg(Alias = "v")]
        public bool IsValidProp { get; set; }

        protected override string[] Validate()
        {
            List<string> baseValidation = base.Validate().ToList();

            if (!IsValidProp)
                baseValidation.Add("Not IsValidProp!");

            return baseValidation.ToArray();
        }
    }

    internal class AliasesWithDifferentCaseTestCmdLineObject : CmdLineObject
    {
        [CmdLineArg(Aliases = new[] {"F", "f"})]
        public string Family { get; set; }
    }

    internal enum Car
    {
        Tesla,
        Ferrari,
        Lamborghini,
        Kia
    }

    internal class DefaultAssignmentDelimiterCmdLineObject : CmdLineObject
    {
        [CmdLineArg(ShowInUsage = DefaultBoolean.True)]
        [System.ComponentModel.Description("The name of the user")]
        public string Name { get; set; }
    }

    [CmdLineOptions(AssignmentDelimiter = ':')]
    internal class ColonAssignmentDelimiterCmdLineObject : CmdLineObject
    {
        [CmdLineArg(ShowInUsage = DefaultBoolean.True)]
        [System.ComponentModel.Description("The name of the user")]
        public string Name { get; set; }

        [CmdLineArg]
        public int Count { get; set; }
    }

    [CmdLineOptions(AssignmentDelimiter = ':')]
    internal class ColonAssignmentDelimiterArrayArgCmndLineObject : CmdLineObject
    {
        [CmdLineArg]
        public string[] Names { get; set; }
    }
}