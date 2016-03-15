using BizArk.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.ConsoleApp
{

	/// <summary>
	/// Attribute that can be applied to a command-line object to provide some additional information. Sending in an instance of CmdLineOptions to the parser will override these values.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CmdLineOptionsAttribute : Attribute
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of CmdLineOptionsAttribute.
		/// </summary>
		public CmdLineOptionsAttribute()
		{
			// Start with the default options.
			CmdLineOptions = new CmdLineOptions();
		}

		/// <summary>
		/// Creates an instance of CmdLineOptionsAttribute.
		/// </summary>
		/// <param name="defaultArgName"></param>
		public CmdLineOptionsAttribute(string defaultArgName)
			: this()
		{
			DefaultArgNames = new string[] { defaultArgName };
		}

		#endregion

		#region Fields and Properties

		internal CmdLineOptions CmdLineOptions { get; private set; }

		/// <summary>
		/// Gets or sets the names/aliases of the default properties for the command-line.
		/// </summary>
		public string[] DefaultArgNames
		{
			get { return CmdLineOptions.DefaultArgNames; }
			set { CmdLineOptions.DefaultArgNames = value; }
		}

		/// <summary>
		/// Gets or sets the string used to identify argument names.
		/// </summary>
		public string ArgumentPrefix
		{
			get { return CmdLineOptions.ArgumentPrefix; }
			set { CmdLineOptions.ArgumentPrefix = value; }
		}

		/// <summary>
		/// Gets or sets the rule for comparing the names/aliases. By default this is set to OrdinalIgnoreCase.
		/// </summary>
		public StringComparison Comparer
		{
			get { return CmdLineOptions.Comparer; }
			set { CmdLineOptions.Comparer = value; }
		}

		/// <summary>
		/// Gets or sets the delimiter between the argument name and its value.
		/// </summary>
		public string AssignmentDelimiter
		{
			get { return CmdLineOptions.AssignmentDelimiter; }
			set { CmdLineOptions.AssignmentDelimiter = value; }
		}

		/// <summary>
		/// Gets or sets array elements separator, default value is ","
		/// </summary>
		public string ArraySeparator
		{
			get { return CmdLineOptions.ArraySeparator; }
			set { CmdLineOptions.ArraySeparator = value; }
		}

		#endregion

	}

}
