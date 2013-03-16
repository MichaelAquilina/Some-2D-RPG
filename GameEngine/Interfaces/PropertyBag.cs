using System.Collections.Generic;
using System.Xml;
using System;

namespace GameEngine.Interfaces
{
    //Represents an Object that allows for meta-data to be stored in a dictionary
    //Provides support for loading property information from Xml Node objects automatically
    public abstract class PropertyBag
    {
        private Dictionary<string, string> _properties = new Dictionary<string, string>();

        public T GetProperty<T>(string Name, T Default)
        {
            return HasProperty(Name) ? (T) Convert.ChangeType(_properties[Name], typeof(T)) : Default;
        }

        public string GetProperty(string Name, string Default=null)
        {
            return HasProperty(Name)? _properties[Name] : Default;
        }

        public void SetProperty(string Name, string Value)
        {
            _properties[Name] = Value;
        }

        public bool HasProperty(string Name)
        {
            return _properties.ContainsKey(Name);
        }

        /// <summary>
        /// Load properties from the specified XmlNode. An Xml Element under the name "properties"
        /// will have its content loaded automatically using this method. If no properties node is
        /// found in the specified node, then the function will do nothing.
        /// </summary>
        /// <param name="SelectedNode">XmlNode to extract the properties from</param>
        public void LoadProperties(XmlNode SelectedNode)
        {
            XmlNode propertiesNode = SelectedNode.SelectSingleNode("properties");

            if (propertiesNode == null) return;

            foreach (XmlNode propertyNode in propertiesNode.SelectNodes("property"))
            {
                string name = propertyNode.Attributes["name"].Value;
                string value = propertyNode.Attributes["value"].Value;

                SetProperty(name, value);
            }
        }
    }
}
