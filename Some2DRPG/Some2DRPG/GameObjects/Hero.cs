using System;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Some2DRPG.Shaders;
using GameEngine;
using ShadowKillGame.GameObjects;
using GameEngine.GameObjects;
using System.Collections.Generic;

namespace Some2DRPG.GameObjects
{
    public class Hero : NPC
    {
        const int INPUT_DELAY = 0;
        const float MOVEMENT_SPEED = 2.4f;
        double PrevGameTime = 0;

        public float LightX { get { return X; } }
        public float LightY { get { return Y; } }

        public bool CollisionDetection { get; set; }

        public BasicLightSource LightSource { get; set; }

        private List<Entity> prevIntersectingEntities;

        public Hero(int PX, int PY) :
            base(PX, PY, NPC.MALE_HUMAN)
        {
            CollisionDetection = true;
            LightSource = new BasicLightSource();
            LightSource.RadiusX = 32 * 8;
            LightSource.RadiusY = 32 * 8;
            LightSource.Color = Color.White;
            LightSource.PositionType = LightPositionType.Relative;
        }

        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
        }

        //TODO REMOVE
        private bool ContainsItem(string[] array, string item)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == item) return true;

            return false;
        }

        public override void Update(GameTime gameTime, TeeEngine Engine)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            float prevX = X;
            float prevY = Y;

            Tile prevTile = Engine.Map.GetPxTopMostTile(X, Y);
            float moveSpeedModifier = prevTile.GetProperty<float>("MoveSpeed", 1.0f);

            if (gameTime.TotalGameTime.TotalMilliseconds - PrevGameTime > INPUT_DELAY)
            {
                bool moved = false;

                if (keyboardState.IsKeyDown(Keys.A))
                {
                    bool reset = !CurrentDrawableState.StartsWith("Slash");
                        
                    CurrentDrawableState = "Slash_" + Direction;
                    moved = true;

                    ///if (reset) Animations.ResetAnimations(CurrentAnimation, gameTime);
                }

                //MOVEMENT BASED KEYBOARD EVENTS
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    CurrentDrawableState = "Walk_Up";
                    Direction = Direction.Up;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Y -= MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    CurrentDrawableState = "Walk_Down";
                    Direction = Direction.Down;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    Y += MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    CurrentDrawableState = "Walk_Left";
                    Direction = Direction.Left;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    X -= MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    CurrentDrawableState = "Walk_Right";
                    Direction = Direction.Right;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    X += MOVEMENT_SPEED * moveSpeedModifier;
                }

                //Set animation to idle of no movements where made
                if (moved == false)
                    CurrentDrawableState = "Idle_" + Direction;

                //prevent from going out of range
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                if (X >= Engine.Map.pxWidth - 1) X = Engine.Map.pxWidth - 1;
                if (Y >= Engine.Map.pxHeight - 1) Y = Engine.Map.pxHeight - 1;

                if (CollisionDetection)
                {
                    //iterate through each layer and determine if the tile is passable
                    int tileX = (int)X / Engine.Map.pxTileWidth;
                    int tileY = (int)Y / Engine.Map.pxTileHeight;

                    int pxTileX = tileX * Engine.Map.pxTileWidth;
                    int pxTileY = tileY * Engine.Map.pxTileHeight;
                    int pxTileWidth = Engine.Map.pxTileWidth;
                    int pxTileHeight = Engine.Map.pxTileHeight;

                    Tile currentTile = Engine.Map.GetPxTopMostTile(X, Y);
                    bool impassable = currentTile.HasProperty("Impassable");

                    //CORRECT ENTRY AND EXIT MOVEMENT BASED ON TILE PROPERTIES
                    //TODO
                    //to improve structure
                    //Current very very ineffecient way of checking Entry
                    string[] entryPoints = currentTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] exitPoints = prevTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    bool top = prevY < pxTileY;
                    bool bottom = prevY > pxTileY + pxTileHeight;
                    bool left = prevX < pxTileX;
                    bool right = prevX > pxTileX + pxTileWidth;

                    //ensure entry points
                    impassable |= top && !ContainsItem(entryPoints, "Top");
                    impassable |= bottom && !ContainsItem(entryPoints, "Bottom");
                    impassable |= left && !ContainsItem(entryPoints, "Left");
                    impassable |= right && !ContainsItem(entryPoints, "Right");

                    //ensure exit points
                    impassable |= top && !ContainsItem(exitPoints, "Bottom");
                    impassable |= bottom && !ContainsItem(exitPoints, "Top");
                    impassable |= left && !ContainsItem(exitPoints, "Right");
                    impassable |= right && !ContainsItem(exitPoints, "Left");

                    //IF THE MOVEMENT WAS DEEMED IMPASSABLE, CORRECT IT
                    //if impassable, adjust X and Y accordingly
                    float padding = 0.001f;
                    if (impassable)
                    {
                        if (prevY <= pxTileY && Y > pxTileY)
                            Y = pxTileY - padding;
                        else
                            if (prevY >= pxTileY + pxTileHeight && Y < pxTileY + pxTileHeight)
                                Y = pxTileY + pxTileHeight + padding;

                        if (prevX <= pxTileX && X > pxTileX)
                            X = pxTileX - padding;
                        else
                            if (prevX >= pxTileX + pxTileWidth && X < pxTileX + pxTileWidth)
                                X = pxTileX + pxTileWidth + padding;
                    }
                }

                //Change the radius of the LightSource overtime using a SINE wave pattern
                LightSource.PX = this.X;
                LightSource.PY = this.Y;
                LightSource.RadiusX = (float)(32 * (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3)));
                LightSource.RadiusY = (float)(32 * (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3)));

                //EXAMPLE OF HOW THE QUAD TREE INTERSECTING ENTITIES FUNCTION CAN WORK
                //TODO: Add PER PIXEL collision detection to each one of these entities
                if (prevIntersectingEntities != null)
                    foreach (Entity entity in prevIntersectingEntities)
                        entity.Opacity = 1.0f;

                prevIntersectingEntities = Engine.QuadTree.GetIntersectingEntites(this.CurrentBoundingBox);
                foreach (Entity entity in prevIntersectingEntities)
                {
                    if ( entity!= this && 
                         entity.CurrentBoundingBox.Intersects(CurrentBoundingBox) &&
                         entity.Y > this.Y )
                        entity.Opacity = 0.8f;
                }
            }  
        }
    }
}
