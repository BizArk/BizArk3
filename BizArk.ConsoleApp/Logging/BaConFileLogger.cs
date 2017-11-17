using System;
using System.IO;
using BizArk.Core;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.ConsoleApp.Logging
{
	/// <summary>
	/// Logs messages to a file.
	/// </summary>
	public class BaConFileLogger : BaConFormattedLogger
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of BaConFileLogger.
		/// </summary>
		/// <param name="filePath"></param>
		public BaConFileLogger(string filePath)
		{
			if (filePath.IsEmpty()) throw new ArgumentNullException("filePath");

			MessageTemplate = "{Now} [{Level}] {Message}";
			FilePath = filePath;
		}

		#endregion

		#region Fields and Properties
		
		/// <summary>
		/// Gets and sets the file path to append the log to.
		/// </summary>
		public string FilePath { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Appends the log message to a file. Creates the file if it doesn't exist.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="msg"></param>
		/// <param name="ex"></param>
		/// <returns></returns>
		public override void WriteLogMessage(BaConLogLevel level, string msg, Exception ex)
		{
			var log = FormatMessage(level, msg, ex);
			if (!log.HasValue()) return;
			File.AppendAllText(FilePath, log);
		}

		#endregion

	}
}
