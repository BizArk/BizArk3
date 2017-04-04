using System;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Represents a mathmatical range of values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Range<T> where T : IComparable
	{

		/// <summary>
		/// Creates an instance of Range.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public Range(T start, T end)
		{
			Start = start;
			End = end;
		}

		/// <summary>
		/// Gets the start of the range.
		/// </summary>
		public T Start { get; private set; }

		/// <summary>
		/// Gets the end of the range.
		/// </summary>
		public T End { get; private set; }

		/// <summary>
		/// Determines if the given value is within the range of acceptable values.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Includes(T value)
		{
			if (Start != null && Start.CompareTo(value) > 0)
				return false;
			if (End != null && End.CompareTo(value) < 0)
				return false;
			return true;
		}

	}
}
