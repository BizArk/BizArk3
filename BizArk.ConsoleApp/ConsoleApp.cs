using System;
using System.ComponentModel;

namespace BizArk.ConsoleApp
{

	/// <summary>
	/// Interface used by BaCon to manage a conole app.
	/// </summary>
	public interface IConsoleApp
	{
        /// <summary>
        /// The method to call to start running the console application.
        /// </summary>
        /// <returns>Environment.ExitCode</returns>
        int Start();

		/// <summary>
		/// Gets a value that determines if the process should pause before exiting.
		/// </summary>
		bool Wait { get; }

		/// <summary>
		/// Gets a value that determines if help text should be displayed instead of running the console app.
		/// </summary>
		bool Help { get; }

		/// <summary>
		/// Called if an exception is raised. Return true to indicate the error is handled. If handled, the BaCon object won't display help or set the ExitCode.
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		bool Error(Exception ex);
	}

	/// <summary>
	/// A base class that can be used so developers don't have to implement all the methods.
	/// </summary>
	public abstract class BaseConsoleApp : IConsoleApp
	{

		/// <summary>
		/// Performs application-defined tasks associated with 
		/// freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// The method to call to start running the console application.
		/// </summary>
		/// <returns>Environment.ExitCode</returns>
		public abstract int Start();

		/// <summary>
		/// Gets or sets a value that determines if help text should be displayed instead of running the console app.
		/// </summary>
		[CmdLineArg("?", ShowInUsage = false)]
		[Description("If true, displays the help text.")]
		public bool Help { get; set; }

		/// <summary>
		/// Gets or sets a value that determines if the process should pause before exiting.
		/// </summary>
		[CmdLineArg(ShowInUsage = false)]
		[Description("If true, waits for a key to be pressed before exiting the application.")]
		public bool Wait { get; set; }

		/// <summary>
		/// Called if an exception is raised. Return true to indicate the error is handled. If handled, the BaCon object won't display help or set the ExitCode.
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public virtual bool Error(Exception ex)
		{
			return false;
		}

	}

}
