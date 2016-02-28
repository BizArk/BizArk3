using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizArk.ConsoleApp.Parser;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BizArk.ConsoleApp.Tests
{
	[TestClass]
	public class CmdLineParserTest
	{
		[TestMethod]
		public void BasicInstanceWithDefaultOptionsTest()
		{
			// Make sure the defaults aren't changed. The defaults are used
			// for standard Windows console argument parsing.
			var parser = new CmdLineParser<TestCmdLineObj>();
			Assert.IsNotNull(parser.Options);
			Assert.AreEqual("/", parser.Options.ArgumentPrefix);
			Assert.AreEqual(null, parser.Options.AssignmentDelimiter);
			Assert.AreEqual(StringComparison.OrdinalIgnoreCase, parser.Options.Comparer);
			Assert.AreEqual(null, parser.Options.ArraySeparator);
		}

		[TestMethod]
		public void ParseStandardCmdLineTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var args = new string[]
			{
				"/Name", "Billy Bob",
				"/Age", "25",
				"/Occupation", "Stranger",
				"/HasHair", "false",
				"/Children", "Billy", "Bob", "Betty",
				"/PersonType", "Father"
			};

			var results = parser.Parse(args);
			var obj = results.CmdLineObj;
			Assert.IsNotNull(obj);
			Assert.AreEqual("Billy Bob", obj.Name);
			Assert.AreEqual(25, obj.Age);
			Assert.AreEqual("Stranger", obj.Occupation);
			Assert.AreEqual(false, obj.HasHair);
			Assert.AreEqual(3, obj.Children.Length);
			Assert.AreEqual("Billy", obj.Children[0]);
			Assert.AreEqual("Bob", obj.Children[1]);
			Assert.AreEqual("Betty", obj.Children[2]);
			Assert.AreEqual(PersonType.Father, obj.PersonType);

			args = new string[]
			{
				"/Name", "Billy Bob",
				"/Age", "25",
				"/Occupation", "Stranger",
				"/HasHair-", // Use a negation flag.
				"/Children", "Billy", "Bob", "Betty",
				"/PersonType", "Father"
			};

			results = parser.Parse(args);
			obj = results.CmdLineObj;
			Assert.IsNotNull(obj);
			Assert.AreEqual(false, obj.HasHair);
		}

		[TestMethod]
		public void ParseCmdLineAssignmentOpTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var args = new string[]
			{
				"/Name=Billy Bob",
				"/Age=25",
				"/Occupation=Stranger",
				"/HasHair=false",
				"/Children=Billy,Bob,Betty",
				"/PersonType=Father"
			};

			parser.Options.AssignmentDelimiter = "=";
			// Need to set ArraySeparator as well.
			AssertEx.Throws<InvalidOperationException>(() => { parser.Parse(args); });

			parser.Options.ArraySeparator = ",";
			var results = parser.Parse(args);
			var obj = results.CmdLineObj;
			Assert.IsNotNull(obj);
			Assert.AreEqual("Billy Bob", obj.Name);
			Assert.AreEqual(25, obj.Age);
			Assert.AreEqual("Stranger", obj.Occupation);
			Assert.AreEqual(false, obj.HasHair);
			Assert.AreEqual(3, obj.Children.Length);
			Assert.AreEqual("Billy", obj.Children[0]);
			Assert.AreEqual("Bob", obj.Children[1]);
			Assert.AreEqual("Betty", obj.Children[2]);
			Assert.AreEqual(PersonType.Father, obj.PersonType);
		}

		[TestMethod]
		public void ParseCmdLineWithArrayTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var args = new string[]
			{
				"/Name", "Billy Bob",
				"/Age", "25",
				"/Occupation", "Stranger",
				"/HasHair", "false",
				"/Children", "Billy,Bob,Betty",
				"/PersonType", "Father"
			};

			parser.Options.ArraySeparator = ",";
			var results = parser.Parse(args);
			var obj = results.CmdLineObj;
			Assert.IsNotNull(obj);
			Assert.AreEqual("Billy Bob", obj.Name);
			Assert.AreEqual(25, obj.Age);
			Assert.AreEqual("Stranger", obj.Occupation);
			Assert.AreEqual(false, obj.HasHair);
			Assert.AreEqual(3, obj.Children.Length);
			Assert.AreEqual("Billy", obj.Children[0]);
			Assert.AreEqual("Bob", obj.Children[1]);
			Assert.AreEqual("Betty", obj.Children[2]);
			Assert.AreEqual(PersonType.Father, obj.PersonType);

			args = new string[]
			{
				"/Name", "Billy Bob",
				"/Age", "25",
				"/Occupation", "Stranger",
				"/HasHair", "false",
				"/Children", @"Billy\~Bob\~Betty",
				"/PersonType", "Father"
			};

			parser.Options.ArraySeparator = @"\~"; // Use a sequence that might effect the regex.
			results = parser.Parse(args);
			obj = results.CmdLineObj;
			Assert.IsNotNull(obj);
			Assert.AreEqual(3, obj.Children.Length);
			Assert.AreEqual("Billy", obj.Children[0]);
			Assert.AreEqual("Bob", obj.Children[1]);
			Assert.AreEqual("Betty", obj.Children[2]);
		}

		[TestMethod]
		public void ParseCmdLineValidationTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var args = new string[]
			{
				"/Age", "25",
				"/Job", "A really long invalid name",
				"/HasHair", "false",
				"/Children", "Billy", "Bob", "Betty"
			};

			var results = parser.Parse(args);
			Assert.AreEqual(2, results.Errors.Length);
			Assert.AreEqual("Name is required.", results.Errors[0]);
			Assert.AreEqual("Job name too long.", results.Errors[1]);
		}

		[TestMethod]
		public void ParseCmdLineParseErrorTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var args = new string[]
			{
				"/Age", "XXX",
				"/ChildrenAges", "1", "XXX", "3"
			};

			var results = parser.Parse(args);
			Assert.AreEqual(3, results.Errors.Length);
			Assert.AreEqual("Unable to set Age. Value <XXX> is not valid.", results.Errors[0]);
			Assert.AreEqual("Unable to set ChildrenAges. Invalid value.", results.Errors[1]);
			Assert.AreEqual("Name is required.", results.Errors[2]);
		}

		[TestMethod]
		public void ParseCmdLineWithAliasTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var args = new string[]
			{
				"/Name", "Billy Bob",
				"/Age", "25",
				"/Job", "Stranger",
				"/HasHair", "false",
				"/Children", "Billy", "Bob", "Betty"
			};

			var results = parser.Parse(args);
			var obj = results.CmdLineObj;
			Assert.IsNotNull(obj);
			Assert.AreEqual("Billy Bob", obj.Name);
			Assert.AreEqual(25, obj.Age);
			Assert.AreEqual("Stranger", obj.Occupation);
			Assert.AreEqual(false, obj.HasHair);
			Assert.AreEqual(3, obj.Children.Length);
			Assert.AreEqual("Billy", obj.Children[0]);
			Assert.AreEqual("Bob", obj.Children[1]);
			Assert.AreEqual("Betty", obj.Children[2]);
		}

		[TestMethod]
		public void ParseCmdLineWithDefaultTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var args = new string[]
			{
				"Billy Bob",
				"/Age", "25",
				"/Job", "Stranger",
				"/HasHair", "false",
				"/Children", "Billy", "Bob", "Betty"
			};

			var results = parser.Parse(args);
			var obj = results.CmdLineObj;
			Assert.IsNotNull(obj);
			Assert.AreEqual("Billy Bob", obj.Name);
			Assert.AreEqual(25, obj.Age);
			Assert.AreEqual("Stranger", obj.Occupation);
			Assert.AreEqual(false, obj.HasHair);
			Assert.AreEqual(3, obj.Children.Length);
			Assert.AreEqual("Billy", obj.Children[0]);
			Assert.AreEqual("Bob", obj.Children[1]);
			Assert.AreEqual("Betty", obj.Children[2]);

			args = new string[]
			{
				"Billy Bob", // Name
				"Stranger", // Job
				"/Age", "25",
				"/HasHair", "false",
				"/Children", "Billy", "Bob", "Betty"
			};
			results = parser.Parse(args);
			obj = results.CmdLineObj;
			Assert.IsNotNull(obj);
			Assert.AreEqual("Billy Bob", obj.Name);
			Assert.AreEqual(25, obj.Age);
			Assert.AreEqual("Stranger", obj.Occupation);
			Assert.AreEqual(false, obj.HasHair);
			Assert.AreEqual(3, obj.Children.Length);
			Assert.AreEqual("Billy", obj.Children[0]);
			Assert.AreEqual("Bob", obj.Children[1]);
			Assert.AreEqual("Betty", obj.Children[2]);
		}

		[TestMethod]
		public void GetCmdLinePropertiesTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var obj = new TestCmdLineObj();

			var props = parser.GetCmdLineProperties(obj);
			Assert.AreEqual(7, props.Count);

			var prop = props["Name"];
			Assert.IsNotNull(prop);
			Assert.AreEqual("Name", prop.Name);

			prop = props["N"]; // Use an abbreviated name.
			Assert.IsNotNull(prop);
			Assert.AreEqual("Name", prop.Name);

			prop = props["age"]; // Use a different case.
			Assert.IsNotNull(prop);
			Assert.AreEqual("Age", prop.Name);

			Assert.IsFalse(prop.ValueSet);
			prop.Value = 23;
			Assert.IsTrue(prop.ValueSet);
			Assert.AreEqual(23, obj.Age);
		}

		[TestMethod]
		public void ParseQueryStringTest()
		{
			var options = new CmdLineOptions();

			// No arguments.
			var args = BaCon.QueryStringToArgs(null, options);
			Assert.AreEqual(0, args.Length);

			// No arguments.
			args = BaCon.QueryStringToArgs("", options);
			Assert.AreEqual(0, args.Length);

			// No arguments.
			args = BaCon.QueryStringToArgs("&&&", options);
			Assert.AreEqual(0, args.Length);

			// Single argument.
			var qs = "hello=world";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(2, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("world", args[1]);

			// Single argument.
			qs = "&&&hello=world&&&";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(2, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("world", args[1]);

			// Multiple arguments.
			qs = "hello=world&adios=mundo";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(4, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("world", args[1]);
			Assert.AreEqual("/adios", args[2]);
			Assert.AreEqual("mundo", args[3]);

			// Arguments and flag.
			qs = "hello=world&goodbye";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(3, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("world", args[1]);
			Assert.AreEqual("/goodbye", args[2]);

			// Arguments and negative flag.
			qs = "hello=world&goodbye-";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(3, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("world", args[1]);
			Assert.AreEqual("/goodbye-", args[2]);

			// Full URL.
			qs = "http://www.theworld.com/?hello=world&adios=mundo";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(4, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("world", args[1]);
			Assert.AreEqual("/adios", args[2]);
			Assert.AreEqual("mundo", args[3]);

			// Shouldn't include fragment.
			qs = "http://www.theworld.com/?hello=world&adios=mundo#something";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(4, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("world", args[1]);
			Assert.AreEqual("/adios", args[2]);
			Assert.AreEqual("mundo", args[3]);

			// Encoded value.
			qs = "hello=the+world";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(2, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("the world", args[1]);

			// Encoded value.
			qs = "hello=the%20world";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(2, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("the world", args[1]);

			// Encoded value.
			qs = "hello=%22Aardvarks+lurk%2C+OK%3F%22";
			args = BaCon.QueryStringToArgs(qs, options);
			Assert.AreEqual(2, args.Length);
			Assert.AreEqual("/hello", args[0]);
			Assert.AreEqual("\"Aardvarks lurk, OK?\"", args[1]);
		}

		[TestMethod]
		public void TransformAssignedArgsTest()
		{
			var parser = new CmdLineParser<TestCmdLineObj>();
			var args = new string[]
			{
				"/Name=Billy Bob",
				"/Children=Billy,Bob,Betty"
			};

			parser.Options.AssignmentDelimiter = "=";
			var newArgs = parser.TransformAssignedArgs(args).ToArray();
			Assert.AreEqual(4, newArgs.Length);
			Assert.AreEqual("/Name", newArgs[0]);
			Assert.AreEqual("Billy Bob", newArgs[1]);
			Assert.AreEqual("/Children", newArgs[2]);
			Assert.AreEqual("Billy,Bob,Betty", newArgs[3]);
		}

		#region TestCmdLineObj

		[CmdLineOptions(DefaultArgNames = new string[] { "Name", "Job" })]
		private class TestCmdLineObj
		{

			[Required(ErrorMessage = "Name is required.")]
			public string Name { get; set; }

			public int Age { get; set; }

			[CmdLineArg("Job")]
			[StringLength(10, ErrorMessage = "Job name too long.")]
			public string Occupation { get; set; }

			public bool HasHair { get; set; }

			public string[] Children { get; set; }

			public PersonType PersonType { get; set; }

			public int[] ChildrenAges { get; set; }
		}

		private enum PersonType
		{
			Father,
			Mother,
			Child
		}

		#endregion

	}
}
