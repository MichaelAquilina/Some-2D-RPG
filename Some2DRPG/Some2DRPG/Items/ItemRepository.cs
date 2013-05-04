using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.Drawing;
using GameEngine.Extensions;
using Microsoft.Xna.Framework.Content;

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
                Item item = new Item();
                item.Name = XmlExtensions.GetAttributeValue(itemNode, "Name", null, true);

                item.FriendlyName = itemNode.SelectSingleNode("FriendlyName").InnerText;
                item.Description = itemNode.SelectSingleNode("Description").InnerText;
                item.Weight = XmlExtensions.GetAttributeValue<float>(itemNode, "Weight", 0);
                item.ItemType = (ItemType)Enum.Parse(typeof(ItemType), XmlExtensions.GetAttributeValue(itemNode, "ItemType", null, true));

                foreach(XmlNode drawableSetNode in itemNode.SelectNodes("Drawables/Drawable"))
                {
                    string src = XmlExtensions.GetAttributeValue(drawableSetNode, "src");
                    DrawableSet.LoadDrawableSetXml(item.Drawables, src, content);
                }

                GameItems.Add(item.Name, item);
            }
        }
    }
}
