using System;
using System.ComponentModel.DataAnnotations;

namespace BizArk.Core.DataAnnotations
{

	/// <summary>
	/// Allows for custom validation without having to implement a validation attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class CustomAttribute : ValidationAttribute
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of SetAttribute.
		/// </summary>
		/// <param name="validate">Function used to determine if the value is valid.</param>
		public CustomAttribute(Func<object, bool> validate)
			: base("custom")
		{
			Validator = validate;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the function used to validate the value.
		/// </summary>
		public Func<object, bool> Validator { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Checks that the value of the data field is valid.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override bool IsValid(object value)
		{
			if (value == null) return true; // Use RequiredAttribute to validate for a value.
			return Validator(value);
		}

		#endregion

	}
}
