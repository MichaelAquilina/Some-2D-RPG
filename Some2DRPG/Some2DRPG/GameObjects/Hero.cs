using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using ShadowKillGame.GameObjects;
using Some2DRPG.Shaders;

namespace Some2DRPG.GameObjects
{
    public class Hero : NPC
    {
        const int INPUT_DELAY = 0;
        const float MOVEMENT_SPEED = 2.9f;
        double PrevGameTime = 0;

        public bool CollisionDetection { get; set; }

        public BasicLightSource LightSource { get; set; }

        private List<Entity> prevIntersectingEntities;

        public Hero(float x, float y) :
            base(x, y, NPC.MALE_HUMAN)
        {
            HP = 2000;
            XP = 0;

            CollisionDetection = true;
            LightSource = new BasicLightSource();
            LightSource.RadiusX = 32 * 8;
            LightSource.RadiusY = 32 * 8;
            LightSource.Color = Color.White;
            LightSource.PositionType = LightPositionType.Relative;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
        }

        // TODO REMOVE.
        private bool ContainsItem(string[] array, string item)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == item) return true;

            return false;
        }

        public override void Update(GameTime gameTime, TeeEngine Engine)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            float prevX = Pos.X;
            float prevY = Pos.Y;

            Tile prevTile = Engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);
            float moveSpeedModifier = prevTile.GetProperty<float>("MoveSpeed", 1.0f);

            if (gameTime.TotalGameTime.TotalMilliseconds - PrevGameTime > INPUT_DELAY)
            {
                bool moved = false;

                // ATTACK KEY.
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    bool reset = !CurrentDrawableState.StartsWith("Slash");

                    CurrentDrawableState = "Slash_" + Direction;
                    moved = true;

                    if (reset) Drawables.ResetState(CurrentDrawableState, gameTime);
                }

                // MOVEMENT BASED KEYBOARD EVENTS.
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    CurrentDrawableState = "Walk_Up";
                    Direction = Direction.Up;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Pos.Y -= MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    CurrentDrawableState = "Walk_Down";
                    Direction = Direction.Down;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Pos.Y += MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    CurrentDrawableState = "Walk_Left";
                    Direction = Direction.Left;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Pos.X -= MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    CurrentDrawableState = "Walk_Right";
                    Direction = Direction.Right;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Pos.X += MOVEMENT_SPEED * moveSpeedModifier;
                }

                // Set animation to idle of no movements where made.
                if (moved == false)
                    CurrentDrawableState = "Idle_" + Direction;

                // Prevent from going out of range.
                if (Pos.X < 0) Pos.X = 0;
                if (Pos.Y < 0) Pos.Y = 0;
                if (Pos.X >= Engine.Map.pxWidth - 1) Pos.X = Engine.Map.pxWidth - 1;
                if (Pos.Y >= Engine.Map.pxHeight - 1) Pos.Y = Engine.Map.pxHeight - 1;

                if (CollisionDetection)
                {
                    // Iterate through each layer and determine if the tile is passable.
                    int tileX = (int) Pos.X / Engine.Map.pxTileWidth;
                    int tileY = (int) Pos.Y / Engine.Map.pxTileHeight;

                    int pxTileX = tileX * Engine.Map.pxTileWidth;
                    int pxTileY = tileY * Engine.Map.pxTileHeight;
                    int pxTileWidth = Engine.Map.pxTileWidth;
                    int pxTileHeight = Engine.Map.pxTileHeight;

                    Tile currentTile = Engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);
                    bool impassable = currentTile.HasProperty("Impassable");

                    // CORRECT ENTRY AND EXIT MOVEMENT BASED ON TILE PROPERTIES
                    // TODO
                    // to improve structure
                    // Current very very ineffecient way of checking Entry
                    string[] entryPoints = currentTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] exitPoints = prevTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    bool top = prevY < pxTileY;
                    bool bottom = prevY > pxTileY + pxTileHeight;
                    bool left = prevX < pxTileX;
                    bool right = prevX > pxTileX + pxTileWidth;

                    // Ensure entry points.
                    impassable |= top && !ContainsItem(entryPoints, "Top");
                    impassable |= bottom && !ContainsItem(entryPoints, "Bottom");
                    impassable |= left && !ContainsItem(entryPoints, "Left");
                    impassable |= right && !ContainsItem(entryPoints, "Right");

                    // Ensure exit points.
                    impassable |= top && !ContainsItem(exitPoints, "Bottom");
                    impassable |= bottom && !ContainsItem(exitPoints, "Top");
                    impassable |= left && !ContainsItem(exitPoints, "Right");
                    impassable |= right && !ContainsItem(exitPoints, "Left");

                    // IF THE MOVEMENT WAS DEEMED IMPASSABLE, CORRECT IT.
                    // if impassable, adjust X and Y accordingly.
                    float padding = 0.001f;
                    if (impassable)
                    {
                        if (prevY <= pxTileY && Pos.Y > pxTileY)
                            Pos.Y = pxTileY - padding;
                        else
                            if (prevY >= pxTileY + pxTileHeight && Pos.Y < pxTileY + pxTileHeight)
                                Pos.Y = pxTileY + pxTileHeight + padding;

                        if (prevX <= pxTileX && Pos.X > pxTileX)
                            Pos.X = pxTileX - padding;
                        else
                            if (prevX >= pxTileX + pxTileWidth && Pos.X < pxTileX + pxTileWidth)
                                Pos.X = pxTileX + pxTileWidth + padding;
                    }
                }

                // Change the radius of the LightSource overtime using a SINE wave pattern.
                LightSource.PX = Pos.X;
                LightSource.PY = Pos.Y;
                LightSource.RadiusX = (float)(32 * (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3)));
                LightSource.RadiusY = (float)(32 * (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3)));

                // EXAMPLE OF HOW THE QUAD TREE INTERSECTING ENTITIES FUNCTION CAN WORK
                // TODO: Add PER PIXEL collision detection to each one of these entities
                //if (prevIntersectingEntities != null)
                //    foreach (Entity entity in prevIntersectingEntities)
                //        entity.Opacity = 1.0f;

                prevIntersectingEntities = Engine.QuadTree.GetIntersectingEntites(this.CurrentBoundingBox);
                foreach (Entity entity in prevIntersectingEntities)
                {
                    if (entity is Bat)
                    {
                        if (CurrentDrawableState.Contains("Slash") &&
                            this.IntersectsWith(entity, gameTime, "Weapon", "Body"))
                        {
                            Bat bat = (Bat)entity;
                            bat.HP -= 10;
                        }
                    }
                    //else
                    //if ( entity!= this && 
                    //     entity.CurrentBoundingBox.Intersects(CurrentBoundingBox) &&
                    //     entity.Pos.Y > this.Pos.Y )
                    //    entity.Opacity = 0.8f;
                }
            }  
        }
    }
}
