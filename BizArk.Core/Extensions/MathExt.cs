
using System;
namespace BizArk.Core.Extensions.MathExt
{

	/// <summary>
	/// Extension methods for numeric values.
	/// </summary>
	public static class MathExt
	{

		/// <summary>
		/// Makes sure the value is between the values.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static T Between<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return min;
			if (val.CompareTo(max) > 0) return max;
			return val;
		}

		/// <summary>
		/// Makes sure the value is between the values.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool IsBetween<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return false;
			if (val.CompareTo(max) > 0) return false;
			return true;
		}

	}
}
