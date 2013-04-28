using System;
using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Info;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.DataStructures
{
    public class HashList : ICollider
    {
        public int BoxWidth { get; private set; }
        public int BoxHeight { get; private set; }

        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }

        int _boxCountX;
        int _boxCountY;

        // 1D Array representing the global list of hash lists
        List<Entity>[] _entityHashlists;

        public HashList(int boxWidth, int boxHeight)
        {
            this.BoxWidth = boxWidth;
            this.BoxHeight = boxHeight;
        }

        public void Construct(int txWidth, int txHeight, int tileWidth, int tileHeight)
        {
            this.PixelWidth = txWidth * tileWidth;
            this.PixelHeight = txHeight * tileHeight;

            this._boxCountX = (int)Math.Ceiling(((float)PixelWidth) / BoxWidth);
            this._boxCountY = (int)Math.Ceiling(((float)PixelHeight) / BoxHeight);

            _entityHashlists = new List<Entity>[_boxCountX * _boxCountY];

            // Create all the required Lists during construction
            for (int i = 0; i < _boxCountX; i++)
            {
                for (int j = 0; j < _boxCountY; j++)
                {
                    int index = j * _boxCountX + i;
                    _entityHashlists[index] = new List<Entity>();
                }
            }
        }

        public void Update(Entity entity)
        {
            Remove(entity.prevBoundingBox, entity);
            Add(entity.CurrentBoundingBox, entity);
        }

        public void Add(Entity entity)
        {
            Add(entity.CurrentBoundingBox, entity);
        }

        public void Remove(Entity entity)
        {
            Remove(entity.CurrentBoundingBox, entity);
        }

        public List<Entity> GetIntersectingEntites(FRectangle pxRegion)
        {
            List<Entity> result = new List<Entity>();
            foreach (List<Entity> entities in GetHashList(pxRegion))
                foreach (Entity entity in entities)
                    if (!result.Contains(entity))
                        result.Add(entity);

            return result;
        }

        public void DrawDebugInfo(
            ViewPortInfo viewPort,
            SpriteBatch spriteBatch,
            Rectangle destRectangle,
            SpriteFont spriteFont,
            float globalDispX, float globalDispY)
        {
        }

        #region Internal Methods

        internal void Add(FRectangle boundingBox, Entity entity)
        {
            foreach (List<Entity> entityList in GetHashList(boundingBox))
                entityList.Add(entity);
        }

        internal void Remove(FRectangle boundingBox, Entity entity)
        {
            foreach (List<Entity> entityList in GetHashList(boundingBox))
                entityList.Remove(entity);
        }

        internal int GetIndex(float x, float y)
        {
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x >= PixelWidth) x = PixelWidth - 1;
            if (y >= PixelHeight) y = PixelHeight - 1;

            int i = (int)Math.Floor(x / BoxHeight);
            int j = (int)Math.Floor(y / BoxWidth);

            return i + j * _boxCountX;
        }

        internal List<List<Entity>> GetHashList(FRectangle boundingBox)
        {
            List<List<Entity>> result = new List<List<Entity>>();

            int topLeftIndex = GetIndex(boundingBox.Left, boundingBox.Top);
            int topRightIndex = GetIndex(boundingBox.Right, boundingBox.Top);
            int bottomLeftIndex = GetIndex(boundingBox.Left, boundingBox.Bottom);

            int width = topRightIndex - topLeftIndex + 1;
            int height = (bottomLeftIndex - topLeftIndex) / _boxCountY + 1;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = topLeftIndex + i + j * _boxCountX;
                    result.Add(_entityHashlists[index]);
                }
            }

            return result;
        }

        #endregion

        public override string ToString()
        {
            return string.Format(
                "HashList: BoxWidth={0}, BoxHeight={1}, PixelWidth={2}, PixelHeight={3}", 
                BoxWidth, BoxHeight, PixelWidth, PixelHeight);
        }
    }
}
