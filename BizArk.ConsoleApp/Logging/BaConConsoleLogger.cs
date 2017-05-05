using System;
using System.Collections.Generic;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.ConsoleApp.Logging
{
	public class BaConConsoleLogger : BaConFormattedLogger
	{

		public Dictionary<BaConLogLevel, ConsoleColor?> LevelColors { get; private set; } = new Dictionary<BaConLogLevel, ConsoleColor?>()
		{
			{ BaConLogLevel.Trace, BaCon.Theme.LogTraceColor },
			{ BaConLogLevel.Debug, BaCon.Theme.LogDebugColor },
			{ BaConLogLevel.Info, BaCon.Theme.LogInfoColor },
			{ BaConLogLevel.Warn, BaCon.Theme.LogWarnColor },
			{ BaConLogLevel.Error, BaCon.Theme.LogErrorColor },
			{ BaConLogLevel.Fatal, BaCon.Theme.LogFatalColor },
		};

		/// <summary>
		/// Write the log message.
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
			LevelColors.TryGetValue(level, out var clr);
			return new BaConColor(clr ?? Console.ForegroundColor);
		}

	}
}
