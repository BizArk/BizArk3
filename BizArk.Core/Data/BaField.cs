using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Extensions.TypeExt;

namespace BizArk.Core.Data
{

	/// <summary>
	/// Contains the value and other information about a field.
	/// </summary>
	public class BaField
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of BaField
		/// </summary>
		/// <param name="obj">The object this belongs to.</param>
		/// <param name="name">Name of the field.</param>
		/// <param name="fieldType">The data type for the field.</param>
		/// <param name="dflt">Default value for the field. Used to determine if the field has changed. If null, it is converted to the default value for fieldType.</param>
		internal BaField(BaObject obj, string name, Type fieldType, object dflt = null)
		{
			if(obj == null) throw new ArgumentNullException(nameof(obj));
			if (name.IsEmpty()) throw new ArgumentNullException(nameof(name));
			if (FieldType == null) throw new ArgumentNullException(nameof(fieldType));

			Object = obj;
			Name = name;
			FieldType = fieldType;
			if (dflt == null || dflt == DBNull.Value)
			{
				mDefaultValue = ConvertEx.GetDefaultEmptyValue(fieldType);
			}
			else
			{
				VerifyValue(dflt, "default value");
				mDefaultValue = dflt;
			}
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Get the object this field belongs to.
		/// </summary>
		public BaObject Object { get; private set; }

		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		public string Name { get; private set; }

		private object mValue = null;
		/// <summary>
		/// Gets or sets the value of the field.
		/// </summary>
		public object Value
		{
			get
			{
				if (IsSet)
					return mValue;
				else
					return mDefaultValue;
			}
			set
			{
				if (mValue == value) return;
				VerifyValue(value, "value");
				mValue = value;
				IsSet = true;
				Update();
				Object.OnPropertyChanged(Name);
			}
		}

		private object mDefaultValue = null;
		/// <summary>
		/// Gets or sets the default value for the field. IsChanged is true if the default differs from Value. DefaultValue is returned if getting the value and it is not set.
		/// </summary>
		public object DefaultValue
		{
			get { return mDefaultValue; }
			set
			{
				// The values are the same. No reason to do anything else
				if (mDefaultValue == value) return;
				VerifyValue(value, "default value");
				mDefaultValue = value;
				Update();
			}
		}

		/// <summary>
		/// Gets a flag that determines if the value is set or not.
		/// </summary>
		public bool IsSet { get; private set; } = false;

		/// <summary>
		/// Gets a flag that determines if the field has been modified from it's default value.
		/// </summary>
		public bool IsChanged { get; private set; } = false;

		/// <summary>
		/// Gets the data type for the field.
		/// </summary>
		public Type FieldType { get; private set; } = typeof(object);

		#endregion

		#region Methods

		private void Update()
		{
			if (!IsSet)
				IsChanged = false;
			else if (Value == null && DefaultValue == null)
				IsChanged = false;
			else if (Value == null || DefaultValue == null)
				IsChanged = true;
			else
				IsChanged = !Value.Equals(DefaultValue);
		}

		private void VerifyValue(object value, string valName)
		{
			if (value == null)
			{
				if (!FieldType.AllowNull())
					throw new InvalidOperationException($"The {valName} for {Name} cannot be null. Expecting {FieldType.FullName}");

				return;
			}

			if (!FieldType.IsAssignableFrom(value.GetType()))
				throw new InvalidOperationException($"The {valName} for {Name} is not of the correct type. The type is {value.GetType().FullName}, expecting {FieldType.FullName}.");
		}

		#endregion

	}

}
