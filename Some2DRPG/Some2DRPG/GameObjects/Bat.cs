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

        private const double AGRO_DISTANCE = 200;
        private double _randomModifier;

        private float _moveSpeed = 1.5f;

        public Bat(int PX, int PY)
            :base(PX, PY)
        {
            this._randomModifier = randomGenerator.NextDouble();
            this.Visible = true;
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            // Store previous position for latter calculations.
            Vector2 prevPos = Pos;
            Hero player = (Hero) engine.GetEntity("Player");

            double distance = Vector2.Distance(player.Pos, this.Pos);

            if (distance < AGRO_DISTANCE)
            {
                // Move towards the player for an attack move.
                double angle = Math.Atan2(
                    player.Pos.Y - this.Pos.Y, 
                    player.Pos.X - this.Pos.X
                    );

                Pos.X += (float) (Math.Cos(angle) * _moveSpeed);
                Pos.Y += (float) (Math.Sin(angle) * _moveSpeed);
            }
            else
            {
                // Perform a standard patrol action.
                Pos.X += (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds - _randomModifier * 90) * 2);
            }

            // Determine the animation based on the change in position.
            if (Math.Abs(prevPos.X - Pos.X) > Math.Abs(prevPos.Y - Pos.Y))
            {
                if (prevPos.X < Pos.X)
                    this.CurrentDrawableState = "Right";
                if (prevPos.X > Pos.X)
                    this.CurrentDrawableState = "Left";
            }
            else
            {
                if (prevPos.Y < Pos.Y)
                    this.CurrentDrawableState = "Down";
                if (prevPos.Y > Pos.Y)
                    this.CurrentDrawableState = "Up";
            }
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
