using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BizArk.Core.Extensions.DrawingExt;

namespace BizArk.Core.Extensions.ImageExt
{
    /// <summary>
    /// Extension methods for images.
    /// </summary>
    public static class ImageExt
    {

        /// <summary>
        /// Saves the image to the temp directory and opens it in the default application.
        /// </summary>
        /// <param name="img"></param>
        public static bool Open(this Image img)
        {
            if (img == null) return false;

            string imgPath;
            if (img.RawFormat.Equals(ImageFormat.MemoryBmp))
            {
                // memory bitmaps cannot be saved, so convert it to a regular bitmap image.
                imgPath = GetUniqueFileName("bmp");
                img.Save(imgPath, ImageFormat.Bmp);
            }
            else
            {
                var ext = img.GetExtension();
                if (string.IsNullOrEmpty(ext)) return false;
                imgPath = GetUniqueFileName(ext);
                img.Save(imgPath);
            }

            Process.Start(imgPath);
            return true;
        }

        private static string GetUniqueFileName(string ext)
        {
            int i = 0;
            string path;
            string tempDir = Application.GetTempPath();
            do
            {
                i++;
                path = Path.Combine(tempDir, string.Format("{0}.{1}", i, ext));
            } while (File.Exists(path));
            return path;
        }

        /// <summary>
        /// Gets the default extension that can be used for the file name of the image.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string GetExtension(this Image img)
        {
            if (ImageFormat.Jpeg.Equals(img.RawFormat))
                return "jpg";
            else if (ImageFormat.Gif.Equals(img.RawFormat))
                return "gif";
            else if (ImageFormat.Bmp.Equals(img.RawFormat))
                return "bmp";
            else if (ImageFormat.Emf.Equals(img.RawFormat))
                return "emf";
            else if (ImageFormat.Exif.Equals(img.RawFormat))
                return "exif";
            else if (ImageFormat.Icon.Equals(img.RawFormat))
                return "ico";
            else if (ImageFormat.Png.Equals(img.RawFormat))
                return "png";
            else if (ImageFormat.Tiff.Equals(img.RawFormat))
                return "tif";
            else if (ImageFormat.Wmf.Equals(img.RawFormat))
                return "wmf";
            else
                return null;
        }

        /// <summary>
        /// Proportionally resizes an image to fit within the given dimensions.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static Image Resize(this Image img, int maxWidth, int maxHeight)
        {
            return Resize(img, new Size(maxWidth, maxHeight), ResizeMethod.FitStretch);
        }

        /// <summary>
        /// Resizes an image.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Image Resize(this Image img, int maxWidth, int maxHeight, ResizeMethod method)
        {
            return Resize(img, new Size(maxWidth, maxHeight), method);
        }

        /// <summary>
        /// Resizes an image.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="max"></param>
        /// <param name="method"></param>
        /// <returns>The resized image. If the image does not need to be resized, returns the original image.</returns>
        public static Image Resize(this Image img, Size max, ResizeMethod method)
        {
            Rectangle srcRect;
            Rectangle destRect;
            Bitmap result;
            Size sz;

            switch (method)
            {
                case ResizeMethod.Fit:
                    if (img.Width <= max.Width && img.Height <= max.Height)
                        return img;
                    sz = img.Size.ResizeMax(max);
                    srcRect = new Rectangle(0, 0, img.Width, img.Height);
                    destRect = new Rectangle(0, 0, sz.Width, sz.Height);
                    result = new Bitmap(sz.Width, sz.Height);
                    break;
                case ResizeMethod.FitStretch:
                    if (img.Width == max.Width && img.Height <= max.Height
                        || img.Width <= max.Width && img.Height == max.Height)
                        return img;
                    sz = img.Size.ResizeMin(max).ResizeMax(max);
                    srcRect = new Rectangle(0, 0, img.Width, img.Height);
                    destRect = new Rectangle(0, 0, sz.Width, sz.Height);
                    result = new Bitmap(sz.Width, sz.Height);
                    break;
                case ResizeMethod.Fill:
                    if (img.Width <= max.Width && img.Height <= max.Height)
                        return img;

                    sz = img.Size.ResizeMin(max);

                    if (sz.Width == max.Width)
                        // The height needs to be cropped.
                        srcRect = new Rectangle();
                    else // sz.Height == max.Height
                        // The width needs to be cropped.
                        srcRect = new Rectangle();

                    destRect = new Rectangle(0, 0, max.Width, max.Height);
                    result = new Bitmap(max.Width, max.Height);
                    break;
                case ResizeMethod.FillStretch:
                    if (img.Width == max.Width && img.Height == max.Height)
                        return img;
                    srcRect = Rectangle.Empty;
                    destRect = Rectangle.Empty;
                    result = new Bitmap(max.Width, max.Height);
                    break;
                case ResizeMethod.Stretch:
                    if (img.Width == max.Width && img.Height == max.Height)
                        return img;
                    srcRect = new Rectangle(0, 0, img.Width, img.Height);
                    destRect = new Rectangle(0, 0, max.Width, max.Height);
                    result = new Bitmap(destRect.Width, destRect.Height);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("method", string.Format("{0} is not a known ResizeMethod.", method.ToString()));
            }

            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);
            return result;
        }

        /// <summary>
        /// Determines if the path is the path to an image file. Supports jpg, jpeg, gif, bmp, emf, exif, ico, png, tif, and wmf.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsImage(string path)
        {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return false;

            ext = ext.ToLower();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                case ".emf":
                case ".exif":
                case ".ico":
                case ".png":
                case ".tif":
                case ".wmf":
                    return true;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Methods of resizing.
    /// </summary>
    public enum ResizeMethod
    {
        /// <summary>Shrink to fit within the given dimensions. Might not fill the given dimensions. If smaller than the given dimensions, will not be resized.</summary>
        Fit,
        /// <summary>Shrink or expand to fit within the given dimensions. Might not fill the given dimensions.</summary>
        FitStretch,
        /// <summary>Shrink and crop to fit within the given dimensions. If smaller than the given dimensions, will not be resized.</summary>
        Fill,
        /// <summary>Shrink or expand and crop to fit within the given dimensions.</summary>
        FillStretch,
        /// <summary>Shrink or expand to fill the given dimensions. Does not maintain original proportions.</summary>
        Stretch
    }

}
