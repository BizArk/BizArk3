using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BizArk.Core.Extensions.ArrayExt;

namespace BizArk.Core.Extensions.StringExt
{
    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
        /// </summary>
        /// <param name="str">The string to wrap.</param>
        /// <param name="maxLength">The maximum number of characters per line.</param>
        /// <param name="prefix">Adds this string to the beginning of each line that has been broken (used for indenting text).</param>
        /// <returns></returns>
        public static string Wrap(this string str, int maxLength, string prefix = "")
        {
            var lines = WrappedLines(str, maxLength, prefix);
            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
        /// </summary>
        /// <param name="str">The string to wrap.</param>
        /// <param name="maxLength">The maximum number of characters per line.</param>
        /// <param name="prefix">Adds this string to the beginning of each line that has been broken (used for indenting text).</param>
        /// <returns></returns>
        public static string[] WrappedLines(this string str, int maxLength, string prefix)
        {
            if (string.IsNullOrEmpty(str)) return new string[] { };
            if (maxLength <= 0) return new string[] { str };

            var lines = new List<string>();
            var re = new Regex("^(?<start>[ \t]+)?(?<text>.*)$");
            foreach (string line in str.Lines())
            {
                var m = re.Match(line);
                var start = m.Groups["start"].Value;
                var text = m.Groups["text"].Value;
                if (text.Trim().IsEmpty())
                {
                    // Empty line. ignore whitespace.
                    lines.Add("");
                    continue;
                }

                var textMaxLen = maxLength - start.Length;
                var i = 0;
                do
                {
                    string partial;
                    var e = i + textMaxLen;
                    if (e >= text.Length)
                        partial = text.Substring(i);
                    else
                    {
                        while (!char.IsWhiteSpace(text[e]) && e > i) e--;
                        if (e <= i)
                            // No whitespace characters. Just break the line at the max length.
                            e = i + textMaxLen;
                        partial = text.Substring(i, e - i);
                    }

                    if (i == 0)
                        // First line, no prefix.
                        lines.Add(start + partial.Trim());
                    else
                        lines.Add(prefix + start + partial.Trim());

                    i = e;
                } while (i < text.Length);
            }

            return lines.ToArray();
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
                    retVals[i] = ConvertEx.ChangeType<T>(vals[i].Trim());
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
        /// Shortcut for string.Format.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string str, params object[] args)
        {
            if (str == null) return null;
            return string.Format(str, args);
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
        public static char[] Vowels = { 'a', 'e', 'i', 'o', 'u' };

        /// <summary>
        /// Determines if the character is a vowel.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsVowel(this char ch)
        {
            return Vowels.Contains(ch);
        }

    }
}
