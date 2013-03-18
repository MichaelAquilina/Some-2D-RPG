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
        public float TX { get; set; }
        public float TY { get; set; }

        //Relative Width and Height for animations this Entity will render. 1.0f by Default.
        public float rxWidth { get; set; }
        public float rxHeight { get; set; }

        public bool Visible { get; set; }
        public bool IsOnScreen { get; internal set; }

        //Relative Origin to the Width and height of each animation
        public Vector2 Origin { get; set; }

        public DrawableSet Drawables { get; set; }
        public string CurrentDrawable { get; set; }

        public Entity()
        {
            Init();
        }

        public Entity(float TX, float TY, float rxWidth=1, float rxHeight=1, bool Visible=true)
        {
            this.TX = TX;
            this.TY = TY;
            this.rxWidth = rxWidth;
            this.rxHeight = rxHeight;
            this.Visible = Visible;
            this.IsOnScreen = false;

            Init();
        }

        private void Init()
        {
            this.Origin = Vector2.Zero;
            this.Drawables = new DrawableSet();
        }

        /// <summary>
        /// Gets the bounding box for this entity at the specified GameTime and using the specified
        /// Tile Width and Height (In Pixels). The result of this method will be returned in pixel units
        /// and will change from one GameTime instance to another depending on the contents of the entities
        /// Drawables. If no drawables are found in the entity, then a rectangle with 0 width and 0 height
        /// will be returned.
        /// </summary>
        /// <param name="GameTime">The Current GameTime.</param>
        /// <param name="pxTileWidth">The Width of the viewport tiles in Pixels.</param>
        /// <param name="pxTileHeight">The Height of the viewport tiles in Pixels.</param>
        /// <returns>A Rectangle object specifying the bounding box of this Entity in Pixels.</returns>
        public Rectangle GetPxBoundingBox(GameTime GameTime, int pxTileWidth, int pxTileHeight)
        {
            List<GameDrawableInstance> drawables = Drawables[CurrentDrawable];

            int pxPosX = (int) Math.Floor(TX * pxTileWidth);
            int pxPosY = (int) Math.Floor(TY * pxTileHeight);

            if (drawables.Count == 0) return new Rectangle(pxPosX, pxPosY, 0, 0);

            int minX = Int32.MaxValue;
            int minY = Int32.MaxValue;
            int maxX = Int32.MinValue;
            int maxY = Int32.MinValue;

            foreach (GameDrawableInstance draw in drawables)
            {
                Rectangle pxDrawRectangle = draw.Drawable.GetSourceRectangle(GameTime);
                Vector2 rxDrawOrigin = draw.Drawable.rxDrawOrigin;

                int pxWidth = (int) Math.Ceiling(pxDrawRectangle.Width * this.rxWidth);
                int pxHeight = (int)Math.Ceiling(pxDrawRectangle.Height * this.rxHeight);
                int pxFrameX = (int)Math.Ceiling(pxPosX + -1 * rxDrawOrigin.X * pxWidth);
                int pxFrameY = (int)Math.Ceiling(pxPosY + -1 * rxDrawOrigin.Y * pxHeight);

                if (pxFrameX < minX) minX = pxFrameX;
                if (pxFrameY < minY) minY = pxFrameY;
                if (pxFrameX + pxDrawRectangle.Width > maxX) maxX = pxFrameX + pxWidth;
                if (pxFrameY + pxDrawRectangle.Height > maxY) maxY = pxFrameY + pxHeight;
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
            return string.Format("Entity: txPos=({0},{1}), Width={2}, Height={3}, Visible={4}, IsOnScreen={5}", 
                TX, TY, rxWidth, rxHeight, Visible, IsOnScreen);
        }
    }
}
