﻿using System;
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
            LoadAnimationXML(@"Animations/plate_armor_torso_walkcycle.anim", Content);
            LoadAnimationXML(@"Animations/plate_armor_legs_walkcycle.anim", Content);
            LoadAnimationXML(@"Animations/male_walkcycle.anim", Content);
            LoadAnimationXML(@"Animations/male_slash.anim", Content);
            CurrentAnimationSetName = "Walk_Right";

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
