using BizArk.Core.Extensions.TypeExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace BizArk.Core.Extensions.AttributeExt
{
	/// <summary>
	/// Provides extension methods for working with attributes.
	/// </summary>
	public static class AttributeExt
	{
		/// <summary>
		/// Gets the specified attribute from the PropertyDescriptor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this PropertyDescriptor prop) where T : Attribute
		{
			foreach (Attribute att in prop.Attributes)
			{
				var tAtt = att as T;
				if (tAtt != null) return tAtt;
			}
			return null;
		}

		/// <summary>
		/// Gets the specified attributes from the PropertyDescriptor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetAttributes<T>(this PropertyDescriptor prop) where T : Attribute
		{
			var atts = new List<T>();
			foreach (Attribute att in prop.Attributes)
			{
				var tAtt = att as T;
				if (tAtt != null)
					atts.Add(tAtt);
			}
			return atts;
		}

		/// <summary>
		/// Gets the specified attribute from the PropertyDescriptor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prop"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this PropertyInfo prop, bool inherit) where T : Attribute
		{
			var atts = prop.GetCustomAttributes(typeof(T), inherit);
			if (atts.Length == 0) return null;
			return atts[0] as T;
		}

		/// <summary>
		/// Gets the specified attribute from the type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this Type type, bool inherit) where T : Attribute
		{
			var atts = type.GetCustomAttributes(typeof(T), inherit);
			if (atts.Length == 0) return null;
			return atts[0] as T;
		}

		/// <summary>
		/// Gets the specified attribute for the assembly.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="asm"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this Assembly asm) where T : Attribute
		{
			if (asm == null) return null;

			var atts = asm.GetCustomAttributes(typeof(T), false);
			if (atts == null) return null;
			if (atts.Length == 0) return null;

			return (T)atts[0];
		}

		/// <summary>
		/// Gets the specified attribute from the PropertyDescriptor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="inherit"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this object obj, bool inherit) where T : Attribute
		{
			if (obj == null) return null;

			var type = obj.GetType();
			if (type.IsDerivedFrom(typeof(PropertyDescriptor)))
				return GetAttribute<T>((PropertyDescriptor)obj);
			else if (type.IsDerivedFrom(typeof(PropertyInfo)))
				return GetAttribute<T>((PropertyInfo)obj, inherit);
			else if (type.IsDerivedFrom(typeof(Assembly)))
				return GetAttribute<T>((Assembly)obj, inherit);
			else if (type.IsDerivedFrom(typeof(Type)))
				return GetAttribute<T>((Type)obj, inherit);
			else
				return GetAttribute<T>(type, inherit);
		}

		/// <summary>
		/// Gets the specified attribute from the Enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="val"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(this Enum val) where T : Attribute
		{
			var fi = val.GetType().GetField(val.ToString());
			var atts = fi.GetCustomAttributes(typeof(T), false);
			if (atts.Length == 0) return null;
			return (T)atts[0];
		}

		/// <summary>
		/// Gets the value from the DescriptionAttribute for the given enumeration value.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetDescription(this Enum e)
		{
			var att = GetAttribute<DescriptionAttribute>(e);
			if (att == null) return "";
			return att.Description;
		}

	}
}
