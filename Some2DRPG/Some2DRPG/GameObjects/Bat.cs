using System;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Some2DRPG.GameObjects
{
    public class Bat : Entity
    {
        private static Random randomGenerator = new Random();

        private double _randomModifier;

        public Bat(int PX, int PY)
            :base(PX, PY)
        {
            this._randomModifier = randomGenerator.NextDouble();
            this.Visible = true;
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            Vector2 prevPos = Pos;

            Pos.X += (float) (Math.Cos(gameTime.TotalGameTime.TotalSeconds - _randomModifier*90) * 2);

            // Determine the animation based on the change in position
            if (prevPos.X < Pos.X)
                this.CurrentDrawableState = "Right";
            if (prevPos.X > Pos.X)
                this.CurrentDrawableState = "Left";
            if (prevPos.Y < Pos.Y)
                this.CurrentDrawableState = "Down";
            if (prevPos.Y > Pos.Y)
                this.CurrentDrawableState = "Up";
        }

        public override void LoadContent(ContentManager content)
        {
            double startTimeMS = randomGenerator.NextDouble() * 4000;

            Animation.LoadAnimationXML(
                Drawables, 
                "Animations/Monsters/bat.anim", 
                content, startTimeMS
                );

            CurrentDrawableState = "Left";
        }
    }
}
