using System;
using System.IO;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Manages a temporary file. Deletes the file when disposed.
	/// </summary>
	public class TempFile
		: IDisposable
	{

		#region Initialization and Destruction

		/// <summary>
		/// Creates an instance of TempFile.
		/// </summary>
		public TempFile()
		{
			TempPath = FileEx.GetUniqueFileName();
		}

		/// <summary>
		/// Creates an instance of TempFile.
		/// </summary>
		/// <param name="ext">The extension for the file.</param>
		public TempFile(string ext)
		{
			TempPath = FileEx.GetUniqueFileName(ext);
		}

		/// <summary>
		/// Creates an instance of TempFile.
		/// </summary>
		/// <param name="dir">The path to the directory.</param>
		/// <param name="template">The template for the file name. Place a {0} where the counter should go (ex, MyPicture{0}.jpg).</param>
		public TempFile(string dir, string template)
		{
			TempPath = FileEx.GetUniqueFileName(dir, template);
		}

		/// <summary>
		/// Deletes the temp file if it exists.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			// Use SupressFinalize in case a subclass
			// of this type implements a finalizer.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Deletes the temp file if it exists.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (Disposed) return;
			Delete();
			Disposed = true;
		}

		#endregion

		#region Fields and Properties

		/// <summary>
		/// Gets a flag that determines if the temp file has been disposed.
		/// </summary>
		public bool Disposed { get; private set; }

		/// <summary>
		/// Gets the path to the temporary file.
		/// </summary>
		public string TempPath { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Appends the contents to the temp file. Creates the temp file if it doesn't already exist.
		/// </summary>
		/// <param name="contents"></param>
		public void Append(string contents)
		{
			EnsureDir();
			File.AppendAllText(TempPath, contents);
		}

		/// <summary>
		/// Creates the temp file and writes the contents to it. 
		/// </summary>
		/// <param name="contents"></param>
		public void Write(string contents)
		{
			EnsureDir();
			File.WriteAllText(TempPath, contents);
		}

		/// <summary>
		/// Creates the temp file and writes the contents to it.
		/// </summary>
		/// <param name="contents"></param>
		public void Write(byte[] contents)
		{
			EnsureDir();
			File.WriteAllBytes(TempPath, contents);
		}

		private bool mDirectoryCreated = false;
		private void EnsureDir()
		{
			// For performance reasons, we only want to check
			// the directory once.
			if (mDirectoryCreated) return;

			var dir = Path.GetDirectoryName(TempPath);
			FileEx.EnsureDirectory(dir);
			mDirectoryCreated = true;
		}

		/// <summary>
		/// Deletes the temp file if it exists.
		/// </summary>
		public void Delete()
		{
			FileEx.DeleteFile(TempPath);
		}

		#endregion

	}
}
