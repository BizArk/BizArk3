using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizArk.Core.Extensions.FormatExt;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Represents a named value. Immutable.
	/// </summary>
	public class NameValue
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of NameValue.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="valueType"></param>
		public NameValue(string name, object value, Type valueType)
		{
			Name = name;
			Value = value;
			ValueType = valueType;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the name for the pair.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the value for the pair.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		/// Gets the expected type for the value.
		/// </summary>
		public Type ValueType { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Provides a debug friendly string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var str = Value as string;
			if (Value == null)
				str = "[NULL]";
			else if (str == null)
				str = ConvertEx.ToString(Value);
			else
				str = "\"" + str + "\"";
			return "{0}={1}".Fmt(Name, str);
		}

		#endregion

	}


	/// <summary>
	/// Represents a named typed value. Immutable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NameValue<T> : NameValue
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of NameValue.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public NameValue(string name, object value)
			: base(name, value, typeof(T))
		{
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the value for the pair.
		/// </summary>
		public new T Value
		{
			get
			{
				return (T)base.Value;
			}
		}

		#endregion

	}


}
