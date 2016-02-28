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
            mTempPath = FileUtil.GetUniqueFileName();
        }

        /// <summary>
        /// Creates an instance of TempFile.
        /// </summary>
        /// <param name="ext">The extension for the file.</param>
        public TempFile(string ext)
        {
            mTempPath = FileUtil.GetUniqueFileName(ext);
        }

        /// <summary>
        /// Creates an instance of TempFile.
        /// </summary>
        /// <param name="dir">The path to the directory.</param>
        /// <param name="template">The template for the file name. Place a {0} where the counter should go (ex, MyPicture{0}.jpg).</param>
        public TempFile(string dir, string template)
        {
            mTempPath = FileUtil.GetUniqueFileName(dir, template);
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

        private string mTempPath;
        /// <summary>
        /// 
        /// </summary>
        public string TempPath
        {
            get { return mTempPath; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the temp file and writes the contents to it. 
        /// </summary>
        /// <param name="contents"></param>
        public void Write(string contents)
        {
            EnsureDir();
            File.WriteAllText(mTempPath, contents);
        }

        /// <summary>
        /// Appends the contents to the temp file. Creates the temp file if it doesn't already exist.
        /// </summary>
        /// <param name="contents"></param>
        public void Append(string contents)
        {
            EnsureDir();
            File.AppendAllText(mTempPath, contents);
        }

        /// <summary>
        /// Creates the temp file and writes the contents to it.
        /// </summary>
        /// <param name="contents"></param>
        public void Write(byte[] contents)
        {
            EnsureDir();
            File.WriteAllBytes(mTempPath, contents);
        }

        private void EnsureDir()
        {
            var dir = Path.GetDirectoryName(mTempPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// Deletes the temp file if it exists.
        /// </summary>
        public void Delete()
        {
            FileUtil.DeleteFile(mTempPath);
        }

        #endregion

    }
}
