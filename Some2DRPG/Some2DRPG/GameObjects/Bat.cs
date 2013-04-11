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
            Pos.X += (float) (Math.Cos(gameTime.TotalGameTime.TotalSeconds - _randomModifier*90) * 2);

            //for testing (performance)
            engine.QuadTree.GetIntersectingEntites(CurrentBoundingBox);
        }

        public override void LoadContent(ContentManager content)
        {
            Animation.LoadAnimationXML(
                Drawables, 
                "Animations/Monsters/bat.anim", 
                content, "", 0,
                randomGenerator.NextDouble() * 4000
                );
            CurrentDrawableState = "Down";
        }
    }
}
