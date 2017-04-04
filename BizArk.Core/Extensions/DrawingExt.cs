using System;
using System.Drawing;
using BizArk.Core.Util;

namespace BizArk.Core.Extensions.DrawingExt
{

	/// <summary>
	/// Extensions for classes within the Drawing namespace.
	/// </summary>
	public static class DrawingExt
	{

		/// <summary>
		/// Proportionally resizes a Size structure so that it is no smaller than the min size.
		/// </summary>
		/// <param name="sz"></param>
		/// <param name="min"></param>
		/// <returns></returns>
		public static Size ResizeMin(this Size sz, Size min)
		{
			return ResizeMin(sz, min.Width, min.Height);
		}

		/// <summary>
		/// Proportionally resizes a Size structure so that it is no smaller than the min size.
		/// </summary>
		/// <param name="sz"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static Size ResizeMin(this Size sz, int width, int height)
		{
			int tmpHeight = sz.Height;
			int tmpWidth = sz.Width;

			if (tmpHeight >= height && tmpWidth >= width)
				// The size is already larger than the min.
				return new Size(tmpWidth, tmpHeight);

			double actualRatio = (double)tmpWidth / (double)tmpHeight;
			double minRatio = (double)width / (double)height;
			double resizeRatio;

			if (actualRatio < minRatio)
				// width is the determinate side.
				resizeRatio = (double)width / (double)tmpWidth;
			else
				// height is the determinate side.
				resizeRatio = (double)height / (double)tmpHeight;

			tmpHeight = (int)Math.Round(tmpHeight * resizeRatio);
			tmpWidth = (int)Math.Round(tmpWidth * resizeRatio);

			return new Size(tmpWidth, tmpHeight);
		}

		/// <summary>
		/// Proportionally resizes a Size structure so that it is no larger than the max size.
		/// </summary>
		/// <param name="sz"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static Size ResizeMax(this Size sz, Size max)
		{
			return ResizeMax(sz, max.Width, max.Height);
		}

		/// <summary>
		/// Proportionally resizes a Size structure so that it is no larger than the max size.
		/// </summary>
		/// <param name="sz"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static Size ResizeMax(this Size sz, int width, int height)
		{
			int tmpHeight = sz.Height;
			int tmpWidth = sz.Width;

			if (tmpHeight <= height && tmpWidth <= width)
				// The size is already smaller than the max.
				return new Size(tmpWidth, tmpHeight);

			double actualRatio = (double)tmpWidth / (double)tmpHeight;
			double maxRatio = (double)width / (double)height;
			double resizeRatio;

			if (actualRatio > maxRatio)
				// width is the determinate side.
				resizeRatio = (double)width / (double)tmpWidth;
			else
				// height is the determinate side.
				resizeRatio = (double)height / (double)tmpHeight;

			tmpWidth = (int)Math.Round(tmpWidth * resizeRatio);
			tmpHeight = (int)Math.Round(tmpHeight * resizeRatio);

			return new Size(tmpWidth, tmpHeight);
		}

		/// <summary>
		/// Proportionally resizes a Size structure so that at least one side is no larger than the max size.
		/// </summary>
		/// <param name="sz"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static Size ResizeOverflow(this Size sz, Size max)
		{
			return ResizeOverflow(sz, max.Width, max.Height);
		}

		/// <summary>
		/// Proportionally resizes a Size structure so that at least one side is no larger than the max size.
		/// </summary>
		/// <param name="sz"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static Size ResizeOverflow(this Size sz, int width, int height)
		{
			int tmpHeight = sz.Height;
			int tmpWidth = sz.Width;

			if (tmpHeight <= height || tmpWidth <= width)
				// The size is already smaller than the max.
				return new Size(tmpWidth, tmpHeight);

			// If we send in the size to ResizeMin it won't be modified because it is already going to be larger than the
			// given size. So we resize it so that it is smaller than or equal to the requested size first then ResizeMin
			// will expand it to the requested size.
			sz = ResizeMax(sz, width, height);
			return ResizeMin(sz, width, height);
		}

