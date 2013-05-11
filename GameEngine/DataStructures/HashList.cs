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
        public static Color[] debugColors = new Color[]{
            Color.White, Color.GreenYellow, 
            Color.Lime, Color.Gold, Color.CornflowerBlue, 
            Color.Blue, Color.Purple, Color.Magenta, 
            Color.DarkMagenta, Color.DarkOrange, Color.Red, Color.Black
        };

        public int BoxWidth { get; private set; }
        public int BoxHeight { get; private set; }

        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }

        int _boxCountX;
        int _boxCountY;

        // 1D Array representing the global list of hash lists
        List<Entity>[] _hashLists;

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

            _hashLists = new List<Entity>[_boxCountX * _boxCountY];

            // Create all the required Lists during construction
            for (int i = 0; i < _boxCountX; i++)
            {
                for (int j = 0; j < _boxCountY; j++)
                {
                    int index = j * _boxCountX + i;
                    _hashLists[index] = new List<Entity>();
                }
            }
        }

        public void Update(Entity entity)
        {
            Remove(entity.PreviousBoundingBox, entity);
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

        public List<Entity> GetIntersectingEntites(Rectangle pxRegion)
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
            float actualBoxWidth = BoxWidth * viewPort.ActualZoom;
            float actualBoxHeight = BoxHeight * viewPort.ActualZoom;

            int startBoxX = (int) (globalDispX / actualBoxWidth);
            int startBoxY = (int) (globalDispY / actualBoxHeight);

            int viewBoxCountX = (int) Math.Ceiling(destRectangle.Width / actualBoxWidth) + 1;
            int viewBoxCountY = (int) Math.Ceiling(destRectangle.Height / actualBoxHeight) + 1;

            for (int i = 0; i < viewBoxCountX; i++)
            {
                for (int j = 0; j < viewBoxCountY; j++)
                {
                    int I = i + startBoxX;
                    int J = j + startBoxY;

                    if (I < 0 || J < 0 || I >= _boxCountX || J >= _boxCountY)
                        continue;

                    int index = I + J * _boxCountX;
                    int x = (int)(I * BoxWidth * viewPort.ActualZoom - globalDispX);
                    int y = (int)(J * BoxHeight * viewPort.ActualZoom - globalDispY);
                    int width = (int) (BoxWidth * viewPort.ActualZoom) - 1;
                    int height = (int)(BoxHeight * viewPort.ActualZoom) - 1;

                    int entityCount = _hashLists[index].Count;
                    Color drawColor = debugColors[Math.Min(entityCount, debugColors.Length - 1)];
                    string debugText = string.Format("{0}\n\r{1}", index, entityCount);

                    Vector2 debugTextMeasurements = spriteFont.MeasureString(debugText);

                    SpriteBatchExtensions.DrawRectangle(
                        spriteBatch,
                        new Rectangle(x, y, width, height),
                       drawColor, 0);

                    if (debugTextMeasurements.X < width && 
                        debugTextMeasurements.Y < height)
                    {
                        SpriteBatchExtensions.DrawCenteredString(
                            spriteBatch,
                            spriteFont,
                            debugText,
                            new Vector2(x, y) + new Vector2(width, height) / 2,
                            drawColor);
                    }   
                }
            }
        }

        #region Internal Methods

        internal void Add(Rectangle boundingBox, Entity entity)
        {
            foreach (List<Entity> entityList in GetHashList(boundingBox))
                entityList.Add(entity);
        }

        internal void Remove(Rectangle boundingBox, Entity entity)
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

        internal List<List<Entity>> GetHashList(Rectangle boundingBox)
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
                    result.Add(_hashLists[index]);
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
