using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Drawing;
using ShadowKill.Shaders;
using Microsoft.Xna.Framework.Input;
using GameEngine.Helpers;

namespace ShadowKill.GameObjects
{
    public enum Direction { Left, Right, Up, Down };

    public class NPC : Entity
    {
        public const string MALE_HUMAN = @"Animations/male_npc.anim";
        public const string FEMALE_HUMAN = @"Animations/female_npc.anim";

        public const string PLATE_ARMOR_LEGS = @"Animations/Plate Armor/plate_armor_legs_walkcycle.anim";
        public const string PLATE_ARMOR_TORSO = @"Animations/Plate Armor/plate_armor_torso_walkcycle.anim";
        public const string PLATE_ARMOR_FEET = @"Animations/Plate Armor/plate_armor_feet_walkcycle.anim";
        public const string PLATE_ARMOR_HANDS = @"Animations/Plate Armor/plate_armor_hands_walkcycle.anim";
        public const string PLATE_ARMOR_HEAD = @"Animations/Plate Armor/plate_armor_head_walkcycle.anim";
        public const string PLATE_ARMOR_SHOULDERS = @"Animations/Plate Armor/plate_armor_shoulders_walkcycle.anim";

        public Direction Direction { get; set; }

        public string Head { get; set; }
        public string Torso { get; set; }
        public string Feet { get; set; }
        public string Shoulders { get; set; }
        public string Legs { get; set; }
        public string Hands { get; set; }
        public string Race { get; set; }

        public NPC(float X, float Y, float Width, float Height) :
            base(X, Y, Width, Height)
        {
            Direction = Direction.Right;
        }

        public override void LoadContent(ContentManager Content)
        {
            if(Shoulders != null ) LoadAnimationXML(Shoulders, Content, 6);
            if(Head != null) LoadAnimationXML(Head, Content, 5);
            if(Hands != null) LoadAnimationXML(Hands, Content, 4);
            if(Feet != null ) LoadAnimationXML(Feet, Content, 3);
            if(Torso != null ) LoadAnimationXML(Torso, Content, 2);
            if(Legs != null )LoadAnimationXML(Legs, Content, 1);
            LoadAnimationXML(Race, Content, 0);
            CurrentAnimation = "Walk_Right";
        }

        public override void UnloadContent()
        {

        }
    }
}
