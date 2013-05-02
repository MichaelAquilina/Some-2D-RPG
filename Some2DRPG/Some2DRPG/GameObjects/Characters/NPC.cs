using GameEngine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Some2DRPG.GameObjects.Characters
{
    public enum Direction { Left, Right, Up, Down };

    public class NPC : CollidableEntity
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

        public NPC()
        {
            Construct();
        }

        public NPC(string baseRace)
        {
            Construct(0, 0, baseRace);
        }

        public NPC(float x, float y, string baseRace)
        {
            Construct(x, y, baseRace);
        }

        private void Construct(float x=0, float y=0, string baseRace=MALE_HUMAN)
        {
            this.HP = 0;
            this.XP = 0;
            this.Coins = 0;
            this.Pos = new Vector2(x, y);
            this.ScaleX = 1.0f;
            this.ScaleY = 1.0f;
            this.CollisionGroup = "Shadow";

            this.Direction = Direction.Right;
            this.BaseRace = baseRace;
        }

        public override void LoadContent(ContentManager content)
        {
            if (Weapon != null) Animation.LoadAnimationXML(Drawables, Weapon, content);
            if (Shoulders != null) Animation.LoadAnimationXML(Drawables, Shoulders, content);
            if (Head != null) Animation.LoadAnimationXML(Drawables, Head, content);
            if (Hands != null) Animation.LoadAnimationXML(Drawables, Hands, content);
            if (Feet != null) Animation.LoadAnimationXML(Drawables, Feet, content);
            if (Torso != null) Animation.LoadAnimationXML(Drawables, Torso, content);
            if (Legs != null) Animation.LoadAnimationXML(Drawables, Legs, content);
            Animation.LoadAnimationXML(Drawables, BaseRace, content);

            CurrentDrawableState = "Idle_Left";
        }
    }
}
