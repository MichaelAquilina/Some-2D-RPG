using System;
using System.Collections.Generic;
using GameEngine;
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

        public bool CollisionDetection { get; set; }

        public BasicLightSource LightSource { get; set; }

        private List<Entity> prevIntersectingEntities;

        public Hero()
            :base(NPC.MALE_HUMAN)
        {
            Construct(0, 0);

            CollisionDetection = true;
            Head = NPC.PLATE_ARMOR_HEAD;
            Legs = NPC.PLATE_ARMOR_LEGS;
            Feet = NPC.PLATE_ARMOR_FEET;
            Shoulders = NPC.PLATE_ARMOR_SHOULDERS;
            Torso = NPC.PLATE_ARMOR_TORSO;
            Hands = NPC.PLATE_ARMOR_HANDS;
            Weapon = NPC.WEAPON_LONGSWORD;
        }

        public Hero(float x, float y) :
            base(x, y, NPC.MALE_HUMAN)
        {
            Construct(x, y);
        }

        private void Construct(float x, float y)
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

        public override void PostInitialize(GameTime gameTime, TeeEngine engine)
        {
            LightShader lightShader = (LightShader) engine.GetPostGameShader("LightShader");
            lightShader.LightSources.Add(LightSource);
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

            Vector2 movement = Vector2.Zero;
            float prevX = Pos.X;
            float prevY = Pos.Y;

            Tile prevTile = Engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);
            float moveSpeedModifier = prevTile.GetProperty<float>("MoveSpeed", 1.0f);

            // ATTACK KEY.
            if (keyboardState.IsKeyDown(Keys.A))
            {
                bool reset = !CurrentDrawableState.StartsWith("Slash");
                CurrentDrawableState = "Slash_" + Direction;

                if (reset) Drawables.ResetState(CurrentDrawableState, gameTime);
            }
            else
            {
                // MOVEMENT BASED KEYBOARD EVENTS.
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    CurrentDrawableState = "Walk_Up";
                    Direction = Direction.Up;

                    movement.Y--;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    CurrentDrawableState = "Walk_Down";
                    Direction = Direction.Down;

                    movement.Y++;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    CurrentDrawableState = "Walk_Left";
                    Direction = Direction.Left;

                    movement.X--;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    CurrentDrawableState = "Walk_Right";
                    Direction = Direction.Right;

                    movement.X++;
                }

                // Set animation to idle of no movements where made.
                if (movement.Length() == 0)
                    CurrentDrawableState = "Idle_" + Direction;
                else
                {
                    movement.Normalize();
                    Pos += movement * MOVEMENT_SPEED * moveSpeedModifier;
                }
            }

            // Prevent from going out of range.
            if (Pos.X < 0) Pos.X = 0;
            if (Pos.Y < 0) Pos.Y = 0;
            if (Pos.X >= Engine.Map.pxWidth - 1) Pos.X = Engine.Map.pxWidth - 1;
            if (Pos.Y >= Engine.Map.pxHeight - 1) Pos.Y = Engine.Map.pxHeight - 1;

            if (CollisionDetection)
            {
                // Iterate through each layer and determine if the tile is passable.
                int tileX = (int) Pos.X / Engine.Map.TileWidth;
                int tileY = (int) Pos.Y / Engine.Map.TileHeight;

                int pxTileX = tileX * Engine.Map.TileWidth;
                int pxTileY = tileY * Engine.Map.TileHeight;
                int pxTileWidth = Engine.Map.TileWidth;
                int pxTileHeight = Engine.Map.TileHeight;

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

            prevIntersectingEntities = Engine.QuadTree.GetIntersectingEntites(this.CurrentBoundingBox);
            foreach (Entity entity in prevIntersectingEntities)
            {
                // TODO: Should be more general than just a Bat.
                // In the future it should be something along the lines of if NPC.IsEnemy()
                if (entity is Bat)
                {
                    if (CurrentDrawableState.Contains("Slash") &&
                        Entity.IntersectsWith(this, "Weapon", entity, "Body", gameTime))
                    {
                        Bat bat = (Bat)entity;
                        bat.HP -= 10;
                    }
                }
            }
        }
    }
}
