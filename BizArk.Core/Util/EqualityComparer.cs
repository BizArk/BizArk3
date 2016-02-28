using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BizArk.Core.Util
{

    /// <summary>
    /// Provides a way to compare two values using a lambda expression.
    /// </summary>
    public class EqualityComparer : IEqualityComparer
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of EqualityComparer.
        /// </summary>
        /// <param name="comparer">A function that compares two values and returns true if they are equal.</param>
        /// <param name="hashCode">Returns the hash key for the object. Must be set to work properly with LINQ.</param>
        public EqualityComparer(Func<object, object, bool> comparer, Func<object, int> hashCode = null)
        {
            Comparer = comparer;
            HashCode = hashCode;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the function that compares two values and returns true if they are equal.
        /// </summary>
        public Func<object, object, bool> Comparer { get; private set; }

        /// <summary>
        /// Gets the the function that gets the hash code for the object. Must be set to work properly with LINQ.
        /// </summary>
        public Func<object, int> HashCode { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public new bool Equals(object x, object y)
        {
            return Comparer(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}
