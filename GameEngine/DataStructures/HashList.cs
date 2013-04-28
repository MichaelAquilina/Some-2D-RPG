using System;
using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Interfaces;

namespace GameEngine.DataStructures
{
    public class HashList : ICollider
    {
        // 1D Array representing the global list of hash lists
        int _storageWidth;
        int _storageHeight;
        int _boxCountX;
        int _boxCountY;

        int _pixelWidth;
        int _pixelHeight;
        List<Entity>[] _entityHashlists;

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
            if (x >= _pixelWidth) x = _pixelWidth - 1;
            if (y >= _pixelHeight) y = _pixelHeight - 1;

            int i = (int)Math.Floor(x / _storageHeight);
            int j = (int)Math.Floor(y / _storageWidth);

            return i + j * _boxCountX;
        }

        internal List<List<Entity>> GetHashList(FRectangle boundingBox)
        {
            List<List<Entity>> result = new List<List<Entity>>();
            int topLeftIndex = GetIndex(boundingBox.Left, boundingBox.Top);
            int topRightIndex = GetIndex(boundingBox.Right, boundingBox.Top);
            int bottomLeftIndex = GetIndex(boundingBox.Left, boundingBox.Bottom);

            int width = topRightIndex - topLeftIndex + 1;
            int height = (bottomLeftIndex - topLeftIndex)/_boxCountY + 1;

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

        public HashList(int boxWidth, int boxHeight)
        {
            this._storageWidth = boxWidth;
            this._storageHeight = boxHeight;
        }

        public void Construct(int txWidth, int txHeight, int tileWidth, int tileHeight)
        {
            this._pixelWidth = txWidth * tileWidth;
            this._pixelHeight = txHeight * tileHeight;

            this._boxCountX = (int)Math.Ceiling(((float)_pixelWidth) / _storageWidth);
            this._boxCountY = (int)Math.Ceiling(((float)_pixelHeight) / _storageHeight);

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

        public void Update(Entity entity, bool addOnMissing = true)
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
    }
}
