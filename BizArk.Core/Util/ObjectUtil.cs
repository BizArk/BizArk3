﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizArk.Core.Extensions.StringExt;
using System.Diagnostics;
using System.Reflection;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Provides utility methods for working with objects.
	/// </summary>
	public static class ObjectUtil
	{

		/// <summary>
		/// Converts the object to a dictionary with name/value pairs.
		/// </summary>
		/// <param name="obj">Can be a POCO, DataRow, IDataReader.</param>
		/// <returns></returns>
		public static IDictionary<string, object> ToPropertyBag(object obj)
		{
			// Check to see if the object is already a property bag.
			var dict = obj as IDictionary<string, object>;
			if (dict != null) return dict;

			if (TryPropertyBagFromDataRow(obj as DataRow, out dict))
				return dict;
			if (TryPropertyBagFromDataReader(obj as IDataReader, out dict))
				return dict;
			if (TryPropertyBagFromDictionary(obj as IDictionary, out dict))
				return dict;

			// Provides access to sub properties as well.
			return new ObjectDictionary(obj);
		}

		private static bool TryPropertyBagFromDictionary(IDictionary props, out IDictionary<string, object> dict)
		{
			dict = null;
			if (props == null) return false;

			if (props.Count == 0)
			{
				dict = new Dictionary<string, object>();
				return true;
			}

			dict = new Dictionary<string, object>();
			foreach (var key in props.Keys)
			{
				var name = key as string;
				if (name == null)
				{
					// All the keys must be of type string for this to work.
					dict = null;
					return false;
				}
				dict.Add(name, props[key]);
			}
			return true;
		}

		private static bool TryPropertyBagFromDataReader(IDataReader props, out IDictionary<string, object> dict)
		{
			dict = null;
			if (props == null) return false;

			dict = new Dictionary<string, object>();
			for (var i = 0; i < props.FieldCount; i++)
			{
				var name = props.GetName(i);
				var value = props.GetValue(i);
				dict.Add(name, value);
			}

			return true;
		}

		private static bool TryPropertyBagFromDataRow(DataRow props, out IDictionary<string, object> dict)
		{
			dict = null;
			if (props == null) return false;

			dict = new Dictionary<string, object>();
			foreach (DataColumn col in props.Table.Columns)
			{
				dict.Add(col.ColumnName, props[col]);
			}

			return true;
		}

		/// <summary>
		/// Gets the value from the object with the given name. Uses . notation to find values in object graphs. Supports array syntax as well, but does not support method calls. If any of the properties in the chain are null, dflt is returned. Uses ConvertEx to return the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="key"></param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public static T GetValue<T>(object obj, string key, T dflt = default(T))
		{
			var val = GetValue(obj, key);
			if (val == null)
				return dflt;
			else
				return ConvertEx.To<T>(val);
		}

		/// <summary>
		/// Gets the value from the object with the given name. Uses . notation to find values in object graphs. Supports array syntax as well, but does not support method calls. If any of the properties in the chain are null, null is returned.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="key">Property name chain. Use . to separate properties. Use [#] for array lookup. Use [KEY] (quotes not required) for dictionary lookup.</param>
		/// <returns></returns>
		public static object GetValue(object obj, string key)
		{
			object value;
			if (TryGetValue(obj, key, out value))
				return value;
			else
				throw new ArgumentException($"Invalid property name in '{key}'.");
		}

		/// <summary>
		/// Tries to get the value. Returns false if the key is not valid. If we simply can't reach the object in question, returns true, but value will be null.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="dflt">Default value if unable to get the value.</param>
		/// <returns></returns>
		public static bool TryGetValue<T>(object obj, string key, out T value, T dflt = default(T))
		{
			value = dflt;

			object objVal;
			if (!TryGetValue(obj, key, out objVal))
				return false;

			if (objVal != null)
				value = ConvertEx.To<T>(objVal);

			return true;
		}

		/// <summary>
		/// Tries to get the value. Returns false if the key is not valid. If we simply can't reach the object in question, returns true, but value will be null.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryGetValue(object obj, string key, out object value)
		{
			value = null;

			if (key.IsEmpty())
				throw new ArgumentNullException("key");

			if (obj == null) return true;

			var propertyNames = key.Split('.');
			var currentObj = obj;
			foreach (var propName in GetPropertyNames(key))
			{
				if (propName.StartsWith("["))
				{
					var indexer = propName.Trim('[', ']');
					currentObj = GetIndexedValue(currentObj, indexer);
					if (currentObj == null) return true;
				}
				else
				{
					var prop = FindProperty(currentObj, propName);
					if (prop != null)
					{
						currentObj = prop.GetValue(currentObj);
					}
					else
					{
						var propBag = ToPropertyBag(currentObj);
						if (!propBag.TryGetValue(propName, out currentObj))
							return false;
					}
					if (currentObj == null) return true;
				}

			}

			value = currentObj;
			return true;
		}

		/// <summary>
		/// Gets the property names from the key. Indexed properties are returned with square brackets.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static IEnumerable<string> GetPropertyNames(string key)
		{
			if (key.IsEmpty()) yield break;

			var idx = 0;
			var startIdx = 0;
			//test[hello].whatever
			while (idx < key.Length)
			{
				if (key[idx] == '[')
				{
					// Return anything we've gathered up to this point.
					if (idx != startIdx)
						yield return key.Substring(startIdx, idx - startIdx);

					var endIdx = key.IndexOf(']', idx);
					if (endIdx < 0)
						throw new ArgumentException($"'{key}' contains an invalid indexer.");

					// Make sure to include the [ and ].
					yield return key.Substring(idx, (endIdx + 1) - idx);

					idx = endIdx + 1;
					if (idx >= key.Length) yield break;
					if (key[idx] != '.')
						throw new ArgumentException($"Unexpected character in '{key}' at character {idx}. Expecting a '.'");
					idx += 1; // Move past the .
					startIdx = idx; // Start gathering the next property name.
				}
				else if (key[idx] == '.')
				{
					yield return key.Substring(startIdx, idx - startIdx);
					startIdx = idx + 1;
				}

				idx++;
			}

			if (startIdx < idx)
				yield return key.Substring(startIdx, idx - startIdx);
		}

		private static object GetIndexedValue(object obj, string indexer)
		{
			var list = obj as IList;
			if (list != null)
			{
				int idx;
				if (!ConvertEx.Try(indexer, out idx))
					throw new ArgumentException($"'[{indexer}]' is not a valid index argument for an array.");
				return list[idx];
			}

			var dict = obj as IDictionary;
			if (dict != null)
				return dict[indexer];

			throw new ArgumentException($"Type {obj.GetType().FullName} is not a supported type for an indexed property. Only IList and IDictionary objects are supported.");
		}

		private static PropertyInfo FindProperty(object obj, string name)
		{
			return obj.GetType().GetProperty(name);
			//Debug.WriteLine(propx.Name);
			//var props = TypeDescriptor.GetProperties(obj);
			//foreach (PropertyDescriptor prop in props)
			//{
			//	if (prop.Name == name)
			//		return prop;
			//}
			//return null;
		}

	}
}
