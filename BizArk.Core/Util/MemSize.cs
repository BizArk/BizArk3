using System;
using System.Globalization;
using System.Text.RegularExpressions;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.Core.Util
{

	/// <summary>
	/// This class represents the size for memory on a computer.
	/// </summary>
	/// <remarks>
	/// <para>There are two standards for calculating size. One standard uses base 1024 and the other uses base 1000. </para>
	/// <para>The base 1024 standard is called IEC and is the one that most software developers understand and use.
	/// To convert you multiply the higher order number by 1024 to get the lower order number (eg, 1 KiB = 1024 bytes).
	/// Due to widespread confusion in the retail industry, this is no longer the standard for memory size and the prefixes
	/// associated with them are now changed to be KiB, MiB, GiB, and TiB.</para>
	/// <para>The base 1000 standard is called SI and is easier to understand by the consumer market. To convert you mutiply
	/// the higher order number by 1000 to get the lower order number (eg, 1 KB = 1000 bytes). The prefixes for these are
	/// KB, MB, GB, and TB.</para>
	/// <para>For more information on this, see http://en.wikipedia.org/wiki/Binary_prefix.</para>
	/// </remarks>
	public class MemSize
		: IFormattable
	{

		#region Initialization and Destruction

		/// <summary></summary>
		public MemSize(long totalBytes)
		{
			if (totalBytes < 0)
				throw new ArgumentException("totalBytes must be 0 or a positive number.", "totalBytes");
			TotalBytes = totalBytes;
		}

		/// <summary>
		/// Parses the string to determine a memory size. NOTE: Using a floating point number will likely result in a small amount of imprecision. Do not rely on the byte count being exact.
		/// </summary>
		/// <param name="fmt">A string that starts with a number and ends with one of the supported format strings. Ex 123GiB</param>
		/// <returns></returns>
		public static MemSize Parse(string fmt)
		{
			return Parse(fmt, false);
		}

		/// <summary>
		/// Parses the string to determine a memory size. NOTE: Using a floating point number will likely result in a small amount of imprecision. Do not rely on the byte count being exact.
		/// </summary>
		/// <param name="fmt">A string that starts with a number and ends with one of the supported format strings. Ex 123GiB</param>
		/// <param name="useIEC">If true, always uses IEC format. So sending in 1KB is interpreted as 1KiB.</param>
		/// <returns></returns>
		public static MemSize Parse(string fmt, bool useIEC)
		{
			if (fmt.IsEmpty())
				throw new ArgumentNullException("fmt");

			if (TryParse(fmt, useIEC, out var sz))
				return sz;

			throw new ArgumentException("Invalid MemSize format.");
		}

		/// <summary>
		/// Parses the string to determine a memory size. NOTE: Using a floating point number will likely result in a small amount of imprecision. Do not rely on the byte count being exact.
		/// </summary>
		/// <param name="fmt">A string that starts with a number and ends with one of the supported format strings. Ex 123GiB</param>
		/// <param name="outSz">Output parameter.</param>
		/// <returns></returns>
		public static bool TryParse(string fmt, out MemSize outSz)
		{
			return TryParse(fmt, false, out outSz);
		}

		/// <summary>
		/// Parses the string to determine a memory size. NOTE: Using a floating point number will likely result in a small amount of imprecision. Do not rely on the byte count being exact.
		/// </summary>
		/// <param name="fmt">A string that starts with a number and ends with one of the supported format strings. Ex 123GiB</param>
		/// <param name="useIEC">If true, always uses IEC format. So sending in 1KB is interpreted as 1KiB.</param>
		/// <param name="outSz">Output parameter.</param>
		/// <returns></returns>
		public static bool TryParse(string fmt, bool useIEC, out MemSize outSz)
		{
			outSz = null;

			if (fmt.IsEmpty()) return false;

			var re = new Regex(@"^(?<sz>\d+(\.\d+)?)(\s?(?<fmt>[A-Z]+))?$", RegexOptions.IgnoreCase);
			var m = re.Match(fmt);
			if (!m.Success) return false;

			if (decimal.TryParse(m.Groups["sz"].Value, out var fmtSz))
			{
				var fmtDesignator = m.Groups["fmt"].Value ?? "";

				switch (fmtDesignator.ToUpperInvariant())
				{
					case "":
					case "B":
					case "BYTE":
					case "BYTES":
						; // Leave sz alone
						break;

					case "TIB":
						fmtSz *= cNumBytesInTebibyte;
						break;
					case "GIB":
						fmtSz *= cNumBytesInGibibyte;
						break;
					case "MIB":
						fmtSz *= cNumBytesInMebibyte;
						break;
					case "KIB":
						fmtSz *= cNumBytesInKibibyte;
						break;

					case "TB":
						fmtSz *= useIEC ? cNumBytesInTebibyte : cNumBytesInTerabyte;
						break;
					case "GB":
						fmtSz *= useIEC ? cNumBytesInGibibyte : cNumBytesInGigabyte;
						break;
					case "MB":
						fmtSz *= useIEC ? cNumBytesInMebibyte : cNumBytesInMegabyte;
						break;
					case "KB":
						fmtSz *= useIEC ? cNumBytesInKibibyte : cNumBytesInKilobyte;
						break;

					default:
						return false;
				}

				outSz = new MemSize((int)Math.Round(fmtSz, 0));
				return true;
			}

			return false;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Represents a 0 byte memory size.
		/// </summary>
		public static readonly MemSize Zero = new MemSize(0);

		/// <summary></summary>
		public const long cNumBytesInKilobyte = 1000;
		/// <summary></summary>
		public const long cNumBytesInMegabyte = cNumBytesInKilobyte * 1000;
		/// <summary></summary>
		public const long cNumBytesInGigabyte = cNumBytesInMegabyte * 1000;
		/// <summary></summary>
		public const long cNumBytesInTerabyte = cNumBytesInGigabyte * 1000;
		/// <summary></summary>
		public const long cNumBytesInKibibyte = 1024;
		/// <summary></summary>
		public const long cNumBytesInMebibyte = cNumBytesInKibibyte * 1024;
		/// <summary></summary>
		public const long cNumBytesInGibibyte = cNumBytesInMebibyte * 1024;
		/// <summary></summary>
		public const long cNumBytesInTebibyte = cNumBytesInGibibyte * 1024;

		/// <summary>
		/// Gets the total number of bytes for this MemSize.
		/// </summary>
		public long TotalBytes { get; private set; }

		/// <summary>
		/// Gets the total number of Kilobytes (SI, x1000) for this MemSize.
		/// </summary>
		public double TotalKilobytes
		{
			get { return (double)TotalBytes / cNumBytesInKilobyte; }
		}

		/// <summary>
		/// Gets the total number of Megabytes (SI, x1000) for this MemSize.
		/// </summary>
		public double TotalMegabytes
		{
			get { return (double)TotalBytes / cNumBytesInMegabyte; }
		}

		/// <summary>
		/// Gets the total number of Gigabytes (SI, x1000) for this MemSize.
		/// </summary>
		public double TotalGigabytes
		{
			get { return (double)TotalBytes / cNumBytesInGigabyte; }
		}

		/// <summary>
		/// Gets the total number of Terabytes (SI, x1000) for this MemSize.
		/// </summary>
		public double TotalTerabytes
		{
			get { return (double)TotalBytes / cNumBytesInTerabyte; }
		}

		/// <summary>
		/// Gets the total number of Kibibytes (IEC, x1024) for this MemSize.
		/// </summary>
		public double TotalKibibytes
		{
			get { return (double)TotalBytes / cNumBytesInKibibyte; }
		}

		/// <summary>
		/// Gets the total number of Mebibytes (IEC, x1024) for this MemSize.
		/// </summary>
		public double TotalMebibytes
		{
			get { return (double)TotalBytes / cNumBytesInMebibyte; }
		}

		/// <summary>
		/// Gets the total number of Gibibytes (IEC, x1024) for this MemSize.
		/// </summary>
		public double TotalGibibytes
		{
			get { return (double)TotalBytes / cNumBytesInGibibyte; }
		}

		/// <summary>
		/// Gets the total number of Tebibytes (IEC, x1024) for this MemSize.
		/// </summary>
		public double TotalTebibytes
		{
			get { return (double)TotalBytes / cNumBytesInTebibyte; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a string representation of the size. 
		/// </summary>
		/// <param name="totalBytes"></param>
		/// <returns></returns>
		public static string GetString(long totalBytes)
		{
			var sz = new MemSize(totalBytes);
			return sz.ToString();
		}

		/// <summary>
		/// Returns a string representation of the size. 
		/// </summary>
		/// <param name="totalBytes"></param>
		/// <param name="format">Can be IEC, SI (see http://en.wikipedia.org/wiki/Binary_prefix), KB, MB, GB, TB, KiB, MiB, GiB, or TiB. If null, IEC is assumed. Any other values will show the total number of bytes. If you want an IEC value with an SI prefix (old style), add a * to the format (ex, "IEC*"). This is ignored if using an SI format.</param> 
		/// <returns></returns>
		public static string GetString(long totalBytes, string format)
		{
			var sz = new MemSize(totalBytes);
			return sz.ToString(format);
		}

		/// <summary>
		/// Returns a string representation of the size. 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString("IEC", CultureInfo.CurrentCulture.NumberFormat);
		}

		/// <summary>
		/// Returns a string representation of the size. 
		/// </summary>
		/// <param name="format">Can be IEC, SI (see http://en.wikipedia.org/wiki/Binary_prefix), KB, MB, GB, TB, KiB, MiB, GiB, or TiB. If null, IEC is assumed. Any other values will show the total number of bytes. If you want an IEC value with an SI prefix (old style), add a * to the format (ex, "IEC*"). This is ignored if using an SI format.</param> 
		/// <returns></returns>
		public string ToString(string format)
		{
			return ToString(format, CultureInfo.CurrentCulture.NumberFormat);
		}

		/// <summary>
		/// Returns a string representation of the size. 
		/// </summary>
		/// <param name="format">Can be IEC, SI (see http://en.wikipedia.org/wiki/Binary_prefix), KB, MB, GB, TB, KiB, MiB, GiB, or TiB. If null, IEC is assumed. Any other values will show the total number of bytes. If you want an IEC value with an SI prefix (old style), add a * to the format (ex, "IEC*"). This is ignored if using an SI format.</param> 
		/// <param name="formatProvider"></param>
		/// <returns></returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			var wkFmt = DetectFormat(format);
			double val = 0;
			string designator = null;

			switch (wkFmt.fmt.ToUpperInvariant())
			{
				case "KIB":
					val = TotalKibibytes;
					designator = (wkFmt.useSIFormat ? "KB" : "KiB");
					break;
				case "MIB":
					val = TotalMebibytes;
					designator = (wkFmt.useSIFormat ? "MB" : "MiB");
					break;
				case "GIB":
					val = TotalGibibytes;
					designator = (wkFmt.useSIFormat ? "GB" : "GiB");
					break;
				case "TIB":
					val = TotalTebibytes;
					designator = (wkFmt.useSIFormat ? "TB" : "TiB");
					break;
				case "KB":
					val = TotalKilobytes;
					designator = "KB";
					break;
				case "MB":
					val = TotalMegabytes;
					designator = "MB";
					break;
				case "GB":
					val = TotalGigabytes;
					designator = "GB";
					break;
				case "TB":
					val = TotalTerabytes;
					designator = "TB";
					break;
				default:
					// Always show as long (no decimal places).
					return string.Format("{0:N0} bytes", TotalBytes);
			}

			return val.ToString("N" + wkFmt.decimalPlaces) + " " + designator;

		}

		private (string fmt, bool useSIFormat, int decimalPlaces) DetectFormat(string format)
		{
			var fmt = format.IfEmpty("IEC");
			var useSIFormat = false;
			int decimalPlaces;
			var re = new Regex(@"^(?<fmt>(bytes|IEC|SI|KiB|MiB|GiB|TiB|KB|MB|GB|TB))(?<nbr>\d+)?(?<sifmt>\*)?$", RegexOptions.IgnoreCase);

			var m = re.Match(fmt);
			fmt = m.Groups["fmt"].Value;
			if (fmt.IsEmpty()) throw new FormatException($"The format {format} is not a valid MemSize format string.");

			if (!int.TryParse(m.Groups["nbr"].Value, out decimalPlaces))
				decimalPlaces = 1;

			useSIFormat = (m.Groups["sifmt"].Value == "*");

			switch (fmt.ToUpperInvariant())
			{
				case "IEC":
					if (TotalBytes < (cNumBytesInKilobyte * .75))
						fmt = "bytes";
					else if (TotalBytes < (cNumBytesInMegabyte * .75))
						fmt = "KiB";
					else if (TotalBytes < (cNumBytesInGigabyte * .75))
						fmt = "MiB";
					else if (TotalBytes < (cNumBytesInTerabyte * .75))
						fmt = "GiB";
					else
						fmt = "TiB";
					break;
				case "SI":
					if (TotalBytes < (cNumBytesInKilobyte * .75))
						fmt = "bytes";
					else if (TotalBytes < (cNumBytesInMegabyte * .75))
						fmt = "KB";
					else if (TotalBytes < (cNumBytesInGigabyte * .75))
						fmt = "MB";
					else if (TotalBytes < (cNumBytesInTerabyte * .75))
						fmt = "GB";
					else
						fmt = "TB";
					break;
			}

			return (fmt, useSIFormat, decimalPlaces);
		}

		#endregion

		#region Operators

		/// <summary>
		/// Converts a MemSize to a long by returning the total number of bytes.
		/// </summary>
		/// <param name="sz"></param>
		/// <returns></returns>
		public static implicit operator long(MemSize sz)
		{
			return sz.TotalBytes;
		}

		/// <summary>
		/// Converts a long that represents a number of bytes to a MemSize.
		/// </summary>
		/// <param name="numBytes"></param>
		/// <returns></returns>
		public static implicit operator MemSize(long numBytes)
		{
			return new MemSize(numBytes);
		}

		#endregion

	}
}
