
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
        public static short Between(this short val, short min, short max)
        {
            if (val <= min) return min;
            if (val >= max) return max;
            return val;
        }

        /// <summary>
        /// Makes sure the value is between the values.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Between(this int val, int min, int max)
        {
            if (val <= min) return min;
            if (val >= max) return max;
            return val;
        }

        /// <summary>
        /// Makes sure the value is between the values.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static long Between(this long val, long min, long max)
        {
            if (val <= min) return min;
            if (val >= max) return max;
            return val;
        }

        /// <summary>
        /// Determines if the value is between the values.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(this short val, short min, short max)
        {
            if (val < min) return false;
            if (val > max) return false;
            return true;
        }

        /// <summary>
        /// Determines if the value is between the values.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(this int val, int min, int max)
        {
            if (val < min) return false;
            if (val > max) return false;
            return true;
        }

        /// <summary>
        /// Determines if the value is between the values.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(this long val, long min, long max)
        {
            if (val < min) return false;
            if (val > max) return false;
            return true;
        }

    }
}
