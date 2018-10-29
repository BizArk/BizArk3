using BizArk.Core.Extensions.StringExt;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// Build a complex SqlCommand object.
	/// </summary>
	public class SelectCmdBuilder
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SelectCmdBuilder.
        /// </summary>
        /// <param name="baseTable">This is the primary table for the select command. It can be in the form "MyTable mt" and also include hints or anything else that is legal syntax (include joins, etc).</param>
        public SelectCmdBuilder(string baseTable)
        {
            if (baseTable.IsEmpty()) throw new ArgumentNullException("baseTable");
            BaseTable = baseTable;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the base table for the select command.
        /// </summary>
        public string BaseTable { get; private set; }

        /// <summary>
        /// Gets or sets a flag that determines if the DISTINCT keyword should be included in the SQL.
        /// </summary>
        public bool Distinct { get; set; }

        /// <summary>
        /// Gets or sets the number of records to return. Set to null to get all the records.
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// Gets or sets the name of the row number field when using the paged CreateCmd. This must have a value or the query won't work.
        /// </summary>
        public string RowNbrFldName { get; set; } = "RowNbr";

        /// <summary>
        /// Gets or sets the name of the count field when using the paged CreateCmd. Set this to null to omit the count from the query.
        /// </summary>
        public string CountFldName { get; set; } = "TotalRows";

        /// <summary>
        /// Gets the list of fields that you can add to. Fields are not validated, they are simply added to the list of fields in the SQL.
        /// </summary>
        public List<string> Fields { get; } = new List<string>();

        /// <summary>
        /// Gets the list of joins that you can add to. Should be in the form of "JOIN MyTable mt ON (mt.SomeID = xx.SomeID)". Joins are not validated, they are simply added to the list of joins in the SQL.
        /// </summary>
        public List<string> Joins { get; } = new List<string>();

        /// <summary>
        /// Gets the list of criteria that you can add to. Criteria is always put joined using AND. Criteria is not validated, it is simply added to the criteria in the SQL.
        /// </summary>
        public List<string> Criteria { get; } = new List<string>();

        /// <summary>
        /// Gets the list of order by fields that you can add to. Should be in the form of "MyField ASC" or "xx.MyField DESC".
        /// </summary>
        public List<string> OrderBy { get; } = new List<string>();

        /// <summary>
        /// Gets the list of parameters that you can add to.
        /// </summary>
        public SqlParameterList Parameters { get; } = new SqlParameterList();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the SELECT SQL statement that will be used in the command.
        /// </summary>
        /// <param name="count">If true, gets the sql that will tell you how many records are available for this criteria.</param>
        /// <param name="rowNbrFldName">If set, includes a column with the given name that is the sequential number of the row in the query (based on the OrderBy). Ignored if count is true.</param>
        /// <param name="countFldName">If set, includes a column with the given name that counts all of the rows that will be returned in the query. Ignored if count is true.</param>
        /// <returns></returns>
        private string GetSql(bool count = false, string rowNbrFldName = null, string countFldName = null)
        {
            var sql = new StringBuilder();

            sql.Append("SELECT ");

            // Add the fields (or equivalent).
            if (count)
                sql.AppendLine("COUNT(*)");
            else
            {
                // Add SQL decorators.
                if (Distinct) sql.Append("DISTINCT ");
                if (Top != null) sql.Append($" TOP {Top} ");

                // If we are specifying DISTINCT or TOP ###, start the fields on the second line.
                if (Distinct || Top != null) sql.Append("\n\t\t");

                if (rowNbrFldName.HasValue())
                    sql.Append($"ROW_NUMBER() OVER(ORDER BY {string.Join(", ", OrderBy)}) AS {rowNbrFldName},\n\t\t");

                if (countFldName.HasValue())
                    sql.Append($"COUNT(*) OVER() AS {countFldName},\n\t\t");

                if (Fields.Count == 0)
                    sql.AppendLine("*");
                else if (Fields.Count < 5)
                    // If there are less than 5 fields, just put them on a single line.
                    sql.AppendLine(string.Join(", ", Fields));
                else
                    sql.AppendLine(string.Join(",\n\t\t", Fields));
            }

            sql.AppendLine($"\tFROM {BaseTable}");

            if (Joins.Count > 0)
                sql.AppendLine("\t\t" + string.Join("\n\t\t", Joins));

            if (Criteria.Count > 0)
            {
                sql.Append("\tWHERE ");
                sql.AppendLine(string.Join("\n\t\tAND ", Criteria));
            }

            if (!count && countFldName.IsEmpty() && OrderBy.Count > 0)
            {
                sql.Append("\tORDER BY ");
                sql.AppendLine(string.Join(", ", OrderBy));
            }

            return sql.ToString();
        }

		/// <summary>
		/// Creates the command that will return all of the results.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public SqlCommand CreateCmd()
        {
            var cmd = new SqlCommand(GetSql());
            cmd.Parameters.AddRange(Parameters.ToArray());
            return cmd;
        }

		/// <summary>
		/// Creates a command that will tell you how many results there are for this SQL.
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public SqlCommand CreateCountCmd()
        {
            var cmd = new SqlCommand(GetSql(true));
            cmd.Parameters.AddRange(Parameters.ToArray());
            return cmd;
        }

		/// <summary>
		/// Creates a command that will return a single page of results.
		/// </summary>
		/// <param name="skip">The number of results to skip before returning results (essentially the page)</param>
		/// <param name="take">The number of results per page.</param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public SqlCommand CreateCmd(int skip, int take)
		{
            if (OrderBy.Count == 0)
                throw new InvalidOperationException("The OrderBy fields must be specified in order to use paging.");

            var cmd = new SqlCommand(GetSql(rowNbrFldName: RowNbrFldName, countFldName: CountFldName));

            cmd.CommandText = $@"WITH qry AS
(
{cmd.CommandText})
SELECT *
	FROM qry
	WHERE {RowNbrFldName} BETWEEN {skip} AND {skip + take}
	ORDER BY {RowNbrFldName}
";

            cmd.Parameters.AddRange(Parameters.ToArray());
            return cmd;
        }

        #endregion

    }
}
