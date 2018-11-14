using BizArk.Core;
using BizArk.Core.Extensions.EnumerableExt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BizArk.Data.SqlServer.SqlClientExt
{

	/// <summary>
	/// Extension methods for working with objects in the System.Data.SqlClient namespace.
	/// </summary>
	public static class SqlClientExt
	{

		#region Add Parameters

		/// <summary>
		/// This will add an array of parameters to a SqlCommand. This is used for an IN statement.
		/// Use the returned value for the IN part of your SQL call. (i.e. SELECT * FROM table WHERE field IN ({paramNameRoot}))
		/// </summary>
		/// <param name="cmd">The SqlCommand object to add parameters to.</param>
		/// <param name="paramNameRoot">What the parameter should be named followed by a unique value for each value. This value surrounded by {} in the CommandText will be replaced.</param>
		/// <param name="values">The array of strings that need to be added as parameters.</param>
		/// <param name="dbType">One of the System.Data.SqlDbType values. If null, determines type based on T.</param>
		/// <param name="size">The maximum size, in bytes, of the data within the column. The default value is inferred from the parameter value.</param>
		public static IEnumerable<SqlParameter> AddParameters<T>(this SqlCommand cmd, string paramNameRoot, IEnumerable<T> values, SqlDbType? dbType = null, int? size = null)
		{
			// Call method in DataExt since it has almost all the same basic code other than we want to set SqlDbType instead of DbType.
			var parameters = DataExt.DataExt.AddParameters(cmd, paramNameRoot, values, (DbType?)null, (int?)null);
			var sqlparams = new List<SqlParameter>();

			// We need to convert the parameters to SqlParameter and set the SqlDbType and Size properties.
			foreach (var param in parameters)
			{
				var sqlparam = param as SqlParameter;
				sqlparams.Add(sqlparam);

				if (dbType.HasValue)
					sqlparam.SqlDbType = dbType.Value;
				if (size.HasValue)
					sqlparam.Size = size.Value;
			}

			return sqlparams;
		}

		#endregion

		#region Debug SQL

		/// <summary>
		/// Converts the command into TSql that can be executed in Sql Server Management Studio.
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public static string DebugText(this SqlCommand cmd)
		{
			// No parameters, no problem.
			if (cmd.Parameters.Count == 0)
				return DebugSqlCommandText(cmd);

			var sb = new StringBuilder();

			foreach (SqlParameter param in cmd.Parameters)
			{
				var paramName = DebugSqlParamName(param);
				var type = DebugSqlType(param);
				var value = DebugSqlValue(param);
				sb.Append($"DECLARE {paramName} AS {type} = {value}\n");
			}

			sb.Append(DebugSqlCommandText(cmd));

			return sb.ToString();
		}

		private static string DebugSqlCommandText(SqlCommand cmd)
		{
			switch (cmd.CommandType)
			{
				case CommandType.Text:
					return cmd.CommandText;
				case CommandType.StoredProcedure:
					var sb = new StringBuilder();

					sb.Append($"EXEC {cmd.CommandText} ");

					var first = true;
					foreach (SqlParameter param in cmd.Parameters)
					{
						if (!first)
							sb.Append(", ");
						first = false;
						sb.Append($"{DebugSqlParamName(param)}");
					}

					return sb.ToString();
				default:
					throw new ArgumentException($"'{cmd.CommandType.ToString()}' is not a supported CommandType for DebugSql.", "cmd");
			}
		}

		private static object DebugSqlParamName(SqlParameter param)
		{
			if (param.ParameterName.StartsWith("@"))
				return param.ParameterName;
			else
				return "@" + param.ParameterName;
		}

		private static object DebugSqlValue(SqlParameter param)
		{
			var val = param.Value;
			if (val == null) return "NULL";
			if (val == DBNull.Value) return "NULL";

			switch (param.SqlDbType)
			{
				case SqlDbType.Char:
				case SqlDbType.Text:
				case SqlDbType.Time:
				case SqlDbType.VarChar:
				case SqlDbType.Xml:
				case SqlDbType.Date:
				case SqlDbType.DateTime:
				case SqlDbType.DateTime2:
				case SqlDbType.DateTimeOffset:
					return val; // $"'{val.ToString().Replace("'", "''")}'";

				case SqlDbType.NChar:
				case SqlDbType.NText:
				case SqlDbType.NVarChar:
					return $"N'{val.ToString().Replace("'", "''")}'";

				case SqlDbType.Binary:
				case SqlDbType.VarBinary:
					var bytes = val as IEnumerable<byte>;
					return $"0x{bytes.ToHex()}";

				case SqlDbType.Bit:
					return ConvertEx.ToBool(val) ? "1" : "0";

				default:
					return val.ToString();
			}
		}

		private static string DebugSqlType(SqlParameter param)
		{
			switch (param.SqlDbType)
			{
				case SqlDbType.Char:
				case SqlDbType.VarChar:
				case SqlDbType.NChar:
				case SqlDbType.NText:
				case SqlDbType.NVarChar:
				case SqlDbType.Binary:
				case SqlDbType.VarBinary:
					return $"{param.SqlDbType.ToString().ToUpper()}({param.Size})";
				default:
					return param.SqlDbType.ToString().ToUpper();
			}
		}

		#endregion

	}
}
