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

            // PLATE GLOVES

            Item plateGloves = new Item();
            plateGloves.Name = "PlateGloves";
            plateGloves.FriendlyName = "Platemail Gloves";
            plateGloves.Description = "A standard Imperial Plate Mail pair of Gloves used by all Imperial Guards in the realm";
            plateGloves.Weight = 0.5f;
            plateGloves.ItemType = ItemType.Gloves;

            Animation.LoadAnimationXML(plateGloves.Drawables, "Animations/Plate Armor/plate_armor_hands.anim", content);

            GameItems.Add(plateGloves.Name, plateGloves);

            // PLATE PANTS

            Item platePants = new Item();
            platePants.Name = "PlatePants";
            platePants.FriendlyName = "Platemail Pants";
            platePants.Description = "A standard Imperial Plate Mail pair of Pants used by all Imperial Guards in the realm";
            platePants.Weight = 5.5f;
            platePants.ItemType = ItemType.Pants;

            Animation.LoadAnimationXML(platePants.Drawables, "Animations/Plate Armor/plate_armor_legs.anim", content);

            GameItems.Add(platePants.Name, platePants);

            // PLATE BOOTS

            Item plateBoots = new Item();
            plateBoots.Name = "PlateBoots";
            plateBoots.FriendlyName = "Platemail Boots";
            plateBoots.Description = "A standard Imperial Plate Mail pair of Boots used by all Imperial Guards in the realm";
            plateBoots.Weight = 2.5f;
            plateBoots.ItemType = ItemType.Boots;

            Animation.LoadAnimationXML(plateBoots.Drawables, "Animations/Plate Armor/plate_armor_feet.anim", content);

            GameItems.Add(plateBoots.Name, plateBoots);
        }
    }
}
