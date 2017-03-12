using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// The base class for classes that access the database. A repository should have fairly simple database commands and no business logic.
	/// </summary>
	public class BaRepository : IDisposable, ISupportBaDatabase
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaRepository.
		/// </summary>
		/// <param name="db">The database to use for the repository.</param>
		/// <param name="disposeDB">If true, the database passed in is disposed when the repository is disposed.</param>
		public BaRepository(ISupportBaDatabase db, bool disposeDB = true)
		{
			if (db == null) throw new ArgumentNullException("db");
			if (db.Database == null) throw new ArgumentException("Unable to access the database.", "db");

			DisposeDatabase = disposeDB;
			Database = db.Database;
		}

		/// <summary>
		/// Cleans up any resources that the repository is using.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Cleans up any resources that the repository is using.
		/// </summary>
		/// <param name="disposing">True if called from Dispose, false if called from finalizer.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (Database != null && DisposeDatabase)
			{
				Database.Dispose();
				Database = null;
			}
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets or sets a value that determines if the database should be disposed when the repository is disposed.
		/// </summary>
		public bool DisposeDatabase { get; set; }

		/// <summary>
		/// Gets the database for this repository instance.
		/// </summary>
		public BaDatabase Database { get; private set; }

		#endregion

	}
}
