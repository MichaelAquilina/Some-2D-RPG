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

        public static T GetAttributeValue<T>(XmlNode node, string name, T defaultValue, bool throwOnNotFound=false)
        {
            if (node.Attributes[name] == null)
                if (throwOnNotFound)
                    throw new KeyNotFoundException(string.Format("The Specified Attribute '{0}' was not Found", name));
                else
                    return defaultValue;

            return (T) ReflectionExtensions.SmartConvert(node.Attributes[name].Value, typeof(T));
        }

        public static string GetAttributeValue(XmlNode node, string name, string defaultValue = null, bool throwOnNotFound=false)
        {
            return GetAttributeValue<string>(node, name, defaultValue, throwOnNotFound);
        }
    }
}
