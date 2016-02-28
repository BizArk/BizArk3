using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using BizArk.Core.Extensions.ExceptionExt;

namespace BizArk.Core.Web
{

    /// <summary>
    /// This is a helper class to easily make web requests. 
    /// It is intended as a replacement for WebClient. It 
    /// includes the ability to upload multiple files, post 
    /// form values, set a timeout, run asynchrounously, 
    /// and reports progress.
    /// </summary>
    public class WebHelper : IDisposable
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of WebHelper.
        /// </summary>
        /// <param name="url">The URL must be a valid http url.</param>
        public WebHelper(string url)
            : this(new Uri(url))
        {
        }

        /// <summary>
        /// Creates an instance of WebHelper.
        /// </summary>
        /// <param name="url">The URL must be a valid http url.</param>
        public WebHelper(Uri url)
        {
            Url = url;
            Options = new WebHelperOptions();
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
            if (mWait != null)
            {
                mWait.Dispose();
                mWait = null;
            }
            Disposed = true;
        }

        #endregion

        #region Fields and Properties

        private ManualResetEvent mWait;
        private AsyncOperation mAsyncOperation = null;
        private Thread mRequestThread = null;
        private long mRequestContentLength = 0;
        private long mResponseContentLength = 0;

        /// <summary>
        /// Gets a flag that determines if the temp file has been disposed.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Gets the url for the web request.
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// Gets or sets the content type for the request. If null, will determine the content type based on what needs to be sent. ContentTypes are a one-use thing. If set, it will need to be set for each call.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ContentType ContentType { get; set; }

        /// <summary>
        /// Gets a value that determines if an asynchronous request is already in progress.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                if (mAsyncOperation == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Gets a value that determines if the current asynchronous request has been cancelled.
        /// </summary>
        public bool CancellationPending { get; private set; }

        /// <summary>
        /// Gets or sets the options to use for the request.
        /// </summary>
        public WebHelperOptions Options { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Makes a request.
        /// </summary>
        /// <returns></returns>
        public WebHelperResponse MakeRequest()
        {
            if (IsBusy) throw new InvalidOperationException("A request has already been made. WebHelper only supports a single request at a time.");

            // Make sure the thread is dead. We don't want to make a request on the thread.
            if (mRequestThread != null)
            {
                try
                {
                    mRequestThread.Abort();
                }
                catch (Exception ex)
                {
                    // ignore exceptions.
                    Debug.WriteLine(ex.GetDetails());
                }
            }
            mRequestThread = null;
            mWait = null;

            return MakeRequest_Internal();
        }

        /// <summary>
        /// Makes the web request asynchronously.
        /// </summary>
        public WaitHandle MakeRequestAsync()
        {
            if (IsBusy) throw new InvalidOperationException("An asynchronous request is already in progress. WebHelper only supports a single request at a time.");

            mWait = new ManualResetEvent(false);
            mAsyncOperation = AsyncOperationManager.CreateOperation(null);
            mRequestThread = new Thread(() =>
            {
                MakeRequest_Internal();
            });
            mRequestThread.Start();
            return mWait;
        }

        private WebHelperResponse MakeRequest_Internal()
        {
            // Make sure we have valid options.
            if (Options == null) Options = new WebHelperOptions();

            CancellationPending = false;

            WebHelperResponse whResponse = null;
            try
            {
                HttpWebRequest request = null;
                bool sent = false;

                var parameters = new WebParameters(Options.Values);
                using (var contentType = ContentType ?? ContentType.CreateContentType(Options.Method, parameters))
                {
                    request = CreateRequest(contentType);
                    sent = contentType.SendRequest(this, request);
                }

                RequestCompletedEventArgs completedArgs = null;
                if (!sent)
                {
                    if (!CancellationPending) throw new InvalidOperationException("Unable to complete request. Unknown error.");
                    completedArgs = new RequestCompletedEventArgs(Options.State, true);
                }
                else
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                        whResponse = ProcessResponse(response);

                    // even if a cancellation is pending, if we recieved the response, complete the request as normal.
                    if (whResponse != null)
                        completedArgs = new RequestCompletedEventArgs(whResponse, Options.State, false);
                    else if (CancellationPending)
                        completedArgs = new RequestCompletedEventArgs(whResponse, Options.State, true);
                    else
                        throw new InvalidOperationException("Unable to receive response. Unknown error.");
                }

                Post((arg) =>
                {
                    OnRequestCompleted((RequestCompletedEventArgs)arg);
                }, completedArgs);

            }
            catch (Exception ex)
            {
                var completedArgs = new RequestCompletedEventArgs(ex, Options.State, false);
                Post((arg) =>
                {
                    // Regardless of the outcome, RequestCompleted is guaranteed to be raised.
                    // Caller is responsible for checking the status of the request object to
                    // see if it is successful or not.
                    OnRequestCompleted((RequestCompletedEventArgs)arg);
                }, completedArgs);

                if (mAsyncOperation == null)
                    // Don't throw the exception if running on a thread. It can't be caught
                    // and makes it difficult to debug (cannot continue in Visual Studio).
                    throw;
            }
            finally
            {
                if (mAsyncOperation != null)
                {
                    using (var done = new ManualResetEvent(false))
                    {
                        // Use the ManualResetEvent to ensure that the operation completes before
                        // we exit the method. 
                        // This is used to ensure that the Wait method will not continue until 
                        // after the final operation has been completed.
                        mAsyncOperation.PostOperationCompleted((arg) =>
                        {
                            mAsyncOperation = null;
                            done.Set();
                        }, null);
                        done.WaitOne();
                    }
                }
            }
            return whResponse;
        }

