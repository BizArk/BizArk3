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
	/// Extension methods for working with sql objects.
	/// </summary>
	public static class SqlClientExt
	{

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
