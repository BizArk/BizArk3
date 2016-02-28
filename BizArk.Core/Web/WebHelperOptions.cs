using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;

namespace BizArk.Core.Web
{

    /// <summary>
    /// Options for WebHelper.
    /// </summary>
    public class WebHelperOptions
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of WebHelperOptions.
        /// </summary>
        public WebHelperOptions()
        {
            Headers = new WebHeaderCollection();
            ResponseEncoding = Encoding.Default;
            BufferSize = 1024;
            Values = new WebParameters();
            Timeout = TimeSpan.FromSeconds(100);
            Method = HttpMethod.Get;
            UserAgent = "";
            KeepAlive = true;
            AllowAutoRedirect = true;
            UseCompression = true;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets or sets the method for the web request. The default is GET but might be different based on the content type.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the timeout for the web request. If null, uses the default value for HttpWebRequest (100 seconds). For no timeout, set to TimeSpan.MaxValue.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the parameters for the request. This can be an object with the properties as parameters (recommend anonymous object) or it can be a WebParameterDictionary. To upload files, use a UploadFile or FileInfo object.
        /// </summary>
        public object Values { get; set; }

        /// <summary>
        /// Gets the headers for the request.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public WebHeaderCollection Headers { get; private set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if http keep-alives are used. The default is true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if redirects are followed. Default is false.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// Gets or sets the state object that will be sent through the events. The state object is not used internally.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object State { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if compression should be used. The default is true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool UseCompression { get; set; }

        /// <summary>
        /// Gets or sets the default encoding for the response. This is only used if the response does not define an encoding and you are trying to convert the response to a string.
        /// </summary>
        public Encoding ResponseEncoding { get; set; }

        /// <summary>
        /// Gets or sets the size of the buffer used to write to the request stream and read from the response stream. This provides control of how often progress is reported.
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        /// Method definition for PrepareRequest.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="request"></param>
        public delegate void PrepareRequestDelegate(WebHelper helper, HttpWebRequest request);
        /// <summary>
        /// Gets or sets a method that will be called after WebHelper has prepared the HttpWebRequest, but before the request is made. This will be called on the thread that made the initial request.
        /// </summary>
        public PrepareRequestDelegate PrepareRequest { get; set; }

        /// <summary>
        /// Method definition for ReportProgress.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="bytesSent"></param>
        /// <param name="bytesToSend"></param>
        /// <param name="bytesReceieved"></param>
        /// <param name="bytesToReceive"></param>
        public delegate void ReportProgressDelegate(WebHelper helper, long bytesSent, long bytesToSend, long? bytesReceieved, long? bytesToReceive);
        /// <summary>
        /// Gets or sets a method that will be called when reporting progress. This will be called on the thread that made the initial request.
        /// </summary>
        public ReportProgressDelegate ReportProgress { get; set; }

        /// <summary>
        /// Method definition for RequestComplete.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="response"></param>
        /// <param name="ex"></param>
        /// <param name="cancelled"></param>
        public delegate void RequestCompleteDelegate(WebHelper helper, WebHelperResponse response, Exception ex, bool cancelled);
        /// <summary>
        /// Gets or sets a method that will be called when the request completes. Called if the request is successful, failed, or canceled. This will be called on the thread that made the initial request.
        /// </summary>
        public RequestCompleteDelegate RequestComplete { get; set; }
        
        #endregion

    }
}
