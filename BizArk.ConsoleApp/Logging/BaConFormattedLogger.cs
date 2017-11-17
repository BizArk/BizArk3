using System;
using System.Text;
using System.Threading;
using BizArk.Core;
using BizArk.Core.Extensions.FormatExt;

namespace BizArk.ConsoleApp.Logging
{

	/// <summary>
	/// Base class for handling formatted log messages.
	/// </summary>
	public abstract class BaConFormattedLogger : BaConLogger
	{

		/// <summary>
		/// Gets or sets the format to use for the message. Uses StringTemplate for formatting. Supports the following replacements: Message, Now, Level, ProcessID, ThreadID.
		/// </summary>
		public string MessageTemplate { get; set; } = "{Message}";

		/// <summary>
		/// Gets or sets the format to use for the first exception. Uses StringTemplate for formatting. Supports the following replacements: ExceptionMsg, StackTrace, ExceptionType, Now, Level, ProcessID, ThreadID.
		/// </summary>
		public string ErrorTemplate1 { get; set; } = "{ExceptionType}: {ExceptionMsg}\n{StackTrace}";

		/// <summary>
		/// Gets or sets the format to use for inner exceptions. Set to null to not display them. Uses StringTemplate for formatting. Supports the following replacements: ExceptionMsg, StackTrace, ExceptionType, Now, Level, ProcessID, ThreadID.
		/// </summary>
		public string ErrorTemplateN { get; set; } = "********************\n{ExceptionType}: {ExceptionMsg}\n{StackTrace}";

		/// <summary>
		/// Formats the log message.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="msg"></param>
		/// <param name="ex"></param>
		/// <returns></returns>
		protected virtual string FormatMessage(BaConLogLevel level, string msg, Exception ex)
		{
			if (MessageTemplate == null) return msg;

			var log = new StringBuilder();

			var tmp = FormatMessage(level, msg);
			if (tmp != null)
				log.AppendLine(tmp);

			tmp = FormatError(level, ex);
			if (tmp != null)
				log.AppendLine(tmp);

			return log.ToString();
		}

		/// <summary>
		/// Applies the MessageTemplate to the message.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		protected virtual string FormatMessage(BaConLogLevel level, string msg)
		{
			var tmp = MessageTemplate.Tmpl(new
			{
				Message = msg,
				Now = DateTime.Now,
				Level = level,
				ProcessID = Application.ProcessID,
				ThreadID = Thread.CurrentThread.ManagedThreadId
			});
			return tmp;
		}

		/// <summary>
		/// Applies the ErrorTemplate to the exception.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="ex"></param>
		/// <returns></returns>
		protected virtual string FormatError(BaConLogLevel level, Exception ex)
		{
			if (ex == null) return null;

			var log = new StringBuilder();
			var tmpl = ErrorTemplate1;
			var curEx = ex;
			while (curEx != null)
			{
				if (tmpl != null)
				{
					var data = new
					{
						ExceptionMsg = ex.Message,
						StackTrace = ex.StackTrace,
						ExceptionType = ex.GetType().FullName,
						Now = DateTime.Now,
						Level = level,
						ProcessID = Application.ProcessID,
						ThreadID = Thread.CurrentThread.ManagedThreadId
					};

					var str = tmpl.Tmpl(data);
					log.AppendLine(str);
				}

				curEx = curEx.InnerException;
				tmpl = ErrorTemplateN;
				if (tmpl == null) break;
			}

			return log.ToString();
		}

	}


}