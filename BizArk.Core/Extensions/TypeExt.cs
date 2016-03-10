using System;
using System.Collections.Generic;
using System.Reflection;

namespace BizArk.Core.Extensions.TypeExt
{
    /// <summary>
    /// Provides extension methods for Type.
    /// </summary>
    public static class TypeExt
    {
        /// <summary>
        /// Determines if the type implements the given interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool Implements(this Type type, Type interfaceType)
        {
            foreach (Type i in type.GetInterfaces())
                if (i == interfaceType) return true;
            return false;
        }

        /// <summary>
        /// Determines if the type is derived from the given base type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsDerivedFrom(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines if the type is an instance of a generic type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsDerivedFromGenericType(this Type type, Type genericType)
        {
            var typeTmp = type;
            while (typeTmp != null)
            {
                if (typeTmp.IsGenericType && typeTmp.GetGenericTypeDefinition() == genericType)
                    return true;

                typeTmp = typeTmp.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Creates a new instance of the type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object Instantiate(this Type type, params object[] args)
        {
            return ClassFactory.CreateObject(type, args);
        }

        /// <summary>
        /// Gets the C# name of the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetCSharpName(this Type type)
        {
            var name = type.Name;

            if (type == typeof(bool)) name = "bool";
            else if (type == typeof(byte)) name = "byte";
            else if (type == typeof(sbyte)) name = "sbyte";
            else if (type == typeof(char)) name = "char";
            else if (type == typeof(short)) name = "short";
            else if (type == typeof(ushort)) name = "ushort";
            else if (type == typeof(int)) name = "int";
            else if (type == typeof(uint)) name = "uint";
            else if (type == typeof(long)) name = "long";
            else if (type == typeof(ulong)) name = "ulong";
            else if (type == typeof(float)) name = "float";
            else if (type == typeof(double)) name = "double";
            else if (type == typeof(decimal)) name = "decimal";
            else if (type == typeof(string)) name = "string";

            if (type.IsValueType && type.AllowNull()) name += "?";

            return name;
        }

        /// <summary>
        /// Determines if the type corresponds to one of the built in numeric types (such as int, double, etc).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type type)
        {
            type = GetTrueType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the underlying type if the type is Nullable, otherwise just returns the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTrueType(this Type type)
        {
            // check for Nullable enums. 
            // Null values should be handled by DefaultValueConversionStrategy, but we need to be able
            // to get the actual type of the enum here.
            if (IsDerivedFromGenericType(type, typeof(Nullable<>)))
                return type.GetGenericArguments()[0];
            else
                return type;
        }

        /// <summary>
        /// Gets the fields that are of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static FieldInfo[] GetFields<T>(this Type type, BindingFlags flags = BindingFlags.Default)
        {
            var flds = new List<FieldInfo>();
            foreach (var fld in type.GetFields(flags))
            {
                if (fld.FieldType.IsDerivedFrom(typeof(T)))
                    flds.Add(fld);
            }

            return flds.ToArray();
        }

		/// <summary>
		/// Determines if the type supports null.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool AllowNull(this Type type)
		{
			if (type == null) return false;
			if (!type.IsValueType) return true;
			return Nullable.GetUnderlyingType(type) == null;
		}

    }
}
