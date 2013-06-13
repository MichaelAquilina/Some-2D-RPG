
namespace Some2DRPG.Items
{
    public enum ArmorSlot { HeadGear, Vest, Gloves, Pants, Boots }

    public class Armor : Item
    {
        public ArmorSlot ArmorSlot { get; set; }
    }
}
