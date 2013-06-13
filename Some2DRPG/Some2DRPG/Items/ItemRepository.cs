using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.Drawing;
using GameEngine.Extensions;
using Microsoft.Xna.Framework.Content;
using System.Reflection;

namespace Some2DRPG.Items
{
    public class ItemRepository
    {
        public static Dictionary<string, Item> GameItems = new Dictionary<string, Item>();

        public static void LoadRepositoryXml(string filePath, ContentManager content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filePath);

            foreach (XmlNode itemNode in document.SelectNodes("ItemRepository/Item"))
            {
                Item item = null;
                string itemClass = XmlExtensions.GetAttributeValue(itemNode, "Class", null);

                // Use the specified Item class if found.
                if (itemClass != null)
                    item = (Item)Assembly.GetExecutingAssembly().CreateInstance(itemClass);
                else
                    item = new Item();


                // Load set of standard Item properties.
                item.Name = XmlExtensions.GetAttributeValue(itemNode, "Name", null, true);
                item.FriendlyName = itemNode.SelectSingleNode("FriendlyName").InnerText;
                item.Description = itemNode.SelectSingleNode("Description").InnerText;
                item.Weight = XmlExtensions.GetAttributeValue<float>(itemNode, "Weight", 0);

                // Load associated drawable files.
                foreach(XmlNode drawableSetNode in itemNode.SelectNodes("Drawables/Drawable"))
                {
                    string src = XmlExtensions.GetAttributeValue(drawableSetNode, "src");
                    DrawableSet.LoadDrawableSetXml(item.Drawables, src, content);
                }

                // Load Properties specified in the XML file.
                XmlNode rootPropertyNode = itemNode.SelectSingleNode("Properties");
                if (rootPropertyNode != null)
                {
                    foreach (XmlNode propertyNode in rootPropertyNode.ChildNodes)
                        ReflectionExtensions.SmartSetProperty(item, propertyNode.Name, propertyNode.InnerText);
                }

                GameItems.Add(item.Name, item);
            }
        }
    }
}
