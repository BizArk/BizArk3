using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Provides a way to clean up after something is no longer needed. Must be wrapped in using().
	/// </summary>
	public class Cleanup : IDisposable
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of Cleanup.
		/// </summary>
		/// <param name="clean"></param>
		public Cleanup(Action clean)
		{
			Cleaner = clean ?? throw new ArgumentNullException("clean");
		}

		/// <summary>
		/// Calls the Cleanup method.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes the Cleanup object.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (!Disposed)
				Cleaner();
			Disposed = true;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets the cleanup action.
		/// </summary>
		public Action Cleaner { get; private set; }

		/// <summary>
		/// Gets a value that determines if the cleaner has already been run.
		/// </summary>
		public bool Disposed { get; private set; }

		#endregion

	}
}
