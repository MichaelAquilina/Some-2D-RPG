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

        public string Name { get; internal set; }                         // Name currently assigned to this Entity in the Engine.

        public float ScaleX { get; set; }                                 // Amount to scale the Entities X value by (where 1.0=100%).
        public float ScaleY { get; set; }                                 // Amount ot scale the Entities Y value by (where 1.0=100%).
                                                                             
        public float Opacity { get; set; }                                // Opacity for the Entity. This effect stacks with whats specified in each drawables Color.
        public bool Visible { get; set; }                                 // Hide or Show the entity.
        public bool IsOnScreen { get; internal set; }                     // is the Entity currently on the screen or not.
        public FRectangle CurrentBoundingBox { get; internal set; }       // the last bounding box generated during the TeeEngine update.
                                                                             
        public DrawableSet Drawables { get; set; }                        // The set of drawable instances associated with this Entity.
        public string CurrentDrawableState { get; set; }                  // The current Drawables enabled.
                                                                             
        internal FRectangle prevBoundingBox;                              // The previous BoundingBox that was assigned to this Entity.

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

        // Helper method that wraps the static IntersectsWith Entity method. Assumes the state to check is the current drawable state.
        public bool IntersectsWith(Entity targetEntity, GameTime gameTime, string thisGroup=null, string targetEntityGroup=null)
        {
            return Entity.IntersectsWith(
                this, CurrentDrawableState, thisGroup, 
                targetEntity, targetEntity.CurrentDrawableState, targetEntityGroup, 
                gameTime);
        }

        // Checks if Two Entities are intersecting each other assuming the specified states, gameTime and group filter.
        // This check is actually rather ineffecient. O(n^2) time complexity. In reality though, we would be smart about
        // what to compare - hence the group and state filters. The number of comparasins between drawables should be kept
        // to a minimum in order to maintain performance.
        public static bool IntersectsWith(
            Entity entity1, string entity1State, string entity1Group,
            Entity entity2, string entity2State, string entity2Group, GameTime gameTime)
        {
            List<GameDrawableInstance> entity1Instances = entity1.Drawables.GetByState(entity1State);
            List<GameDrawableInstance> entity2Instances = entity2.Drawables.GetByState(entity2State);

            foreach (GameDrawableInstance instanceForEntity1 in entity1Instances)
            {
                if (entity1Group == null || instanceForEntity1._associatedGroup == entity1Group)
                {
                    Rectangle sourceRectangleEntity1 = instanceForEntity1.GetSourceRectangle(gameTime);
                    
                    float entity1SourceWidth = sourceRectangleEntity1.Width * entity1.ScaleX;
                    float entity1SourceHeight = sourceRectangleEntity1.Height * entity1.ScaleY;

                    FRectangle absBoundingRectEntity1 = new FRectangle(
                        entity1.Pos.X - entity1SourceWidth * instanceForEntity1.Drawable.Origin.X,
                        entity1.Pos.Y - entity1SourceHeight * instanceForEntity1.Drawable.Origin.Y,
                        entity1SourceWidth,
                        entity1SourceHeight
                        );

                    foreach (GameDrawableInstance instanceForEntity2 in entity2Instances)
                    {
                        if (entity2Group == null || instanceForEntity2._associatedGroup == entity2Group)
                        {
                            Rectangle sourceRectangleEntity2 = instanceForEntity2.GetSourceRectangle(gameTime);
          
                            float entity2SourceWidth = sourceRectangleEntity2.Width * entity2.ScaleX;
                            float entity2SourceHeight = sourceRectangleEntity2.Height * entity2.ScaleY;

                            FRectangle absBoundingRectEntity2 = new FRectangle(
                                entity2.Pos.X - entity2SourceWidth * instanceForEntity2.Drawable.Origin.X,
                                entity2.Pos.Y - entity2SourceHeight * instanceForEntity2.Drawable.Origin.Y,
                                entity2SourceWidth,
                                entity2SourceHeight
                                );

                            // Check if the two bounding boxes intersect
                            if (absBoundingRectEntity1.Intersects(absBoundingRectEntity2))
                                return true;
                        }
                    }
                }
            }

            // No Intersection tests passed.
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
