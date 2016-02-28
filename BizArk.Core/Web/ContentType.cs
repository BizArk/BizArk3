using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using BizArk.Core.Extensions.WebExt;
using BizArk.Core.Util;

namespace BizArk.Core.Web
{

    /// <summary>
    /// Represents a particular content type for the request. Uses the strategy pattern for creating the request.
    /// </summary>
    public abstract class ContentType
        : IDisposable
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of ContentType.
        /// </summary>
        /// <param name="parameters"></param>
        protected ContentType(WebParameters parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            if (mDisposed) return;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object. Part of the Disposable pattern (http://msdn.microsoft.com/en-us/library/b1yfkh5e%28VS.80%29.aspx).
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed) return;
            mDisposed = true;
        }

        /// <summary>
        /// Creates a new instance of a ContentType based on the parameters sent in.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static ContentType CreateContentType(HttpMethod method, WebParameters parameters)
        {
            if (parameters.Files.Count > 0 || parameters.Binary.Count > 0)
                return new MultipartFormDataContentType(parameters);
            else if (parameters.Count > 0 && method != HttpMethod.Get && method != HttpMethod.Delete)
                return new ApplicationUrlEncodedContentType(parameters);
            else
                return new NoContentType(parameters);
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the parameters to upload minus the files.
        /// </summary>
        public WebParameters Parameters { get; private set; }

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
        /// Creates an HttpWebRequest for the URL and prepares whatever it needs to.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public abstract void PrepareRequest(HttpWebRequest request, WebHelper helper);

        /// <summary>
        /// Sends the request to the server. Does not get the response. Content types are only for sending, not receiving.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract bool SendRequest(WebHelper helper, HttpWebRequest request);

        /// <summary>
        /// Returns the url for the request. 
        /// </summary>
        /// <param name="webHelper"></param>
        /// <returns></returns>
        public virtual Uri GetUrl(WebHelper webHelper)
        {
            return webHelper.Url;
        }

        #endregion

    }

    /// <summary>
    /// Handles simple GET operations.
    /// </summary>
    public class NoContentType
        : ContentType
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of NoContentType.
        /// </summary>
        /// <param name="parameters"></param>
        public NoContentType(WebParameters parameters)
            : base(parameters)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an HttpWebRequest for the URL and prepares whatever it needs to.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public override void PrepareRequest(HttpWebRequest request, WebHelper helper)
        {
            request.Method = helper.Options.Method == HttpMethod.Delete ? "DELETE" : "GET";
        }

        /// <summary>
        /// Returns the url for the request. Will encode the form values into the url.
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public override Uri GetUrl(WebHelper helper)
        {
            var sb = new StringBuilder();
            sb.Append(helper.Url);

            var valSep = sb.ToString().Contains("?") ? "&" : "?";
            foreach (var param in Parameters.Values)
            {
                sb.AppendFormat("{0}{1}={2}", valSep, param.Name, param.Text.UrlEncode());
                valSep = "&";
            }

            return new Uri(sb.ToString());
        }

        /// <summary>
        /// Sends the request to the server. Does not get the response. Content types are only for sending, not receiving.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override bool SendRequest(WebHelper helper, HttpWebRequest request)
        {
            // Nothing to write to the stream. Everything necessary is included in the headers.
            return true;
        }

