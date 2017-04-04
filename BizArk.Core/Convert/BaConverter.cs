using System;
using System.Collections.Generic;
using System.Reflection;
using BizArk.Core.Convert.Strategies;
using BizArk.Core.Extensions.TypeExt;

namespace BizArk.Core.Convert
{

	/// <summary>
	/// This class provides the conversion for ConvertEx. BaConverter is not threadsafe. 
	/// However, ConvertEx uses it in a thread-safe manner.
	/// </summary>
	/// <threadsafe static="false" instance="false"></threadsafe>
	internal class BaConverter
	{

		#region Conversion

		private IConvertStrategy[] mStrategies = new IConvertStrategy[]
		{
			new NullValueConversionStrategy(), // This should always be first so other conversion strategies will always have a non-null value.
			new AssignableFromConversionStrategy(),
			new EnumConversionStrategy(),
			new StringToBoolConversionStrategy(), // This should go before ConvertibleConversionStrategy or the value might be incorrectly processed.
			new ConvertibleConversionStrategy(),
			new TypeConverterConversionStrategy(),
			new StaticMethodConversionStrategy(),
			new CtorConversionStrategy(),
			new ConvertMethodConversionStrategy(),
		};

		/// <summary>
		/// Converts the value to the specified type. 
		/// Checks for a TypeConverter, conversion methods, 
		/// and the IConvertible interface.
		/// </summary>
		/// <param name="value">The value to convert from.</param>
		/// <param name="to">The type to convert to.</param>
		/// <param name="convertedValue">The converted value. Only holds a converted value if Try returns true.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">This conversion is not supported. -or-value is null and conversionType is a value type.</exception>
		/// <exception cref="System.ArgumentNullException">conversionType is null.</exception>
		public bool Try(object value, Type to, out object convertedValue)
		{
			if (to == null)
				throw new ArgumentNullException("to");

			foreach (var strategy in mStrategies)
			{
				if (strategy.TryConvert(value, to, out convertedValue))
					return true;
			}

			convertedValue = null; // Set this just before returning in case one of the strategies sets this to something odd.
			return false;
		}

		#endregion

		#region IsEmpty

		private Dictionary<Type, object[]> mEmptyValues = new Dictionary<Type, object[]>();
		private Dictionary<Type, object> mDefaultEmptyValues = new Dictionary<Type, object>();

		/// <summary>
		/// Checks to see if the value is empty. The value is empty if it is null, DBNull, or matches the MinValue, MaxValue, or Empty fields of the values type.
		/// </summary>
		/// <param name="value">The value to check if it is empty or not.</param>
		/// <returns></returns>
		public bool IsEmpty(object value)
		{
			if (value == null) return true;
			if (value == DBNull.Value) return true;

			Type type = value.GetType();
			RegisterEmptyValues(type);

			foreach (var emptyVal in mEmptyValues[type])
			{
				if (value.Equals(emptyVal))
					return true;
			}
			return false;
		}

		private static object GetStaticFieldValue(Type type, string fieldName)
		{
			var prop = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
			if (prop == null) return null;
			return prop.GetValue(type);
		}

		/// <summary>
		/// Registers the default empty values for this type for use in IsEmpty.
		/// </summary>
		/// <param name="type"></param>
		private void RegisterEmptyValues(Type type)
		{
			if (type == null) return;
			if (mEmptyValues.ContainsKey(type)) return;

			var empties = GetEmptyValues(type);
			mEmptyValues.Add(type, empties);
		}

		/// <summary>
		/// Gets the values that represent empty for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private object[] GetEmptyValues(Type type)
		{
			var empties = new List<object>();
			empties.Add(GetDefaultEmptyValue(type));

			object emptyVal;
			emptyVal = GetStaticFieldValue(type, "MinValue");
			if (emptyVal != null) empties.Add(emptyVal);
			emptyVal = GetStaticFieldValue(type, "MaxValue");
			if (emptyVal != null) empties.Add(emptyVal);
			emptyVal = GetStaticFieldValue(type, "Empty");
			if (emptyVal != null) empties.Add(emptyVal);

			return empties.ToArray();
		}

		/// <summary>
		/// Gets the default value that represents empty for the given type.
		/// </summary>
		/// <returns></returns>
		public object GetDefaultEmptyValue(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (type.AllowNull())
				return null;
			else
				return Activator.CreateInstance(type);
		}

		#endregion

	}
}
