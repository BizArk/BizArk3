using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.Core.Extensions.ArrayExt
{
	/// <summary>
	/// Provides extension methods for string arrays.
	/// </summary>
	public static class ArrayExt
	{

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
			return RemoveEmpties((IEnumerable<string>)arr).ToArray();
		}

		/// <summary>
		/// Creates a new array that contains the non-empty elements of the given array.
		/// </summary>
		/// <param name="arr"></param>
		/// <returns></returns>
		public static IEnumerable<T> RemoveEmpties<T>(this IEnumerable<T> arr)
		{
			var vals = new List<T>();
			foreach(var val in arr)
			{
				if (!ConvertEx.IsEmpty(val))
					vals.Add(val);
			}

			return vals;
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

		/// <summary>
		/// Converts the collection of bytes into a hex strings (no prefix).
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string ToHex(this IEnumerable<byte> bytes)
		{
			var hex = new StringBuilder();
			foreach (byte b in bytes)
				hex.AppendFormat("{0:X2}", b);
			return hex.ToString();
		}

	}
}