        #endregion

    }

    /// <summary>
    /// Handles application/x-www-form-urlencoded content types. Used for uploading form values. Will not upload files.
    /// </summary>
    public class ApplicationUrlEncodedContentType
        : ContentType
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of ApplicationUrlEncodedContentType.
        /// </summary>
        /// <param name="parameters"></param>
        public ApplicationUrlEncodedContentType(WebParameters parameters)
            : base(parameters)
        {
        }

        #endregion

        #region Fields and Properties

        private byte[] mData;
        /// <summary>
        /// Gets the url encoded string of form values.
        /// </summary>
        public byte[] Data
        {
            get { return mData; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an HttpWebRequest for the URL and prepares whatever it needs to.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public override void PrepareRequest(HttpWebRequest request, WebHelper helper)
        {
            request.ContentType = "application/x-www-form-urlencoded";
            mData = Encoding.UTF8.GetBytes(WebUtil.GetUrlEncodedData(Parameters));
            request.ContentLength = mData.Length;
            request.Method = helper.Options.Method == HttpMethod.Put ? "PUT" : "POST";
            if (mData.Length > 5242880)
                // if the content is more than 5MiB don't store it in the cache or there will be memory problems.
                request.AllowWriteStreamBuffering = false;
        }

        /// <summary>
        /// Sends the request to the server. Does not get the response. Content types are only for sending, not receiving.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override bool SendRequest(WebHelper helper, HttpWebRequest request)
        {
            using (Stream s = request.GetRequestStream())
            {
                s.Write(mData, 0, mData.Length);
            }
            helper.ReportRequestProgress(request.ContentLength);
            return true;
        }

        #endregion

    }

    /// <summary>
    /// Handles multipart/form-data content types. Used for uploading files.
    /// </summary>
    public class MultipartFormDataContentType
        : ContentType
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of MultipartFormDataContentType.
        /// </summary>
        public MultipartFormDataContentType(WebParameters parameters)
            : base(parameters)
        {
            PartBoundary = "---------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (mParts != null)
            {
                foreach (var part in mParts)
                    part.Dispose();
                mParts = null;
            }
        }

        #endregion

        #region Fields and Properties

        private long mContentLength = 0;
        /// <summary>
        /// Gets the total content length for the data to be sent to the server.
        /// </summary>
        public long ContentLength
        {
            get { return mContentLength; }
        }

        private string mPartBoundary;
        /// <summary>
        /// Gets or sets the part boundary. Used when ContentType = multipart_form_data.
        /// </summary>
        public string PartBoundary
        {
            get { return mPartBoundary; }
            set
            {
                mPartBoundary = value;
                mPartFooter = Encoding.UTF8.GetBytes("--" + mPartBoundary + "--\r\n");
            }
        }

        private byte[] mPartFooter;
        /// <summary>
        /// Gets the footer used between parts.
        /// </summary>
        public byte[] PartFooter
        {
            get { return mPartFooter; }
        }

        private MimePart[] mParts = null;

        #endregion

        #region Methods

        /// <summary>
        /// Creates an HttpWebRequest for the URL and prepares whatever it needs to.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public override void PrepareRequest(HttpWebRequest request, WebHelper helper)
        {
            request.ContentType = "multipart/form-data; boundary=" + mPartBoundary;
            mParts = GetParts(helper);
            mContentLength = CalcContentLength(mParts, mPartFooter);
            request.ContentLength = mContentLength;
            request.Method = helper.Options.Method == HttpMethod.Put ? "PUT" : "POST";
            if (mContentLength > 5242880)
                // if the content is more than 5MiB don't store it in the cache or there will be memory problems.
                request.AllowWriteStreamBuffering = false;
        }

        /// <summary>
        /// Sends the request to the server. Does not get the response. Content types are only for sending, not receiving.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override bool SendRequest(WebHelper helper, HttpWebRequest request)
        {
            using (Stream s = request.GetRequestStream())
            {
                byte[] buffer = new byte[helper.Options.BufferSize];
                byte[] fileFooter = Encoding.UTF8.GetBytes("\r\n");
                int read;
                long bytesSent = 0;

                foreach (MimePart part in mParts)
                {
                    s.Write(part.Header, 0, part.Header.Length);

                    while ((read = part.Data.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (helper.CancellationPending)
                            return false;

                        s.Write(buffer, 0, read);
                        bytesSent += read;

                        helper.ReportRequestProgress(bytesSent);
                    }

                    part.Data.Dispose();

                    s.Write(fileFooter, 0, fileFooter.Length);
                }

                s.Write(mPartFooter, 0, mPartFooter.Length);
            }
            return true;
        }

        private long CalcContentLength(MimePart[] parts, byte[] footer)
        {
            var contentLength = 0L;
            foreach (MimePart part in parts)
                contentLength += part.ContentLength;
            contentLength += footer.Length;

            return contentLength;
        }

        private MimePart[] GetParts(WebHelper helper)
        {
            var parts = new List<MimePart>();

            foreach (var param in Parameters.Values)
            {
                var s = new MemoryStream(Encoding.UTF8.GetBytes(param.Text));
                var part = new MimePart(s);

                part.Headers["Content-Disposition"] = "form-data; name=\"" + param.Name + "\"";
                part.Prepare(mPartBoundary);

                parts.Add(part);
            }

            foreach (var param in Parameters.Binary)
            {
                var s = new MemoryStream(param.Data);
                var part = new MimePart(s);

                part.Headers["Content-Disposition"] = "form-data; name=\"" + param.Name + "\"";
                part.Prepare(mPartBoundary);

                parts.Add(part);
            }

            // This does not strictly conform to RFC2388 (http://www.faqs.org/rfcs/rfc2388.html). The RFC states that 
            // if you upload multiple files they should be in a multipart/mixed part within a multipart/form-data.
            // However, this seems to work with IIS7 which is good enough for now.
            foreach (var param in Parameters.Files)
            {
                var s = param.File.GetStream();
                var part = new MimePart(s);

                part.Headers["Content-Disposition"] = "form-data; name=\"" + param.Name + "\"; filename=\"" + param.File.FileName + "\"";
                part.Headers["Content-Type"] = param.File.ContentType;
                part.Prepare(mPartBoundary);

                parts.Add(part);
            }

            return parts.ToArray();
        }

        #endregion

    }

}
