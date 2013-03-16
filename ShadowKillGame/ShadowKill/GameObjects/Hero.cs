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

        //TODO REMOVE
        private bool Contains(string[] array, string item)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == item) return true;

            return false;
        }

        public override void Update(GameTime gameTime, TiledMap Map)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            //Change the radius overtime using a SINE wave pattern
            LightRadiusX = (float) (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3));
            LightRadiusY = (float) (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3));

            float prevX = X;
            float prevY = Y;
            Tile prevTile = Map.GetTopMostTile((int) X, (int)Y);
            float moveSpeedModifier = prevTile.GetProperty<float>("MoveSpeed", 1.0f);

            if (gameTime.TotalGameTime.TotalMilliseconds - PrevGameTime > INPUT_DELAY)
            {
                bool moved = false;

                if (keyboardState.IsKeyDown(Keys.A))
                {
                    bool reset = !CurrentDrawable.StartsWith("Slash");
                        
                    CurrentDrawable = "Slash_" + Direction;
                    moved = true;

                    ///if (reset) Animations.ResetAnimations(CurrentAnimation, gameTime);
                }

                //MOVEMENT BASED KEYBOARD EVENTS
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    CurrentDrawable = "Walk_Up";
                    Direction = Direction.Up;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Y -= MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    CurrentDrawable = "Walk_Down";
                    Direction = Direction.Down;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Y += MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    CurrentDrawable = "Walk_Left";
                    Direction = Direction.Left;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    X -= MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    CurrentDrawable = "Walk_Right";
                    Direction = Direction.Right;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    X += MOVEMENT_SPEED * moveSpeedModifier;
                }

                //Set animation to idle of no movements where made
                if (moved == false)
                    CurrentDrawable = "Idle_" + Direction;


                //iterate through each layer and determine if the tile is passable
                int tileX = (int)X;
                int tileY = (int)Y;

                Tile currentTile = Map.GetTopMostTile(tileX, tileY);
                bool impassable = currentTile.HasProperty("Impassable");

                //CORRECT ENTRY AND EXIT MOVEMENT BASED ON TILE PROPERTIES
                //TODO
                //to improve structure
                //Current very very ineffecient way of checking Entry
                string[] entryPoints = currentTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                string[] exitPoints = prevTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

                bool top = prevY < tileY;
                bool bottom = prevY > tileY + 1;
                bool left = prevX < tileX;
                bool right = prevX > tileX + 1;

                //ensure entry points
                impassable |= top && !Contains(entryPoints, "Top");
                impassable |= bottom && !Contains(entryPoints, "Bottom");
                impassable |= left && !Contains(entryPoints, "Left");
                impassable |= right && !Contains(entryPoints, "Right");

                //ensure exit points
                impassable |= top && !Contains(exitPoints, "Bottom");
                impassable |= bottom && !Contains(exitPoints, "Top");
                impassable |= left && !Contains(exitPoints, "Right");
                impassable |= right && !Contains(exitPoints, "Left");

                //IF THE MOVEMENT WAS DEEMED IMPASSABLE, CORRECT IT
                //if impassable, adjust X and Y accordingly
                float padding = 0.00001f;
                if (impassable)
                {
                    if (prevY <= tileY && Y > tileY)
                        Y = tileY - padding;
                    else
                    if (prevY >= tileY + 1 && Y < tileY + 1)
                        Y = tileY + 1 + padding;

                    if (prevX <= tileX && X > tileX)
                        X = tileX - padding;
                    else
                    if (prevX >= tileX + 1 && X < tileX + 1)
                        X = tileX + 1 + padding;
                }

                //prevent from going out of range
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                if (X >= Map.Width - 1) X = Map.Width - 1;
                if (Y >= Map.Height - 1) Y = Map.Height - 1;
            }  
        }
    }
}
