using System;
using System.Collections.Generic;

namespace BizArk.Core.Util
{
    /// <summary>
    /// Provides efficient storage for cached items.
    /// </summary>
    public class Cache
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates a new instance of Cache.
        /// </summary>
        public Cache()
        {
            // The default expiration time is 5 minutes.
            DefaultExpiration = new TimeSpan(0, 5, 0);
        }

        #endregion

        #region Fields and Properties

        private Dictionary<string, CacheItem> mItems = new Dictionary<string, CacheItem>();

        /// <summary>
        /// Gets or sets a value in the cache. Can be set even if the item hasn't been set before. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The object that was cached or null if it hasn't been cached yet.</returns>
        public object this[string key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value, DefaultExpiration); }
        }

        /// <summary>
        /// Gets or sets the default expiration date.
        /// </summary>
        public TimeSpan DefaultExpiration { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Puts a value into the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            SetValue(key, value, DefaultExpiration);
        }

        /// <summary>
        /// Puts a value into the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        public void SetValue(string key, object value, TimeSpan expiration)
        {
            if (mItems.ContainsKey(key))
                mItems.Remove(key);

            // We don't store nulls.
            if (value == null) return;

            mItems.Add(key, new CacheItem(key, value, DateTime.Now.Add(expiration)));
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key"></param>
        public void ClearValue(string key)
        {
            SetValue(key, null);
        }

        /// <summary>
        /// Gets a value from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The value corresponding to the key. Null if the key is not defined.</returns>
        public object GetValue(string key)
        {
            if (mItems.ContainsKey(key))
            {
                var item = mItems[key];
                if (item.HasExpired)
                    return null;
                else
                    return item.Value;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets a value from the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultVal"></param>
        /// <returns>The value corresponding to the key. defaultVal if the key is not defined.</returns>
        public T GetValue<T>(string key, T defaultVal)
        {
            var value = GetValue(key);
            if (value == null)
                return defaultVal;
            else
                return (T)value;
        }

        /// <summary>
        /// Removes expired items from the cache.
        /// </summary>
        public void PurgeCache()
        {
            var items = mItems.Values;
            foreach (var item in items)
            {
                if (item.HasExpired)
                    mItems.Remove(item.Key);
            }
        }

        /// <summary>
        /// Completely clears the cache.
        /// </summary>
        public void ClearCache()
        {
            mItems.Clear();
        }

        #endregion

        #region CacheItem

        private class CacheItem
        {
            public CacheItem(string key, object value, DateTime expirationDate)
            {
                Key = key;
                ExpirationDate = expirationDate;
                mValRef = new WeakReference(value);
            }

            private WeakReference mValRef;

            public string Key { get; private set; }

            /// <summary>
            /// Must call HasExpired before getting the value.
            /// </summary>
            public object Value 
            {
                get
                {
                    if (mValRef.IsAlive)
                        return mValRef.Target;
                    else
                        return null;
                }
            }
            public DateTime ExpirationDate { get; private set; }
            public bool HasExpired
            {
                get 
                {
                    if (!mValRef.IsAlive)
                        return false;
                    else if (DateTime.Now < ExpirationDate)
                        return false;
                    else
                        return true;
                }
            }
        }

        #endregion

    }
}
