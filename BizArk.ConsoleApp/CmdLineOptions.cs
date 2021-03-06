﻿using BizArk.Core.Extensions.AttributeExt;
using System;

namespace BizArk.ConsoleApp
{

	/// <summary>
	/// Command-line options for BaCon apps.
	/// </summary>
	public class CmdLineOptions
	{

		#region Initialization and Destruction

		/// <summary>
		/// Gets the default options for the type. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static CmdLineOptions GetOptions<T>()
		{
			// Check to see if we can get options from the class attribute.
			var att = typeof(T).GetAttribute<CmdLineOptionsAttribute>(false);
			return att?.CmdLineOptions 
				?? new CmdLineOptions(); // We must have options for parsing.
		}

		#endregion


		#region Fields and Properties

		/// <summary>
		/// Gets or sets the names/aliases of the default properties for the command-line.
		/// </summary>
		public string[] DefaultArgNames { get; set; }

		/// <summary>
		/// Gets or sets the string used to identify argument names. Default value is '/'.
		/// </summary>
		public string ArgumentPrefix { get; set; } = "/";

		/// <summary>
		/// Gets or sets the rule for comparing the names/aliases. Default value is OrdinalIgnoreCase.
		/// </summary>
		public StringComparison Comparer { get; set; } = StringComparison.OrdinalIgnoreCase;

		/// <summary>
		/// Gets or sets the delimiter between the argument name and its value. Default value is null (name/value pairs are sent in as individual arguments).
		/// </summary>
		public string AssignmentDelimiter { get; set; } = null;

		/// <summary>
		/// Gets or sets array elements separator. Default value is null (values sent in as individual arguments).
		/// </summary>
		public string ArraySeparator { get; set; } = null;

		/// <summary>
		/// Gets or sets the enumeration that contains the exit codes. The enumeration should start with a value of 0 for success.
		/// </summary>
		public Type ExitCodes { get; set; }

		/// <summary>
		/// Gets or sets the exit code used when invalid arguments are found. If null, 1 is returned.
		/// </summary>
		public int? InvalidArgsExitCode { get; set; } = null;

		/// <summary>
		/// Gets or sets the exit code used when a fatal error occurs. If null, -1 is returned.
		/// </summary>
		public int? FatalErrorExitCode { get; set; } = null;

		#endregion

	}
}
