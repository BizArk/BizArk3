using BizArk.Core.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// Represents a database table.
	/// </summary>
	public class BaTableObject : BaObject
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of BaTableObject.
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="strict">If true, only fields added can be set or retrieved. If false, getting a field that doesn't exist returns null and setting a field that doesn't exist automatically adds it.</param>
		public BaTableObject(DataTable schema, bool strict) : base(strict, schema)
		{
			mSchema = schema;
			TableName = schema.TableName;
			InitReadonlyFields();
		}

		/// <summary>
		/// Creates an instance of BaTableObject from a schema instance.
		/// </summary>
		/// <param name="schema"></param>
		/// <param name="strict">If true, only fields added can be set or retrieved. If false, getting a field that doesn't exist returns null and setting a field that doesn't exist automatically adds it.</param>
		public BaTableObject(BaTableObject schema, bool strict) : base(strict, schema)
		{
			mSchema = schema.mSchema;
			TableName = schema.TableName;
			InitReadonlyFields();
		}

		private void InitReadonlyFields()
		{
			foreach (DataColumn col in mSchema.Columns)
			{
				if (col.ReadOnly)
					ReadonlyFields.Add(col.ColumnName);
				else if (col.AutoIncrement)
					ReadonlyFields.Add(col.ColumnName);
			}
		}

		#endregion

		#region Fields and Properties

		private DataTable mSchema;

		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		public string TableName { get; private set; }

		/// <summary>
		/// Gets a flag that determines if the object should be inserted or updated.
		/// </summary>
		public bool IsNew
		{
			get
			{
				var key = GetKey();
				if (key == null) return true;
				if (key.Length == 0) return true;

				// To be an update, all of the key fields must be set.
				return key.All(fld => !fld.IsSet);
			}
		}

		/// <summary>
		/// Gets the list of read-only fields. The fields can still be set, but they won't be included in GetChanges.
		/// </summary>
		protected List<string> ReadonlyFields { get; private set; } = new List<string>();

		#endregion

		#region Methods

		/// <summary>
		/// Returns a dictionary of changed values. Ignores readonly fields.
		/// </summary>
		/// <param name="restricted">List of fields that are restricted. These will be added to list of ignored fields.</param>
		/// <returns></returns>
		public override IDictionary<string, object> GetChanges(params string[] restricted)
		{
			var ignore = restricted.ToList();
			ignore.AddRange(ReadonlyFields);

			// Add any fields that aren't in the table schema.
			ignore.AddRange(Fields.Where(fld => !mSchema.Columns.Contains(fld.Name)).Select(fld => fld.Name));

			// Add the key to the list of ignored fields if
			// this is not a new record.
			if (!IsNew)
				ignore.AddRange(GetKey().Select(fld => fld.Name));

			return base.GetChanges(ignore.ToArray());
		}

		private BaField[] mKey;
		/// <summary>
		/// Gets the key that can be used to save the object.
		/// </summary>
		/// <returns></returns>
		public BaField[] GetKey()
		{
			if (mKey == null)
			{
				var key = new List<BaField>();

				foreach (var col in mSchema.PrimaryKey)
				{
					var fld = Fields[col.ColumnName];
					key.Add(fld);
				}

				mKey = key.ToArray();
			}

			return mKey;
		}

		#endregion

	}
}
