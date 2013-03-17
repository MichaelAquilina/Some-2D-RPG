using GameEngine.Drawing;
using GameEngine.Interfaces;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace GameEngine.GameObjects
{
    //An abstract Entity class that should be inherited by objects which are to be visible within the game world.
    //Any map objects, NPCs or playable characters should inherit from this class in order to be used by the
    //game engine.
    public class Entity : ILoadable
    {
        //X and Y Tile position on the Map
        public float X { get; set; }
        public float Y { get; set; }

        //Relative Width and Height for animations this Entity will render. 1.0f by Default.
        public float Width { get; set; }
        public float Height { get; set; }

        public bool Visible { get; set; }
        public bool OnScreen { get; internal set; }

        public bool BoundingBoxVisible { get; set; }

        //Relative Origin to the Width and height of each animation
        public Vector2 Origin { get; set; }

        public DrawableSet Drawables { get; set; }
        public string CurrentDrawable { get; set; }

        public Entity()
        {
            Init();
        }

        public Entity(float X, float Y, float Width=1, float Height=1, bool Visible=true)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.Visible = Visible;
            this.OnScreen = false;

            Init();
        }

        private void Init()
        {
            this.Origin = Vector2.Zero;
            this.BoundingBoxVisible = false;
            this.Drawables = new DrawableSet();
        }

        public Rectangle GetBoundingBox(GameTime GameTime)
        {
            List<GameDrawableInstance> drawables = Drawables[CurrentDrawable];

            if (drawables.Count == 0) return new Rectangle(0, 0, 0, 0);                             //a null rectangle for no drawables
            if (drawables.Count == 1) return drawables[0].Drawable.GetSourceRectangle(GameTime);    //if there is only one, just return it

            int minX = Int32.MaxValue;
            int minY = Int32.MaxValue;
            int maxX = Int32.MinValue;
            int maxY = Int32.MaxValue;

            foreach (GameDrawableInstance draw in drawables)
            {
                Rectangle drawRectangle = draw.Drawable.GetSourceRectangle(GameTime);
                if (drawRectangle.X < minX) minX = drawRectangle.X;
                if (drawRectangle.Y < minY) minY = drawRectangle.Y;
                if (drawRectangle.X > maxX) maxX = drawRectangle.X;
                if (drawRectangle.Y > maxY) maxY = drawRectangle.Y;
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        //TODO: Probably needs to be updated to allow interaction with other entities in the TileEngine instance
        public virtual void Update(GameTime GameTime, TiledMap Map)
        {
        }

        public virtual void LoadContent(ContentManager Content)
        {
        }

        public virtual void UnloadContent()
        {
        }

        public override string ToString()
        {
            return string.Format("Entity: Pos=({0},{1}), Width={2}, Height={3}, Visible={4}, OnScreen={5}", 
                X, Y, Width, Height, Visible, OnScreen);
        }
    }
}
