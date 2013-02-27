using System;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using ShadowKill.Shaders;

namespace ShadowKill.GameObjects
{
    public class Hero : NPC, ILightSource
    {
        const int INPUT_DELAY = 30;
        const float MOVEMENT_SPEED = 0.2f;
        double PrevGameTime = 0;

        public float LightX { get { return X; } }
        public float LightY { get { return Y; } }

        public LightPositionType PositionType { get { return Shaders.LightPositionType.Relative; } }
        public Color LightColor { get; set; }
        public float LightRadiusX { get; set; }
        public float LightRadiusY { get; set; }

        public Hero(float X, float Y) :
            base(X, Y, NPC.MALE_HUMAN)
        {
            Direction = Direction.Right;
            LightRadiusX = 8.0f;
            LightRadiusY = 8.0f;
            LightColor = Color.White;
        }

        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
        }

        public override void Update(GameTime gameTime, TiledMap Map)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            //Change the radius overtime using a SINE wave pattern
            LightRadiusX = (float) (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3));
            LightRadiusY = (float) (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3));

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

                //prevent from walking on impassable terrain
                Tile tile = Map.GetTile((int)X, (int)Y, 0);
                if (tile.Properties.ContainsKey("Impassable"))
                    LightColor = Color.Red;
                else
                    LightColor = Color.White;

                //prevent from going out of range
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                if (X >= Map.Width - 1) X = Map.Width - 1;
                if (Y >= Map.Height - 1) Y = Map.Height - 1;
            }  
        }
    }
}
