using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using BizArk.Core.Extensions.WebExt;

namespace BizArk.Core.Web
{

    /// <summary>
    /// Dynamic class that stores parameters to be sent with the web request.
    /// </summary>
    public class WebParameters
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of WebParameters.
        /// </summary>
        public WebParameters()
        {
        }

        /// <summary>
        /// Creates an instance of WebParamters.
        /// </summary>
        /// <param name="values">The properties of this object will be converted to web parameters.</param>
        public WebParameters(object values)
        {
            Add(values);
        }

        #endregion

        #region Fields and Properties

        private List<WebTextParameter> mValues = new List<WebTextParameter>();
        /// <summary>
        /// Gets the parameters to upload.
        /// </summary>
        public List<WebTextParameter> Values
        {
            get { return mValues; }
        }

        private List<WebBinaryParameter> mBinary = new List<WebBinaryParameter>();
        /// <summary>
        /// Gets the list of binary values to upload.
        /// </summary>
        public List<WebBinaryParameter> Binary
        {
            get { return mBinary; }
        }

        private List<WebFileParameter> mFiles = new List<WebFileParameter>();
        /// <summary>
        /// Get the list of files to upload.
        /// </summary>
        public List<WebFileParameter> Files
        {
            get { return mFiles; }
        }

        /// <summary>
        /// Gets the total number of parameters.
        /// </summary>
        public int Count
        {
            get
            {
                var count = 0;
                count += mValues.Count;
                count += mBinary.Count;
                count += mFiles.Count;
                return count;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the properties of the object to the parameters.
        /// </summary>
        /// <param name="values"></param>
        public void Add(object values)
        {
            if (values == null) return;

            var props = TypeDescriptor.GetProperties(values);
            foreach (PropertyDescriptor prop in props)
            {
                var value = prop.GetValue(values);
                if (value == null) continue; // Do not send null values.

                // Check to see if this is a binary data type.
                var binary = value as byte[];
                if (binary != null)
                {
                    Binary.Add(new WebBinaryParameter(prop.Name, binary));
                    continue;
                }

                // Check to see if this is a file.
                var fi = value as FileInfo;
                if (fi != null) value = (UploadFile)fi;
                var file = value as UploadFile;
                if (file != null)
                {
                    Files.Add(new WebFileParameter(prop.Name, file));
                    continue;
                }

                // Convert the value to a string and add it to the values list.
                Values.Add(new WebTextParameter(prop.Name, ConvertEx.ToString(value)));
            }
        }

        #endregion

    }

    /// <summary>
    /// Base class for web parameters.
    /// </summary>
    public abstract class WebParameter
    {

        /// <summary>
        /// Creates an instance of WebParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected WebParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Factory method to create the parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WebParameter CreateParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");

            var fi = value as FileInfo;
            if (fi != null) return new WebFileParameter(name, (UploadFile)fi);

            var file = value as UploadFile;
            if (file != null) return new WebFileParameter(name, file);

            var data = value as byte[];
            if (data != null) return new WebBinaryParameter(name, data);

            var text = ConvertEx.ToString(value);
            return new WebTextParameter(name, text);
        }

    }

    /// <summary>
    /// Web parameter for files.
    /// </summary>
    public class WebFileParameter : WebParameter
    {

        internal WebFileParameter(string name, UploadFile file)
            : base(name, file)
        {
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        public UploadFile File { get { return (UploadFile)Value; } }

        /// <summary>
        /// Displays the parameter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}=[file:{1}]", Name, File.FileName);
        }

    }

    /// <summary>
    /// Web parameter for byte arrays.
    /// </summary>
    public class WebBinaryParameter : WebParameter
    {

        internal WebBinaryParameter(string name, byte[] data)
            : base(name, data)
        {
        }

        /// <summary>
        /// Gets the byte array.
        /// </summary>
        public byte[] Data { get { return (byte[])Value; } }

        /// <summary>
        /// Displays the parameter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}=[data:{1}]", Name, Data.Length);
        }

    }

    /// <summary>
    /// Web parameter for text.
    /// </summary>
    public class WebTextParameter : WebParameter
    {

        internal WebTextParameter(string name, string text)
            : base(name, text)
        {
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get { return (string)Value; } }

        /// <summary>
        /// Displays the parameter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Text.UrlEncode());
        }

    }

}
