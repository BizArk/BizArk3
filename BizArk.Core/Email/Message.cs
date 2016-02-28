using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace BizArk.Core.Email
{
    /// <summary>
    /// 
    /// </summary>
    public class Message
    {
        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of Message.
        /// </summary>
        public Message()
        {
            To = new EmailAddressList(EmailAddressType.To);
            CC = new EmailAddressList(EmailAddressType.CC);
            BCC = new EmailAddressList(EmailAddressType.BCC);
            Attachments = new AttachmentList();
            mSendAsyncCompleteCallback = new SendOrPostCallback(SendAsyncWorkerCompleted);
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// 
        /// </summary>
        public EmailAddressList To { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EmailAddressList CC { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EmailAddressList BCC { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets attachments for the email.
        /// </summary>
        public AttachmentList Attachments { get; set; }

        /// <summary>
        /// Gets or sets the subject of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body of the email.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets a value that determines if the Send or ShowMailClient methods are currently running.
        /// </summary>
        public bool IsSending { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the clients email client with this email using MAPISendMail.
        /// </summary>
        public SendStatus ShowMailClient()
        {
            return Send(true);
        }

        /// <summary>
        /// Sends the email message using MAPISendMail.
        /// </summary>
        public SendStatus Send()
        {
            return Send(false);
        }

        private SendStatus Send(bool showDlg)
        {
            if (IsSending) throw new InvalidOperationException("ShowMailClient or Send has already been called.");

            IsSending = true;
            try
            {
                return MAPI32.SendMail(this, showDlg);
            }
            finally
            {
                IsSending = false;
            }
        }

        #endregion

        #region Async

        private delegate void SendWorkerEventHandler(bool showDlg, AsyncOperation async);
        private SendOrPostCallback mSendAsyncCompleteCallback;

        /// <summary>
        /// Displays the clients email client with this email asynchrounously using MAPISendMail.
        /// </summary>
        public void ShowMailClientAsync()
        {
            SendAsync(true);
        }

        /// <summary>
        /// Sends the email message asynchronously using MAPISendMail.
        /// </summary>
        public void SendAsync()
        {
            SendAsync(false);
        }

        private void SendAsync(bool showDlg)
        {
            if (IsSending) throw new InvalidOperationException("ShowMailClient or Send has already been called.");
            IsSending = true;

            var key = new object();
            var asyncOp = AsyncOperationManager.CreateOperation(key);
            var t = new Thread(SendAsyncWorker);
            t.SetApartmentState(ApartmentState.STA);
            t.Start(new SendAsyncThreadArgs() { ShowDialog = showDlg, Operation = asyncOp });
        }

        private void SendAsyncWorker(object args)
        {
            var sendArgs = args as SendAsyncThreadArgs;
            SendAsyncCompletedEventArgs e;
            try
            {
                var status = MAPI32.SendMail(this, sendArgs.ShowDialog);
                e = new SendAsyncCompletedEventArgs(status);
            }
            catch (Exception ex)
            {
                e = new SendAsyncCompletedEventArgs(ex);
            }
            finally
            {
                IsSending = false;
            }

            sendArgs.Operation.PostOperationCompleted(mSendAsyncCompleteCallback, e);
        }

        private void SendAsyncWorkerCompleted(object args)
        {
            OnSendAsyncCompleted(args as SendAsyncCompletedEventArgs);
        }

        /// <summary>
        /// Delegate for SendAsyncCompleted event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SendAsyncCompletedDelegate(object sender, SendAsyncCompletedEventArgs e);
        /// <summary>
        /// Raised when the SendAsync or ShowMailClientAsync methods complete.
        /// </summary>
        public event SendAsyncCompletedDelegate SendAsyncCompleted;
        /// <summary>
        /// Raises the SendAsyncCompleted event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSendAsyncCompleted(SendAsyncCompletedEventArgs e)
        {
            if (SendAsyncCompleted == null) return;
            SendAsyncCompleted(this, e);
        }

        #endregion

        #region SendAsyncThreadArgs

        private class SendAsyncThreadArgs
        {
            public bool ShowDialog { get; set; }
            public AsyncOperation Operation { get; set; }
        }

        #endregion

    }

    /// <summary>
    /// Event args for SendComplete event.
    /// </summary>
    public class SendAsyncCompletedEventArgs
        : EventArgs
    {
        /// <summary>
        /// Creates an instance of SendAsyncCompletedEventArgs.
        /// </summary>
        /// <param name="status"></param>
        public SendAsyncCompletedEventArgs(SendStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// Creates an instance of SendAsyncCompletedEventArgs.
        /// </summary>
        /// <param name="ex"></param>
        public SendAsyncCompletedEventArgs(Exception ex)
        {
            Exception = ex;
        }

        /// <summary>
        /// Gets the status from the send method. Check to make sure there wasn't an exception.
        /// </summary>
        public SendStatus Status { get; private set; }

        /// <summary>
        /// Gets the exception associated with the send method (if it threw an exception).
        /// </summary>
        public Exception Exception { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AttachmentList
        : List<string>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class EmailAddressList
        : List<EmailAddress>
    {
        /// <summary>
        /// Creates an instance of EmailAddressList.
        /// </summary>
        /// <param name="addrType"></param>
        public EmailAddressList(EmailAddressType addrType)
        {
            AddressType = addrType;
        }

        /// <summary>
        /// Gets the type of address this is.
        /// </summary>
        public EmailAddressType AddressType { get; private set; }

        /// <summary>
        /// Adds a single email address.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public EmailAddress Add(string name, string address)
        {
            var addr = new EmailAddress();
            addr.Name = name;
            addr.Address = address;
            Add(addr);
            return addr;
        }

        /// <summary>
        /// Add a single email address without a name.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public EmailAddress Add(string address)
        {
            var addr = new EmailAddress();
            addr.Name = "";
            addr.Address = address;
            Add(addr);
            return addr;
        }

        /// <summary>
        /// Adds a range of email addresses.
        /// </summary>
        /// <param name="addresses"></param>
        public void AddRange(IEnumerable<string> addresses)
        {
            foreach (var address in addresses)
                Add(address);
        }
    }

    /// <summary>
    /// Stores an email address.
    /// </summary>
    public class EmailAddress
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Address { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum EmailAddressType
    {
        /// <summary></summary>
        Orig = 0,
        /// <summary></summary>
        To = 1,
        /// <summary></summary>
        CC = 2,
        /// <summary></summary>
        BCC = 3
    }

}
