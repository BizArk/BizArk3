using System;
using System.Text;

namespace BizArk.Core.Extensions.ExceptionExt
{

    /// <summary>
    /// Provides extension method for exceptions.
    /// </summary>
    public static class ExceptionExt
    {
        /// <summary>
        /// Gets the details of an exception suitable for display.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetDetails(this Exception ex)
        {
            var details = new StringBuilder();

            while (ex != null)
            {
                details.AppendLine(ex.GetType().FullName);
                details.AppendLine(ex.Message);
                details.AppendLine(ex.StackTrace);

                ex = ex.InnerException;
                if (ex != null)
                {
                    details.AppendLine();
                    details.AppendLine(new string('#', 70));
                }
            }

            return details.ToString();
        }
    }
}