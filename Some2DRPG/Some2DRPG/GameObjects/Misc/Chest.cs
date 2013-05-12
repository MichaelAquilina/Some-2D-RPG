using System;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Some2DRPG.GameObjects.Characters;

namespace Some2DRPG.GameObjects.Misc
{
    public class Chest : CollidableEntity
    {
        SoundEffect[] _openSfx;

        public Chest()
        {
            Construct(0, 0);
        }

        public Chest(float x, float y)
        {
            Construct(x, y);
        }

        void Construct(float x, float y)
        {
            this.Pos = new Vector2(x, y);
            this.CollisionGroup = "collision";
            this.Immovable = true;
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            Hero player = (Hero) engine.GetEntity("Player");
            KeyboardState keyboardState = Keyboard.GetState();

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.S, this, true)
                && Entity.IntersectsWith(this, "interaction", player, "Shadow", gameTime))
            {
                if (CurrentDrawableState != "Open")
                {
                    Random random = new Random();

                    _openSfx[random.Next(1)].Play();

                    CurrentDrawableState = "Open";
                    Drawables.ResetState("Open", gameTime);

                    for (int i = 0; i < 10; i++)
                    {
                        Coin coin = new Coin(this.Pos.X, this.Pos.Y, 100, (CoinType)random.Next(3));
                        coin.Pos.X += (float) ((random.NextDouble() - 0.5) * 100);
                        coin.Pos.Y += (float) ((random.NextDouble() - 0.5) * 100);

                        engine.AddEntity(coin);
                    }
                }
            }
        }

        public override void LoadContent(ContentManager content)
        {
            DrawableSet.LoadDrawableSetXml(Drawables, "Animations/Misc/chests.anim", content);
            CurrentDrawableState = "Closed";

            _openSfx = new SoundEffect[2];
            _openSfx[0] = content.Load<SoundEffect>("Sounds/Metallic/metal_button_press1");
            _openSfx[1] = content.Load<SoundEffect>("Sounds/Metallic/metal_button_press2");
        }
    }
}
