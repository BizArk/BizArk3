using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.ConsoleApp
{

	/// <summary>
	/// Base exception thrown for command-line parsing errors.
	/// </summary>
    public class CmdLineException : ApplicationException
    {

		/// <summary>
		/// Creates an instance of CmdLineException.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerEx"></param>
		public CmdLineException(string message, Exception innerEx = null)
            : base(message, innerEx)
        {

        }

    }

	/// <summary>
	/// Exception with a specific property.
	/// </summary>
    public class CmdLineArgException : CmdLineException
    {

		/// <summary>
		/// Creates an instance of CmdLineArgException.
		/// </summary>
		/// <param name="argName"></param>
		/// <param name="message"></param>
		/// <param name="innerEx"></param>
		public CmdLineArgException(string argName, string message, Exception innerEx = null)
            : base(message, innerEx)
        {
            ArgName = argName;
        }

		/// <summary>
		/// Gets the name of the argument the exception was generated for.
		/// </summary>
        public string ArgName { get; private set; }

    }
}
