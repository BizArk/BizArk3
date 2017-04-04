using System;
using System.Collections.Generic;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Provides a way to compare two values using a lambda expression.
	/// </summary>
	public class EqualityComparer<T> : IEqualityComparer<T>
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of EqualityComparer.
		/// </summary>
		/// <param name="comparer">A function that compares two values and returns true if they are equal.</param>
		/// <param name="hashCode">Returns the hash key for the object. Must be set to work properly with LINQ.</param>
		public EqualityComparer(Func<T, T, bool> comparer, Func<T, int> hashCode = null)
		{
			Comparer = comparer;
			HashCode = hashCode ?? new Func<T, int>((obj) => obj.GetHashCode());
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the function that compares two values and returns true if they are equal.
		/// </summary>
		public Func<T, T, bool> Comparer { get; private set; }

		/// <summary>
		/// Gets the the function that gets the hash code for the object. Must be set to work properly with LINQ.
		/// </summary>
		public Func<T, int> HashCode { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(T x, T y)
		{
			return Comparer(x, y);
		}

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(T obj)
		{
			return HashCode(obj);
		}

		#endregion
	}
}
