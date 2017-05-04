using System.Text;
using BizArk.Core.Util;

namespace BizArk.Core.Extensions.FormatExt
{
	/// <summary>
	/// Provides extension methods to format values.
	/// </summary>
	public static class FormatExt
	{

		/// <summary>
		/// Formats a string.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string Fmt(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static string Fmt(this short val)
		{
			return FmtNum<short>(val, 0);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static string Fmt(this short? val)
		{
			return FmtNum<short?>(val, 0);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static string Fmt(this int val)
		{
			return FmtNum<int>(val, 0);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static string Fmt(this int? val)
		{
			return FmtNum<int?>(val, 0);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static string Fmt(this long val)
		{
			return FmtNum<long>(val, 0);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static string Fmt(this long? val)
		{
			return FmtNum<long?>(val, 0);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="precision">Number of decimal places to show. If less than 0, uses the current cultures default.</param>
		/// <returns></returns>
		public static string Fmt(this decimal val, int precision = -1)
		{
			return FmtNum<decimal>(val, precision);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="precision">Number of decimal places to show. If less than 0, uses the current cultures default.</param>
		/// <returns></returns>
		public static string Fmt(this decimal? val, int precision = -1)
		{
			return FmtNum<decimal?>(val, precision);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="precision">Number of decimal places to show. If less than 0, uses the current cultures default.</param>
		/// <returns></returns>
		public static string Fmt(this float val, int precision = -1)
		{
			return FmtNum<float>(val, precision);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="precision">Number of decimal places to show. If less than 0, uses the current cultures default.</param>
		/// <returns></returns>
		public static string Fmt(this float? val, int precision = -1)
		{
			return FmtNum<float?>(val, precision);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="precision">Number of decimal places to show. If less than 0, uses the current cultures default.</param>
		/// <returns></returns>
		public static string Fmt(this double val, int precision = -1)
		{
			return FmtNum<double>(val, precision);
		}

		/// <summary>
		/// Formats a numeric value.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="precision">Number of decimal places to show. If less than 0, uses the current cultures default.</param>
		/// <returns></returns>
		public static string Fmt(this double? val, int precision = -1)
		{
			return FmtNum<double?>(val, precision);
		}

		/// <summary>
		/// Formats a currency value.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="precision">Number of decimal places to show. If less than 0, uses the current cultures default.</param>
		/// <returns></returns>
		public static string FmtCurrency(this decimal val, int precision = -1)
		{
			if (precision < 0)
				return val.ToString("C"); // use the default.
			else
				return string.Format("{0:C" + precision + "}", val);
		}

		/// <summary>
		/// Formats a currency value.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="precision">Number of decimal places to show. If less than 0, uses the current cultures default.</param>
		/// <returns></returns>
		public static string FmtCurrency(this decimal? val, int precision = -1)
		{
			if (val == null) return "";
			return FmtCurrency(val.Value, precision);
		}

		private static string FmtNum<T>(T val, int precision)
		{
			if (val == null) return "";
			if (precision < 0)
				return string.Format("{0:N}", val);
			else
				return string.Format("{0:N" + precision + "}", val);
		}

		/// <summary>
		/// Uses a StringTemplate to replace the values in the string.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static string Tmpl(this string template, object values)
		{
			var tmpl = new StringTemplate(template);
			return tmpl.Format(values);
		}
		
	}
}
