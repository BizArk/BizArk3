# BizArk v3 [![build status](http://teamcity.codebetter.com/app/rest/builds/buildType:id:BizArk_Bizark30/statusIcon)](http://teamcity.codebetter.com/viewType.html?buildTypeId=BizArk_Bizark30&guest=1) [![code coverage](https://img.shields.io/teamcity/coverage/BizArk_Bizark30.svg)](http://teamcity.codebetter.com/viewType.html?buildTypeId=BizArk_Bizark30&guest=1)

[![Join the chat at https://gitter.im/BizArk/BizArk3](https://badges.gitter.im/BizArk/BizArk3.svg)](https://gitter.im/BizArk/BizArk3?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
BizArk provides a collection of tools to make building line-of-business applications easier. 

## Version 3
This is the 3rd version of BizArk. Although much of the code was ported from v2, not all of it was and some of it has changed so if you are coming from v2, you might need to make changes to your code. In particular, the command-line parsing was moved into its own project, ConvertEx has been refactored and improved, WebHelper has been removed, and the database utilities have also been removed. If the WebHelper or database utilites were useful to you and you think they should be ported to v3, please leave a comment and we'll consider it.

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
