# BizArk.BaCon
BaCon stands for **B**iz**A**rk **Con**sole Application, which is a simple, but very powerful command-line parsing utility. Here are some features of the BaCon library.

- **Automatic POCO Initialization** - POCO *(Plain Old CLR Object)* properties are automatically set from command-line arguments.
- **Automatic Help** - Help text is displayed automatically when the user requests it or there is an error. The text is nicely formatted. All you have to do is provide the description using the System.ComponentModel.DescriptionAttribute applied to your properties.
- **Automatic Validation** - the command-line arguments are validated using System.ComponentModel.DataAnnotations validation attributes that you can apply to your properties. The validation messages show up in the help text as well so people will know what valid values the program accepts.
- **Click-Once Support** - Can initialize the command-line object from a query string (useful for Click-Once apps). You don't even have to change your code. BaCon can detect if the arguments should be read from the command-line or from the Click-Once deployment URL.
- **Data Type Conversion** - The BaCon parser can convert the command-line argument into a large variety of data types, including all of the basic types (string, int, DateTime, etc). Basically anything that can be converted from a string to the data type of the property can be parsed. Uses the powerful and customizable BizArk.Core.ConvertEx to perform the type conversions.
- **Supports Arrays** - Yes, the BaCon parser can set array properties as well.
- **Aliases Property Names** - Define alternative names for your properties that can be used to set them from the command-line.
- **Partial name recognition** - You don’t need to spell out the full name or alias, just spell enough for the parser to disambiguate the property/alias from the others (me.exe /N MyNamedValue). 
- **Default Properties** - Multiple properties can be setup so they are set without using the name (me.exe MyDfltValue AnotherDfltValue /Name NamedValue).
- **Use Flags** - Boolean properties can be set simply by defining the name. If you want to set the property to false, just place a dash (-) immediately after the name (me.exe /MyFlag-).
- **Customize the Parser** - Easily customize the way the BaCon parser reads the command-line. Use a different prefix to find the argument names and use an assignment operator to denote the value. So instead of the default [me.exe /Name Value] you can do this instead [me.exe -Name=Value].
- **Displays App Title and Description** - Displays the title, version, and description of the application when the application starts. These are set in the project properties -> Application -> Assembly Information (or you can set it directly in AssemblyInfo.cs).

A Simple POCO Example
=====================

Here is an examplee of a simple command-line app using a POCO (Plain Old CLR Object). This example does not provide help, but the BaCon object does have some methods that can display the help for you, but you will need to decorate your object to display anything interesting.

	class Program
	{
		static void Main(string[] args)
		{
			var results = BaCon.ParseArgs<SampleCmdLineArgs>();
			//todo: might want to check the results for errors.
			var myArgs = results.CmdLineObj;
			BaCon.WriteLine("First={0}".Fmt(myArgs.MyFirstProperty), ConsoleColor.White);
			BaCon.WriteLine("Second={0}".Fmt(myArgs.AnotherProperty));
		}
	}

	public class SampleCmdLineArgs
	{
		public string MyFirstProperty { get; set; }
		public int AnotherProperty { get; set; }
	}

And you would call it like this...

	MySampleApp.exe /MyFirstProperty "So Long, and Thanks for All the Fish" /AnotherProperty 42

The Power of BaCon
==================

