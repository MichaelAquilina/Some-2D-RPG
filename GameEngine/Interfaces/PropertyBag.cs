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

        public bool GetBooleanProperty(string Name, bool Default = true)
        {
            return HasProperty(Name) ? Convert.ToBoolean(Name) : Default;
        }

        public double GetDoubleProperty(string Name, double Defualt = 0)
        {
            return HasProperty(Name) ? Convert.ToDouble(_properties[Name]) : Defualt;
        }

        public decimal GetDecimalProperty(string Name, decimal Default = 0)
        {
            return HasProperty(Name) ? Convert.ToDecimal(_properties[Name]) : Default;
        }

        public int GetInt32Property(string Name, int Default=0)
        {
            return HasProperty(Name) ? Convert.ToInt32(_properties[Name]) : Default;
        }

        public long GetInt64Property(string Name, int Default = 0)
        {
            return HasProperty(Name) ? Convert.ToInt64(_properties[Name]) : Default;
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