		/// <summary>
		/// Changes the coordinates for the rectangle.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="parent"></param>
		/// <param name="style"></param>
		/// <returns></returns>
		public static Rectangle Position(this Rectangle rect, Rectangle parent, PositionStyle style)
		{
			var pt = Position(rect.Size, parent, style);
			return new Rectangle(pt.X, pt.Y, rect.Width, rect.Height);
		}

		/// <summary>
		/// Gets the location to place an object of the given size within a parent area.
		/// </summary>
		/// <param name="sz"></param>
		/// <param name="parent"></param>
		/// <param name="style"></param>
		/// <returns></returns>
		public static Point Position(this Size sz, Rectangle parent, PositionStyle style)
		{
			var leftX = parent.X;
			var centerX = (parent.Width / 2) - (sz.Width / 2) + parent.X;
			var rightX = parent.X + parent.Width - sz.Width;
			var topY = parent.Y;
			var centerY = (parent.Height / 2) - (sz.Height / 2) + parent.Y;
			var bottomY = parent.Y + parent.Height - sz.Height;

			switch (style)
			{
				case PositionStyle.TopLeft:
					return new Point(leftX, topY);
				case PositionStyle.TopCenter:
					return new Point(centerX, topY);
				case PositionStyle.TopRight:
					return new Point(rightX, topY);
				case PositionStyle.MiddleLeft:
					return new Point(leftX, centerY);
				case PositionStyle.MiddleCenter:
					return new Point(centerX, centerY);
				case PositionStyle.MiddleRight:
					return new Point(rightX, centerY);
				case PositionStyle.BottomLeft:
					return new Point(leftX, bottomY);
				case PositionStyle.BottomCenter:
					return new Point(centerX, bottomY);
				case PositionStyle.BottomRight:
					return new Point(rightX, bottomY);
				default:
					throw new ArgumentException(string.Format("'{0}' is not a known style.", style), "style");
			}
		}

		/// <summary>
		/// Sets the Top property.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="top"></param>
		public static void SetTop(this Rectangle rect, int top)
		{
			rect.Y = top;
		}

		/// <summary>
		/// Sets the Left property.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="left"></param>
		public static void SetLeft(this Rectangle rect, int left)
		{
			rect.X = left;
		}

		/// <summary>
		/// Sets the Right property.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="right"></param>
		public static void SetRight(this Rectangle rect, int right)
		{
			rect.Width = right - rect.Left;
		}

		/// <summary>
		/// Sets the bottom property.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="bottom"></param>
		public static void SetBottom(this Rectangle rect, int bottom)
		{
			rect.Height = bottom - rect.Top;
		}

		/// <summary>
		/// Gets a string that represents the number of bytes.
		/// </summary>
		/// <param name="numBytes"></param>
		/// <returns></returns>
		public static string ToMemSize(this long numBytes)
		{
			if (numBytes < 1000)
				return string.Format("{0} bytes", numBytes);
			if (numBytes < 1000000)
				return string.Format("{0:0.0} KB", (double)numBytes / 1000);
			else
				return string.Format("{0:0.0} MB", (double)numBytes / 1000000);
		}

		/// <summary>
		/// Gets the number of bytes for an image.
		/// </summary>
		/// <param name="img"></param>
		/// <returns></returns>
		public static MemSize SizeOf(this Image img)
		{
			//var sz = img.Height * img.Width;
			//return sz*4;
			var imgBytes = ConvertEx.To<byte[]>(img);
			return imgBytes.LongLength;
		}

	}

	/// <summary>
	/// Location of an element.
	/// </summary>
	public enum PositionStyle
	{
		/// <summary>Aligned to the top left</summary>
		TopLeft,
		/// <summary>Aligned to the top center</summary>
		TopCenter,
		/// <summary>Aligned to the top right</summary>
		TopRight,
		/// <summary>Aligned to the middle left</summary>
		MiddleLeft,
		/// <summary>Aligned to the middle center</summary>
		MiddleCenter,
		/// <summary>Aligned to the middle right</summary>
		MiddleRight,
		/// <summary>Aligned to the bottom left</summary>
		BottomLeft,
		/// <summary>Aligned to the bottom center</summary>
		BottomCenter,
		/// <summary>Aligned to the bottom right</summary>
		BottomRight
	}

}