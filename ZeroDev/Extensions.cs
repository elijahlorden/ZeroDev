using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ZeroDev
{
    static class Extensions
    {
        public static XmlAttribute Set(this XmlAttribute attr, String val)
        {
            attr.Value = val;
            return attr;
        }

        public static XmlNode AppendTo(this XmlNode node, XmlNode parent)
        {
            parent.AppendChild(node);
            return node;
        }

        public static XmlAttribute AppendTo(this XmlAttribute attr, XmlNode parent)
        {
            parent.Attributes.Append(attr);
            return attr;
        }

        public static XmlNode GetChild(this XmlNode node, String name)
        {
            return node.ChildNodes.Cast<XmlNode>().Where(n => n.Name.Equals(name)).FirstOrDefault();
        }

        public static Boolean HasAttributeWithValue(this XmlNode node, String propName, String val)
        {
            XmlAttribute attr = node.Attributes.Cast<XmlAttribute>().Where(a => a.Name.Equals(propName) && a.Value != null && a.Value.Equals(val)).FirstOrDefault();
            return attr != null;
        }

        public static XmlNode SetAttrib(this XmlNode node, String propName, String val)
        {
            XmlAttribute attr = node.Attributes[propName] ?? node.OwnerDocument.CreateAttribute(propName).AppendTo(node);
            return node;
        }

        public static String GetAttribute(this XmlNode node, String propName)
        {
            return (node.Attributes[propName] != null) ? node.Attributes[propName].Value ?? "" : "";
        }

    }
}
