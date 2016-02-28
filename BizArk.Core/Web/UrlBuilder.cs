using System;
using System.Text;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.Core.Web
{

    /// <summary>
    /// The UrlBuilder allows you to easily create a properly formatted URL including encoding of parameter values.
    /// </summary>
    public class UrlBuilder
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of UrlBuilder.
        /// </summary>
        public UrlBuilder()
        {
            Protocol = "http";
            Path = new UrlSegmentList();
            Parameters = new UrlParamList();
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets or sets the protocol for the URL (e.g. http).
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the domain name or IP for the server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port number. Less than or equal to 0 prevents the port from appearing in the URL.
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the host:port (e.g. www.redwerb.com:8080).
        /// </summary>
        public string Authority
        {
            get
            {
                if (string.IsNullOrEmpty(Host)) return "";
                if (Port == null) return Host;
                return Host + ":" + Port;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Host = null;
                    Port = null;
                    return;
                }
                
                var tmp = (value + ":").Split(':');
                
                Host = tmp[0].IfEmpty(null);

                int port;
                if (int.TryParse(tmp[1], out port))
                    Port = port;
                else
                    Port = null;
            }
        }

        /// <summary>
        /// Gets the path for the URL. This is represented as a list of names. Index 0 will be displayed first (e.g. /Path0/Path1/.../PathN).
        /// </summary>
        public UrlSegmentList Path { get; private set; }

        /// <summary>
        /// Gets the list of query string parameters.
        /// </summary>
        public UrlParamList Parameters { get; private set; }

        private string mAnchor;
        /// <summary>
        /// Gets or sets the anchor for the page.
        /// </summary>
        public string Anchor
        {
            get { return mAnchor; }
            set { mAnchor = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the URI for this URL.
        /// </summary>
        /// <returns></returns>
        public Uri ToUri()
        {
            return new Uri(this.ToString(true));
        }

        /// <summary>
        /// Returns the properly formatted URL.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        /// Gets the URI for this URL.
        /// </summary>
        /// <param name="includeParams">Used by WebClient to not include the parameters in the url.</param>
        /// <returns></returns>
        protected Uri ToUri(bool includeParams)
        {
            return new Uri(this.ToString(includeParams));
        }

        /// <summary>
        /// Returns the properly formatted URL.
        /// </summary>
        /// <param name="includeParams">Used by WebClient to not include the parameters in the url.</param>
        /// <returns></returns>
        protected string ToString(bool includeParams)
        {
            if (string.IsNullOrEmpty(Host)) return "";

            var sb = new StringBuilder();
            sb.AppendFormat(@"{0}://{1}", Protocol, Authority);

            foreach (string segment in Path)
                sb.AppendFormat(@"/{0}", segment);

            if (includeParams && Parameters.Count > 0)
                sb.Append("?" + Parameters.ToString());

            if (!string.IsNullOrEmpty(mAnchor))
                sb.Append("#" + mAnchor);

            return sb.ToString();
        }

        #endregion

    }

}
