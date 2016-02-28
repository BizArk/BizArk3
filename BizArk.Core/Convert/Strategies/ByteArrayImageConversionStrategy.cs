using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BizArk.Core.Extensions.TypeExt;

namespace BizArk.Core.Convert.Strategies
{
    /// <summary>
    /// Uses the IConvertible interface to convert the value.
    /// </summary>
    public class ByteArrayImageConversionStrategy
        : IConvertStrategy
    {

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
                var imgBytes = value as byte[];
                if (imgBytes == null) return null;

                MemoryStream ms = new MemoryStream(imgBytes, 0, imgBytes.Length);
                ms.Write(imgBytes, 0, imgBytes.Length);
                return Image.FromStream(ms, true);
            }
            else
            {
                var img = value as Image;
                if (img == null) return null;

                using (MemoryStream ms = new MemoryStream())
                {
                    if (img.RawFormat.Equals(ImageFormat.MemoryBmp))
                        img.Save(ms, ImageFormat.Bmp);
                    else
                        img.Save(ms, img.RawFormat);
                    return ms.ToArray();
                }
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
            if (from == typeof(byte[]) && to.IsDerivedFrom(typeof(Image))) return true;
            if (from.IsDerivedFrom(typeof(Image)) && to == typeof(byte[])) return true;
            return false;
        }

    }
}
