using BizArk.Core.Extensions.ObjectExt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// Used in `SelectCmdBuilder` to hold parameters while building the command.
	/// </summary>
	public class SqlParameterList : List<SqlParameter>
	{

		/// <summary>
		/// Add a SqlParameter to the list. If the value is empty, will use DbNull.
		/// </summary>
		/// <param name="name">Name of the parameter. Does not need @ in front.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="dbType">The database type. Optional, will guess best type based on the type of the value.</param>
		/// <param name="size">The size of the parameter. Used for strings, blobs, etc.</param>
		/// <returns></returns>
		public SqlParameter AddWithValue(string name, object value, SqlDbType? dbType = null, int? size = null)
		{
			var param = new SqlParameter(name, value.IfEmpty(DBNull.Value));

			if (dbType.HasValue) param.SqlDbType = dbType.Value;
			if (size.HasValue) param.Size = size.Value;

			this.Add(param);
			return param;
		}

		/// <summary>
		/// Adds the properties of the object as parameters. Uses BizArk.Core.Util.ObjectUtil.ToPropertyBag to get values.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public IEnumerable<SqlParameter> AddValues(object values)
		{
			var propBag = values.ToPropertyBag();
			var sqlParams = new List<SqlParameter>();
			foreach (var prop in propBag)
			{
				sqlParams.Add(AddWithValue(prop.Key, prop.Value));
			}
			return sqlParams;
		}

	}

}
