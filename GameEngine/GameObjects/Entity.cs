using System;
using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameEngine.DataStructures;

namespace GameEngine.GameObjects
{
    //An abstract Entity class that should be inherited by objects which are to be visible within the game world.
    //Any map objects, NPCs or playable characters should inherit from this class in order to be used by the
    //game engine.
    public class Entity : ILoadable
    {
        //X and Y position on the Map
        public float PX { get; set; }
        public float PY { get; set; }

        public string Name { get; internal set; }                         //Name currently assigned to this Entity in the Engine

        public float ScaleX { get; set; }                                 //Amount to scale the Entities X value by (where 1.0=100%)
        public float ScaleY { get; set; }                                 //Amount ot scale the Entities Y value by (where 1.0=100%)

        public float Opacity { get; set; }                                //Opacity for the Entity. This effect stacks with whats specified in each drawables Color
        public bool Visible { get; set; }                                 //Hide or Show the entity
        public bool IsOnScreen { get; internal set; }                     //is the Entity currently on the screen or not
        public FRectangle CurrentPxBoundingBox { get; internal set; }     //the last bounding box generated during the TeeEngine update

        public DrawableSet Drawables { get; set; }                        //The set of drawable instances associated with this Entity
        public string CurrentDrawable { get; set; }                       //The current Drawables enabled

        internal FRectangle prevPxBoundingBox;                            //the previous BoundingBox that was assigned to this Entity
        internal bool requiresAddition = false;                           //boolean flag notifying if the Entity needs addition into the QuadTree

        public Entity()
        {
            Init();
        }

        public Entity(float PX, float PY, float rxWidth=1, float rxHeight=1, bool Visible=true)
        {
            this.PX = PX;
            this.PY = PY;
            this.ScaleX = rxWidth;
            this.ScaleY = rxHeight;
            this.Visible = Visible;
            this.IsOnScreen = false;

            Init();
        }

        private void Init()
        {
            this.Opacity = 1.0f;
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
        public FRectangle GetPxBoundingBox(GameTime GameTime)
        {
            List<GameDrawableInstance> drawables = Drawables[CurrentDrawable];

            if (drawables.Count == 0) return new FRectangle(PX, PY, 0, 0);

            float minX = Int32.MaxValue;
            float minY = Int32.MaxValue;
            float maxX = Int32.MinValue;
            float maxY = Int32.MinValue;

            foreach (GameDrawableInstance draw in drawables)
            {
                Rectangle pxDrawRectangle = draw.Drawable.GetSourceRectangle(GameTime);
                Vector2 rxDrawOrigin = draw.Drawable.rxDrawOrigin;

                float pxWidth  = pxDrawRectangle.Width * this.ScaleX;
                float pxHeight = pxDrawRectangle.Height * this.ScaleY;
                float pxFrameX = PX + -1 * rxDrawOrigin.X * pxWidth;
                float pxFrameY = PY + -1 * rxDrawOrigin.Y * pxHeight;

                if (pxFrameX < minX) minX = pxFrameX;
                if (pxFrameY < minY) minY = pxFrameY;
                if (pxFrameX + pxDrawRectangle.Width > maxX) maxX = pxFrameX + pxWidth;
                if (pxFrameY + pxDrawRectangle.Height > maxY) maxY = pxFrameY + pxHeight;
            }

            return new FRectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public virtual void Update(GameTime GameTime, TeeEngine Engine)
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
                PX, PY, ScaleX, ScaleY, Visible, IsOnScreen);
        }
    }
}
