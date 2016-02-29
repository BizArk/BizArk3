using System;
using System.Collections.Generic;

namespace BizArk.Core.Extensions.ArrayExt
{
    /// <summary>
    /// Provides extension methods for string arrays.
    /// </summary>
    public static class ArrayExt
    {
        #region Shrink

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static Array Shrink(this Array arr, int startIndex, int endIndex)
        {
            if (arr == null) return null;
            if (startIndex >= arr.Length) return Array.CreateInstance(arr.GetType().GetElementType(), 0);
            if (endIndex < startIndex) return Array.CreateInstance(arr.GetType().GetElementType(), 0);
            if (startIndex < 0) startIndex = 0;

            int length = (endIndex - startIndex) + 1;
            Array retArr = Array.CreateInstance(arr.GetType().GetElementType(), length);
            for (int i = startIndex; i <= endIndex; i++)
                retArr.SetValue(arr.GetValue(i), i - startIndex);

            return retArr;
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string[] Shrink(this string[] arr, int startIndex)
        {
            return Shrink((Array)arr, startIndex, arr.Length - 1) as string[];
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static string[] Shrink(this string[] arr, int startIndex, int endIndex)
        {
            return Shrink((Array)arr, startIndex, endIndex) as string[];
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int[] Shrink(this int[] arr, int startIndex)
        {
            return Shrink((Array)arr, startIndex, arr.Length - 1) as int[];
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static int[] Shrink(this int[] arr, int startIndex, int endIndex)
        {
            return Shrink((Array)arr, startIndex, endIndex) as int[];
        }

        #endregion

        #region Convert

        /// <summary>
        /// Converts the array to a different type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] Convert<T>(this Array arr)
        {
            return (T[])Convert(arr, typeof(T));
        }

        /// <summary>
        /// Converts the array to a different type.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public static Array Convert(this Array arr, Type elementType)
        {
            if (arr.GetType().GetElementType() == elementType)
                return arr.Copy();

            Array retArr = Array.CreateInstance(elementType, arr.Length);
            for (int i = 0; i < arr.Length; i++)
                retArr.SetValue(ConvertEx.To(arr.GetValue(i), elementType), i);
            return retArr;
        }

        #endregion

        #region RemoveEmpties

        /// <summary>
        /// Creates a new array that contains the non-empty elements of the given array.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string[] RemoveEmpties(this string[] arr)
        {
            return RemoveEmpties((Array)arr) as string[];
        }

        /// <summary>
        /// Creates a new array that contains the non-empty elements of the given array.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Array RemoveEmpties(this Array arr)
        {
            var vals = new List<object>();
            for (int i = 0; i < arr.Length; i++)
            {
                var val = arr.GetValue(i);
                if (!ConvertEx.IsEmpty(val))
                    vals.Add(val);
            }

            Array retArr = Array.CreateInstance(arr.GetType().GetElementType(), vals.Count);
            for (int i = 0; i < vals.Count; i++)
                retArr.SetValue(vals[i], i);

            return retArr;
        }

        #endregion

        #region Split

        /// <summary>
        /// Splits a string on the given char and if trim is true, removes leading and trailing whitespace characters from each element.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="separator">The char used to split the string.</param>
        /// <param name="trim">If true, removes leading and trailing whitespace characters from each element.</param>
        /// <returns></returns>
        public static string[] Split(this string str, char separator, bool trim)
        {
            return Split(str, separator, trim, false);
        }

        /// <summary>
        /// Splits a string on the given char and if trim is true, removes leading and trailing whitespace characters from each element.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="separator">The char used to split the string.</param>
        /// <returns></returns>
        public static T[] Split<T>(this string str, char separator)
        {
            return Split(str, separator, true, false).Convert<T>();
        }

        /// <summary>
        /// Splits a string on the given char and if trim is true, removes leading and trailing whitespace characters from each element.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="separator">The char used to split the string.</param>
        /// <param name="trim">If true, removes leading and trailing whitespace characters from each element.</param>
        /// <param name="removeEmpties">Removes empty elements from the string.</param>
        /// <returns></returns>
        public static string[] Split(this string str, char separator, bool trim, bool removeEmpties)
        {
            string[] strs;
            if (removeEmpties)
                strs = str.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            else
                strs = str.Split(separator);

            if (trim)
            {
                var strl = new List<string>();
                for (int i = 0; i < strs.Length; i++)
                {
                    var s = strs[i].Trim();
                    if (removeEmpties && s == "")
                    {
                        // don't add this value.
                    }
                    else
                        strl.Add(s);
                }
                strs = strl.ToArray();
            }
            return strs;
        }
        
        /// <summary>
        /// Splits a string on the given char and if trim is true, removes leading and trailing whitespace characters from each element.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="separator">The char used to split the string.</param>
        /// <param name="removeEmpties">Removes empty elements from the string.</param>
        /// <returns></returns>
        public static T[] Split<T>(this string str, char separator, bool removeEmpties)
        {
            return Split(str, separator, true, removeEmpties).Convert<T>();
        }

        #endregion

        #region Join

        /// <summary>
        /// Joins the elements of an array together as a string using the given separator.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this Array arr, string separator)
        {
            var vals = new List<string>();
            foreach (var val in arr)
                vals.Add(ConvertEx.ToString(val));
            return string.Join(separator, vals.ToArray());
        }

        #endregion

        #region Append

        /// <summary>
        /// Concatenates the two arrays together and returns a new array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static T[] Append<T>(this T[] arr, T[] arr2)
        {
            var newArr = new List<T>(arr);
            newArr.AddRange(arr2);
            return newArr.ToArray();
        }

        /// <summary>
        /// Adds the value to the end of the array and returns the new array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Append<T>(this T[] arr, T value)
        {
            var newArr = new List<T>(arr);
            newArr.Add(value);
            return newArr.ToArray();
        }

        #endregion

        /// <summary>
        /// Searches for the specified object and returns the index of the first occurrence
        /// within the entire one-dimensional System.Array.
        /// </summary>
        /// <param name="arr">The one-dimensional System.Array to search.</param>
        /// <param name="val">The object to locate in array.</param>
        /// <returns>
        /// The index of the first occurrence of value within the entire array, if found;
        /// otherwise, the lower bound of the array minus 1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">arr is null</exception>
        /// <exception cref="System.RankException">arr is multidimensional.</exception>
        public static int IndexOf(this Array arr, object val)
        {
            return Array.IndexOf(arr, val);
        }

        /// <summary>
        /// Determines if the array contains the given value.
        /// </summary>
        /// <param name="arr">The one-dimensional System.Array to search.</param>
        /// <param name="val">The object to locate in array.</param>
        /// <returns>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">arr is null</exception>
        /// <exception cref="System.RankException">arr is multidimensional.</exception>
        public static bool Contains(this Array arr, object val)
        {
            if (Array.IndexOf(arr, val) < arr.GetLowerBound(0))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Copies the array to a new array of the same type.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Array Copy(this Array arr)
        {
            var newArr = Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
            arr.CopyTo(newArr, 0);
            return newArr;
        }
    }
}