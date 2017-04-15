# BizArk.BaCon

[View the documentation in the Wiki.](https://github.com/BizArk/BizArk3/wiki/BizArk.ConsoleApp)

V2 to V3 Migration
==================

V3 of the BizArk command-line parser is a complete overhaul and rethinking of how the parser should work. For all those that are coming from v2, you will need to modify your code to work with v3. As long as you didn't use some of the more esoteric features of v2, I believe that it shouldn't take long to update the application. Here's what you'll need to do:

1. Reference both `BizArk.Core` and `BizArk.ConsoleApp` assemblies.
2. Use the new `BizArk.ConsoleApp` namespace.
3. Change the base class of your command-line object from `CmdLineObj` to `BaseConsoleApp`.
4. Remove any base overrides that you were using.
5. Override the `Start` method. Move your code for your command-line application into here.
6. In Main, call `BaCon.Start<YourConsoleApp>();`
