using System.Collections.Generic;
using System.Xml;
using System;

namespace GameEngine.GameObjects
{
    //Represents an Object that allows for meta-data to be stored in a dictionary
    //Provides support for loading property information from Xml Node objects automatically
    public abstract class PropertyBag
    {
        public ICollection<string> PropertyValues { get { return _properties.Values; } }
        public ICollection<string> PropertyKeys { get { return _properties.Keys; } }

        private Dictionary<string, string> _properties = new Dictionary<string, string>();

        public T GetProperty<T>(string name, T defaultValue)
        {
            return HasProperty(name) ? (T) Convert.ChangeType(_properties[name], typeof(T)) : defaultValue;
        }

        public string GetProperty(string name, string defaultValue=null)
        {
            return HasProperty(name)? _properties[name] : defaultValue;
        }

        public void SetProperty(string name, string value)
        {
            _properties[name] = value;
        }

        public bool HasProperty(string name)
        {
            return _properties.ContainsKey(name);
        }

        /// <summary>
        /// Load properties from the specified XmlNode. An Xml Element under the name "properties"
        /// will have its content loaded automatically using this method. If no properties node is
        /// found in the specified node, then the function will do nothing.
        /// </summary>
        /// <param name="selectedNode">XmlNode to extract the properties from</param>
        public void LoadProperties(XmlNode selectedNode)
        {
            XmlNode propertiesNode = selectedNode.SelectSingleNode("properties");

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
