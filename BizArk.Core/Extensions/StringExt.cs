using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BizArk.Core.Extensions.ArrayExt;
using System.Text;
using BizArk.Core.Util;

namespace BizArk.Core.Extensions.StringExt
{
	/// <summary>
	/// Provides extension methods for strings.
	/// </summary>
	public static class StringExt
	{

		/// <summary>
		/// Gets or sets the size of a TAB (number of spaces). Used for calculating word wraps.
		/// </summary>
		public static byte DefaultWrapTabWidth { get; set; } = 4;

		/// <summary>
		/// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
		/// </summary>
		/// <param name="str">The string to wrap.</param>
		/// <param name="maxWidth">The maximum number of characters per line.</param>
		/// <param name="prefix">Adds this string to the beginning of each line that has been broken (used for indenting text).</param>
		/// <returns></returns>
		public static string Wrap(this string str, int maxWidth, string prefix = "")
		{
			return Wrap(str, new StringWrapOptions() { MaxWidth = maxWidth, Prefix = prefix });
		}

		/// <summary>
		/// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
		/// </summary>
		/// <param name="str">The string to wrap.</param>
		/// <param name="options">The options used for wrapping a string.</param>
		/// <returns></returns>
		public static string Wrap(this string str, StringWrapOptions options)
		{
			var lines = WrappedLines(str, options);
			return string.Join(Environment.NewLine, lines);
		}

		/// <summary>
		/// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
		/// </summary>
		/// <param name="str">The string to wrap.</param>
		/// <param name="maxWidth">The maximum number of characters per line.</param>
		/// <param name="prefix">Adds this string to the beginning of each line that has been broken (used for indenting text).</param>
		/// <returns></returns>
		public static string[] WrappedLines(this string str, int maxWidth, string prefix)
		{
			return WrappedLines(str, new StringWrapOptions() { MaxWidth = maxWidth, Prefix = prefix });
		}

		/// <summary>
		/// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
		/// </summary>
		/// <param name="str">The string to wrap.</param>
		/// <param name="options">The options used for wrapping a string.</param>
		/// <returns></returns>
		public static string[] WrappedLines(this string str, StringWrapOptions options)
		{
			if (string.IsNullOrEmpty(str)) return new string[] { };
			if (options.MaxWidth <= 0) return new string[] { str };

			var prefix = options.Prefix ?? ""; // Make sure we have something so we don't have to keep checking for null.
			var prefixWidth = MeasureWidth(prefix, options.TabWidth);
			if (prefixWidth >= options.MaxWidth)
				throw new ArgumentException("The width of the prefix must be less than the max width of the line.", "options");

			var lines = new List<string>();
			foreach (string line in str.Lines())
			{
				var lineWidth = 0;

				if (line.Trim() == "")
				{
					// Empty line. Just use an empty string in case there is white space
					// wider than the line. We don't want to wrap white spaces if that
					// is all that there is on the line since the user won't see it.
					lines.Add("");
					continue;
				}

				// We want to keep any white space at the front of the line, but the
				// white space at the end could cause an extra line that appears empty.
				var chars = line.TrimEnd();

				var sb = new StringBuilder();
				// Iterate through each character on the line so that we
				// can adjust for TABs and other odd characters.
				for (var i = 0; i < chars.Length; i++)
				{
					var ch = chars[i];
					var charWidth = 1;

					if (ch == '\t')
					{
						// Tabs have variable width based on the max width they allow.
						// This allows text to have something that looks like columns
						// It's not recommended to have embedded TABs in text for wrapped
						// lines, but we need to support it.

						charWidth = CalcTabWidth(lineWidth, options.TabWidth);
					}
					else if (char.IsControl(ch))
					{
						// TAB is considered a control character, but it can be printed, so put this check in the else.

						// Ignore control characters. They aren't printed, and don't add to the length of the string.
						continue;
					}

					// Check to see if adding this char will push the width of the line past the max.
					if (lineWidth + charWidth <= options.MaxWidth)
					{
						sb.Append(ch);
						lineWidth += charWidth;
					}
					else if (char.IsWhiteSpace(ch))
					{
						// If this is a white space, then we know we are at the end of a word.
						lines.Add(sb.ToString());
						sb.Clear();
						sb.Append(prefix);

						// Ignore white space at beginning of new wrapped line.
						// It would make the left column of text appear jagged.
						lineWidth = prefixWidth;
					}
					else
					{
						// Need to find the beginning of the current word and remove it from 
						// the line that we are building. We'll add it to the new line instead.
						// The prefix is not included when finding a whitespace character.
						var idx = -1;
						for (var j = sb.Length - 1; j >= prefix.Length; j--)
						{
							if (char.IsWhiteSpace(sb[j]))
							{
								idx = j;
								break;
							}
						}

						// There weren't any white space characters or only the first char is a white space. 
						// Unable to break at word boundary, so break the word at the character.
						if (idx <= 0)
						{
							lines.Add(sb.TrimEnd()); // Don't want any TABs at the end of the line causing problems.
							sb.Clear();
							lineWidth = prefixWidth + charWidth;
							sb.Append(prefix + ch);
						}
						else
						{
							lines.Add(sb.Substring(0, idx));
							var end = sb.Substring(idx + 1);
							sb.Clear();
							sb.Append(prefix + end); // We know there are no TABs (white space) or control chars, so we can assume all chars have length of 1.
							lineWidth = prefixWidth + end.Length + charWidth;
							sb.Append(ch);
						}
					}
				}

				// Add the end of the line.
				if (sb.Trim() != "")
					lines.Add(sb.ToString());
			}

			return lines.ToArray();
		}

