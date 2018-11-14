using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace BizArk.Data
{

	/// <summary>
	/// A wrapper around `DbTransaction` to make working with transactions simpler. Can only be created by calling `BaDatabase.BeginTransaction`.
	/// </summary>
	public class BaTransaction : IDisposable
	{

		#region Initialization and Destruction

		private BaTransaction(BaDatabase db)
		{
			Database = db;
		}

		/// <summary>
		/// Starts a new transaction.
		/// </summary>
		internal static BaTransaction Begin(BaDatabase db)
		{
			var bat = new BaTransaction(db);
			bat.mPreviousTransaction = db.Transaction; // Support nested transactions. NOTE: Sql Server does not support nested tranactions.

			bat.Transaction = db.Connection.BeginTransaction();
			return bat;
		}

		/// <summary>
		/// Starts a new transaction.
		/// </summary>
		/// <returns></returns>
		internal static async Task<BaTransaction> BeginAsync(BaDatabase db)
		{
			var bat = new BaTransaction(db);
			bat.mPreviousTransaction = db.Transaction; // Support nested transactions.

			var conn = await db.GetConnectionAsync().ConfigureAwait(false);
			bat.Transaction = conn.BeginTransaction();
			return bat;
		}

		/// <summary>
		/// If the transaction hasn't been committed, it will be rolled back.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// If the transaction hasn't been committed, it will be rolled back.
		/// </summary>
		/// <param name="disposing">True if called from Dispose, false if called from finalizer.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (Transaction != null)
			{
				Rollback();
			}
		}

		#endregion

		#region Fields and Properties

		private BaTransaction mPreviousTransaction;

		/// <summary>
		/// Gets the database this transaction belongs to.
		/// </summary>
		public BaDatabase Database { get; private set; }

		/// <summary>
		/// Gets the transaction that this object wraps.
		/// </summary>
		public DbTransaction Transaction { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Rolls the transaction back and invalidates this object. This is automatically called in Dispose if Commit is not called.
		/// </summary>
		public void Rollback()
		{
			Transaction.Rollback();
			CloseTransaction();
		}

		/// <summary>
		/// Commits the transaction and invalidates this object.
		/// </summary>
		public void Commit()
		{
			Transaction.Commit();
			CloseTransaction();
		}

		private void CloseTransaction()
		{
			Transaction.Dispose();
			Transaction = null;
			Database.Transaction = mPreviousTransaction;
			mPreviousTransaction = null;
		}

		#endregion

	}
}
