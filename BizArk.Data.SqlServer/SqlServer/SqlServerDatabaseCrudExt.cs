using BizArk.Core.Util;
using BizArk.Data.DataExt;
using BizArk.Data.ExtractExt;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.Data.SqlServer.CrudExt
{

	/// <summary>
	/// Provides SqlServer specific extension methods for CRUD operations.
	/// </summary>
	public static class SqlServerDatabaseCrudExt
	{

		/// <summary>
		/// Inserts a new record into the table.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName">Name of the table to insert into.</param>
		/// <param name="values">The values that will be added to the table. Can be anything that can be converted to a property bag.</param>
		/// <returns>The newly inserted row.</returns>
		public static dynamic Insert(this BaDatabase db, string tableName, object values)
		{
			var cmd = db.Connection.CreateCommand();
			PrepareInsertCmd(cmd, tableName, values);
			return db.GetDynamic(cmd);
		}

		/// <summary>
		/// Inserts a new record into the table.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName">Name of the table to insert into.</param>
		/// <param name="values">The values that will be added to the table. Can be anything that can be converted to a property bag.</param>
		/// <returns>The newly inserted row.</returns>
		public async static Task<dynamic> InsertAsync(this BaDatabase db, string tableName, object values)
		{
			var conn = await db.GetConnectionAsync();
			var cmd = conn.CreateCommand();
			PrepareInsertCmd(cmd, tableName, values);
			return await db.GetDynamicAsync(cmd).ConfigureAwait(false);
		}

		/// <summary>
		/// Creates the insert command.
		/// </summary>
		/// <remarks>Internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		internal static void PrepareInsertCmd(DbCommand cmd, string tableName, object values)
		{
			var sb = new StringBuilder();

			var propBag = ObjectUtil.ToPropertyBag(values);

			var inFields = new StringBuilder();
			var valFields = new StringBuilder();
			foreach (var val in propBag)
			{
				if (inFields.Length > 0) inFields.Append(", ");
				inFields.Append(val.Key);

				if (valFields.Length > 0) valFields.Append(", ");
				var literal = GetValueLiteral(val.Value as string);
				if (literal != null)
				{
					valFields.Append(literal);
				}
				else
				{
					valFields.Append($"@{val.Key}");
					cmd.AddParameter(val.Key, val.Value, true);
				}
			}

			sb.AppendLine($"INSERT INTO {tableName} ({inFields})");
			sb.AppendLine("\tOUTPUT INSERTED.*");
			sb.AppendLine($"\tVALUES ({valFields});");

			cmd.CommandText = sb.ToString();
		}

		/// <summary>
		/// Updates the table with the given values.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName">Name of the table to update.</param>
		/// <param name="key">Key of the record to update. Can be anything that can be converted to a property bag.</param>
		/// <param name="values">The values that will be updated. Can be anything that can be converted to a property bag.</param>
		/// <returns></returns>
		public static int Update(this BaDatabase db, string tableName, object key, object values)
		{
			var cmd = db.Connection.CreateCommand();
			PrepareUpdateCmd(cmd, tableName, key, values);
			return db.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// Updates the table with the given values.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName">Name of the table to update.</param>
		/// <param name="key">Key of the record to update. Can be anything that can be converted to a property bag.</param>
		/// <param name="values">The values that will be updated. Can be anything that can be converted to a property bag.</param>
		/// <returns></returns>
		public async static Task<int> UpdateAsync(this BaDatabase db, string tableName, object key, object values)
		{
			var conn = await db.GetConnectionAsync();
			var cmd = conn.CreateCommand();
			PrepareUpdateCmd(cmd, tableName, key, values);
			return await db.ExecuteNonQueryAsync(cmd).ConfigureAwait(false);
		}

		/// <summary>
		/// Creates the update command.
		/// </summary>
		/// <remarks>Internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		internal static void PrepareUpdateCmd(DbCommand cmd, string tableName, object key, object values)
		{
			var sb = new StringBuilder();

			sb.AppendLine($"UPDATE {tableName} SET");

			var propBag = ObjectUtil.ToPropertyBag(values);
			var keys = propBag.Keys.ToArray();
			for (var i = 0; i < keys.Length; i++)
			{
				var val = propBag[keys[i]];

				sb.Append($"\t\t{keys[i]} = ");

				var literal = GetValueLiteral(val as string);
				if (literal != null)
				{
					sb.Append(literal);
				}
				else
				{
					sb.Append($"@{keys[i]}");
					cmd.AddParameter(keys[i], val, true);
				}

				if (i < keys.Length - 1)
					sb.Append(",");
				sb.AppendLine();
			}

			var criteria = PrepareCriteria(cmd, key);
			if (criteria != null)
				sb.AppendLine(criteria);

			cmd.CommandText = sb.ToString();
		}

		/// <summary>
		/// Deletes the records with the given key.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName">Name of the table to remove records from.</param>
		/// <param name="key">Key of the record to delete. Can be anything that can be converted to a property bag.</param>
		/// <returns></returns>
		public static int Delete(this BaDatabase db, string tableName, object key)
		{
			var cmd = db.Connection.CreateCommand();
			PrepareDeleteCmd(cmd, tableName, key);
			return db.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// Deletes the records with the given key.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tableName">Name of the table to remove records from.</param>
		/// <param name="key">Key of the record to delete. Can be anything that can be converted to a property bag.</param>
		/// <returns></returns>
		public async static Task<int> DeleteAsync(this BaDatabase db, string tableName, object key)
		{
			var conn = await db.GetConnectionAsync();
			var cmd = conn.CreateCommand();
			PrepareDeleteCmd(cmd, tableName, key);
			return await db.ExecuteNonQueryAsync(cmd).ConfigureAwait(false);
		}

		/// <summary>
		/// Creates the delete command.
		/// </summary>
		/// <remarks>Internal so it can be called from the unit tests.</remarks>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		internal static void PrepareDeleteCmd(DbCommand cmd, string tableName, object key)
		{
			var sb = new StringBuilder();

			sb.AppendLine($"DELETE FROM {tableName}");

			var criteria = PrepareCriteria(cmd, key);
			if (criteria != null)
				sb.AppendLine(criteria);

			cmd.CommandText = sb.ToString();
		}

		private static string PrepareCriteria(DbCommand cmd, object key)
		{
			if (key == null) return null;

			var criteria = new StringBuilder();
			var propBag = ObjectUtil.ToPropertyBag(key);
			foreach (var val in propBag)
			{
				if (criteria.Length > 0)
					criteria.Append("\n\t\tAND ");
				criteria.Append($"{val.Key} = ");

				var literal = GetValueLiteral(val.Value as string);
				if (literal != null)
				{
					criteria.Append(literal);
				}
				else
				{
					criteria.Append($"@{val.Key}");
					cmd.AddParameter(val.Key, val.Value, true);
				}
			}
			return $"\tWHERE {criteria}";
		}

		private static string GetValueLiteral(string str)
		{
			if (str == null) return null;
			if (!str.StartsWith("[[")) return null;
			if (!str.EndsWith("]]")) return null;
			return str.Substring(2, str.Length - 4);
		}

	}
}
