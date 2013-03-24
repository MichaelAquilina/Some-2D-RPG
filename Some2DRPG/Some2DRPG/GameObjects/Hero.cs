using System;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using ShadowKill.Shaders;
using GameEngine;
using ShadowKillGame.GameObjects;
using GameEngine.GameObjects;
using System.Collections.Generic;

namespace ShadowKill.GameObjects
{
    public class Hero : NPC
    {
        const int INPUT_DELAY = 30;
        const float MOVEMENT_SPEED = 0.2f;
        double PrevGameTime = 0;

        public float LightX { get { return TX; } }
        public float LightY { get { return TY; } }

        public bool CollisionDetection { get; set; }

        public BasicLightSource LightSource { get; set; }

        private List<Entity> prevIntersectingEntities;

        public Hero(float X, float Y) :
            base(X, Y, NPC.MALE_HUMAN)
        {
            CollisionDetection = true;
            LightSource = new BasicLightSource();
            LightSource.RadiusX = 8.0f;
            LightSource.RadiusX = 8.0f;
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

            float prevX = TX;
            float prevY = TY;
            Tile prevTile = Engine.Map.GetTopMostTile((int) TX, (int)TY);
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
                    TY -= MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    CurrentDrawable = "Walk_Down";
                    Direction = Direction.Down;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    TY += MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    CurrentDrawable = "Walk_Left";
                    Direction = Direction.Left;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    TX -= MOVEMENT_SPEED * moveSpeedModifier;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    CurrentDrawable = "Walk_Right";
                    Direction = Direction.Right;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    TX += MOVEMENT_SPEED * moveSpeedModifier;
                }

                //Set animation to idle of no movements where made
                if (moved == false)
                    CurrentDrawable = "Idle_" + Direction;

                if (CollisionDetection)
                {
                    //iterate through each layer and determine if the tile is passable
                    int tileX = (int)TX;
                    int tileY = (int)TY;

                    Tile currentTile = Engine.Map.GetTopMostTile(tileX, tileY);
                    bool impassable = currentTile.HasProperty("Impassable");

                    //CORRECT ENTRY AND EXIT MOVEMENT BASED ON TILE PROPERTIES
                    //TODO
                    //to improve structure
                    //Current very very ineffecient way of checking Entry
                    string[] entryPoints = currentTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] exitPoints = prevTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    bool top = prevY < tileY;
                    bool bottom = prevY > tileY + 1;
                    bool left = prevX < tileX;
                    bool right = prevX > tileX + 1;

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
                    float padding = 0.00001f;
                    if (impassable)
                    {
                        if (prevY <= tileY && TY > tileY)
                            TY = tileY - padding;
                        else
                            if (prevY >= tileY + 1 && TY < tileY + 1)
                                TY = tileY + 1 + padding;

                        if (prevX <= tileX && TX > tileX)
                            TX = tileX - padding;
                        else
                            if (prevX >= tileX + 1 && TX < tileX + 1)
                                TX = tileX + 1 + padding;
                    }
                }

                //prevent from going out of range
                if (TX < 0) TX = 0;
                if (TY < 0) TY = 0;
                if (TX >= Engine.Map.txWidth - 1) TX = Engine.Map.txWidth - 1;
                if (TY >= Engine.Map.txHeight - 1) TY = Engine.Map.txHeight - 1;

                //Change the radius of the LightSource overtime using a SINE wave pattern
                LightSource.TX = this.TX;
                LightSource.TY = this.TY;
                LightSource.RadiusX = (float)(8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3));
                LightSource.RadiusY = (float)(8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3));

                //EXAMPLE OF HOW THE QUAD TREE INTERSECTING ENTITIES FUNCTION CAN WORK
                //TODO: Add PER PIXEL collision detection to each one of these entities
                if (prevIntersectingEntities != null)
                    foreach (Entity entity in prevIntersectingEntities)
                        entity.Opacity = 1.0f;

                prevIntersectingEntities = Engine.QuadTree.GetIntersectingEntites(this.CurrentPxBoundingBox);
                foreach (Entity entity in prevIntersectingEntities)
                {
                    if ( entity!= this && 
                         entity.CurrentPxBoundingBox.Intersects(CurrentPxBoundingBox) &&
                         entity.TY > this.TY )
                        entity.Opacity = 0.8f;
                }
            }  
        }
    }
}
