using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Some2DRPG.GameObjects
{
    // Represents an Entity that can collide with the terrain.
    public class CollidableEntity : Entity
    {
        public bool TerrainCollisionEnabled { get; set; }
        public bool EntityCollisionEnabled { get; set; }

        public bool Immovable { get; set; }

        Vector2 _prevPos = Vector2.Zero;
        Tile _prevTile = null;

        public CollidableEntity()
        {
            TerrainCollisionEnabled = true;
            EntityCollisionEnabled = true;
            Immovable = false;
        }

        // TODO REMOVE.
        private bool ContainsItem(string[] array, string item)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == item) return true;

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
                bool impassable = currentTile.HasProperty("Impassable");

                // CORRECT ENTRY AND EXIT MOVEMENT BASED ON TILE PROPERTIES
                // TODO
                // to improve structure
                // Current very very ineffecient way of checking Entry
                string[] entryPoints = currentTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] exitPoints = _prevTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                bool top = _prevPos.Y < pxTileY;
                bool bottom = _prevPos.Y > pxTileY + pxTileHeight;
                bool left = _prevPos.X < pxTileX;
                bool right = _prevPos.X > pxTileX + pxTileWidth;

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
                    if (_prevPos.Y <= pxTileY && Pos.Y > pxTileY)
                        Pos.Y = pxTileY - padding;
                    else
                        if (_prevPos.Y >= pxTileY + pxTileHeight && Pos.Y < pxTileY + pxTileHeight)
                            Pos.Y = pxTileY + pxTileHeight + padding;

                    if (_prevPos.X <= pxTileX && Pos.X > pxTileX)
                        Pos.X = pxTileX - padding;
                    else
                        if (_prevPos.X >= pxTileX + pxTileWidth && Pos.X < pxTileX + pxTileWidth)
                            Pos.X = pxTileX + pxTileWidth + padding;
                }
            }

            _prevPos = Pos;
            _prevTile = engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);

            if (EntityCollisionEnabled)
            {
                List<Entity> intersectingEntities = engine.GetIntersectingEntities(CurrentBoundingBox);
                foreach (Entity entity in intersectingEntities)
                {
                    if (entity != this && entity is CollidableEntity)
                    {
                        CollidableEntity collidableEntity = (CollidableEntity)entity;

                        if (collidableEntity.EntityCollisionEnabled
                            && Entity.IntersectsWith(this, "Shadow", entity, "Shadow", gameTime))
                        {
                            // Naive implementation.
                            Vector2 difference = entity.Pos - this.Pos;
                            if (difference.Length() > 0)
                            {
                                difference.Normalize();

                                if (!collidableEntity.Immovable)
                                    collidableEntity.Pos += difference;

                                if (!this.Immovable)
                                    this.Pos -= difference;
                            }
                        }
                    }
                }
            }
        }
    }
}
