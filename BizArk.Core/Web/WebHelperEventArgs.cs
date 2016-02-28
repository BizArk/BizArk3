using System;
using System.IO;
using System.Net;

namespace BizArk.Core.Web
{

    /// <summary>
    /// 
    /// </summary>
    public class RequestCompletedEventArgs
        : EventArgs
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of RequestCompletedEventArgs.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="state"></param>
        /// <param name="cancelled"></param>
        public RequestCompletedEventArgs(WebHelperResponse response, object state, bool cancelled)
            : this(response, null, state, cancelled)
        {
        }

        /// <summary>
        /// Creates an instance of RequestCompletedEventArgs.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="state"></param>
        /// <param name="cancelled"></param>
        public RequestCompletedEventArgs(Exception ex, object state, bool cancelled)
            : this(null, ex, state, cancelled)
        {
        }

        /// <summary>
        /// Creates an instance of RequestCompletedEventArgs.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="cancelled"></param>
        public RequestCompletedEventArgs(object state, bool cancelled)
            : this(null, null, state, cancelled)
        {
        }

        /// <summary>
        /// Creates an instance of RequestCompletedEventArgs.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ex"></param>
        /// <param name="state"></param>
        /// <param name="cancelled"></param>
        public RequestCompletedEventArgs(WebHelperResponse response, Exception ex, object state, bool cancelled)
        {
            mResponse = response;
            mError = ex;
            mState = state;
            mCancelled = cancelled;
        }

        #endregion

        #region Fields and Properties

        private WebHelperResponse mResponse;
        /// <summary>
        /// Gets the result.
        /// </summary>
        public WebHelperResponse Response
        {
            get { return mResponse; }
        }

        private Exception mError;
        /// <summary>
        /// Gets the error, if any, associated with the request.
        /// </summary>
        public Exception Error
        {
            get { return mError; }
        }

        private object mState;
        /// <summary>
        /// Gets the state associated with the request.
        /// </summary>
        public object State
        {
            get { return mState; }
        }

        private bool mCancelled = false;
        /// <summary>
        /// Gets a value that determines if the request was cancelled before it was completed.
        /// </summary>
        public bool Cancelled
        {
            get { return mCancelled; }
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class WebHelperProgressChangedEventArgs
        : EventArgs
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of WebHelperProgressChangedEventArgs.
        /// </summary>
        /// <param name="bytesToSend"></param>
        /// <param name="bytesSent"></param>
        /// <param name="bytesToReceive"></param>
        /// <param name="bytesReceived"></param>
        /// <param name="state"></param>
        public WebHelperProgressChangedEventArgs(long bytesToSend, long bytesSent, long? bytesToReceive, long? bytesReceived, object state)
        {
            BytesToSend = bytesToSend;
            BytesSent = bytesSent;
            BytesToReceive = bytesToReceive;
            BytesReceived = bytesReceived;
            mState = state;

            Func<long, long, int> CalcPct = (a1, a2) =>
            {
                if (a1 >= a2 || a2 == 0)
                    return 100;
                else
                    return (int)(a1 / a2);
            };
            SendProgressPercent = CalcPct(bytesToSend, bytesSent);
            ResponseProgressPercent = CalcPct(bytesToReceive.GetValueOrDefault(0), bytesReceived.GetValueOrDefault(0));
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the number of bytes to send.
        /// </summary>
        public long BytesToSend { get; private set; }

        /// <summary>
        /// Gets the number of bytes sent.
        /// </summary>
        public long BytesSent { get; private set; }

        /// <summary>
        /// Gets the progress for the request.
        /// </summary>
        public int SendProgressPercent { get; private set; }

        /// <summary>
        /// Gets the number of bytes received.
        /// </summary>
        public long? BytesReceived { get; private set; }

        /// <summary>
        /// Gets the number of bytes in the response. If the response hasn't been sent yet, this is an estimate based on WebHelper.EstimatedResponseLength.
        /// </summary>
        public long? BytesToReceive { get; private set; }

        /// <summary>
        /// Gets the progress for the response.
        /// </summary>
        public int ResponseProgressPercent { get; private set; }

        private object mState;
        /// <summary>
        /// Gets the state associated with the request.
        /// </summary>
        public object State
        {
            get { return mState; }
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class PrepareRequestEventArgs
        : EventArgs
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of PrepareRequestEventArgs.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="state"></param>
        public PrepareRequestEventArgs(HttpWebRequest request, object state)
        {
            mRequest = request;
            mState = state;
        }

        #endregion

        #region Fields and Properties

        private HttpWebRequest mRequest;
        /// <summary>
        /// Gets the HttpWebRequest that can be modified prior to sending it to the server.
        /// </summary>
        public HttpWebRequest Request
        {
            get { return mRequest; }
        }

        private object mState;
        /// <summary>
        /// Gets the state associated with the request.
        /// </summary>
        public object State
        {
            get { return mState; }
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class ProcessResponseStreamEventArgs
        : EventArgs
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of ProcessResponseStreamEventArgs.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="response"></param>
        /// <param name="state"></param>
        public ProcessResponseStreamEventArgs(Stream s, HttpWebResponse response, object state)
        {
            mResponseStream = s;
            mResponse = response;
            mState = state;
        }

        #endregion

        #region Fields and Properties

        private Stream mResponseStream;
        /// <summary>
        /// Gets the stream associated with the response.
        /// </summary>
        public Stream ResponseStream
        {
            get { return mResponseStream; }
        }

        private HttpWebResponse mResponse;
        /// <summary>
        /// Gets the response.
        /// </summary>
        public HttpWebResponse Response
        {
            get { return mResponse; }
        }

        private bool mHandled = false;
        /// <summary>
        /// Gets or sets a value that determines if the stream has been processed. Prevents the helper from processing the stream.
        /// </summary>
        public bool Handled
        {
            get { return mHandled; }
            set { mHandled = value; }
        }

        private object mResult;
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public object Result
        {
            get { return mResult; }
            set { mResult = value; }
        }

        private object mState;
        /// <summary>
        /// Gets the state associated with the request.
        /// </summary>
        public object State
        {
            get { return mState; }
        }

        #endregion

    }

}