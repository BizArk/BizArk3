using System;

namespace BizArk.ConsoleApp.Logging
{

	/// <summary>
	/// Interface used to customize logging.
	/// </summary>
	public abstract class BaConLogger
	{

		/// <summary>
		/// Gets or sets the minimum level of message to log.
		/// </summary>
		public BaConLogLevel MinLogLevel { get; set; } = BaConLogLevel.Trace;

		/// <summary>
		/// Write the log message.
		/// </summary>
		/// <param name="level">The log level of the message.</param>
		/// <param name="msg">The message to log.</param>
		/// <param name="ex">An optional exception to write to the log.</param>
		public abstract void WriteLogMessage(BaConLogLevel level, string msg, Exception ex);

		/// <summary>
		/// Determines if the log message should be written or not.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		protected internal virtual bool ShouldWriteLogMessage(BaConLogLevel level, string msg, Exception ex)
		{
			return level >= MinLogLevel;
		}

	}

}