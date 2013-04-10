using GameEngine;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;

namespace Some2DRPG.GameObjects
{
    public class Chest : Entity
    {
        public Chest(float X, float Y)
            :base(X,Y)
        {
        }

        public override void Update(GameTime GameTime, TeeEngine Engine)
        {
            Hero player = (Hero) Engine.GetEntity("Player");
            KeyboardState keyboardState = Keyboard.GetState();

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.S, true)
                && this.CurrentBoundingBox.Intersects(player.CurrentBoundingBox))
            {
                if (CurrentDrawableState != "Open")
                {
                    Random random = new Random();

                    CurrentDrawableState = "Open";
                    Drawables.ResetState("Open", GameTime);

                    for (int i = 0; i < 10; i++)
                    {
                        Coin coin = new Coin(this.Pos.X, this.Pos.Y, 100, (CoinType)random.Next(3));
                        coin.Pos.X+= (float) ((random.NextDouble() - 0.5) * 100);
                        coin.Pos.Y += (float)((random.NextDouble() - 0.5) * 100);

                        Engine.AddEntity(coin);
                    }
                }
            }
        }

        public override void LoadContent(ContentManager Content)
        {
            Animation.LoadAnimationXML(Drawables, "Animations/Misc/chests.anim", Content);
            CurrentDrawableState = "Closed";
        }
    }
}
