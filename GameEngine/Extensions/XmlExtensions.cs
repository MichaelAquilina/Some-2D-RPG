using System;
using System.Collections.Generic;
using System.Xml;

namespace GameEngine.Extensions
{
    public static class XmlExtensions
    {
        public static bool HasAttribute(XmlNode node, string name)
        {
            return node.Attributes[name] != null;
        }

        public static T GetAttributeValue<T>(XmlNode node, string name, T defaultValue, bool throwOnNotFound=false) where T : IConvertible
        {
            if (node.Attributes[name] == null)
                if (throwOnNotFound)
                    throw new KeyNotFoundException("The Specified Attribute " + name + " was not Found");
                else
                    return defaultValue;

            return (T)Convert.ChangeType(node.Attributes[name].Value, typeof(T));
        }

        public static string GetAttributeValue(XmlNode node, string name, string defaultValue = null)
        {
            return node.Attributes[name] == null ? defaultValue : node.Attributes[name].Value;
        }
    }
}
