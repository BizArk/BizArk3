using System;
using System.Reflection;

namespace BizArk.Core.Convert.Strategies
{

	/// <summary>
	/// Uses a typed constructor to convert the value.
	/// </summary>
	public class CtorConversionStrategy
		 : IConvertStrategy
	{

		/// <summary>
		/// Changes the type of the value.
		/// </summary>
		/// <param name="value">The object to convert.</param>
		/// <param name="to">The type to convert the value to.</param>
		/// <param name="convertedValue">Return the value if converted.</param>
		/// <returns>True if able to convert the value.</returns>
		public bool TryConvert(object value, Type to, out object convertedValue)
		{
			convertedValue = null;
			if (value == null) return false;

			try
			{
				var from = value.GetType();
				var ctor = GetCtor(from, to);
				if (ctor == null) return false;
				convertedValue = ctor.Invoke(new object[] { value });
				return true;
			}
			catch (Exception) { } // Ignore exceptions. Keep trying other strategies.

			return false;
		}

		private static ConstructorInfo GetCtor(Type from, Type to)
		{
			return to.GetConstructor(new Type[] { from });
		}

	}
}
