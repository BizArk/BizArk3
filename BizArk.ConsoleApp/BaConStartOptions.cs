using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.ConsoleApp
{

	/// <summary>
	/// Start options for BizArk Console apps.
	/// </summary>
    public class BaConStartOptions
    {

		#region Construction and Destruction

		/// <summary>
		/// Creates an instance of BaConStartOptions.
		/// </summary>
		public BaConStartOptions()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            var prompt = "* Press any key to continue. *";
            sb.AppendLine(new string('*', prompt.Length));
            sb.AppendLine(prompt);
            sb.AppendLine(new string('*', prompt.Length));            
            PressAnyKey = sb.ToString();
        }

		#endregion

		#region Properties and Fields

		/// <summary>
		/// Gets or sets the command-line options for the object.
		/// </summary>
		public CmdLineOptions CmdLineOptions { get; set; } = new CmdLineOptions();

		/// <summary>
		/// Gets or sets the value to display before exiting to prompt the user to press any key to exit.
		/// </summary>
		public string PressAnyKey { get; set; }

        /// <summary>
        /// Gets or sets the text to display an argument error message. Should be formatted with {message} for the error message.
        /// </summary>
        public string ArgErrorWrapper { get; set; } = "Error: {message}\n*******************************************";

		#endregion

	}
}