		/// <summary>
		/// Calculate the width of the tab based on where it is in the string.
		/// </summary>
		/// <param name="tabStart">The width of the string prior to the tab.</param>
		/// <param name="tabWidth">The max width of the tab.</param>
		/// <returns></returns>
		private static byte CalcTabWidth(int tabStart, byte tabWidth)
		{
			var width = tabWidth - (tabStart % tabWidth);
			return (byte)width;
		}

		/// <summary>
		/// This method measures the width of the string, taking tabs into account. Should only be used for beginning of a line or tabs will be off.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="tabWidth"></param>
		/// <returns></returns>
		private static int MeasureWidth(string str, byte tabWidth)
		{
			if (str == null) return 0;

			var width = 0;

			for (var i = 0; i < str.Length; i++)
			{
				var ch = str[i];
				if (ch == '\t')
				{
					//todo: Calculate based on location in string.
					width += tabWidth;
				}
				else if (!char.IsControl(ch))
					width += 1;
			}

			return width;
		}

		/// <summary>
		/// Splits the string into lines.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string[] Lines(this string str)
		{
			var lines = new List<string>();
			using (var sr = new StringReader(str))
			{
				string line = sr.ReadLine();
				while (line != null)
				{
					lines.Add(line);
					line = sr.ReadLine();
				}
			}

			return lines.ToArray();
		}

