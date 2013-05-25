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
        /// The previous tile this CollidableEntity was one before the current Update routine.
        /// </summary>
        private Tile _prevTile = null;
        private Rectangle PrevPxCollisionBoundingBox;

        #endregion

        public CollidableEntity()
        {
            TerrainCollisionEnabled = true;
            EntityCollisionEnabled = true;
            Immovable = false;
            PrevPxCollisionBoundingBox = new Rectangle(0, 0, 0, 0);
        }

        // TODO REMOVE.
        private bool ContainsItem<T>(T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i].Equals(item)) return true;

            return false;
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
                        // Determine the area of intersection.
                        Rectangle intersection = Rectangle.Intersect(
                            entity.CurrentPxCollisionBoundingBox,
                            this.CurrentPxCollisionBoundingBox
                            );

                        if (PrevPxCollisionBoundingBox.Right <= entity.CurrentPxCollisionBoundingBox.Left &&
                            CurrentPxCollisionBoundingBox.Right >= entity.CurrentPxCollisionBoundingBox.Left)
                        {
                            this.Pos.X -= intersection.Width;
                            if (!entity.Immovable) entity.Pos.X += intersection.Width;
                        }
                        else if (PrevPxCollisionBoundingBox.Left >= entity.CurrentPxCollisionBoundingBox.Right &&
                            CurrentPxCollisionBoundingBox.Left <= entity.CurrentPxCollisionBoundingBox.Right)
                        {
                            this.Pos.X += intersection.Width;
                            if (!entity.Immovable) entity.Pos.X -= intersection.Width;
                        }

                        if (PrevPxCollisionBoundingBox.Bottom <= entity.CurrentPxCollisionBoundingBox.Top &&
                            CurrentPxCollisionBoundingBox.Bottom >= entity.CurrentPxCollisionBoundingBox.Top)
                        {
                            this.Pos.Y -= intersection.Height;
                            if (!entity.Immovable) entity.Pos.Y += intersection.Height;
                        }
                        else if (PrevPxCollisionBoundingBox.Top >= entity.CurrentPxCollisionBoundingBox.Bottom &&
                            CurrentPxCollisionBoundingBox.Top <= entity.CurrentPxCollisionBoundingBox.Bottom)
                        {
                            this.Pos.Y += intersection.Height;
                            if (!entity.Immovable) entity.Pos.Y -= intersection.Height;
                        }

                        // Recalculate any changes made during the collision response process.
                        CurrentPxCollisionBoundingBox = GetPxBoundingBox(gameTime, CollisionGroup);
                    }
                }
            }

            if (TerrainCollisionEnabled && _prevTile != null)
            {
                int tileX = (int)Pos.X / engine.Map.TileWidth;
                int tileY = (int)Pos.Y / engine.Map.TileHeight;

                int pxTileX = tileX * engine.Map.TileWidth;
                int pxTileY = tileY * engine.Map.TileHeight;
                int pxTileWidth = engine.Map.TileWidth;
                int pxTileHeight = engine.Map.TileHeight;

                Tile currentTile = engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);
                bool impassable = currentTile.HasProperty(ImpassableTerrainProperty);

                // CORRECT ENTRY AND EXIT MOVEMENT BASED ON TILE PROPERTIES
                // TODO
                // to improve structure
                // Current very very ineffecient way of checking Entry
                string[] entryPoints = currentTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] exitPoints = _prevTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                bool top = PrevPos.Y < pxTileY;
                bool bottom = PrevPos.Y > pxTileY + pxTileHeight;
                bool left = PrevPos.X < pxTileX;
                bool right = PrevPos.X > pxTileX + pxTileWidth;

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
                    if (PrevPos.Y <= pxTileY && Pos.Y > pxTileY)
                        Pos.Y = pxTileY - padding;
                    else
                        if (PrevPos.Y >= pxTileY + pxTileHeight && Pos.Y < pxTileY + pxTileHeight)
                            Pos.Y = pxTileY + pxTileHeight + padding;

                    if (PrevPos.X <= pxTileX && Pos.X > pxTileX)
                        Pos.X = pxTileX - padding;
                    else
                        if (PrevPos.X >= pxTileX + pxTileWidth && Pos.X < pxTileX + pxTileWidth)
                            Pos.X = pxTileX + pxTileWidth + padding;
                }
            }

            PrevPos = Pos;
            _prevTile = engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);
        }
    }
}
