using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;

namespace Some2DRPG.GameObjects
{
    /// <summary>
    /// Derive from this Entity class rather than Entity if you wish the Entity to be collidable
    /// with the terrain or with other Entities within the game. It is important that any overriden
    /// update routines in subclasses make sure to include base.Update() at the end of their call
    /// to make use of the collision logic available in this class.
    /// </summary>
    public class CollidableEntity : Entity
    {
        #region Properties and Members

        /// <summary>
        /// The property to search for in Tiles to check if it is considered Impassable.
        /// </summary>
        public static string ImpassableTerrainProperty = "Impassable";

        /// <summary>
        /// List of CollidableEntity objects currently intersecting this CollidableEntity.
        /// </summary>
        public List<CollidableEntity> IntersectingEntities { get; internal set; }

        /// <summary>
        /// Boolean flag to enable or disable Terrain Collision response. Enabled by default.
        /// </summary>
        public bool TerrainCollisionEnabled { get; set; }
        
        /// <summary>
        /// Boolean flag to enable or disable Entity Collision response. Enabled by default.
        /// </summary>
        public bool EntityCollisionEnabled { get; set; }

        /// <summary>
        /// String name of the drawable group that will act as the collision group for calculations.
        /// </summary>
        public string CollisionGroup { get; set; }

        /// <summary>
        /// Similiar to the Entity 'CurrentPxBoundingBox' property, except it only caters for collision items.
        /// </summary>
        public Rectangle CurrentPxCollisionBoundingBox { get; internal set; }

        /// <summary>
        /// Boolean flag specifying if this object should not be movable when performing collision response.
        /// </summary>
        public bool Immovable { get; set; }

        /// <summary>
        /// The previous position of this Entity before it moved in the current Update routine.
        /// </summary>
        public Vector2 PrevPos = Vector2.Zero;

        /// <summary>
        /// The previous Collision Bounding Box that was available before movent in an Update routine.
        /// </summary>
        private Rectangle PrevPxCollisionBoundingBox;

        #endregion

        public CollidableEntity()
        {
            Immovable = false;
            TerrainCollisionEnabled = true;
            EntityCollisionEnabled = true;
            PrevPxCollisionBoundingBox = Rectangle.Empty;
            CurrentPxCollisionBoundingBox = Rectangle.Empty;
        }

        // TODO REMOVE.
        private bool ContainsItem<T>(T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i].Equals(item)) return true;

            return false;
        }

        private Vector2 CollisionResponse(Rectangle prevBoundingBox1, Rectangle boundingBox1, Rectangle boundingBox2)
        {
            // Determine the area of intersection.
            Rectangle intersection = Rectangle.Intersect(boundingBox1, boundingBox2);
            Vector2 result = new Vector2();

            if (prevBoundingBox1.Right <= boundingBox2.Left &&
                boundingBox1.Right >= boundingBox2.Left)
            {
                result.X -= intersection.Width;
            }
            else if (prevBoundingBox1.Left >= boundingBox2.Right &&
                boundingBox1.Left <= boundingBox2.Right)
            {
                result.X += intersection.Width;
            }

            if (prevBoundingBox1.Bottom <= boundingBox2.Top &&
                boundingBox1.Bottom >= boundingBox2.Top)
            {
                result.Y -= intersection.Height;
            }
            else if (prevBoundingBox1.Top >= boundingBox2.Bottom &&
                boundingBox1.Top <= boundingBox2.Bottom)
            {
                result.Y += intersection.Height;
            }

            return result;
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            // Prevent from going out of range.
            if (Pos.X < 0) Pos.X = 0;
            if (Pos.Y < 0) Pos.Y = 0;
            if (Pos.X >= engine.Map.pxWidth - 1) Pos.X = engine.Map.pxWidth - 1;
            if (Pos.Y >= engine.Map.pxHeight - 1) Pos.Y = engine.Map.pxHeight - 1;

            PrevPxCollisionBoundingBox = CurrentPxCollisionBoundingBox;
            CurrentPxCollisionBoundingBox = GetPxBoundingBox(gameTime, CollisionGroup);

            if (EntityCollisionEnabled)
            {
                IntersectingEntities = engine.GetIntersectingEntities<CollidableEntity>(CurrentBoundingBox);
                foreach (CollidableEntity entity in IntersectingEntities)
                {
                    if (entity != this
                        && entity.EntityCollisionEnabled
                        && entity.CurrentPxCollisionBoundingBox.Intersects(this.CurrentPxCollisionBoundingBox))
                    {
                        Vector2 response = CollisionResponse(
                            PrevPxCollisionBoundingBox, 
                            CurrentPxCollisionBoundingBox, 
                            entity.CurrentPxCollisionBoundingBox);

                        this.Pos += response;
                        if (!entity.Immovable) entity.Pos -= response;

                        // Recalculate any changes made during the collision response process.
                        CurrentPxCollisionBoundingBox = GetPxBoundingBox(gameTime, CollisionGroup);
                    }
                }
            }

            if (TerrainCollisionEnabled)
            {
                int Top = CurrentPxCollisionBoundingBox.Top / engine.Map.TileHeight;
                int Bottom = CurrentPxCollisionBoundingBox.Bottom / engine.Map.TileHeight;
                int Left = CurrentPxCollisionBoundingBox.Left / engine.Map.TileWidth;
                int Right = CurrentPxCollisionBoundingBox.Right / engine.Map.TileWidth;

                for (int i = Left; i <= Right; i++)
                {
                    for (int j = Top; j <= Bottom; j++)
                    {
                        Tile currTile = engine.Map.GetTxTopMostTile(i, j);
                        Rectangle currBounds = new Rectangle(
                            i * engine.Map.TileWidth,
                            j * engine.Map.TileHeight,
                            engine.Map.TileWidth,
                            engine.Map.TileHeight);

                        if (currTile.HasProperty(ImpassableTerrainProperty))
                        {
                            Vector2 response = CollisionResponse(
                                PrevPxCollisionBoundingBox,
                                CurrentPxCollisionBoundingBox,
                                currBounds);

                            this.Pos += response;
                            CurrentPxCollisionBoundingBox = GetPxBoundingBox(gameTime, CollisionGroup);
                        }
                    }
                }
            }
            PrevPos = Pos;
        }
    }
}
