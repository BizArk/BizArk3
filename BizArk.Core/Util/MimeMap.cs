using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BizArk.Core.Extensions.StringExt;
using Microsoft.Win32;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Provides a lookup service to get the MimeType based on a file extension. The default data comes from the Mime.Types file that is included in this project and the registry.
	/// </summary>
	public static class MimeMap
	{

		#region Initialization and Destruction

		static MimeMap()
		{
			if (sMimeMap == null)
				sMimeMap = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

			try
			{
				InitializeFromRegistry();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Failed to initialize MimeMap from the Registry. Error: {0}", ex.Message);
			}

			var localDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var mimePath = Path.Combine(localDir, "Mime.Types");
			if (File.Exists(mimePath))
				Initialize(File.ReadAllText(mimePath));
		}

		/// <summary>
		/// Initializes the mime map from the registry.
		/// </summary>
		private static void InitializeFromRegistry()
		{
			var root = Registry.ClassesRoot;
			foreach (var keyName in root.GetSubKeyNames())
			{
				if (keyName.IsEmpty()) continue;
				if (!keyName.StartsWith(".")) continue; // not an extension.
				var extKey = root.OpenSubKey(keyName);
				if (extKey == null) continue;
				var mimeType = extKey.GetValue("Content Type") as string;
				if (mimeType.IsEmpty()) continue;
				RegisterMimeType(mimeType, keyName);
			}
		}

		/// <summary>
		/// Initializes the mime map from the string.
		/// </summary>
		/// <param name="mimeTypes">List of mime types. Uses the format in Apache Mime.Types format. View at http://svn.apache.org/viewvc/httpd/httpd/trunk/docs/conf/mime.types?view=markup.</param>
		public static void Initialize(string mimeTypes)
		{
			using (var sr = new System.IO.StringReader(mimeTypes))
			{
				string line;
				// all extensions and mime types should be lower case.
				while ((line = sr.ReadLine()) != null)
				{
					line = line.Trim();
					if (line.StartsWith("#")) continue;

					var mimeType = Regex.Split(line, @"[ \t]+");
					if (mimeType.Length < 2) continue; // invalid
					for (int i = 1; i < mimeType.Length; i++)
						RegisterMimeType(mimeType[0], mimeType[i]);
				}
			}
		}

		#endregion

		#region Fields and Properties

		private static Dictionary<string, string> sMimeMap;

		#endregion

		#region Methods

		/// <summary>
		/// Register a mime type.
		/// </summary>
		/// <param name="mimeType">The mime type. Ex: text/plain.</param>
		/// <param name="extensions">List of extensions for this mime type</param>
		public static void RegisterMimeType(string mimeType, params string[] extensions)
		{
			// extensions shouldn't start with . but allow them to be sent.
			mimeType = mimeType.Trim().ToLower();

			foreach (var extension in extensions)
			{
				var ext = extension.Trim().TrimStart('.').ToLower();

				if (sMimeMap.ContainsKey(ext))
					sMimeMap[ext] = mimeType;
				else
					sMimeMap.Add(ext, mimeType);
			}
		}

		/// <summary>
		/// Gets the mime type based on the extension.
		/// </summary>
		/// <param name="ext"></param>
		/// <returns></returns>
		public static string GetMimeType(string ext)
		{
			// don't need the .
			ext = ext.TrimStart('.').ToLower();
			if (sMimeMap.ContainsKey(ext))
				return sMimeMap[ext];
			else
				return null;
		}

		#endregion

	}
}
