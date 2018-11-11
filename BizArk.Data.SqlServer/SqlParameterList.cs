using BizArk.Core.Extensions.ObjectExt;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BizArk.Data
{

	/// <summary>
	/// Provides some useful functions for a list of SqlParameters.
	/// </summary>
	public class SqlParameterList : List<SqlParameter>
	{

		/// <summary>
		/// Add a SqlParameter to the list. If the value is empty, will use DbNull.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public SqlParameter AddWithValue(string name, object value)
		{
			var param = new SqlParameter(name, value.IfEmpty(DBNull.Value));
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
			foreach(var prop in propBag)
			{
				sqlParams.Add(AddWithValue(prop.Key, prop.Value));
			}
			return sqlParams;
		}

	}

}
