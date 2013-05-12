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
        // The property to search for in Tiles to check if it is considered Impassable.
        public static string ImpassableTerrainProperty = "Impassable";

        public List<CollidableEntity> IntersectingEntities { get; internal set; }

        // Boolean flags to enable or disable Terrain Collisions and Entity Collision
        public bool TerrainCollisionEnabled { get; set; }
        public bool EntityCollisionEnabled { get; set; }

        // Name of the group with which to perform intersection checks for this entity.
        public string CollisionGroup { get; set; }

        // Boolean flag specifying if this object should not be movable when performing collision response.
        public bool Immovable { get; set; }

        public Vector2 prevPos = Vector2.Zero;
        Tile _prevTile = null;

        public CollidableEntity()
        {
            TerrainCollisionEnabled = true;
            EntityCollisionEnabled = true;
            Immovable = false;
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

            if (TerrainCollisionEnabled && _prevTile != null)
            {
                // Iterate through each layer and determine if the tile is passable.
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

                bool top = prevPos.Y < pxTileY;
                bool bottom = prevPos.Y > pxTileY + pxTileHeight;
                bool left = prevPos.X < pxTileX;
                bool right = prevPos.X > pxTileX + pxTileWidth;

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
                    if (prevPos.Y <= pxTileY && Pos.Y > pxTileY)
                        Pos.Y = pxTileY - padding;
                    else
                        if (prevPos.Y >= pxTileY + pxTileHeight && Pos.Y < pxTileY + pxTileHeight)
                            Pos.Y = pxTileY + pxTileHeight + padding;

                    if (prevPos.X <= pxTileX && Pos.X > pxTileX)
                        Pos.X = pxTileX - padding;
                    else
                        if (prevPos.X >= pxTileX + pxTileWidth && Pos.X < pxTileX + pxTileWidth)
                            Pos.X = pxTileX + pxTileWidth + padding;
                }
            }

            prevPos = Pos;
            _prevTile = engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);

            if (EntityCollisionEnabled)
            {
                IntersectingEntities = engine.GetIntersectingEntities<CollidableEntity>(CurrentBoundingBox);
                foreach (CollidableEntity entity in IntersectingEntities)
                {
                    if (entity != this
                        && entity.EntityCollisionEnabled
                        && Entity.IntersectsWith(
                                this, this.CollisionGroup, 
                                entity, entity.CollisionGroup, 
                                gameTime)
                        )
                    {
                        // Naive Collision Response.
                        Vector2 difference = entity.Pos - this.Pos;
                        if (difference.Length() > 0)
                        {
                            difference.Normalize();

                            if (!entity.Immovable)
                                entity.Pos += difference;

                            if (!this.Immovable)
                                this.Pos -= difference;
                        }
                    }
                }
            }
        }
    }
}
