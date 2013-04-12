using System;
using System.Collections.Generic;
using GameEngine.DataStructures;
using GameEngine.Drawing;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.GameObjects
{
    // An abstract Entity class that should be inherited by objects which are to be visible within the game world.
    // Any map objects, NPCs or playable characters should inherit from this class in order to be used by the
    // game engine.
    public class Entity : ILoadable
    {
        // X and Y position on the Map
        public Vector2 Pos;

        public string Name { get; internal set; }                         //Name currently assigned to this Entity in the Engine

        public float ScaleX { get; set; }                                 //Amount to scale the Entities X value by (where 1.0=100%)
        public float ScaleY { get; set; }                                 //Amount ot scale the Entities Y value by (where 1.0=100%)

        public float Opacity { get; set; }                                //Opacity for the Entity. This effect stacks with whats specified in each drawables Color
        public bool Visible { get; set; }                                 //Hide or Show the entity
        public bool IsOnScreen { get; internal set; }                     //is the Entity currently on the screen or not
        public FRectangle CurrentBoundingBox { get; internal set; }       //the last bounding box generated during the TeeEngine update

        public DrawableSet Drawables { get; set; }                        //The set of drawable instances associated with this Entity
        public string CurrentDrawableState { get; set; }                  //The current Drawables enabled

        internal FRectangle prevBoundingBox;                              //the previous BoundingBox that was assigned to this Entity

        #region Constructors

        public Entity()
        {
            Init();
        }

        public Entity(float x, float y, float scaleX=1, float scaleY=1, bool visible=true)
        {
            this.Pos = new Vector2(x, y);
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.Visible = visible;
            this.IsOnScreen = false;

            Init();
        }

        private void Init()
        {
            this.Opacity = 1.0f;
            this.Drawables = new DrawableSet();
        }

        #endregion

        #region Virtual Methods

        public virtual void Update(GameTime gameTime, TeeEngine engine)
        {
        }

        public virtual void LoadContent(ContentManager content)
        {
        }

        public virtual void UnloadContent()
        {
        }

        #endregion

        /// <summary>
        /// Gets the bounding box for this entity at the specified GameTime and using the specified
        /// Tile Width and Height (In Pixels). The result of this method will be returned in pixel units
        /// and will change from one GameTime instance to another depending on the contents of the entities
        /// Drawables. If no drawables are found in the entity, then a rectangle with 0 width and 0 height
        /// will be returned.
        /// </summary>
        /// <param name="gameTime">The Current GameTime.</param>
        /// <returns>An FRectangle object specifying the bounding box of this Entity in Pixels.</returns>
        public FRectangle GetPxBoundingBox(GameTime gameTime)
        {
            List<GameDrawableInstance> drawables = Drawables.GetByState(CurrentDrawableState);

            if (drawables.Count == 0) return new FRectangle(Pos.X, Pos.Y, 0, 0);

            float minX = Int32.MaxValue;
            float minY = Int32.MaxValue;
            float maxX = Int32.MinValue;
            float maxY = Int32.MinValue;

            foreach (GameDrawableInstance draw in drawables)
            {
                Rectangle pxDrawRectangle = draw.GetSourceRectangle(gameTime);
                Vector2 drawOrigin = draw.Drawable.Origin;

                float pxWidth  = pxDrawRectangle.Width * this.ScaleX;
                float pxHeight = pxDrawRectangle.Height * this.ScaleY;
                float pxFrameX = Pos.X + draw.Offset.X + -1 * drawOrigin.X * pxWidth;
                float pxFrameY = Pos.Y + draw.Offset.Y + -1 * drawOrigin.Y * pxHeight;

                if (pxFrameX < minX) minX = pxFrameX;
                if (pxFrameY < minY) minY = pxFrameY;
                if (pxFrameX + pxDrawRectangle.Width > maxX) maxX = pxFrameX + pxWidth;
                if (pxFrameY + pxDrawRectangle.Height > maxY) maxY = pxFrameY + pxHeight;
            }

            return new FRectangle(minX, minY, maxX - minX, maxY - minY);
        }

        #region Intersection Methods

        public bool IntersectsWith(FRectangle boundingBox, GameTime gameTime, string group = null)
        {
            return IntersectsWith(CurrentDrawableState, boundingBox, gameTime, group);
        }

        // Checks if the specified boundingBox intersects with this Entity - in the specified state and gameTime. Optionally
        // only those drawables that are of the specified group are checked for intersection.
        public bool IntersectsWith(string state, FRectangle boundingBox, GameTime gameTime, string group=null)
        {
            foreach (GameDrawableInstance instance in Drawables.GetByState(state))
            {
                if (group == null || instance._associatedGroup == group)
                {
                    Rectangle sourceRectangle = instance.GetSourceRectangle(gameTime);

                    FRectangle absBoundingBox = new FRectangle(
                        Pos.X + sourceRectangle.X - sourceRectangle.Width * instance.Drawable.Origin.X,
                        Pos.Y + sourceRectangle.Y - sourceRectangle.Height * instance.Drawable.Origin.Y,
                        sourceRectangle.Width,
                        sourceRectangle.Height
                        );

                    if (boundingBox.Intersects(absBoundingBox))
                        return true;
                }
            }

            return false;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Entity: Name={0}, Type={1}, Pos={2}, Width={3}, Height={4}", 
                Name, GetType(), Pos, ScaleX, ScaleY);
        }
    }
}
