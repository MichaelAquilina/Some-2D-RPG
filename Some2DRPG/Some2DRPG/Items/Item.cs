using GameEngine.Drawing;

namespace Some2DRPG.Items
{
    public enum ItemType { HeadGear, Vest, Gloves, Pants, Boots }

    public class Item
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public float Weight { get; set; }

        public ItemType ItemType { get; set; }

        public DrawableSet Drawables { get; set; }

        public Item()
        {
            Drawables = new DrawableSet();
        }
    }
}
