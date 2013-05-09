using System;
using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.GameObjects
{
    /// <summary>
    /// An abstract Entity class that should be inherited by objects which are to be visible within the game world.
    /// Any map objects, NPCs, particles or playable characters should inherit from this class in order to be used 
    /// by the game engine.
    /// </summary>
    public abstract class Entity : ILoadable
    {
        #region Properties and Variables

        /// <summary>
        /// boolean flag specifying if this Entity should always be placed on top during draw operations.
        /// </summary>
        public bool AlwaysOnTop { get; set; }

        /// <summary>
        /// Entities X and Y position on the Map.
        /// </summary>
        public Vector2 Pos;

        /// <summary>
        /// Name currently assigned to this Entity in the Engine.
        /// </summary>
        public string Name { get; internal set; }                         

        /// <summary>
        /// Amount to scale the Entities X value by (where 1.0=100%).
        /// </summary>
        public float ScaleX { get; set; }
        
        /// <summary>
        /// Amount ot scale the Entities Y value by (where 1.0=100%).
        /// </summary>
        public float ScaleY { get; set; }                                 
                                          
        /// <summary>
        /// Opacity for the Entity. This effect stacks with whats specified in each drawables Color.
        /// </summary>
        public float Opacity { get; set; }
        
        /// <summary>
        /// Hide or Show the entity.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Is the Entity currently on the screen or not.
        /// </summary>
        public bool IsOnScreen { get; internal set; }

        /// <summary>
        /// The last bounding box generated during the TeeEngine update.
        /// </summary>
        public FRectangle CurrentBoundingBox { get; internal set; }
                                                                   
        /// <summary>
        /// The set of drawable instances associated with this Entity.
        /// </summary>
        public DrawableSet Drawables { get; set; }
        
        /// <summary>
        /// The current Drawables enabled.
        /// </summary>
        public string CurrentDrawableState { get; set; }                 
                            
        /// <summary>
        /// The previous BoundingBox that was assigned to this Entity.
        /// </summary>
        internal FRectangle PreviousBoundingBox { get; set; }                       

        #endregion

        #region Constructors

        public Entity()
        {
            Construct(0, 0, 1, 1, true);
        }

        public Entity(float x, float y, float scaleX=1, float scaleY=1, bool visible=true)
        {
            Construct(x, y, scaleX, scaleY, visible);
        }

        private void Construct(float x, float y, float scaleX, float scaleY, bool visible)
        {
            this.Pos = new Vector2(x, y);
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.Opacity = 1.0f;
            this.Visible = visible;
            this.AlwaysOnTop = false;
            this.IsOnScreen = false;
            this.Drawables = new DrawableSet();
            this.CurrentDrawableState = null;
        }

        #endregion

        #region Virtual Methods
        
        /// <summary>
        /// Called BEFORE the entity is added to the engines entity list.
        /// The result of the PreCreate method determines if the entity will be added or not.
        /// </summary>
        /// <returns>bool value determining if the engine instance should add this Entity or not.</returns>
        public virtual bool PreCreate(GameTime gameTime, TeeEngine engine)
        {
            return true;
        }

        
        /// <summary>
        /// Called AFTER the entity is added to the engines entity list.
        /// The entity will be garuanteed to have a valid CurrentBoundingBox value and have a place in the QuadTree.
        /// </summary>
        public virtual void PostCreate(GameTime gameTime, TeeEngine engine)
        {
        }

        /// <summary>
        /// Called BEFORE the entity has been removed from the engines entity list.
        /// The result of the PreDestroy method determines if the entity will be removed or not.
        /// </summary>
        /// <returns>bool value determining if the engine instance should destory this entity or not.</returns>       
        public virtual bool PreDestroy(GameTime gameTime, TeeEngine engine)
        {
            return true;
        }

        /// <summary>
        /// Called AFTER the entity has been removed from the engines entity list.
        /// </summary>
        public virtual void PostDestroy(GameTime gameTime, TeeEngine engine)
        {
        }

        public virtual void Update(GameTime gameTime, TeeEngine engine)
        {
        }

        public virtual void LoadContent(ContentManager content)
        {
        }


        #endregion

        #region Public Methods

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
            HashSet<GameDrawableInstance> drawables = Drawables.GetByState(CurrentDrawableState);

            if (drawables == null || drawables.Count == 0) return new FRectangle(Pos.X, Pos.Y, 0, 0);

            float minX = Int32.MaxValue;
            float minY = Int32.MaxValue;
            float maxX = Int32.MinValue;
            float maxY = Int32.MinValue;

            foreach (GameDrawableInstance draw in drawables)
            {
                int drawableWidth = draw.GetWidth(gameTime);
                int drawableHeight = draw.GetHeight(gameTime);

                Vector2 drawOrigin = draw.Drawable.Origin;

                float pxWidth = drawableWidth * this.ScaleX;
                float pxHeight = drawableHeight * this.ScaleY;
                float pxFrameX = Pos.X + draw.Offset.X + -1 * drawOrigin.X * pxWidth;
                float pxFrameY = Pos.Y + draw.Offset.Y + -1 * drawOrigin.Y * pxHeight;

                if (pxFrameX < minX) minX = pxFrameX;
                if (pxFrameY < minY) minY = pxFrameY;
                if (pxFrameX + drawableWidth > maxX) maxX = pxFrameX + pxWidth;
                if (pxFrameY + drawableHeight > maxY) maxY = pxFrameY + pxHeight;
            }

            return new FRectangle(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Helper method that wraps the static IntersectsWith Entity method. Assumes the state to check is the current drawable state.
        /// </summary>
        public static bool IntersectsWith(Entity entity1, string entity1Group, Entity entity2, string entity2Group, GameTime gameTime)
        {
            return Entity.IntersectsWith(
                entity1, entity1.CurrentDrawableState, entity1Group, 
                entity2, entity2.CurrentDrawableState, entity2Group, 
                gameTime);
        }

        
        /// <summary>
        /// Checks if Two Entities are intersecting each other assuming the specified states, gameTime and group filter.
        /// This check is actually rather ineffecient. O(n^2) time complexity. In reality though, we would be smart about
        /// what to compare - hence the group and state filters. The number of comparasins between drawables should be kept
        /// to a minimum in order to maintain performance.
        /// </summary>
        public static bool IntersectsWith(
            Entity entity1, string entity1State, string entity1Group,
            Entity entity2, string entity2State, string entity2Group, 
            GameTime gameTime
            )
        {
            HashSet<GameDrawableInstance> entity1Instances = entity1.Drawables.GetByState(entity1State);
            HashSet<GameDrawableInstance> entity2Instances = entity2.Drawables.GetByState(entity2State);

            if (entity1Instances == null || entity2Instances == null) return false;

            foreach (GameDrawableInstance instanceForEntity1 in entity1Instances)
            {
                if (entity1Group == null || instanceForEntity1._associatedGroup == entity1Group)
                {
                    float entity1SourceWidth = instanceForEntity1.GetWidth(gameTime) * entity1.ScaleX;
                    float entity1SourceHeight = instanceForEntity1.GetHeight(gameTime) * entity1.ScaleY;

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
                            float entity2SourceWidth = instanceForEntity2.GetWidth(gameTime) * entity2.ScaleX;
                            float entity2SourceHeight = instanceForEntity2.GetHeight(gameTime) * entity2.ScaleY;

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

        /// <summary>
        /// Overridable method whose result is used to display entity debug information by the TeeEngine in its draw method 
        /// when 'ShowEntityDebugInfo' is set to true in the DrawingOptions. By Default, the result of this method will be 
        /// the same as the ToString method.
        /// </summary>
        /// <returns>string value which will be used to show entity debugging information during draw calls.</returns>
        public virtual string GetDebugInfo()
        {
            return ToString();
        }

        public override string ToString()
        {
            return string.Format("Entity: Name={0}, Type={1}, Pos={2}, ScaleX={3}, ScaleY={4}", 
                Name, GetType(), Pos, ScaleX, ScaleY);
        }
    }
}
