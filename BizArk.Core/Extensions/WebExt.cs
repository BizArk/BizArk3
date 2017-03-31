using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using BizArk.Core.Extensions.ObjectExt;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.Core.Extensions.WebExt
{
	/// <summary>
	/// Extension methods that are useful when working with web objects.
	/// </summary>
	public static class WebExt
	{

		/// <summary>
		/// Gets the content from a response.
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		public static string GetContentString(this WebResponse response)
		{
			if (response == null) return null;

			using (var s = response.GetResponseStream())
			{
				var sr = new StreamReader(s);
				return sr.ReadToEnd();
			}
		}

		/// <summary>
		/// Encodes a string for safe HTML.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string HtmlEncode(this string str)
		{
			return WebUtility.HtmlEncode(str);
		}

		/// <summary>
		/// Decodes an encoded string.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string HtmlDecode(this string str)
		{
			return WebUtility.HtmlDecode(str);
		}

		/// <summary>
		/// Can be used to encode a query string value.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string UrlEncode(this string str)
		{
			return Uri.EscapeUriString(str);
		}

		/// <summary>
		/// Can be used to decode a url encoded value.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string UrlDecode(this string str)
		{
			return Uri.UnescapeDataString(str);
		}

		/// <summary>
		/// Creates a query string without referencing System.Web. Property values are converted to strings using ConvertEx.ToString(). 
		/// </summary>
		/// <param name="values">Encodes the properties of the class. If values is a NameValueCollection, the values of the collection will be encoded.</param>
		/// <param name="propNames">Optional list of properties to include in the query string. If empty, includes all properties.</param>
		/// <returns></returns>
		public static string ToQueryString(this object values, params string[] propNames)
		{
			if (values == null) return null;

			var sb = new StringBuilder();

			var props = values.ToPropertyBag();
			foreach (var prop in props)
			{
				if (propNames.Length > 0 && !propNames.Contains(prop.Key))
					continue;

				if (sb.Length > 0) sb.Append("&");
				var value = ConvertEx.ToString(prop.Value);
				sb.Append($"{prop.Key}={UrlEncode(value)}");
			}

			return sb.ToString();
		}

		/// <summary>
		/// Transforms a string into an identifier that can be used in a url.
		/// </summary>
		/// <param name="phrase"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static string GenerateSlug(this string phrase, int maxLength = int.MaxValue)
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
