using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			// If it isn't already a property bag, then create one based on the properties of the object.
			dict = new Dictionary<string, object>();
			foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
			{
				dict.Add(prop.Name, prop.GetValue(obj));
			}

			return dict;
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

	}
}
