using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BizArk.Core.Extensions.EnumerableExt
{

	/// <summary>
	/// Provides extension methods for IEnumerable.
	/// </summary>
	public static class EnumerableExt
	{

		#region Convert

		/// <summary>
		/// Converts the array to a different type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arr"></param>
		/// <returns></returns>
		public static IEnumerable<T> Convert<T>(this IEnumerable arr)
		{
			return (IEnumerable<T>)Convert(arr, typeof(T));
		}

		/// <summary>
		/// Converts the array to a different type.
		/// </summary>
		/// <param name="values"></param>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public static IEnumerable Convert(this IEnumerable values, Type elementType)
		{
			if (values.GetType().GetElementType() == elementType)
				return values.Copy(elementType);

			var count = 0;
			Array retArr = Array.CreateInstance(elementType, count);
			var idx = 0;
			foreach (var val in values)
			{
				var newVal = ConvertEx.To(val, elementType);
				retArr.SetValue(newVal, idx);
				idx++;
			}
			return retArr;
		}

		#endregion

		#region Copy

		/// <summary>
		/// Creates a new IEnumerable object with the values from the original copied to it.
		/// </summary>
		/// <param name="original"></param>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public static IEnumerable Copy(this IEnumerable original, Type elementType)
		{
			var newVals = Array.CreateInstance(elementType, original.Count());
			var idx = 0;
			foreach (var val in original)
			{
				newVals.SetValue(val, idx);
				idx++;
			}

			return (IEnumerable)newVals;
		}

		/// <summary>
		/// Creates a new IEnumerable object with the values from the original copied to it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="original"></param>
		/// <returns></returns>
		public static IEnumerable<T> Copy<T>(this IEnumerable<T> original)
		{
			return (IEnumerable<T>)Copy(original, typeof(T));
		}

		#endregion

		#region Count

		/// <summary>
		/// Counts the number of elements.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public static int Count(this IEnumerable values)
		{
			var count = 0;
			var enumerator = values.GetEnumerator();
			while (enumerator.MoveNext())
				count++;
			return count;
		}

		#endregion

		#region RemoveEmpties

		/// <summary>
		/// Creates a new array that contains the non-empty elements of the given array.
		/// </summary>
		/// <param name="arr"></param>
		/// <returns></returns>
		public static IEnumerable<T> RemoveEmpties<T>(this IEnumerable<T> arr)
		{
			var vals = new List<T>();
			foreach (var val in arr)
			{
				if (!ConvertEx.IsEmpty(val))
					vals.Add(val);
			}

			return vals;
		}

		#endregion

		#region ToHex

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

		#endregion

	}
}
