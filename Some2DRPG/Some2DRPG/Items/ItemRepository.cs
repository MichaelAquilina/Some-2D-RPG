using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Drawing;
using Microsoft.Xna.Framework.Content;

namespace Some2DRPG.Items
{
    public class ItemRepository
    {
        public static Dictionary<string, Item> GameItems = new Dictionary<string, Item>();

        public static void LoadItems(ContentManager content)
        {
            // Naming convention for each item should be as follows:
            //       <MaterialType><ItemType>

            // PLATE HELMET

            Item plateHelmet = new Item();
            plateHelmet.Name = "PlateHelmet";
            plateHelmet.FriendlyName = "Platemail Helmet";
            plateHelmet.Description = "A standard Imperial Plate Mail Helmet used by all Imperial Guards in the realm";
            plateHelmet.Weight = 1.5f;
            plateHelmet.ItemType = ItemType.HeadGear;

            Animation.LoadAnimationXML(plateHelmet.Drawables, "Animations/Plate Armor/plate_armor_head.anim", content);

            GameItems.Add(plateHelmet.Name, plateHelmet);

            // PLATE CHEST ARMOR

            Item plateChest = new Item();
            plateChest.Name = "PlateChest";
            plateChest.FriendlyName = "Platemail Chest Armor";
            plateChest.Description = "A standard Imperial Plate Mail Chest Armor used by all Imperial Guards in the realm";
            plateChest.Weight = 10.5f;
            plateChest.ItemType = ItemType.Vest;

            Animation.LoadAnimationXML(plateChest.Drawables, "Animations/Plate Armor/plate_armor_torso.anim", content);
            Animation.LoadAnimationXML(plateChest.Drawables, "Animations/Plate Armor/plate_armor_shoulders.anim", content);

            GameItems.Add(plateChest.Name, plateChest);
        }
    }
}
