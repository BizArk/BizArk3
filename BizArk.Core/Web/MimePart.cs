using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace BizArk.Core.Web
{

    /// <summary>
    /// Represents a part of an Http request.
    /// </summary>
    internal class MimePart
        : IDisposable
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of MimePart.
        /// </summary>
        /// <param name="data"></param>
        public MimePart(Stream data)
        {
            mData = data;
        }

        /// <summary>
        /// Disposes the MimePart.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the MimePart. Part of the Disposable pattern (http://msdn.microsoft.com/en-us/library/b1yfkh5e%28VS.80%29.aspx).
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {
                if (mData != null)
                {
                    mData.Dispose();
                    mData = null;
                }
            }

            mDisposed = true;
        }

        #endregion

        #region Fields and Properties

        private NameValueCollection mHeaders = new NameValueCollection();
        /// <summary>
        /// Gets the headers associated with this part.
        /// </summary>
        public NameValueCollection Headers
        {
            get { return mHeaders; }
        }

        private byte[] mHeader;
        /// <summary>
        /// Gets the headers converted to a byte array. Must call Prepare to use this property.
        /// </summary>
        public byte[] Header
        {
            get { return mHeader; }
        }

        private long mContentLength = -1L;
        /// <summary>
        /// Gets the total number of bytes for this part. Must call Prepare to use this property.
        /// </summary>
        public long ContentLength
        {
            get { return mContentLength; }
        }

        private Stream mData;
        /// <summary>
        /// Gets the stream that contains the data for this part.
        /// </summary>
        public Stream Data
        {
            get { return mData; }
        }

        private bool mDisposed;
        /// <summary>
        /// Determines if the part has been disposed. If true, the data stream is no longer valid.
        /// </summary>
        public bool Disposed
        {
            get { return mDisposed; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepares the part for the request. Sets the Header and ContentLength properties.
        /// </summary>
        /// <param name="boundary"></param>
        public void Prepare(string boundary)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("--");
            sb.Append(boundary);
            sb.AppendLine();
            foreach (string key in mHeaders.AllKeys)
            {
                sb.Append(key);
                sb.Append(": ");
                sb.AppendLine(mHeaders[key]);
            }
            sb.AppendLine();

            mHeader = Encoding.UTF8.GetBytes(sb.ToString());

            // The content length adds two bytes to account for the required line break at the end of every part.
            mContentLength = mHeader.Length + Data.Length + 2;
        }

        #endregion

    }

}