		/// <summary>
		/// Splits the string into words (all white space is removed).
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string[] Words(this string str)
		{
			return str.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>
		/// Shortcut for ConvertEx.IsEmpty. Works because this is an extension method, not a real method.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsEmpty(this string str)
		{
			return ConvertEx.IsEmpty(str);
		}

		/// <summary>
		/// Shortcut for !ConvertEx.IsEmpty. Works because this is an extension method, not a real method.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool HasValue(this string str)
		{
			return !ConvertEx.IsEmpty(str);
		}

		/// <summary>
		/// Gets the string up to the maximum number of characters.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static string Max(this string str, int max)
		{
			return Max(str, max, false);
		}

		/// <summary>
		/// Returns an array split along the separator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="str"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static T[] Split<T>(this string str, params char[] separator)
		{
			if (string.IsNullOrEmpty(str)) return new T[] { };
			var vals = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			int i = -1;
			try
			{
				var retVals = new T[vals.Length];
				for (i = 0; i < vals.Length; i++)
				{
					retVals[i] = ConvertEx.To<T>(vals[i].Trim());
				}
				return retVals;
			}
			catch (Exception ex)
			{
				if (i < 0) throw;
				throw new FormatException(string.Format("Cannot convert value '{0}'", vals[i]), ex);
			}
		}

		/// <summary>
		/// Gets the string up to the maximum number of characters. If ellipses is true and the string is longer than the max, the last 3 chars will be ellipses.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="max"></param>
		/// <param name="ellipses"></param>
		/// <returns></returns>
		public static string Max(this string str, int max, bool ellipses)
		{
			if (str == null) return null;

			if (str.Length <= max) return str;
			if (ellipses)
				return str.Substring(0, max - 3) + "...";
			else
				return str.Substring(0, max);
		}

		/// <summary>
		/// Determines if a string consists of all valid ASCII values.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsAscii(this string str)
		{
			if (string.IsNullOrEmpty(str)) return true;

			foreach (var ch in str)
				if ((int)ch > 127) return false;

			return true;
		}

		/// <summary>
		/// Gets the right side of the string.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Right(this string str, int length)
		{
			if (str == null) return null;
			if (str.Length <= length) return str;
			return str.Substring(str.Length - length);
		}

		/// <summary>
		/// Truncates the string.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Left(this string str, int length)
		{
			if (str == null) return null;
			if (str.Length <= length) return str;
			return str.Substring(0, length);
		}

		/// <summary>
		/// Gets everything to the right of the find string.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="find">The string to find. Looks for the first occurrence.</param>
		/// <returns>Null if find not found.</returns>
		public static string Right(this string str, string find)
		{
			if (find.IsEmpty()) return str;
			if (str.IsEmpty()) return null;
			var idx = str.IndexOf(find);
			if (idx < 0) return null;
			return str.Substring(idx + find.Length);
		}

		/// <summary>
		/// Gets everything to the left of the find string.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="find">The string to find. Looks for the first occurrence.</param>
		/// <returns>Null if find not found.</returns>
		public static string Left(this string str, string find)
		{
			if (find.IsEmpty()) return str;
			if (str.IsEmpty()) return null;
			var idx = str.IndexOf(find);
			if (idx < 0) return null;
			return str.Substring(0, idx);
		}

		/// <summary>
		/// If the string is empty, returns the default.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="dflt"></param>
		/// <returns></returns>
		public static string IfEmpty(this string str, string dflt)
		{
			if (str.IsEmpty()) return dflt;
			return str;
		}

		/// <summary>
		/// Vowels. Used for IsVowel.
		/// </summary>
		public static char[] Vowels = { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };

		/// <summary>
		/// Determines if the character is a vowel.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		public static bool IsVowel(this char ch)
		{
			return Vowels.Contains(ch);
		}

		/// <summary>
		/// Puts the string into a MemoryStream. Resets the MemoryStream to position 0 so it can be read from.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="encoding">The encoding to use to write the string to the stream. If null, uses the default encoding.</param>
		/// <returns></returns>
		public static Stream ToStream(this string str, Encoding encoding = null)
		{
			var ms = new MemoryStream();
			var sw = new StreamWriter(ms, encoding ?? Encoding.Default);
			sw.Write(str);
			sw.Flush();
			ms.Position = 0;
			return ms;
		}

		/// <summary>
		/// Puts the string into an array of bytes.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="encoding">The encoding to use to write the string to array of bytes. If null, uses the default encoding.</param>
		/// <returns></returns>
		public static byte[] ToBytes(this string str, Encoding encoding = null)
		{
			return (encoding ?? Encoding.Default).GetBytes(str);

		}

		#region Security

		/// <summary>
		/// Hashes the string using SHA-256.
		/// </summary>
		/// <param name="str">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA256(this string str, string salt = null)
		{
			return Security.SHA256(str, salt);
		}

		/// <summary>
		/// Hashes the string using SHA-256.
		/// </summary>
		/// <param name="str">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA256(this string str, byte[] salt = null)
		{
			return Security.SHA256(str, salt);
		}

		/// <summary>
		/// Hashes the string using SHA-512.
		/// </summary>
		/// <param name="str">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA512(this string str, string salt = null)
		{
			return Security.SHA512(str, salt);
		}

		/// <summary>
		/// Hashes the string using SHA-512.
		/// </summary>
		/// <param name="str">The value to hash.</param>
		/// <param name="salt">An optional salt.</param>
		/// <returns></returns>
		public static byte[] SHA512(this string str, byte[] salt = null)
		{
			return Security.SHA512(str, salt);
		}

		#endregion

		#region StringBuilder

		/// <summary>
		/// Retrieves a substring from this instance. The substring starts at a specified
		/// character position and continues to the end of the string.
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
		/// <returns>
		/// A string that is equivalent to the substring that begins at startIndex in this
		/// instance, or System.String.Empty if startIndex is equal to the length of this instance.
		/// </returns>
		public static string Substring(this StringBuilder sb, int startIndex)
		{
			return sb.ToString().Substring(startIndex);
		}

		/// <summary>
		/// Retrieves a substring from this instance. The substring starts at a specified
		/// character position and continues to the end of the string.
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
		/// <param name="length">The number of characters in the substring.</param>
		/// <returns>
		/// A string that is equivalent to the substring of length length that begins at
		/// startIndex in this instance, or System.String.Empty if startIndex is equal to
		/// the length of this instance and length is zero.
		/// </returns>
		public static string Substring(this StringBuilder sb, int startIndex, int length)
		{
			return sb.ToString().Substring(startIndex, length);
		}

		/// <summary>
		/// Removes all leading and trailing white-space characters from the current System.String object.
		/// </summary>
		/// <param name="sb"></param>
		/// <returns>
		/// The string that remains after all white-space characters are removed from the
		/// start and end of the current string. If no characters can be trimmed from the
		/// current instance, the method returns the current instance unchanged.
		/// </returns>
		public static string Trim(this StringBuilder sb)
		{
			return sb.ToString().Trim();
		}

		/// <summary>
		/// Removes all trailing occurrences of a set of characters specified in an array
		/// from the current System.String object.
		/// </summary>
		/// <param name="sb"></param>
		/// <returns>
		/// The string that remains after all white-space characters are removed from the
		/// start and end of the current string. If no characters can be trimmed from the
		/// current instance, the method returns the current instance unchanged.
		/// </returns>
		public static string TrimEnd(this StringBuilder sb)
		{
			return sb.ToString().TrimEnd();
		}

		#endregion

		#region Split

		/// <summary>
		/// Splits a string on the given char and if trim is true, removes leading and trailing whitespace characters from each element.
		/// </summary>
		/// <param name="str">The string to split.</param>
		/// <param name="separator">The char used to split the string.</param>
		/// <param name="trim">If true, removes leading and trailing whitespace characters from each element.</param>
		/// <returns></returns>
		public static string[] Split(this string str, char separator, bool trim)
		{
			return Split(str, separator, trim, false);
		}

		/// <summary>
		/// Splits a string on the given char and if trim is true, removes leading and trailing whitespace characters from each element.
		/// </summary>
		/// <param name="str">The string to split.</param>
		/// <param name="separator">The char used to split the string.</param>
		/// <returns></returns>
		public static T[] Split<T>(this string str, char separator)
		{
			return Split(str, separator, true, false).Convert<T>();
		}

		/// <summary>
		/// Splits a string on the given char and if trim is true, removes leading and trailing whitespace characters from each element.
		/// </summary>
		/// <param name="str">The string to split.</param>
		/// <param name="separator">The char used to split the string.</param>
		/// <param name="trim">If true, removes leading and trailing whitespace characters from each element.</param>
		/// <param name="removeEmpties">Removes empty elements from the string.</param>
		/// <returns></returns>
		public static string[] Split(this string str, char separator, bool trim, bool removeEmpties)
		{
			string[] strs;
			if (removeEmpties)
				strs = str.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
			else
				strs = str.Split(separator);

			if (trim)
			{
				var strl = new List<string>();
				for (int i = 0; i < strs.Length; i++)
				{
					var s = strs[i].Trim();
					if (removeEmpties && s == "")
					{
						// don't add this value.
					}
					else
						strl.Add(s);
				}
				strs = strl.ToArray();
			}
			return strs;
		}

		/// <summary>
		/// Splits a string on the given char and if trim is true, removes leading and trailing whitespace characters from each element.
		/// </summary>
		/// <param name="str">The string to split.</param>
		/// <param name="separator">The char used to split the string.</param>
		/// <param name="removeEmpties">Removes empty elements from the string.</param>
		/// <returns></returns>
		public static T[] Split<T>(this string str, char separator, bool removeEmpties)
		{
			return Split(str, separator, true, removeEmpties).Convert<T>();
		}

		#endregion

	}

	/// <summary>
	/// Options for StringExt.Wrap().
	/// </summary>
	public class StringWrapOptions
	{

		/// <summary>
		/// Gets or sets the maximum number of characters in a line.
		/// </summary>
		public int MaxWidth { get; set; }

		/// <summary>
		/// Gets or sets the prefix used for wrapped lines. Will only be applied to lines created after the initial line.
		/// </summary>
		public string Prefix { get; set; } = "";

		/// <summary>
		/// Gets or sets the max width of a TAB character. 
		/// </summary>
		public byte TabWidth { get; set; } = StringExt.DefaultWrapTabWidth;

	}

}
