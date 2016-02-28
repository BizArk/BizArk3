using System;
using System.Globalization;

namespace BizArk.Core.Util
{

    /// <summary>
    /// This class represents the size for memory on a computer.
    /// </summary>
    /// <remarks>
    /// <para>There are two standards for calculating size. One standard uses base 1024 and the other uses base 1000. </para>
    /// <para>The base 1024 standard is called IEC and is the basic one that most computer scientists understand and use.
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
            mTotalBytes = totalBytes;
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
        public const long cNumBytesInMebibyte = cNumBytesInKilobyte * 1024;
        /// <summary></summary>
        public const long cNumBytesInGibibyte = cNumBytesInMegabyte * 1024;
        /// <summary></summary>
        public const long cNumBytesInTebibyte = cNumBytesInGigabyte * 1024;

        private long mTotalBytes;
        /// <summary>
        /// Gets the total number of bytes for this MemSize.
        /// </summary>
        public long TotalBytes
        {
            get { return mTotalBytes; }
        }

        /// <summary>
        /// Gets the total number of Kilobytes (SI, x1000) for this MemSize.
        /// </summary>
        public double TotalKilobytes
        {
            get { return (double)mTotalBytes / cNumBytesInKilobyte; }
        }

        /// <summary>
        /// Gets the total number of Megabytes (SI, x1000) for this MemSize.
        /// </summary>
        public double TotalMegabytes
        {
            get { return (double)mTotalBytes / cNumBytesInMegabyte; }
        }

        /// <summary>
        /// Gets the total number of Gigabytes (SI, x1000) for this MemSize.
        /// </summary>
        public double TotalGigabytes
        {
            get { return (double)mTotalBytes / cNumBytesInGigabyte; }
        }

        /// <summary>
        /// Gets the total number of Terabytes (SI, x1000) for this MemSize.
        /// </summary>
        public double TotalTerabytes
        {
            get { return (double)mTotalBytes / cNumBytesInTerabyte; }
        }

        /// <summary>
        /// Gets the total number of Kibibytes (IEC, x1024) for this MemSize.
        /// </summary>
        public double TotalKibibytes
        {
            get { return (double)mTotalBytes / cNumBytesInKibibyte; }
        }

        /// <summary>
        /// Gets the total number of Mebibytes (IEC, x1024) for this MemSize.
        /// </summary>
        public double TotalMebibytes
        {
            get { return (double)mTotalBytes / cNumBytesInMebibyte; }
        }

        /// <summary>
        /// Gets the total number of Gibibytes (IEC, x1024) for this MemSize.
        /// </summary>
        public double TotalGibibytes
        {
            get { return (double)mTotalBytes / cNumBytesInGibibyte; }
        }

        /// <summary>
        /// Gets the total number of Tebibytes (IEC, x1024) for this MemSize.
        /// </summary>
        public double TotalTebibytes
        {
            get { return (double)mTotalBytes / cNumBytesInTebibyte; }
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
            var useSIFormat = false;
            var wkFmt = format; // working format so we can retain the original for debugging purposes.

            if (wkFmt == null)
                wkFmt = "IEC";
            else if (wkFmt.EndsWith("*"))
            {
                useSIFormat = true;
                wkFmt = wkFmt.TrimEnd('*');
            }

            //todo: it would be cool if we could capture the numeric format too.

            switch (wkFmt.ToUpperInvariant())
            {
                case "IEC":
                    if (mTotalBytes < (cNumBytesInKilobyte / 1.5))
                        return string.Format("{0} bytes", mTotalBytes);
                    else if (mTotalBytes < (cNumBytesInMegabyte / 1.5))
                        return string.Format("{0:0.0} {1}", TotalKibibytes, useSIFormat ? "KB" : "KiB");
                    else if (mTotalBytes < (cNumBytesInGigabyte / 1.5))
                        return string.Format("{0:0.0} {1}", TotalMebibytes, useSIFormat ? "MB" : "MiB");
                    else if (mTotalBytes < (cNumBytesInTerabyte / 1.5))
                        return string.Format("{0:0.0} {1}", TotalGibibytes, useSIFormat ? "GB" : "GiB");
                    else
                        return string.Format("{0:0.0} {1}", TotalTebibytes, useSIFormat ? "TB" : "TiB");
                case "SI":
                    if (mTotalBytes < (cNumBytesInKilobyte / 1.5))
                        return string.Format("{0} bytes", mTotalBytes);
                    else if (mTotalBytes < (cNumBytesInMegabyte / 1.5))
                        return string.Format("{0:0.0} KB", TotalKilobytes);
                    else if (mTotalBytes < (cNumBytesInGigabyte / 1.5))
                        return string.Format("{0:0.0} MB", TotalMegabytes);
                    else if (mTotalBytes < (cNumBytesInTerabyte / 1.5))
                        return string.Format("{0:0.0} GB", TotalGigabytes);
                    else
                        return string.Format("{0:0.0} TB", TotalTerabytes);
                case "KIB":
                    return string.Format("{0:0.0} {1}", TotalKibibytes, useSIFormat ? "KB" : "KiB");
                case "MIB":
                    return string.Format("{0:0.0} {1}", TotalMebibytes, useSIFormat ? "MB" : "MiB");
                case "GIB":
                    return string.Format("{0:0.0} {1}", TotalGibibytes, useSIFormat ? "GB" : "GiB");
                case "TIB":
                    return string.Format("{0:0.0} {1}", TotalTebibytes, useSIFormat ? "TB" : "TiB");
                case "KB":
                    return string.Format("{0:0.0} KB", TotalKilobytes);
                case "MB":
                    return string.Format("{0:0.0} MB", TotalMegabytes);
                case "GB":
                    return string.Format("{0:0.0} GB", TotalGigabytes);
                case "TB":
                    return string.Format("{0:0.0} TB", TotalTerabytes);
                default:
                    return string.Format("{0} bytes", mTotalBytes);
            }
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
