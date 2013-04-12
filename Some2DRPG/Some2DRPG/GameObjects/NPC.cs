using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;

namespace Some2DRPG.GameObjects
{
    public enum Direction { Left, Right, Up, Down };

    public class NPC : Entity
    {
        public const string MALE_HUMAN = @"Animations/Characters/male_npc.anim";
        public const string FEMALE_HUMAN = @"Animations/Characters/female_npc.anim";

        public const string PLATE_ARMOR_LEGS = @"Animations/Plate Armor/plate_armor_legs.anim";
        public const string PLATE_ARMOR_TORSO = @"Animations/Plate Armor/plate_armor_torso.anim";
        public const string PLATE_ARMOR_FEET = @"Animations/Plate Armor/plate_armor_feet.anim";
        public const string PLATE_ARMOR_HANDS = @"Animations/Plate Armor/plate_armor_hands.anim";
        public const string PLATE_ARMOR_HEAD = @"Animations/Plate Armor/plate_armor_head.anim";
        public const string PLATE_ARMOR_SHOULDERS = @"Animations/Plate Armor/plate_armor_shoulders.anim";

        public const string WEAPON_DAGGER = @"Animations/Weapons/dagger.anim";
        public const string WEAPON_LONGSWORD = @"Animations/Weapons/longsword.anim";

        public Direction Direction { get; set; }

        public string Weapon { get; set; }
        public string Head { get; set; }
        public string Torso { get; set; }
        public string Feet { get; set; }
        public string Shoulders { get; set; }
        public string Legs { get; set; }
        public string Hands { get; set; }
        public string BaseRace { get; set; }

        public int HP { get; set; }
        public int XP { get; set; }
        public int Coins { get; set; }

        public NPC(float x, float y, string baseRace) :
            base(x, y, 1.0f, 1.0f)
        {
            this.HP = 0;
            this.XP = 0;
            this.Coins = 0;

            this.Direction = Direction.Right;
            this.BaseRace = baseRace;
        }

        public override void LoadContent(ContentManager content)
        {
            if (Shoulders != null ) Animation.LoadAnimationXML(Drawables, Shoulders, content, 6);
            if (Head != null) Animation.LoadAnimationXML(Drawables, Head, content, 5);
            if (Hands != null) Animation.LoadAnimationXML(Drawables, Hands, content, 4);
            if (Feet != null) Animation.LoadAnimationXML(Drawables, Feet, content, 3);
            if (Torso != null) Animation.LoadAnimationXML(Drawables, Torso, content, 2);
            if (Legs != null) Animation.LoadAnimationXML(Drawables, Legs, content, 1);
            if (Weapon != null) Animation.LoadAnimationXML(Drawables, Weapon, content, 7);
            Animation.LoadAnimationXML(Drawables, BaseRace, content, 0);

            CurrentDrawableState = "Walk_Right";
        }

        public override void UnloadContent()
        {

        }
    }
}
