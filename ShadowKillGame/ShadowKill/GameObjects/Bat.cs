using System;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShadowKill.GameObjects
{
    public class Bat : Entity
    {
        private static Random randomGenerator = new Random();

        private double _randomModifier;

        public Bat(float TX, float TY)
            :base(TX, TY)
        {
            this._randomModifier = randomGenerator.NextDouble();
            this.Origin = new Vector2(1.0f, 0.5f);
            this.Visible = true;
            this.rxWidth = 1.5f;
            this.rxHeight = 1.5f;
        }

        public override void Update(GameTime GameTime, TeeEngine Engine)
        {
            TX += (float)Math.Cos(GameTime.TotalGameTime.TotalSeconds - _randomModifier*90) / 20;

            //for testing
            Engine.QuadTree.GetIntersectingEntites(CurrentPxBoundingBox);
        }

        public override void LoadContent(ContentManager Content)
        {
            Animation.LoadAnimationXML(Drawables, "Animations/Monsters/bat.anim", Content, null, 0);
            CurrentDrawable = "Down";
        }
    }
}
