using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace BizArk.Core.Util
{

	/// <summary>
	/// Provides a way to format a string using named parameters instead of positional parameters.
	/// </summary>
	public class StringTemplate
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of StringTemplate.
        /// </summary>
        /// <param name="template"></param>
        public StringTemplate(string template)
        {
            mTemplate = template;
            CreateFormat();
        }

        #endregion

        #region Fields and Properties

        // This class uses 3 variables to store argument information in order to improve performance.
        // mArgNames array contains the names of the arguments in the order that they are placed into
        // the format string.
        // mArgValues array contains the values of the arguments. They correspond to the names in 
        // mArgNames and can be sent directly to string.Format.
        // mArgIndices dictionary contains the index of the named argument. This helps improve 
        // performance a small amount by not requiring iterating over the name array to find the index
        // when setting and retrieving values.

        private string mFormat = null;

        private Dictionary<string, int> mArgIndices = new Dictionary<string, int>();
        /// <summary>
        /// Gets or sets the named argument. Ignores invalid arguments.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                // Ignore invalid names so that the template can change independently of the code.
                if (!mArgIndices.ContainsKey(name)) return null;
                var i = mArgIndices[name];
                return mArgValues[i];
            }
            set
            {
                if (mArgIndices.ContainsKey(name))
                {
                    var i = mArgIndices[name];
                    mArgValues[i] = value;
                }
            }
        }

        private string mTemplate;
        /// <summary>
        /// Gets the template string.
        /// </summary>
        public string Template
        {
            get { return mTemplate; }
        }

        private object[] mArgValues;
        /// <summary>
        /// Gets the values for the arguments.
        /// </summary>
        public object[] ArgValues
        {
            get { return mArgValues; }
        }

        private string[] mArgNames;
        /// <summary>
        /// Gets the names for the arguments.
        /// </summary>
        public string[] ArgNames
        {
            get { return mArgNames; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the formatted string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(mFormat, mArgValues);
        }

        /// <summary>
        /// Returns the formatted string based on the values in the object.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string ToString(object values)
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(values))
                this[prop.Name] = prop.GetValue(values);
            return ToString();
        }

        /// <summary>
        /// Format a string template with the given values.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Format(string template, object values)
        {
            var tmpl = new StringTemplate(template);
            return tmpl.ToString(values);
        }

        /// <summary>
        /// Called from Format property to parse the template and create a format string.
        /// </summary>
        private void CreateFormat()
        {
            var sb = new StringBuilder();
            var argNames = new List<string>();
            var template = mTemplate.ToCharArray();
            var position = 0;
            while (true)
            {
                if (position >= template.Length) break;

                if (template[position] == '{')
                {
                    var name = GetArgName(template, ref position);
                    var format = GetArgFormat(template, ref position);
                    var i = argNames.IndexOf(name);
                    if (i < 0)
                    {
                        argNames.Add(name);
                        i = argNames.IndexOf(name);
                    }
                    sb.Append("{" + i.ToString() + format + "}");
                }
                else
                    sb.Append(GetLiteral(template, ref position));
            }
            mArgNames = argNames.ToArray();
            mArgValues = new object[mArgNames.Length];
            for (int i = 0; i < mArgNames.Length; i++)
                mArgIndices.Add(mArgNames[i], i);
            mFormat = sb.ToString();
        }

        private static string GetLiteral(char[] template, ref int position)
        {
            var literal = new StringBuilder();

            while (position < template.Length && template[position] != '{')
            {
                // Ignore escape char.
                if (position < template.Length - 1
                    && template[position] == '\\'
                    && (template[position + 1] == '{' || template[position + 1] == '\\'))
                    position++;

                literal.Append(template[position]);

                position++;
            }

            return literal.ToString();
        }

        private static string GetArgName(char[] template, ref int position)
        {
            var name = new StringBuilder();

            position++; // the first position contains the {.

            // Get the name of the argument.
            EatSpaces(template, ref position);
            while (position < template.Length && IsValidNameChar(template[position]))
                name.Append(template[position++]);
            EatSpaces(template, ref position);

            if (position >= template.Length)
                throw new FormatException("Unmatched braces in template.");

            return name.ToString().ToLower();
        }

        private static string GetArgFormat(char[] template, ref int position)
        {
            var format = new StringBuilder();

            if (template[position] != '}')
            {
                while (position < template.Length && template[position] != '}')
                    format.Append(template[position++]);
            }

            if (position >= template.Length)
                throw new FormatException("Unmatched braces in template.");

            position++;
            return format.ToString();
        }

        private static void EatSpaces(char[] template, ref int position)
        {
            while (position < template.Length && template[position] == ' ')
                position++;
        }

        private static bool IsValidNameChar(char ch)
        {
            if (char.IsLetterOrDigit(ch)) return true;
            if (ch == '_') return true;
            if (ch == '.') return true;
            return false;
        }

        #endregion

    }

}
