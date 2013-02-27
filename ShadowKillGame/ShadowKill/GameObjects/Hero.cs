using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShadowKill.Shaders;
using GameEngine.Tiled;
using System;

namespace ShadowKill.GameObjects
{
    public class Hero : NPC, ILightSource
    {
        const int INPUT_DELAY = 30;
        const float MOVEMENT_SPEED = 0.2f;
        double PrevGameTime = 0;

        public Color LightColor { get { return Color.White ; } }
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        public Hero(float X, float Y) :
            base(X, Y, NPC.MALE_HUMAN)
        {
            Direction = Direction.Right;
            RadiusX = 8.0f;
            RadiusY = 8.0f;
        }

        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
        }

        public override void Update(GameTime gameTime, TiledMap Map)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            RadiusX = (float) (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3));
            RadiusY = (float)(8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3));

            if (gameTime.TotalGameTime.TotalMilliseconds - PrevGameTime > INPUT_DELAY)
            {
                bool moved = false;

                if (keyboardState.IsKeyDown(Keys.A))
                {
                    CurrentAnimation = "Slash_" + Direction;
                    moved = true;
                    return;
                }

                //MOVEMENT BASED KEYBOARD EVENTS
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    CurrentAnimation = "Walk_Up";
                    Direction = Direction.Up;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Y -= MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    CurrentAnimation = "Walk_Down";
                    Direction = Direction.Down;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Y += MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    CurrentAnimation = "Walk_Left";
                    Direction = Direction.Left;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    X -= MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    CurrentAnimation = "Walk_Right";
                    Direction = Direction.Right;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    X += MOVEMENT_SPEED;
                }

                if (moved == false)
                    CurrentAnimation = "Idle_" + Direction;

                //prevent from going out of range
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                if (X >= Map.Width - 1) X = Map.Width - 1;
                if (Y >= Map.Height - 1) Y = Map.Height - 1;
            }  
        }
    }
}
