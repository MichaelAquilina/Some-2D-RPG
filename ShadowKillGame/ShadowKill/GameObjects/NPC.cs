using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace ShadowKill.GameObjects
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

        public Direction Direction { get; set; }

        public string Weapon { get; set; }
        public string Head { get; set; }
        public string Torso { get; set; }
        public string Feet { get; set; }
        public string Shoulders { get; set; }
        public string Legs { get; set; }
        public string Hands { get; set; }
        public string BaseRace { get; set; }

        public NPC(float X, float Y, string BaseRace) :
            base(X, Y, 1.5f, 1.5f)
        {
            this.Direction = Direction.Right;
            this.BaseRace = BaseRace;
            this.Origin = new Vector2(0.5f, 1.0f);
        }

        public override void LoadContent(ContentManager Content)
        {
            if(Shoulders != null ) LoadAnimationXML(Shoulders, Content, 6);
            if(Head != null) LoadAnimationXML(Head, Content, 5);
            if(Hands != null) LoadAnimationXML(Hands, Content, 4);
            if(Feet != null ) LoadAnimationXML(Feet, Content, 3);
            if(Torso != null ) LoadAnimationXML(Torso, Content, 2);
            if(Legs != null )LoadAnimationXML(Legs, Content, 1);
            if (Weapon != null) LoadAnimationXML(Weapon, Content, 7);
            LoadAnimationXML(BaseRace, Content, 0);
            CurrentAnimation = "Walk_Right";
        }

        public override void UnloadContent()
        {

        }
    }
}
