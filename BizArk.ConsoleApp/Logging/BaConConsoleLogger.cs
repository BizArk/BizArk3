using System;
using System.Collections.Generic;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.ConsoleApp.Logging
{

	/// <summary>
	/// Logs messages to the console using colored text from BaCon.Theme.
	/// </summary>
	public class BaConConsoleLogger : BaConFormattedLogger
	{

		/// <summary>
		/// Write the log message to the console.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="msg"></param>
		/// <param name="ex"></param>
		public override void WriteLogMessage(BaConLogLevel level, string msg, Exception ex)
		{
			var log = FormatMessage(level, msg, ex);
			if (!log.HasValue()) return;

			using (GetLevelColor(level))
			{
				BaCon.WriteLine(log, indentN: "\t");
			}
		}

		private BaConColor GetLevelColor(BaConLogLevel level)
		{
			switch (level)
			{
				case BaConLogLevel.Trace: return new BaConColor(BaCon.Theme.LogTraceColor);
				case BaConLogLevel.Debug: return new BaConColor(BaCon.Theme.LogDebugColor);
				case BaConLogLevel.Info: return new BaConColor(BaCon.Theme.LogInfoColor);
				case BaConLogLevel.Warn: return new BaConColor(BaCon.Theme.LogWarnColor);
				case BaConLogLevel.Error: return new BaConColor(BaCon.Theme.LogErrorColor);
				case BaConLogLevel.Fatal: return new BaConColor(BaCon.Theme.LogFatalColor);
				default: return new BaConColor(null);
			}
		}

	}
}
