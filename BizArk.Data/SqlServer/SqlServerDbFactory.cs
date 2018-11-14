namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// Factory class for building `SqlServerDatabase` objects. Intended for use with `BaDatabase.Register()`.
	/// </summary>
	public class SqlServerDbFactory
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of SqlServerDbFactory.
		/// </summary>
		public SqlServerDbFactory(string connStr)
		{
			ConnectionString = connStr;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the connection string for the database to create.
		/// </summary>
		public string ConnectionString { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Creates a new instance of SqlServerDatabase.
		/// </summary>
		/// <returns></returns>
		public BaDatabase Create()
		{
			return new SqlServerDatabase(ConnectionString);
		}

		#endregion


	}
}
