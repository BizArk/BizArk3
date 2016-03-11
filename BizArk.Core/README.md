# BizArk.Core
BizArk.Core contains a bunch of useful utilities for building line-of-business applications.

## ConvertEx
Perhaps the most useful utility in BizArk.Core is the data type conversion class. .Net provides a number of different ways to convert between data types, but there is no single way to do everything. BizArk.Core.ConvertEx supports many different conversion strategies.

Here are the conversion strategies supported by ConvertEx. They are processed in order until an acceptible strategy is found.

- **Null** This strategy can convert null to whatever the default value is for your type.
- **Cast** If the type you are converting from can be assigned to what you are converting to, a simple cast is used.
- **Enumeration** Need to convert to or from an enumerated value? No problem. This strategy handles both string and numeric conversions.
- **String-to-bool** Not every boolean is true/false. Sometimes it's Yes/No. This strategy simply uses the StringToBoolConversionStrategy.TrueValues array to determine what strings convert to true, everything else is false.
- **IConvertible** This is one of the most common strategies available in .Net and is the only type that System.Convert can handle. This is typically used to convert between native types (such as int to string).
- **TypeConverter** This is another common .Net conversion strategy, though probably not very well known. It allows you to create a custom type converter that will convert between types. You apply this to your class with a TypeConverterAttribute.
- **Static Methods** This strategy includes casting operators, but also anything else that takes the type you want to convert from and returns the type you want to convert to.
- **Constructor** This strategy looks for a constructor on the type you want to convert to that takes the type you are converting from.
- **Convert Method** Perhaps the simplest way to convert from a custom type to another type is to provide a method that will convert your object to the new type. However, this does require that you name your method correctly. The name should start with "To" and then be followed by the type name. For example, ToInt will convert the type to an integer. 
- **Bytes to Image** A very specific strategy that will convert an array of bytes into an image or vice-versa.
- **Bytes To String** Another specific strategy that will convert an array of bytes into a string or vice-versa.

Sample usage for ConvertEx...

	var myval = ConvertEx.To<string>(123);

This will throw a InvalidCastException if ConvertEx failed to convert the value. This is the same exception that is thrown by System.Convert if it is not able to convert a value.

If you want to simply try to convert the value, but don't want to get an exception, you can use the Try methods.

	string myval;
	if(ConvertEx.Try(123, out myval))
		Debug.WriteLine(myval);

## Extensions
Not everybody agrees with using extensions, but I find them to be very useful for doing common tasks. 

- **FormatExt** Provides a number of format methods that make it super easy to format values. In particular, Fmt method for string formatting is particularly useful: "Hello 0}".Fmt("Jane"). There is also a "Hello {Name}".Tmpl(new { Name = "Jane" }); if you want to use the string interpolation type of formatting.
- **StringExt** A bunch of methods for working with strings. Wrap text on word boundaries, gets the lines from text, split a string into individual words, determine if a string is empty or not (since it is an extension method, it can be used on a null string), get a string up to a max length, get just the left or right part of a string, split a string into an array (can convert to a different type at the same time), etc.
- **DataExt** Provides methods for working with data. In particular, getting data out of DataRow, DataRowView, and IDataReader. Another useful method is a way to add an array of values as parameters to a SqlCommand (AddArrayParameters).
- **ArrayExt** A number of methods for doing interesting things with arrays.
- **AttributeExt** Provides methods for working with attributes.
- **DateExt** Methods for working with dates. Convert a date to a relative time such as "4 seconds ago" or determine if two dates are close to one another, give or take some time.
- **DrawingExt** Convenient methods for working with rectangles, sizes, and images.
- **ExceptionExt** A simple method to get the full details of an exception, including inner exceptions. Useful for verbose logging.
- **ImageExt** Provides some useful methods for resizing images and doing other things with images.
- **MathExt** Methods that are useful when working with numbers. Currently only supports "Between" functions to determine if a value is between two other values.
- **ObjectExt** Provides some useful functions for working with objects. You can easily get property values using the name of the property, convert the object to a property bag, or perform data validation using attributes from the System.ComponentModel.DataAnnotations namespace.
- **StreamExt** Provides a method for writing one stream to another.
- **TypeExt** Useful methods when trying to work with data types. Includes methods for determining if a type inherits from another, can be null, etc.
- **WebExt** Helper methods for working with web data. Can get the body of a WebResponse as a string, plus simple methods for encoding and decoding strings.
- **XmlExt** Useful methods for working with XML data.

## Utilities

- **StringTemplate** If you like string interpolation in C# but need to use it for a non-interned string, StringTemplate might be of use to you. You can create a template using property names instead of indexes. [Additional Details](./Util/StringTemplate.md)
- **ObjectUtil** Can convert an object into a property bag (a string/object dictionary). Can convert from dictionary, DataReader, DataRow, or POCO.
- **ObjectDictionary** Wraps an object and provides an IDictionary interface for accessing its properties.
- **BitMask** Provides a wrapper around an integer that easy access to the different bits. It is expected that you derive a custom class from BitMask and provide properties for each of the bits. The properties should call GetBit and SetBit with the appropriate bit number, starting with 0.
- **ClassFactory** A very simple class factory. Used inside of BizArk.Core to provide a way to change what classes get created for some key classes. To customize classes, just set ClassFactory.Factory to your factory method.
- **EqualityComparer** Useful for handling equality comparisons in Linq queries with a lambda.
- **FileEx** Provides methods above and beyond what System.IO.File provides. RemoveDirectory will recursively delete everything it can, then delete the directory. The method will keep going even if there are errors (useful when you really want a directory gone, but there are a few locked files). DeleteFile will do everything possible to delete a file, including changing the file attributes. Plus a few other helpful methods.
- **FontUtil** This will allow you to register fonts, including fonts embedded in your application, so they can be used by your application. The fonts are only available to your application.
- **MemSize** Provides useful features for working with memory sizes, including relative formatting (figures out if it should show bytes, KB, MB, etc). Provides conversions between IEC and SI standards.
- **MimeMap** Provides mime types based on file extensions. Uses the Windows registry and Mime.Types from Apache. Mime.Types is a separate file in the same directory as BizArk.Core.dll.
- **Range** Represents a mathmatical range of values. Useful for validating data.
- **RemoteDateTime** This is a DateTime that is initialized from a remote source. You can then efficiently get the current DateTime from that source.
- **StringWriterEx** Inherits from StringWriter and allows you to set the Encoding.
- **TempFile** Provides a temp file that will be automatically deleted when the object is disposed.
- **Application** Provides easy access to application properties such as the location of the executable, title, version, etc.
