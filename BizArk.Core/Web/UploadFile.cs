using System.IO;
using BizArk.Core.Util;

namespace BizArk.Core.Web
{

    /// <summary>
    /// Represents a file that will be uploaded using the WebHelper class.
    /// </summary>
    public class UploadFile
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of UploadFile.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="path"></param>
        public UploadFile(string contentType, string path)
        {
            mContentType = contentType;
            mFileName = Path.GetFileName(path);
            mFilePath = path;
        }

        /// <summary>
        /// Creates an instance of UploadFile.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public UploadFile(string contentType, string fileName, Stream data)
        {
            mContentType = contentType;
            mFileName = fileName;
            mData = data;
        }

        #endregion

        #region Fields and Properties

        private string mContentType;
        /// <summary>
        /// Gets or sets the mime type for the file (eg, text/plain, image/jpeg, etc).
        /// </summary>
        public string ContentType
        {
            get { return mContentType; }
            set { mContentType = value; }
        }

        private string mFileName;
        /// <summary>
        /// Gets or sets the name of the file. Option if FilePath is set (the name will come from the file path).
        /// </summary>
        public string FileName
        {
            get { return mFileName; }
            set { mFileName = value; }
        }

        private string mFilePath;
        /// <summary>
        /// Gets or sets the path to the file. Optional, if Data is set, this is ignored.
        /// </summary>
        public string FilePath
        {
            get { return mFilePath; }
            set { mFilePath = value; }
        }

        private Stream mData;
        /// <summary>
        /// Gets or sets the data to upload. Optional, if this is set, FilePath will be ignored.
        /// </summary>
        public Stream Data
        {
            get { return mData; }
            set { mData = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the stream that represents the file to upload.
        /// </summary>
        /// <returns></returns>
        internal Stream GetStream()
        {
            if (mData != null)
                return mData;
            else
                return new FileStream(mFilePath, FileMode.Open, FileAccess.Read);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Convert FileInfo to UploadFile.
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static explicit operator UploadFile(FileInfo fi)
        {
            return new UploadFile(MimeMap.GetMimeType(fi.Extension), fi.FullName); 
        }

        #endregion

    }
}
