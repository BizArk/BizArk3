using System;
using System.Data.SqlClient;

namespace BizArk.Data.SqlServer
{

	/// <summary>
	/// Provides a wrapper around SqlTransaction to support BaDatabase.
	/// </summary>
	public class BaTransaction : IDisposable
	{

		#region Initialization and Destruction

		internal BaTransaction(BaDatabase db)
		{
			Database = db;
			Transaction = db.Connection.BeginTransaction();
		}

		/// <summary>
		/// If the transaction hasn't been committed, it will be rolled back.
		/// </summary>
		public void Dispose()
		{
			if (Transaction != null)
			{
				Transaction.Rollback();
				CloseTransaction();
			}
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the database this transaction belongs to.
		/// </summary>
		public BaDatabase Database { get; private set; }

		/// <summary>
		/// Gets the transaction that this object wraps.
		/// </summary>
		public SqlTransaction Transaction { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Rolls the transaction back and invalidates this object. This is automatically called if Commit is not called.
		/// </summary>
		public void Rollback()
		{
			Transaction.Rollback();
			CloseTransaction();
		}

		/// <summary>
		/// Commits the transaction and invalidates this object. This is automatically called if Commit is not called.
		/// </summary>
		public void Commit()
		{
			Transaction.Rollback();
			CloseTransaction();
		}

		private void CloseTransaction()
		{
			Transaction.Dispose();
			Transaction = null;
			Database.Transaction = null;
		}

		#endregion

	}
}
