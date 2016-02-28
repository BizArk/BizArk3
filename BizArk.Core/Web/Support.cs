using System;
using System.Collections.Generic;
using System.ComponentModel;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Extensions.WebExt;

namespace BizArk.Core.Web
{

    /// <summary>
    /// Represents a parameter in a URL.
    /// </summary>
    public class UrlParam
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of UrlParam.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public UrlParam(string name, string value)
        {
            mName = name;
            mValue = value;
        }

        #endregion

        #region Fields and Properties

        private string mName = "";
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        private string mValue = "";
        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Gets the properly encoded value of the parameter.
        /// </summary>
        public string EncodedValue
        {
            get { return mValue.UrlEncode(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the URL encoded key=value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", mName, EncodedValue);
        }

        #endregion

    }

    /// <summary>
    /// Contains a list of UrlParam objects. 
    /// </summary>
    public class UrlParamList
        : List<UrlParam>
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of UrlParamList.
        /// </summary>
        public UrlParamList()
        {
        }

        /// <summary>
        /// Creates an instance of UrlParamList based on a query string.
        /// </summary>
        /// <param name="queryStr"></param>
        /// <returns></returns>
        public UrlParamList(string queryStr)
        {
            AddRange(queryStr);
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets a url parameter based on it's name. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Null if not found.</returns>
        public UrlParam this[string name]
        {
            get
            {
                foreach (UrlParam p in this)
                    if (p.Name == name) return p;
                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a range of parameters based on a query string.
        /// </summary>
        /// <param name="queryStr"></param>
        /// <returns>The list of new UrlParam objects.</returns>
        public UrlParam[] AddRange(string queryStr)
        {
            var paramList = new List<UrlParam>();

            string[] segments = queryStr.Split('&');
            foreach (string segment in segments)
            {
                string[] parts = segment.Split('=');
                if (parts.Length > 0)
                {
                    string key = parts[0].Trim(new char[] { '?', ' ' });
                    string val = parts[1].Trim();
                    val = val.UrlDecode();
                    paramList.Add(Add(key, val));
                }
            }
            return paramList.ToArray();
        }

        /// <summary>
        /// Adds the properties of the object as parameters to the list.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>The list of new UrlParam objects.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        public UrlParam[] AddRange(object values)
        {
            var paramList = new List<UrlParam>();

            foreach(PropertyDescriptor prop in TypeDescriptor.GetProperties(values))
            {
                var val = prop.GetValue(values);
                var str = ConvertEx.ToString(val);
                paramList.Add(Add(prop.Name, str));
            }

            return paramList.ToArray();
        }

        /// <summary>
        /// Adds a UrlParam to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UrlParam Add(string name, string value)
        {
            var p = new UrlParam(name, value);
            this.Add(p);
            return p;
        }

        /// <summary>
        /// Adds a UrlParam to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UrlParam Add(string name, long value)
        {
            var p = new UrlParam(name, value.ToString());
            this.Add(p);
            return p;
        }

        /// <summary>
        /// Adds a UrlParam to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UrlParam Add(string name, ulong value)
        {
            var p = new UrlParam(name, value.ToString());
            this.Add(p);
            return p;
        }

        /// <summary>
        /// Adds a UrlParam to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UrlParam Add(string name, Guid value)
        {
            var p = new UrlParam(name, value.ToString("B"));
            this.Add(p);
            return p;
        }

        /// <summary>
        /// Removes a parameter from the list.
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            this.Remove(this[name]);
        }

        /// <summary>
        /// Returns the value of the given parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dflt"></param>
        /// <returns></returns>
        public string GetString(string name, string dflt = null)
        {
            var p = this[name];
            if (p == null) return dflt;
            return p.Value;
        }

        /// <summary>
        /// Returns the value of the given parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dflt"></param>
        /// <returns></returns>
        public int? GetInt(string name, int? dflt = null)
        {
            var str = GetString(name);
            if (str.IsEmpty()) return dflt;
            return ConvertEx.ToInt(str);
        }

        /// <summary>
        /// Returns the value of the given parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dflt"></param>
        /// <returns></returns>
        public long? GetLong(string name, long? dflt = null)
        {
            var str = GetString(name);
            if (str.IsEmpty()) return dflt;
            long val;
            return long.TryParse(str, out val) ? val : dflt;
        }

        /// <summary>
        /// Returns the value of the given parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dflt"></param>
        /// <returns></returns>
        public Guid? GetGuid(string name, Guid? dflt = null)
        {
            var str = GetString(name);
            if (str.IsEmpty()) return dflt;
            Guid val;
            return Guid.TryParse(str, out val) ? val : dflt;
        }

        /// <summary>
        /// Returns the encoded query string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var parameters = new List<string>();
            foreach (UrlParam param in this)
                parameters.Add(param.ToString());
            return string.Join("&", parameters.ToArray());
        }

        #endregion

    }

    /// <summary>
    /// Holds parts of a url that will be combined to create the path.
    /// </summary>
    public class UrlSegmentList
        : List<string>
    {

        /// <summary>
        /// Adds a set of segments to the url.
        /// </summary>
        /// <param name="segments"></param>
        public void AddRange(params string[] segments)
        {
            base.AddRange(segments);
        }
    }

    /// <summary>
    /// The different HTTP methods supported by the UrlBuilder submit method.
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>Get methods</summary>
        [Description("GET")]
        Get,
        /// <summary>Post methods</summary>
        [Description("POST")]
        Post,
        /// <summary>Put methods</summary>
        [Description("PUT")]
        Put,
        /// <summary>Delete methods</summary>
        [Description("DELETE")]
        Delete
    }

}
