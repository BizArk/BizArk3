using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace BizArk.Core.Util
{

    /// <summary>
    /// Web related helper methods.
    /// </summary>
    public class WebUtil
    {

        /// <summary>
        /// Creates a query string without referencing System.Web.
        /// </summary>
        /// <param name="values">Encodes the properties of the class. If values is a NameValueCollection, the values of the collection will be encoded.</param>
        /// <returns></returns>
        public static string GetUrlEncodedData(object values)
        {
            var sb = new StringBuilder();
            var nvc = values as NameValueCollection;
            if (nvc == null)
            {
                var props = TypeDescriptor.GetProperties(values);
                foreach (PropertyDescriptor prop in props)
                {
                    var value = ConvertEx.ToString(prop.GetValue(values));
                    if (sb.Length > 0) sb.Append("&");
                    sb.AppendFormat("{0}={1}", prop.Name, Uri.EscapeUriString(value));
                }
            }
            else
            {
                foreach (string key in nvc.AllKeys)
                {
                    var value = nvc[key];
                    if (sb.Length > 0) sb.Append("&");
                    sb.AppendFormat("{0}={1}", key, Uri.EscapeUriString(value));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Transforms a string into an identifier that can be used in a url.
        /// </summary>
        /// <param name="phrase"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string GenerateSlug(string phrase, int maxLength = int.MaxValue)
        {
            string str = phrase.ToLower();

            // remove invalid chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces/hyphens into one space      
            str = Regex.Replace(str, @"[\s-]+", " ").Trim();
            // cut and trim it
            str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim();
            // hyphens
            str = Regex.Replace(str, @"\s", "-");

            return str;
        }

    }
}
