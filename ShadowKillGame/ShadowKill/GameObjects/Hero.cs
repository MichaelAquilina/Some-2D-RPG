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
    public class Hero : Actor, ILoadable, ILightSource
    {
        private Texture2D _lightSource;

        public Hero(float X, float Y) :
            base(X, Y, 1.5f, 1.5f)
        {
        }

        public void LoadContent(ContentManager Content)
        {
            this.LoadAnimationXML(@"Animations/Knuckles.anim", Content);
            SetCurrentAnimation("Idle");

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
