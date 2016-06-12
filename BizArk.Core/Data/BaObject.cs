using BizArk.Core.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace BizArk.Core.Data
{

	/// <summary>
	/// An object that can be used as dynamic. Provides a property bag implementation as well as keeps track of what fields are modified for efficient updates.
	/// </summary>
	public class BaObject : IDictionary<string, object>, IDynamicMetaObjectProvider, INotifyPropertyChanged
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaObject.
		/// </summary>
		/// <param name="strict">If true, only fields added can be set or retrieved. If false, getting a field that doesn't exist returns null and setting a field that doesn't exist automatically adds it.</param>
		/// <param name="schema">An object that contains properties that will be used to initialize the fields of the object. Can be a DataRow, IDataReader, or POCO.</param>
		public BaObject(bool strict, object schema = null)
			: this(new BaObjectOptions(strict, schema))
		{
		}

		/// <summary>
		/// Creates a new instance of BaObject.
		/// </summary>
		/// <param name="options">The options used to setup the BaObject.</param>
		public BaObject(BaObjectOptions options = null)
		{
			Options = options ?? new BaObjectOptions();

			if (Options.Schema != null
				&& !InitFromBaObject(Options.Schema as BaObject)
				&& !InitFromDataRow(Options.Schema as DataRow)
				&& !InitFromDataTable(Options.Schema as DataTable)
				&& !InitFromDataReader(Options.Schema as IDataReader)
				&& !InitSchemaFromObject(Options.Schema, true))
			{ } // Just a simple way to call the init schema methods without a bunch of if/else statements. 

		}

		private bool InitFromBaObject(BaObject schema)
		{
			if (schema == null) return false;
			if (schema == this) return false; // Can't initialize from itself!

			foreach (var fld in schema.Fields)
			{
				// Can't share fields.
				var newfld = Add(fld.Name, fld.FieldType);

				// Setting the DefaultValue can throw an exception 
				// if the field is required and the value is null.
				if (fld.DefaultValue != null)
					newfld.DefaultValue = fld.DefaultValue;
				
				// Validators can be reused. They should be thread-safe.
				newfld.Validators.AddRange(fld.Validators); 
			}

			return true;
		}

		/// <summary>
		/// Initializes the schema. Should be called from the constructor.
		/// </summary>
		/// <param name="schema">The object used to discover the schema.</param>
		/// <param name="setDflt">Determines if we will get the default value from the schema or not.</param>
		protected bool InitSchemaFromObject(object schema, bool setDflt)
		{
			if (schema == null) return false;

			foreach (PropertyDescriptor propDesc in TypeDescriptor.GetProperties(schema))
			{
				object dflt = null;
				if (setDflt)
					dflt = propDesc.GetValue(schema);
				var fld = Add(propDesc.Name, propDesc.PropertyType, dflt);
				var atts = propDesc.Attributes;
				foreach (Attribute att in atts)
				{
					var valAtt = att as ValidationAttribute;
					if (valAtt != null)
						fld.Validators.Add(valAtt);
				}
			}
			return true;
		}

		/// <summary>
		/// Initializes the schema from a DataRow.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		private bool InitFromDataRow(DataRow row)
		{
			if (row == null) return false;

			foreach (DataColumn col in row.Table.Columns)
			{
				var fld = AddDataColumn(col);
				fld.DefaultValue = row[col];
			}
			return true;
		}

		/// <summary>
		/// Initializes the schema from a DataTable.
		/// </summary>
		/// <param name="tbl"></param>
		/// <returns></returns>
		private bool InitFromDataTable(DataTable tbl)
		{
			if (tbl == null) return false;

			foreach (DataColumn col in tbl.Columns)
			{
				AddDataColumn(col);
			}
			return true;
		}

		private BaField AddDataColumn(DataColumn col)
		{
			var fld = Add(col.ColumnName, col.DataType);

			if (!col.AllowDBNull)
				fld.Validators.Required();

			if (col.MaxLength > 0)
				fld.Validators.MaxLength(col.MaxLength);

			return fld;
		}

		/// <summary>
		/// Initializes the schema from an IDataReader.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		private bool InitFromDataReader(IDataReader row)
		{
			if (row == null) return false;

			for (var i = 0; i < row.FieldCount; i++)
			{
				var name = row.GetName(i);
				var type = row.GetFieldType(i);
				var value = row.GetValue(i);
				Add(name, type, value);
			}
			return true;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the options object used to create the BaObject.
		/// </summary>
		public BaObjectOptions Options { get; private set; }

		/// <summary>
		/// Gets the fields in this object.
		/// </summary>
		public BaFieldList Fields { get; } = new BaFieldList();

		/// <summary>
		/// Gets a value that determines if the object has changed.
		/// </summary>
		public bool HasChanged
		{
			get
			{
				foreach (var fld in Fields)
				{
					if (fld.IsChanged)
						return true;
				}
				return false;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds the field to the object.
		/// </summary>
		/// <typeparam name="T">The datatype for the field.</typeparam>
		/// <param name="fldName">Name of the field.</param>
		/// <param name="dflt">Default value for the field. Used to determine if the field has changed.</param>
		/// <returns></returns>
		public BaField Add<T>(string fldName, T dflt)
		{
			if (Fields.ContainsField(fldName))
				throw new ArgumentException("A field already exists with this field name.");
			var fld = new BaField(this, fldName, typeof(T), dflt);
			Fields.Add(fld);
			return fld;
		}

		/// <summary>
		/// Adds the field to the object.
		/// </summary>
		/// <param name="fldName">Name of the field.</param>
		/// <param name="fldType">The data type for the field.</param>
		/// <param name="dflt">Default value for the field. Used to determine if the field has changed. If null, it is converted to the default value for fieldType.</param>
		/// <returns></returns>
		public BaField Add(string fldName, Type fldType, object dflt = null)
		{
			if (Fields.ContainsField(fldName))
				throw new ArgumentException("A field already exists with this field name.");
			var fld = new BaField(this, fldName, fldType, dflt);
			Fields.Add(fld);
			return fld;
		}

		private ICollection<KeyValuePair<string, object>> GetKeyValuePairs()
		{
			return null;
		}

		/// <summary>
		/// Tries to get the value. Returns true if the value is in the object or if strict is false.
		/// </summary>
		/// <param name="fldName">Name of the field to get.</param>
		/// <param name="result">The value contained in the field. Value will be null if the field is not found and strict is false.</param>
		/// <returns>True if the field is found.</returns>
		public bool TryGet(string fldName, out object result)
		{
			result = null;

			var fld = Fields[fldName];
			if (fld != null)
			{
				result = fld.Value;
				return true;
			}

			if (Options.StrictGet)
				return false;

			result = null;
			return true;
		}

		/// <summary>
		/// Tries to set the value. If strict is off and the field is not found, it will be added.
		/// </summary>
		/// <param name="fldName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TrySet(string fldName, object value)
		{
			var fld = Fields[fldName];
			if (fld != null)
			{
				Fields[fldName].Value = value;
				return true;
			}

			if (Options.StrictSet)
				return false;

			var fldType = typeof(object);
			if (value != null) fldType = value.GetType();
			Add(fldName, fldType, value);
			return true;
		}

		/// <summary>
		/// Returns a dictionary of changed values.
		/// </summary>
		/// <param name="ignore">Excludes these fields from the results. Useful for readonly or restricted fields.</param>
		/// <returns></returns>
		public virtual IDictionary<string, object> GetChanges(params string[] ignore)
		{
			var changes = new Dictionary<string, object>();
			foreach (var fld in Fields)
			{
				// Check to see if we should ignore this field.
				// This is useful if there are readonly or 
				// restricted fields.
				if (ignore.Contains(fld.Name)) continue;

				if (fld.IsChanged)
					changes.Add(fld.Name, fld.Value);
			}
			return changes;
		}

		/// <summary>
		/// Updates the default value to be the same as value so that the fields show up as not changed.
		/// </summary>
		public void UpdateDefaults()
		{
			foreach (var fld in Fields)
			{
				if (fld.IsSet) // If it's not set, Value returns the DefaultValue.
					fld.DefaultValue = fld.Value;
			}
		}

		/// <summary>
		/// Uses DataAnnotations to validate the properties of the object.
		/// </summary>
		public ValidationResult[] Validate(bool changedOnly = true)
		{
			var ctx = new ValidationContext(this, null, null);
			var results = new List<ValidationResult>();
			foreach (var fld in Fields)
			{
				if (!fld.IsSet) continue; // Only validate set properties.
				if (changedOnly && !fld.IsChanged) continue;

				ctx.DisplayName = fld.Name;
				Validator.TryValidateValue(fld.Value, ctx, results, fld.Validators);
			}
			return results.ToArray();
		}

		/// <summary>
		/// Fills this instance with data. Only sets fields that are in the schema (others are ignored).
		/// </summary>
		/// <param name="data">Converts to a property bag then sets the values.</param>
		public void Fill(object data)
		{
			if (data == null) return;

			var propBag = ObjectUtil.ToPropertyBag(data);
			foreach(var fld in Fields)
			{
				if (!propBag.ContainsKey(fld.Name)) continue;
				fld.Value = propBag[fld.Name];
			}
		}

		#endregion

		#region IDictionary

		/// <summary>
		/// Gets or sets the value for the given field.
		/// </summary>
		/// <param name="fldName"></param>
		/// <returns></returns>
		public object this[string fldName]
		{
			get
			{
				object value;
				if (TryGet(fldName, out value))
					return value;

				// TryGet only returns false if strict is off.
				throw new ArgumentOutOfRangeException("key", $"Field [{fldName}] not found.");
			}
			set
			{
				// TrySet only returns false if strict is on.
				if (!TrySet(fldName, value))
					throw new ArgumentOutOfRangeException("key", $"Field [{fldName}] not found.");
			}
		}

		int ICollection<KeyValuePair<string, object>>.Count
		{
			get
			{
				return Fields.Count;
			}
		}

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		ICollection<string> IDictionary<string, object>.Keys
		{
			get
			{
				return Fields.Select(fld => fld.Name).ToArray();
			}
		}

		ICollection<object> IDictionary<string, object>.Values
		{
			get
			{
				return Fields.Select(fld => fld.Value).ToArray();
			}
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			((IDictionary<string, object>)this).Add(item.Key, item.Value);
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			var fldType = typeof(object);
			if (value != null)
				fldType = value.GetType();
			var fld = Add(key, fldType);
			fld.Value = value;
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			throw new NotSupportedException("Cannot clear list of fields.");
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			var fld = Fields[item.Key];
			if (fld == null) return false;
			return fld.Value == item.Value;
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return Fields.ContainsField(key);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			var pairs = GetKeyValuePairs();
			return pairs.GetEnumerator();
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			var pairs = GetKeyValuePairs();
			return pairs.GetEnumerator();
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			var fld = Fields[item.Key];
			if (fld == null) return false;
			if (fld.Value != item.Value) return false;
			return Fields.Remove(fld);
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			var fld = Fields[key];
			if (fld == null) return false;
			return Fields.Remove(fld);
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			return TryGet(key, out value);
		}

		#endregion

		#region IDynamicMetaObjectProvider

		// Implement the interface so we can hide the DynamicObject methods.
		private BaDynamicObject mDynamicInstance;

		DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
		{
			if (mDynamicInstance == null)
				mDynamicInstance = new BaDynamicObject(this);
			return new DelegatingMetaObject(mDynamicInstance, parameter, BindingRestrictions.GetTypeRestriction(parameter, this.GetType()), this); //mDynamicInstance.GetMetaObject(parameter);
		}

		#region BaDynamicObject

		/// <summary>
		/// Support for making BaObject dynamic. Hides implementation detail from BaObject (nobody needs to see all those methods).
		/// </summary>
		private class BaDynamicObject : DynamicObject
		{

			#region Initialization and Destruction

			public BaDynamicObject(BaObject obj)
			{
				Object = obj;
			}

			#endregion

			#region Fields and Properties

			public BaObject Object { get; private set; }

			#endregion

			#region Methods

			public override bool TryGetMember(GetMemberBinder binder, out object result)
			{
				return Object.TryGet(binder.Name, out result);
			}

			public override bool TrySetMember(SetMemberBinder binder, object value)
			{
				return Object.TrySet(binder.Name, value);
			}

			#endregion

		}

		#endregion

		#region DelegatingMetaObject

		/// <summary>
		/// This class allows us to redirect MetaObject calls to the correct object.
		/// </summary>
		/// <remarks>This class courtesy of: http://stackoverflow.com/a/17634595/320</remarks>
		private class DelegatingMetaObject : DynamicMetaObject
		{
			private readonly IDynamicMetaObjectProvider mProvider;

			public DelegatingMetaObject(IDynamicMetaObjectProvider innerProvider, Expression expr, BindingRestrictions restrictions)
				: base(expr, restrictions)
			{
				this.mProvider = innerProvider;
			}

			public DelegatingMetaObject(IDynamicMetaObjectProvider innerProvider, Expression expr, BindingRestrictions restrictions, object value)
				: base(expr, restrictions, value)
			{
				this.mProvider = innerProvider;
			}

			public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
			{
				var meta = mProvider.GetMetaObject(Expression.Constant(mProvider));
				return meta.BindInvokeMember(binder, args);
			}

			public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
			{
				var meta = mProvider.GetMetaObject(Expression.Constant(mProvider));
				return meta.BindGetMember(binder);
			}

			public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
			{
				var meta = mProvider.GetMetaObject(Expression.Constant(mProvider));
				return meta.BindSetMember(binder, value);
			}

		}

		#endregion

		#endregion

		#region INotifyPropertyChanged

		/// <summary>
		/// Event raised when a field changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="fldName"></param>
		protected internal void OnPropertyChanged(string fldName)
		{
			var e = new PropertyChangedEventArgs(fldName);
			OnPropertyChanged(e);
		}

		#endregion

	}

}
