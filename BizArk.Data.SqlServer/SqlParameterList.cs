using BizArk.Core.Extensions.ObjectExt;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// Provides some useful functions for a list of SqlParameters.
	/// </summary>
	public class SqlParameterList : List<SqlParameter>
	{

		/// <summary>
		/// Add a SqlParameter to the list.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public SqlParameter AddWithValue(string name, object value)
		{
			var param = new SqlParameter(name, value);
			this.Add(param);
			return param;
		}

		/// <summary>
		/// Adds the properties of the object as parameters. Uses BizArk.Core.Util.ObjectUtil.ToPropertyBag to get values.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public int AddValues(object values)
		{
			var propBag = values.ToPropertyBag();
			var count = 0;
			foreach(var prop in propBag)
			{
				AddWithValue(prop.Key, prop.Value);
				count++;
			}
			return count;
		}

	}

}
