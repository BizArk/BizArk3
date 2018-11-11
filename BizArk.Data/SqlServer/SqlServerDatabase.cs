using BizArk.Core.Extensions.StringExt;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// Sql Server implementation of BaDatabase.
	/// </summary>
	public class SqlServerDatabase : BaDatabase
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of SqlServerDatabase.
		/// </summary>
		public SqlServerDatabase(string connStr)
		{
			if (connStr.IsEmpty()) throw new ArgumentNullException(nameof(connStr));
			ConnectionString = connStr;
		}

		/// <summary>
		/// Disposes the SqlServerDatabase instance.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			// Once the database has been disposed, we shouldn't need this anymore.
			// Null it out so it will fail if anybody attempts to access it after 
			// it's been disposed.
			ConnectionString = null;

			base.Dispose(disposing);
		}

		#endregion

		#region Fields and Properties

		// Internal so it can be viewed in the unit tests.
		internal string ConnectionString { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Instantiates a SqlConnection.
		/// </summary>
		/// <returns></returns>
		protected override DbConnection InstantiateConnection()
		{
			return new SqlConnection(ConnectionString);
		}

		#endregion


	}
}