        /// <summary>
        /// Cancels the request in progress.
        /// </summary>
        public void CancelRequestAsync()
        {
            CancellationPending = true;
        }

        /// <summary>
        /// Waits for the request to complete or until the timeout, whichever comes first.
        /// </summary>
        /// <param name="timeout">Number of milliseconds to wait for the request to complete. Set to null to wait indefinitely.</param>
        /// <returns>True if the request has completed, false if the timeout was reached.</returns>
        public bool Wait(int? timeout = null)
        {
            var wait = mWait; // grab the instance so we don't have any problems with it disapearing.
            if (wait == null) return true;
            if (timeout == null)
                return wait.WaitOne();
            else
                return wait.WaitOne(timeout.Value);
        }

        /// <summary>
        /// Waits for the request to complete or until the timeout, whichever comes first.
        /// </summary>
        /// <param name="timeout">How long to wait for the request to complete.</param>
        /// <returns>True if the request has completed, false if the timeout was reached.</returns>
        public bool Wait(TimeSpan timeout)
        {
            return Wait((int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// Processes the response from the server.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual WebHelperResponse ProcessResponse(HttpWebResponse response)
        {
            //todo: check to see if this is set yet.
            mResponseContentLength = response.ContentLength;

            using (var responseStream = response.GetResponseStream())
            {
                var processArgs = new ProcessResponseStreamEventArgs(responseStream, response, Options.State);
                OnProcessResponseStream(processArgs);
                if (processArgs.Handled)
                    return new WebHelperResponse(processArgs.Result, response.ContentType, response.StatusCode, response.ContentEncoding, response.CharacterSet, Options);

                Stream s;
                if (response.ContentEncoding.ToLower().Contains("deflate"))
                    s = new DeflateStream(responseStream, CompressionMode.Decompress);
                else if (response.ContentEncoding.ToLower().Contains("gzip"))
                    s = new GZipStream(responseStream, CompressionMode.Decompress);
                else
                    s = responseStream;

                var buffer = new byte[Options.BufferSize];
                var bytesRead = 0;
                int read;
                using (var ms = new MemoryStream())
                {
                    while ((read = s.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (CancellationPending) return null;

                        ms.Write(buffer, 0, read);
                        bytesRead += read;
                        ReportResponseProgress(bytesRead);
                    }
                    return new WebHelperResponse(ms.ToArray(), response.ContentType, response.StatusCode, response.ContentEncoding, response.CharacterSet, Options);
                }
            }

        }

        /// <summary>
        /// Creates the request that will be used to contact the server.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        protected virtual HttpWebRequest CreateRequest(ContentType contentType)
        {
            var request = (HttpWebRequest)WebRequest.Create(contentType.GetUrl(this));

            request.Timeout = (int)Options.Timeout.TotalMilliseconds;
            request.Headers = Options.Headers;
            if (Options.UseCompression)
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            if (!string.IsNullOrEmpty(Options.UserAgent)) request.UserAgent = Options.UserAgent;
            request.KeepAlive = Options.KeepAlive;
            request.AllowAutoRedirect = Options.AllowAutoRedirect;

            // let the content type update the request.
            contentType.PrepareRequest(request, this);

            // let the request be customized.
            OnPrepareRequest(new PrepareRequestEventArgs(request, Options.State));

            if (request.ContentLength > 0)
                // Used to determine progress
                mRequestContentLength = request.ContentLength;

            return request;
        }

        /// <summary>
        /// Ensures that certain methods or events are called on the primary thread when running asynchronously.
        /// </summary>
        /// <param name="call"></param>
        /// <param name="arg"></param>
        private void Post(SendOrPostCallback call, object arg)
        {
            if (mAsyncOperation == null)
                call(arg);
            else
            {
                mAsyncOperation.Post((p) =>
                {
                    call(p);
                }, arg);
            }
        }

        private int mLastReqPct = -1;
        /// <summary>
        /// Used by ContentType object to report progress during send.
        /// </summary>
        /// <param name="bytesSent"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ReportRequestProgress(long bytesSent)
        {
            var pct = (int)(((double)bytesSent / mRequestContentLength) * 100);
            if (mLastReqPct != pct)
            {
                // Only raise the progress changed event if the progress percent changed. It can become a performance issue if you raise it every time you write a few bytes.
                var progressArgs = new WebHelperProgressChangedEventArgs(mRequestContentLength, bytesSent, null, null, Options.State);
                Post((arg) =>
                {
                    OnProgressChanged((WebHelperProgressChangedEventArgs)arg);
                }, progressArgs);
                mLastReqPct = pct;
            }
        }

        private int mLastResPct = -1;
        /// <summary>
        /// Used by ContentType object to report progress during send.
        /// </summary>
        /// <param name="bytesRead"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ReportResponseProgress(long bytesRead)
        {
            var pct = (int)(((double)bytesRead / mResponseContentLength) * 100);
            if (mLastResPct != pct)
            {
                // Only raise the progress changed event if the progress percent changed. It can become a performance issue if you raise it every time you write a few bytes.
                var progressArgs = new WebHelperProgressChangedEventArgs(mRequestContentLength, mRequestContentLength, mResponseContentLength, bytesRead, Options.State);
                Post((arg) =>
                {
                    OnProgressChanged((WebHelperProgressChangedEventArgs)arg);
                }, progressArgs);
                mLastResPct = pct;
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Simple method for making a request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static WebHelperResponse MakeRequest(string url, object values = null)
        {
            return MakeRequest(new Uri(url), values);
        }

        /// <summary>
        /// Simple method for making an asynchronous request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        /// <returns>The WaitHandle that can be used to determine when the request is complete.</returns>
        public static WaitHandle MakeRequestAsync(string url, object values = null)
        {
            return MakeRequestAsync(new Uri(url), values);
        }

        /// <summary>
        /// Simple method for making a request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static WebHelperResponse MakeRequest(Uri url, object values = null)
        {
            var web = new WebHelper(url);
            web.Options.Values = values;
            return web.MakeRequest();
        }

        /// <summary>
        /// Simple method for making an asynchronous request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        /// <returns>The WaitHandle that can be used to determine when the request is complete.</returns>
        public static WaitHandle MakeRequestAsync(Uri url, object values = null)
        {
            var web = new WebHelper(url);
            web.Options.Values = values;
            return web.MakeRequestAsync();
        }

        /// <summary>
        /// Simple method for making a request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static WebHelperResponse MakeRequest(string url, WebHelperOptions options)
        {
            return MakeRequest(new Uri(url), options);
        }

        /// <summary>
        /// Simple method for making a request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        /// <returns>The WaitHandle that can be used to determine when the request is complete.</returns>
        public static WaitHandle MakeRequestAsync(string url, WebHelperOptions options)
        {
            return MakeRequestAsync(new Uri(url), options);
        }

        /// <summary>
        /// Simple method for making a request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static WebHelperResponse MakeRequest(Uri url, WebHelperOptions options)
        {
            var web = new WebHelper(url);
            web.Options = options;
            return web.MakeRequest();
        }

        /// <summary>
        /// Simple method for making a request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        /// <returns>The WaitHandle that can be used to determine when the request is complete.</returns>
        public static WaitHandle MakeRequestAsync(Uri url, WebHelperOptions options)
        {
            var web = new WebHelper(url);
            web.Options = options;
            return web.MakeRequestAsync();
        }

        /// <summary>
        /// Simple method for making a request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static WebHelperResponse DownloadFile(string url, string fileName, WebHelperOptions options = null)
        {
            return DownloadFile(new Uri(url), fileName, options);
        }

        /// <summary>
        /// Simple method for making a request using WebHelper.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="options"></param>
        /// <returns>The WaitHandle that can be used to determine when the request is complete.</returns>
        public static WaitHandle DownloadFileAsync(string url, string fileName, WebHelperOptions options = null)
        {
            return DownloadFileAsync(new Uri(url), fileName, options);
        }

        /// <summary>
        /// Simple and performant method to download files or any large amount of content.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="options"></param>
        /// <returns>The WebHelperResponse.Result contains a FileInfo object.</returns>
        public static WebHelperResponse DownloadFile(Uri url, string fileName, WebHelperOptions options = null)
        {
            var web = new WebHelper(url);
            if (options != null) web.Options = options;
            web.ProcessResponseStream += (sender, e) => { ProcessFileResponse((WebHelper)sender, e, fileName); };
            return web.MakeRequest();
        }

        /// <summary>
        /// Simple and performant method to download files or any large amount of content.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="options"></param>
        /// <returns>The WaitHandle that can be used to determine when the request is complete.</returns>
        public static WaitHandle DownloadFileAsync(Uri url, string fileName, WebHelperOptions options = null)
        {
            var web = new WebHelper(url);
            if (options != null) web.Options = options;
            web.ProcessResponseStream += (sender, e) => { ProcessFileResponse((WebHelper)sender, e, fileName); };
            return web.MakeRequestAsync();
        }

        private static void ProcessFileResponse(WebHelper web, ProcessResponseStreamEventArgs e, string fileName)
        {
            e.Handled = true;
            var buffer = new byte[web.Options.BufferSize];
            var bytesRead = 0;
            int read;
            Stream s;

            if (e.Response.ContentEncoding.ToLower().Contains("deflate"))
                s = new DeflateStream(e.ResponseStream, CompressionMode.Decompress);
            else if (e.Response.ContentEncoding.ToLower().Contains("gzip"))
                s = new GZipStream(e.ResponseStream, CompressionMode.Decompress);
            else
                s = e.ResponseStream;

            using (var fs = new FileStream(fileName, FileMode.CreateNew))
            {
                while ((read = s.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, read);
                    bytesRead += read;
                    web.ReportResponseProgress(bytesRead);
                }
                fs.Flush();
            }

            e.Result = new FileInfo(fileName);
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegate for PrepareRequest event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void PrepareRequestHandler(object sender, PrepareRequestEventArgs e);
        /// <summary>
        /// Event raised before the request is made. Allows for customization of the request object before the request is sent. This event is raised on the calling thread. It is recommended that you do not update the UI in this event handler.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event PrepareRequestHandler PrepareRequest;
        /// <summary>
        /// Raises the PrepareRequest event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPrepareRequest(PrepareRequestEventArgs e)
        {
            if (Options.PrepareRequest != null) Options.PrepareRequest(this, e.Request);
            if (PrepareRequest == null) return;
            PrepareRequest(this, e);
        }

        /// <summary>
        /// Delegate for ProgressChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ProgressChangedHandler(object sender, WebHelperProgressChangedEventArgs e);
        /// <summary>
        /// Event raised when the progress changes. This event is raised on the thread that made the request.
        /// </summary>
        public event ProgressChangedHandler ProgressChanged;
        /// <summary>
        /// Raises the ProgressChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnProgressChanged(WebHelperProgressChangedEventArgs e)
        {
            if (Options.ReportProgress != null) Options.ReportProgress(this, e.BytesSent, e.BytesToSend, e.BytesReceived, e.BytesToReceive);
            if (ProgressChanged == null) return;
            ProgressChanged(this, e);
        }

        /// <summary>
        /// Delegate for ProcessResponseStream event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ProcessResponseStreamHandler(object sender, ProcessResponseStreamEventArgs e);
        /// <summary>
        /// Event raised to allow you to process the response. Allows custom handling of the response. This event is raised on the calling thread. It is recommended that you do not update the UI in this event handler.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event ProcessResponseStreamHandler ProcessResponseStream;
        /// <summary>
        /// Raises the ProcessResponseStream event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnProcessResponseStream(ProcessResponseStreamEventArgs e)
        {
            if (ProcessResponseStream == null) return;
            ProcessResponseStream(this, e);
        }

        /// <summary>
        /// Delegate for the RequestCompleted event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void RequestCompletedHandler(object sender, RequestCompletedEventArgs e);
        /// <summary>
        /// Event raised when the request has been completed. This event is raised on the thread that made the request.
        /// </summary>
        public event RequestCompletedHandler RequestCompleted;
        /// <summary>
        /// Raises the RequestCompleted event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestCompleted(RequestCompletedEventArgs e)
        {
            try
            {
                if (Options.RequestComplete != null) Options.RequestComplete(this, e.Response, e.Error, e.Cancelled);
                if (RequestCompleted == null) return;
                RequestCompleted(this, e);
            }
            finally
            {
                if (mWait != null)
                    mWait.Set();
            }
        }

        #endregion

    }

}
