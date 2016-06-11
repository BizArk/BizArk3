using BizArk.Core.DataAnnotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BizArk.Core.Data
{

	/// <summary>
	/// List of validation attributes for validating BaFields.
	/// </summary>
	public class BaValidatorList : List<ValidationAttribute>
	{

		/// <summary>
		/// Adds a credit card validation attribute.
		/// </summary>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList CreditCard(string errMsg = null)
		{
			var att = new CreditCardAttribute();
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a custom validation attribute.
		/// </summary>
		/// <param name="validate">Function used to determine if the value is valid.</param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList Custom(Func<object, bool> validate, string errMsg = null)
		{
			var att = new CustomAttribute(validate);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds an email address validation attribute.
		/// </summary>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList EmailAddress(string errMsg = null)
		{
			var att = new EmailAddressAttribute();
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a maximum length validation attribute.
		/// </summary>
		/// <param name="length">The maximum allowable length of array or string data.</param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList MaxLength(int length, string errMsg = null)
		{
			var att = new MaxLengthAttribute(length);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a minimum length validation attribute.
		/// </summary>
		/// <param name="length">The minimum allowable length of array or string data.</param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList MinLength(int length, string errMsg = null)
		{
			var att = new MinLengthAttribute(length);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a phone validation attribute.
		/// </summary>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList Phone(string errMsg = null)
		{
			var att = new PhoneAttribute();
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a range validation attribute.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList Range(double min, double max, string errMsg = null)
		{
			var att = new RangeAttribute(min, max);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a range validation attribute.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList Range(int min, int max, string errMsg = null)
		{
			var att = new RangeAttribute(min, max);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a required validation attribute.
		/// </summary>
		/// <param name="pattern">Regular expression used to validate value.</param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList RegularExpression(string pattern, string errMsg = null)
		{
			var att = new RegularExpressionAttribute(pattern);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a required validation attribute.
		/// </summary>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList Required(string errMsg = null)
		{
			var att = new RequiredAttribute();
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		//public SetAttribute(params object[] values)
		//public SetAttribute(IEqualityComparer comparer, params object[] values)
		//public SetAttribute(bool ignoreCase, params string[] values)

		/// <summary>
		/// Adds a required validation attribute.
		/// </summary>
		/// <param name="values">Values allowed in the set.</param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList Set(IEnumerable<object> values, string errMsg = null)
		{
			var att = new SetAttribute(values);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a required validation attribute.
		/// </summary>
		/// <param name="values">Values allowed in the set.</param>
		/// <param name="comparer"></param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList Set(IEnumerable<object> values, IEqualityComparer comparer, string errMsg = null)
		{
			var att = new SetAttribute(comparer, values);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a required validation attribute.
		/// </summary>
		/// <param name="values">Values allowed in the set.</param>
		/// <param name="ignoreCase"></param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList Set(IEnumerable<string> values, bool ignoreCase, string errMsg = null)
		{
			var att = new SetAttribute(ignoreCase, values);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a string length validation attribute.
		/// </summary>
		/// <param name="max"></param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList StringLength(int max, string errMsg = null)
		{
			var att = new StringLengthAttribute(max);
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a string length validation attribute.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList StringLength(int min, int max, string errMsg = null)
		{
			var att = new StringLengthAttribute(max);
			att.MinimumLength = min;
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

		/// <summary>
		/// Adds a url validation attribute.
		/// </summary>
		/// <param name="errMsg">Gets or sets an error message to associate with a validation control if validation fails.</param>
		/// <returns></returns>
		public BaValidatorList Url(string errMsg = null)
		{
			var att = new UrlAttribute();
			if (errMsg != null) att.ErrorMessage = errMsg;
			Add(att);
			return this;
		}

	}
}
