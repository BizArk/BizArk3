using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace BizArk.Core.Collections
{

    /// <summary>
    /// Represents a generic collection of key/value pairs. The enumerator returns the values in the order assigned.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class HashList<TKey, TValue>
        : IList<TValue>
    {

		#region Fields and Properties

        private Dictionary<TKey, int> mDictionary = new Dictionary<TKey, int>();
        private List<TValue> mList = new List<TValue>();

        /// <summary>
        /// Gets or sets the value for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get { return GetValue(key); }
            set
            {
				if (mIsReadOnly) throw new InvalidOperationException("The list is readonly.");
                SetValue(key, value);
            }
        }

        /// <summary>
        /// Gets or sets the value at the designated index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue this[int index]
        {
            get { return mList[index]; }
            set
            {
                if (mIsReadOnly) throw new InvalidOperationException("The list is readonly.");
                mList[index] = value;
            }
        }

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        public int Count
        {
            get { return mList.Count; }
        }

        private bool mIsReadOnly = false;
        /// <summary>
        /// Gets a value that determines if the list if readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return mIsReadOnly; }
            protected set { mIsReadOnly = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the value to the list.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Insert(int index, TKey key, TValue value)
        {
            if (mIsReadOnly) throw new InvalidOperationException("The list is readonly.");
            if (mDictionary.ContainsKey(key)) throw new ArgumentException("Key already exists in the collection.");

            mList.Insert(index, value);
            foreach (var item in mDictionary.ToArray())
            {
                if (item.Value >= index)
                    mDictionary[item.Key] = item.Value + 1;
            }
            mDictionary.Add(key, index);
        }

        /// <summary>
        /// Adds the value to the list.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (mIsReadOnly) throw new InvalidOperationException("The list is readonly.");
            if (mDictionary.ContainsKey(key)) throw new ArgumentException("Key already exists in the collection.");
            SetValue(key, value);
        }

        /// <summary>
        /// Removes the item from the list.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            if (mIsReadOnly) throw new InvalidOperationException("The list is readonly.");
            if (!mDictionary.ContainsKey(key)) return false;
            var i = mDictionary[key];
            mDictionary.Remove(key);
            mList.RemoveAt(i);
            foreach (var item in mDictionary.ToArray())
            {
                if (item.Value > i)
                    mDictionary[item.Key] = item.Value - 1;
            }
            return true;
        }

        /// <summary>
        /// Removes the item from the list.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(TValue value)
        {
            if (mIsReadOnly) throw new InvalidOperationException("The list is readonly.");
            var key = GetKey(value);
            return Remove(key);
        }

        /// <summary>
        /// Removes the value at the designated index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (mIsReadOnly) throw new InvalidOperationException("The list is readonly.");
            var key = GetKeyFromIndex(index);
            Remove(key);
        }

        /// <summary>
        /// Removes all the items from the list.
        /// </summary>
        public void Clear()
        {
            if (mIsReadOnly) throw new InvalidOperationException("The list is readonly.");
            mDictionary.Clear();
            mList.Clear();
        }

        /// <summary>
        /// Gets the value from the list.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected TValue GetValue(TKey key)
        {
            var i = mDictionary[key];
            return mList[i];
        }

        /// <summary>
        /// Gets the value from the list. If the key is not in the list, returns the default value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dflt">Default value to return if the key does not exist.</param>
        /// <returns></returns>
        public TValue GetValue(TKey key, TValue dflt)
        {
            TValue value;
            if (TryGetValue(key, out value)) return value;
            return dflt;
        }

        /// <summary>
        /// Gets the value from the list. Returns true if the value exists, otherwise false.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!mDictionary.ContainsKey(key))
            {
                value = default(TValue);
                return false;
            }
            var i = mDictionary[key];
            value = mList[i];
            return true;
        }

        /// <summary>
        /// Sets the value in the list.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected int SetValue(TKey key, TValue value)
        {
            int i;
            if (mDictionary.ContainsKey(key))
            {
                i = mDictionary[key];
                mList[i] = value;
            }
            else
            {
                mList.Add(value);
                i = mList.Count - 1;
                mDictionary.Add(key, i);
            }
            return i;
        }

        /// <summary>
        /// Determines if the key is in the list.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return mDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Determines if the item is in the list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TValue item)
        {
            return mList.Contains(item);
        }

        /// <summary>
        /// Gets the index of the given item.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IndexOf(TKey key)
        {
            if (!mDictionary.ContainsKey(key)) return -1;
            return mDictionary[key];
        }

        /// <summary>
        /// Gets the index of the given item.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(TValue value)
        {
            return mList.IndexOf(value);
        }

        /// <summary>
        /// Gets the key based on the value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TKey GetKey(TValue value)
        {
            var i = mList.IndexOf(value);
            if (i < 0) throw new ArgumentException("Value not found in the collection.");
            foreach (var keyVal in mDictionary)
            {
                if (keyVal.Value == i)
                    return keyVal.Key;
            }
            // This should never happen, but just in case.
            throw new InvalidOperationException("The key was not found in the collection.");
        }

        /// <summary>
        /// Gets the key based on the index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TKey GetKeyFromIndex(int index)
        {
            foreach (var item in mDictionary)
                if (item.Value == index) return item.Key;

            throw new ArgumentOutOfRangeException("The index was not found in the dictionary.");
        }

        /// <summary>
        /// Returns the collection of keys.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return mDictionary.Keys; }
        }

        /// <summary>
        /// Gets the enumerator for the list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the list.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets an array of the values.
        /// </summary>
        /// <returns></returns>
        public TValue[] ToArray()
        {
            return mList.ToArray();
        }

        /// <summary>
        /// Copies the values to an array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
        {
            mList.CopyTo(array, arrayIndex);
        }

        #endregion

        #region Unsupported IList methods

        void IList<TValue>.Insert(int index, TValue item)
        {
            throw new NotSupportedException("Cannot insert values without the key.");
        }

        void ICollection<TValue>.Add(TValue item)
        {
            throw new NotSupportedException("Cannot add values without the key.");
        }

        #endregion

    }
}
