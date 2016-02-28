# BizArk
BizArk provides a collection of tools to make building line-of-business applications easier. 

## Console Application Support
BizArk is probably most popular for the Command-Line Parser. It provides a simple way of getting strongly typed arguments from command-line arguments. The BaCon engine (standing for **B**iz**A**rk **Con**sole) also supports some other very handy features for console applications such as wrapping strings at word boundaries, simple colored text, automatic help (when used with the Command-Line Parser), and much more.

## Data-Type Conversions
If you have a need for data type conversions, look no further than ConvertEx. If it is possible to convert one type to another, ConvertEx can probably do it. It makes use of all the different built-in ways to convert types and also can handle common conventions.

## String Templates
C# 6 provides us with string interpolation which is great, but it only works on literal strings since it is parsed at compile time. BizArk provides string templating to support very similar syntax for non-literal strings.

## Much More
BizArk provides many convenient utilities to make everyday things easier. This includes an Application object for easy access to useful information about the environment, help with fonts, strings, xml, images, etc.

## Check It Out on NuGet
BizArk is available as a NuGet package.

[BizArk.Core](https://www.nuget.org/packages/BizArk.Core/) - Everything but BaCon.

[BizArk.BaCon] - Get the BizArk Command-Line Parser and the other console utilities.
