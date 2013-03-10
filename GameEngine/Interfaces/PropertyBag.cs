using System.Collections.Generic;
using System.Xml;

namespace GameEngine.Interfaces
{
    //Represents an Object that allows for meta-data to be stored in a dictionary
    //Provides support for loading property information from Xml Node objects automatically
    public abstract class PropertyBag
    {
        private Dictionary<string, string> _properties = new Dictionary<string, string>();

        public string GetProperty(string Name)
        {
            return _properties[Name];
        }

        public void SetProperty(string Name, string Value)
        {
            _properties[Name] = Value;
        }

        public bool HasProperty(string Name)
        {
            return _properties.ContainsKey(Name);
        }

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
