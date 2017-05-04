using System;

namespace BizArk.ConsoleApp.Themes
{
	public interface IBaConTheme
	{

		// Standard BaCon elements.
		ConsoleColor? AppDescriptionColor { get; set; }
		ConsoleColor? AppTitleColor { get; set; }
		ConsoleColor? ErrorColor { get; set; }
		ConsoleColor? RequiredArgColor { get; set; }
		ConsoleColor? StandardArgColor { get; set; }
		ConsoleColor? UsageColor { get; set; }

		// BaConConsoleLogger.
		ConsoleColor? LogTraceColor { get; set; }
		ConsoleColor? LogDebugColor { get; set; }
		ConsoleColor? LogInfoColor { get; set; }
		ConsoleColor? LogWarnColor { get; set; }
		ConsoleColor? LogErrorColor { get; set; }
		ConsoleColor? LogFatalColor { get; set; }

	}
}