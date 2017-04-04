using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Wraps an object so it can be accessed via a dictionary interface using dot syntax like -> myobject.contacts["john"].Email
	/// </summary>
	public class ObjectDictionary : IDictionary<string, object>
	{

		#region Initialization and destruction

		/// <summary>
		/// Creates an instance of ObjectDictionary.
		/// </summary>
		/// <param name="obj"></param>
		public ObjectDictionary(object obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");
			Object = obj;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the object that this dictionary wraps.
		/// </summary>
		public object Object { get; private set; }

		private PropertyInfo[] mProperties;
		private PropertyInfo[] Properties
		{
			get
			{
				if (mProperties == null)
					mProperties = Object.GetType().GetProperties();
				return mProperties;
			}
		}

		#endregion

		#region IDictionary

		/// <summary>
		/// Gets/sets the value.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object this[string key]
		{
			get
			{
				return ObjectUtil.GetValue(Object, key);
			}
			set
			{
				throw new NotImplementedException("Cannot set a value using the ObjectDictionary.");
			}
		}

		int ICollection<KeyValuePair<string, object>>.Count
		{
			get
			{
				// The real count is too complex and possibly not even knowable (if values are null).
				return Properties.Count();
			}
		}

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		ICollection<string> IDictionary<string, object>.Keys
		{
			get
			{
				// Only return the first level of properties as keys.
				return Properties.Select(prop =>
				{
					return prop.Name;
				}).ToArray();
			}
		}

		ICollection<object> IDictionary<string, object>.Values
		{
			get
			{
				var values = new List<object>();
				foreach (var prop in Properties)
				{
					values.Add(prop.GetValue(Object));
				}
				return values;
			}
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			object value;
			if (!ObjectUtil.TryGetValue(Object, item.Key, out value))
				return false;

			return (value == item.Value);
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			object value;
			// We don't have a good way of determining if the key is accurate or not.
			return ObjectUtil.TryGetValue(Object, key, out value);

		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			// Just enumerate through the base properties.
			foreach (var prop in Properties)
			{
				yield return new KeyValuePair<string, object>(prop.Name, prop.GetValue(Object));
			}
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetValue(string key, out object value)
		{
			return ObjectUtil.TryGetValue(Object, key, out value);
		}

		#region Not Implemented

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion

	}
}
