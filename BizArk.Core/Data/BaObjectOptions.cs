using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.Core.Data
{
	/// <summary>
	/// Options for the BaObject.
	/// </summary>
	public class BaObjectOptions
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaObjectOptions.
		/// </summary>
		public BaObjectOptions() : this(false, null)
		{
		}

		/// <summary>
		/// Creates a new instance of BaObjectOptions.
		/// </summary>
		/// <param name="strict">Sets both StrictGet and StrictSet to this value.</param>
		public BaObjectOptions(bool strict) : this(strict, null)
		{
		}

		/// <summary>
		/// Creates a new instance of BaObjectOptions.
		/// </summary>
		/// <param name="schema">An object that contains properties that will be used to initialize the fields of the object. Can be a DataRow, IDataReader, or POCO.</param>
		public BaObjectOptions(object schema) : this(false, schema)
		{
		}

		/// <summary>
		/// Creates a new instance of BaObjectOptions.
		/// </summary>
		/// <param name="strict">Sets both StrictGet and StrictSet to this value.</param>
		/// <param name="schema">An object that contains properties that will be used to initialize the fields of the object. Can be a DataRow, IDataReader, or POCO.</param>
		public BaObjectOptions(bool strict, object schema)
		{
			StrictSet = strict;
			StrictGet = strict;
			Schema = schema;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets or sets a flag that determines if getting a field value is strict or not. If strict and the field doesn't exist, an exception is thrown.
		/// </summary>
		public bool StrictGet { get; set; } = false;

		/// <summary>
		/// Gets or sets a flag that determines if setting a field value is strict or not. If strict and the field doesn't exist, an exception is thrown.
		/// </summary>
		public bool StrictSet { get; set; } = false;

		/// <summary>
		/// Gets or sets the object that contains properties that will be used to initialize the fields of the object. Can be a DataRow, IDataReader, or POCO.
		/// </summary>
		public object Schema { get; set; } = null;

		#endregion

	}
}
