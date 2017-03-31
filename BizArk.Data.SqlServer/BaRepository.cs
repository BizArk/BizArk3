using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// The base class for classes that access the database. A repository should have fairly simple database commands and no business logic.
	/// </summary>
	public class BaRepository : IDisposable, ISupportBaDatabase
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates a new instance of BaRepository creating a new instance of BaDatabase.
		/// </summary>
		/// <param name="name">The name or key of the connection string in the config file.</param>
		public BaRepository(string name)
			: this(BaDatabase.Create(name))
		{
			DisposeDatabase = true;
		}

		/// <summary>
		/// Creates a new instance of BaRepository.
		/// </summary>
		/// <param name="db">The database to use for the repository. The database will not be disposed with the repository.</param>
		public BaRepository(ISupportBaDatabase db)
		{
			if (db == null) throw new ArgumentNullException("db");
			if (db.Database == null) throw new ArgumentException("Unable to access the database.", "db");

			DisposeDatabase = false;
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
				Database.Dispose();
			Database = null;
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

		#region Methods

		/// <summary>
		/// Starts a transaction. Must call Dispose on the transaction.
		/// </summary>
		/// <returns></returns>
		public BaTransaction BeginTransaction()
		{
			return Database.BeginTransaction();
		}

		/// <summary>
		/// Creates a transaction and executes the batch within it. If a deadlock is detected, rolls the transaction back and tries again.
		/// </summary>
		/// <param name="batch">The code to execute within a transaction.</param>
		public void TryTransaction(Action batch)
		{
			var attempt = 1;
			while (true)
			{
				try
				{
					using (var trans = BeginTransaction())
					{
						batch();
						trans.Commit();
						return;
					}
				}
				catch (SqlException ex) when (ex.ErrorCode == BaDatabase.cSqlError_Deadlock && attempt <= Database.RetriesOnDeadlock)
				{
					Debug.WriteLine($"Deadlock identified on attempt {attempt}. Retrying.");
					attempt++;

					// If the transaction fails it can leave the connection in a poor state. 
					// Re-establish the connection to be sure we are working from a good connection.
					Database.ResetConnection();
				}
			}
		}

		/// <summary>
		/// Saves the table object.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="restricted"></param>
		public void Save(BaTableObject obj, params string[] restricted)
		{
			if (obj == null) return;

			// Check to see if there are any changes to save.
			var updates = obj.GetChanges(restricted);
			if (updates.Count == 0) return;

			if (obj.IsNew)
			{
				var values = Database.Insert(obj.TableName, updates);
				obj.Fill((object)values);
			}
			else
			{
				var key = new Dictionary<string, object>();
				foreach (var fld in obj.GetKey())
					key.Add(fld.Name, fld.Value);
				Database.Update(obj.TableName, key, updates);
			}
			obj.UpdateDefaults();
		}

		#endregion

	}
}