This next example is the recommended approach to using the BizArk command-line parsing tools. The sample class inherits from BaseConsoleApp which implements IConsoleApp. By implementing this interface, you can call BaCon.Start which will parse the command-line and populate your class, provide error handling, display help text if requested (or there is an error), and call your Start method.

	class Program
	{
		static void Main(string[] args)
		{
			BaCon.Start<SampleConsoleApp>();
		}
	}

	[CmdLineOptions("Name")]
	public class SampleConsoleApp : BaseConsoleApp
	{

		public SampleConsoleApp()
		{
			// Setup the default values.
			HasHair = true;
			Status = MaritalStatus.Unknown;
		}

		[Required]
		[StringLength(20, MinimumLength = 2)]
		[Description("The name of the person.")]
		public string Name { get; set; }

		[Required]
		[Range(0, 120)]
		[Description("The age in years of the person.")]
		public int Age { get; set; }

		[CmdLineArg("Job", ShowInUsage = Core.DefaultBoolean.True)]
		[Description("The type of job the person has.")]
		public string Occupation { get; set; }

		[CmdLineArg(ShowInUsage = Core.DefaultBoolean.True)]
		[Description("Does the person have hair.")]
		public bool HasHair { get; set; }

		[Description("Names of the person's children.")]
		public string[] Children { get; set; }

		[CmdLineArg(ShowInUsage = Core.DefaultBoolean.True)]
		[Description("Marital status of the person.")]
		public MaritalStatus Status { get; set; }

		public override int Start()
		{
			BaCon.WriteLine("Name={0}".Fmt(Name), ConsoleColor.White);
			BaCon.WriteLine("Age={0}".Fmt(Age));
			BaCon.WriteLine("Job={0}".Fmt(Occupation));
			BaCon.WriteLine("HasHair={0}".Fmt(HasHair));
			BaCon.WriteLine("Children={0}".Fmt(string.Join(", ", Children)));

			return 0;
		}

		public enum MaritalStatus
		{
			Single,
			Married,
			ItsComplicated,
			Unknown
		}
	}
	
You would call this sample like this...

	MySampleApp.exe "Billy Bob" /Age 23 /Job Stranger /HasHair /Children Billy Bobby "Cindy Sue" /Status ItsComplicated

Displaying Help Text
====================

Providing good help text for your average command-line tool is usually not something people put much effort into. However, the BaCon parser can provide great help text with very minimal effort on your part. The main thing you need to provide is a description for each of the properties you want to display in the help text. Just apply the System.ComponentModel.DescriptionAttribute to each property to provide the description (see the sample above).

The help text also includes the data type for the argument, whether it is required, the default value, and, if it is an enumeration, the list of possible values. 

If you apply validation attributes from System.ComponentModel.DataAnnotations, the error messages for each of the validation attributes will also be displayed with the help text for the property. This is very useful in letting people know what the valid values are before they start plugging argument values in. However, you should try to make sure the messages make sense for both displaying as an actual error message and also for displaying as part of the help text.

Validating Your Command-Line Arguments
======================================

You can have your command-line arguments validated by applying the validation attributes from System.ComponentModel.DataAnnotations. There are plenty of attributes you can pick from to do everything from marking a property as required, to setting ranges of valid data. If you can't find a validation attribute that works for you, you can easily create your own. Just inherit from ValidationAttribute and provide your own validation logic.

Once the BaCon parser has parsed the command-line, it will run validation against the populated command-line object. It will send back the errors in the ParseResults. If you are using the recommended approach, BaCon.Start(), then you don't need to do anything. BaCon will detect that there are errors and display them at the top of the screen followed by the help text (your Start method will not be called).

V2 to V3 Migration
==================

V3 of the BizArk command-line parser is a complete overhaul and rethinking of how the parser should work. For all those that are coming from v2, you will need to modify your code to work with v3. As long as you didn't use some of the more esoteric features of v2, I believe that it shouldn't take long to update the application. Here's what you'll need to do:

1. Reference both BizArk.Core and BizArk.ConsoleApp assemblies.
2. Use the new BizArk.ConsoleApp namespace.
3. Change the base class of your command-line object from CmdLineObj to BaseConsoleApp.
4. Remove any base overrides that you were using.
5. Override the Start method. Move your code for your command-line application into here.
6. In Main, call BaCon.Start<YourConsoleApp>();

Enjoy BaCon
===========

I hope you like the BizArk command-line parsing utility, that it improves your command-line tools, and makes your users happy. Command-line tools might not be the most glamorous of software projects, but they do tend to live a long time and deserve a good, solid, interface for operations people to use the tool effectively.

If you identify a bug in the project or just found something incorrect or confusing in the documentation, please create an issue on the Issues tab in GitHub. Pull Requests are accepted, but you might want to discuss any large changes with the project moderators before you go too crazy to ensure it is a direction we want to go.
