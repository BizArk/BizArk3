using System;
using System.Text;

namespace BizArk.Core.Convert.Strategies
{

    /// <summary>
    /// Converts a string to a byte[] (and vice-versa).
    /// </summary>
    public class ByteArrayStringConversionStrategy
        : IConvertStrategy
    {

        static ByteArrayStringConversionStrategy()
        {
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Gets the encoding to use for converting the value.
        /// </summary>
        public static Encoding Encoding { get; set; }

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(Type from, Type to, object value, IFormatProvider provider)
        {
            if (from == typeof(byte[]))
            {
                var strBytes = value as byte[];
                if (strBytes == null) return null;
                return Encoding.GetString(strBytes);
            }
            else
            {
                var str = value as string;
                if (str == null) return null;
                return Encoding.GetBytes(str);
            }
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            if (from == typeof(byte[]) && to == typeof(string)) return true;
            if (from == typeof(string) && to == typeof(byte[])) return true;
            return false;
        }

    }
}
