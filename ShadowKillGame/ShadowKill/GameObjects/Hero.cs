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

namespace ShadowKill.GameObjects
{
    public enum Direction { Left, Right, Up, Down };

    public class Hero : Entity, ILoadable, ILightSource
    {
        private Texture2D _lightSource;

        public Direction Direction { get; set; }

        public Hero(float X, float Y) :
            base(X, Y, 1.5f, 1.5f)
        {
            Direction = Direction.Right;
        }

        public void LoadContent(ContentManager Content)
        {
            //TODO: Currently depth depends on the order of these files, should be changed!!!
            LoadAnimationXML(@"Animations/Plate Armor/plate_armor_shoulders_walkcycle.anim", Content, 6);
            LoadAnimationXML(@"Animations/Plate Armor/plate_armor_head_walkcycle.anim", Content, 5);
            LoadAnimationXML(@"Animations/Plate Armor/plate_armor_hands_walkcycle.anim", Content, 4);
            LoadAnimationXML(@"Animations/Plate Armor/plate_armor_feet_walkcycle.anim", Content, 3);
            LoadAnimationXML(@"Animations/Plate Armor/plate_armor_torso_walkcycle.anim", Content, 2);
            LoadAnimationXML(@"Animations/Plate Armor/plate_armor_legs_walkcycle.anim", Content, 1);
            LoadAnimationXML(@"Animations/male_npc.anim", Content, 0);
            CurrentAnimation = "Walk_Right";

            //TODO: Import Soft Shadow prototype when done
            _lightSource = Content.Load<Texture2D>(@"MapObjects/LightSource");
        }

        public void UnloadContent()
        {
            
        }

        public Texture2D GetLightSourceTexture(GameTime gameTime)
        {
            return _lightSource;
        }

        public Rectangle? GetLightSourceRectangle(GameTime gameTime)
        {
            return null;
        }

        public FRectangle GetRelativeDestRectangle(GameTime gameTime)
        {
            return new FRectangle(X - 3, Y - 3, 6, 6);
        }

        public Color GetLightColor(GameTime gameTime)
        {
            return Color.White;
        }
    }
}
