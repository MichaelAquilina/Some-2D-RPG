using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GameEngine.Extensions
{
    public static class XmlExtensions
    {
        public static T GetAttributeValue<T>(this XmlNode Node, string Name, T Default, bool ThrowOnNotFound=false) where T : IConvertible
        {
            if (Node.Attributes[Name] == null)
                if (ThrowOnNotFound)
                    throw new KeyNotFoundException("The Specified Attribute " + Name + " was not Found");
                else
                    return Default;

            return (T)Convert.ChangeType(Node.Attributes[Name].Value, typeof(T));
        }

        public static string GetAttributeValue(this XmlNode Node, string Name, string Default = null)
        {
            return Node.Attributes[Name] == null ? Default : Node.Attributes[Name].Value;
        }
    }
}
