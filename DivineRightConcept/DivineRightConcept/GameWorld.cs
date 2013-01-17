using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace DivineRightConcept
{
    public class GameWorld : GameComponent
    {
        public int WorldWidth { get; private set; }
        public int WorldHeight { get; private set; }

        public Texture2D GroundTextures { get; set; }
        public Texture2D StickManTexture { get; set; }

        //contains actual tile information about the world map
        private int[][] _worldMap;

        public GameWorld(Game Game, int WorldWidth, int WorldHeight)
            :base(Game)
        {
            this.WorldWidth = WorldWidth;
            this.WorldHeight = WorldHeight;
        }

        public override void Initialize()
        {
            //Initialise a (Currently) Random World Map
            Random random = new Random();
            _worldMap = new int[WorldHeight][];
            for (int i = 0; i < _worldMap.Length; i++)
            {
                _worldMap[i] = new int[WorldWidth];
                for (int j = 0; j < _worldMap[i].Length; j++)
                    _worldMap[i][j] = random.Next(0, 4);

            }

            //DUMP MAP COORDINATES FOR DEBUGGING
            TextWriter writer = new StreamWriter("map_coord.txt");
            for (int i = 0; i < _worldMap.Length; i++)
            {
                for (int j = 0; j < _worldMap.Length; j++)
                    writer.Write(_worldMap[i][j].ToString());

                writer.WriteLine();
            }
            writer.Close();

            base.Initialize();
        }

        //consider changing this so that its independent of player location! It should pan to where we tell it
        public void DrawWorldViewPort(SpriteBatch SpriteBatch, int PlayerX, int PlayerY, int TileWidth, int TileHeight, Rectangle DestRectangle)
        {
            //DRAW THE WORLD MAP
            int pxTileWidth = DestRectangle.Width / TileWidth;
            int pxTileHeight = DestRectangle.Height / TileHeight;

            //calculate center position tile
            int centerX = TileWidth / 2;
            int centerY = TileHeight / 2;

            //determine the topleft world coordinate in the view
            int topLeftX = PlayerX - centerX;
            int topLeftY = PlayerY - centerY;

            //Prevent the View from going outisde of the WORLD coordinates
            if (topLeftX < 0) topLeftX = 0;
            if (topLeftY < 0) topLeftY = 0;
            if (topLeftX + centerX * 2 >= WorldWidth) topLeftX = WorldWidth - centerX * 2 - 1;
            if (topLeftY + centerY * 2 >= WorldHeight) topLeftY = WorldHeight - centerY * 2 - 1;

            //draw each tile
            for (int i = 0; i < TileWidth; i++)
                for (int j = 0; j < TileHeight; j++)
                {
                    int tileX = i + topLeftX;
                    int tileY = j + topLeftY;

                    Rectangle tileDestRect = new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight);
                    tileDestRect.X += DestRectangle.X;
                    tileDestRect.Y += DestRectangle.Y;

                    SpriteBatch.DrawGroundTexture(GroundTextures, _worldMap[tileX][tileY], tileDestRect);
                }

            //DRAW THE USERS CHARACTER
            //The relative position of the character should always be (X,Y) - (topLeftX,TopLeftY) where topLeftX and topLeftY have already been corrected
            //in terms of the bounds of the WORLD map coordinates. This allows for panning at the edges
            SpriteBatch.Draw(StickManTexture, new Rectangle(
                (PlayerX - topLeftX) * pxTileWidth + DestRectangle.X, 
                (PlayerY - topLeftY) * pxTileHeight + DestRectangle.Y, 
                pxTileWidth, pxTileHeight), 
                Color.White);
        }
    }
}
