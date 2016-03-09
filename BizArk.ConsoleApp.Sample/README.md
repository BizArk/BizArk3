# BizArk.ConsoleApp.Sample

The sample console app shows how to use BaCon (BizArk Console) to create a simple, easy to maintain console application.

Although BaCon can easily load a simple POCO (Plain Old CLR Object) object from command-line arguments, to get the most out of BaCon, you will want to implement the IConsoleApp interface. The easiest way to implement this interface is to just derive your class from BaseConsoleApp which handles some of the plumbing for you.

If you look in Program.cs, you will see that there are 3 different example uses of BaCon. 

1. **SampleConsoleApp** The most comprehensive sample. Provides detailed information that can be used to display the console help for your application. Also includes validation logic and shows how to use BaCon.WriteLine to display text to the user.

2. **BasicConsoleApp** A simple sample to show how little is actually required to create a fully featured console application. The help might be sparse, but it will still show what arguments are accepted even if it doesn't explain what they are for.

3. **BasicArgs** A sample using a POCO. This sample simply parses the command-line arguments and loads a POCO object. Although it does do validation, it does not display errors (this is something you will need to handle). This method also does not automatically display help, though you can call a method to display the help.

