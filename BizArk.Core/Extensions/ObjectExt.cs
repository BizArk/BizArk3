using System;
using System.ComponentModel;
using System.Linq.Expressions;
using BizArk.Core.Util;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BizArk.Core.Extensions.ObjectExt
{
    /// <summary>
    /// Extends the Object class.
    /// </summary>
    public static class ObjectExt
    {
        /// <summary>
        /// Converts the value to the specified type. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface. Uses <see cref="BizArk.Core.ConvertEx.ChangeType(object, Type, IFormatProvider)"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="obj">The value to convert from.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException">This conversion is not supported. -or-value is null and conversionType is a value type.</exception>
        /// <exception cref="System.ArgumentNullException">conversionType is null.</exception>
        public static T Convert<T>(this object obj)
        {
            return ConvertEx.ChangeType<T>(obj);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetValue(this object obj, string propertyName)
        {
            var prop = TypeDescriptor.GetProperties(obj).Find(propertyName, false);
            if (prop == null)
                throw new ArgumentException(propertyName + " is not a valid property on " + obj.GetType().FullName);
            return prop.GetValue(obj);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this object obj, string propertyName)
        {
            return (T)GetValue(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static int GetInt(this object obj, string propertyName)
        {
            return GetValue<int>(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static int Getint(this object obj, string propertyName)
        {
            return GetValue<int>(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetString(this object obj, string propertyName)
        {
            return GetValue<string>(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool GetBoolean(this object obj, string propertyName)
        {
            return GetValue<bool>(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static decimal GetDecimal(this object obj, string propertyName)
        {
            return GetValue<decimal>(obj, propertyName);
        }

        /// <summary>
        /// Uses DataAnnotations to validate the properties of the object.
        /// </summary>
        /// <param name="obj"></param>
        public static ValidationResult[] Validate(this object obj)
        {
            var ctx = new ValidationContext(obj, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(obj, ctx, results, true);
            return results.ToArray();
        }

        /// <summary>
        /// Gets a collection of name/value pairs based on the public properties of an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static NameValue[] GetNameValues(this object obj)
        {
            var vals = obj as NameValue[];
            if (vals != null) return vals;

            var pairs = new List<NameValue>();

            foreach (var prop in obj.GetType().GetProperties())
                pairs.Add(new NameValue(prop.Name, prop.GetValue(obj, null), prop.PropertyType));

            return pairs.ToArray();
        }

    }
}
