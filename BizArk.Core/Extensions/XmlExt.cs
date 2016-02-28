using System.Xml;

namespace BizArk.Core.Extensions.XmlExt
{
    /// <summary>
    /// Provides extension methods for processing Xml.
    /// </summary>
    public static class XmlExt
    {

        /// <summary>
        /// Gets a string from an attribute or node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetString(this XmlNode node, string name)
        {
            var dfltVal = ConvertEx.GetDefaultEmptyValue<string>();
            return GetString(node, name, dfltVal);
        }

        /// <summary>
        /// Gets a string from an attribute or node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="dfltVal"></param>
        /// <returns></returns>
        public static string GetString(this XmlNode node, string name, string dfltVal)
        {
            var att = node.Attributes[name];
            if (att != null) return att.Value;
            var child = node.SelectSingleNode(name);
            if (child != null) return child.InnerText;
            return dfltVal;
        }

        /// <summary>
        /// Gets an integer from an attribute or node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetInt(this XmlNode node, string name)
        {
            var dfltVal = ConvertEx.GetDefaultEmptyValue<int>();
            return GetInt(node, name);
        }

        /// <summary>
        /// Gets an integer from an attribute or node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="dfltVal"></param>
        /// <returns></returns>
        public static int GetInt(this XmlNode node, string name, int dfltVal)
        {
            var val = GetString(node, name, null);
            if (val == null) return dfltVal;
            return ConvertEx.ToInteger(val);
        }

        /// <summary>
        /// Sets the value of the attribute to the given value. Creates the attribute if it doesn't exist. Uses ConvertEx to convert value to a string.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static XmlAttribute SetAttributeValue(this XmlNode node, string name, object value)
        {
            var att = node.Attributes[name];
            if(att == null)
            {
                att = node.OwnerDocument.CreateAttribute(name);
                node.Attributes.Append(att);
            }
            att.Value = ConvertEx.ToString(value);
            return att;
        }

        /// <summary>
        /// Sets the value of the named element to the given value. Creates the element if it doesn't exist. Uses ConvertEx to convert value to a string.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static XmlElement SetElementValue(this XmlNode node, string name, object value)
        {
            var child = (XmlElement)node.SelectSingleNode(name);
            if(child == null)
            {
                child = node.OwnerDocument.CreateElement(name);
                node.AppendChild(child);
            }            
            child.InnerText = ConvertEx.ToString(value);
            return child;
        }

    }

}
