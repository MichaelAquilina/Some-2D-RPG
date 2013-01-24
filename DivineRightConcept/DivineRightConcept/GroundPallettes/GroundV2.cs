using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Drawing;
using GameEngine.GameObjects;

namespace DivineRightConcept.GroundPallettes
{
    //Should be renamed to something more appropriate
    public class GroundV2 : IGroundPallette
    {
        public int TileCount
        {
            get { return 2; }
        }

        private Texture2D _groundTexture;

        public const int GROUND_TEXTURE_GRASS = 1;
        public const int GROUND_TEXTURE_ROCK = 0;

        //private definitions of composite tilesets
        private const int GROUND_TEXTURE_ROCK_TL = 1;
        private const int GROUND_TEXTURE_ROCK_T  = 2;
        private const int GROUND_TEXTURE_ROCK_TR = 4;
        private const int GROUND_TEXTURE_ROCK_L  = 8;
        private const int GROUND_TEXTURE_ROCK_R  = 16;
        private const int GROUND_TEXTURE_ROCK_BL = 32;
        private const int GROUND_TEXTURE_ROCK_B  = 64;
        private const int GROUND_TEXTURE_ROCK_BR = 128;

        private Rectangle[] _sourceRectangles = new Rectangle[10];
        private Color[] _tileColors = new Color[] { Color.Green, Color.DarkGray };

        public GroundV2()
        {
            _sourceRectangles[0] = new Rectangle(120, 161, 40, 40);
            _sourceRectangles[1] = new Rectangle(280, 81,  40, 40);
            _sourceRectangles[2] = new Rectangle(40,  81,  40, 40);
            _sourceRectangles[3] = new Rectangle(120, 81,  40, 40);
            _sourceRectangles[4] = new Rectangle(200, 81,  40, 40);
            _sourceRectangles[5] = new Rectangle(40,  161, 40, 40);
            _sourceRectangles[6] = new Rectangle(200, 161, 40, 40);
            _sourceRectangles[7] = new Rectangle(40,  241, 40, 40);
            _sourceRectangles[8] = new Rectangle(120, 241, 40, 40);
            _sourceRectangles[9] = new Rectangle(200, 241, 40, 40);
        }

        //TEMPORARY, PLEASE REMOVE ME!!!
        private bool CheckCondition(Map GameMap, int X, int Y, int TileType)
        {
            return !(GameMap.Width==X || GameMap.Height==Y || X<0 || Y<0) && GameMap[X,Y] != TileType;
        }

        public void DrawGroundTexture(SpriteBatch SpriteBatch, Map GameMap, int X, int Y, Rectangle DesRectangle)
        {
            int TileType = GameMap[X, Y];
            //int FinalType = 0;

            //if (TileType == GROUND_TEXTURE_ROCK)
            //{
            //    if (CheckCondition(GameMap, X - 1, Y - 1, TileType)) FinalType |= 1;
            //    if (CheckCondition(GameMap, X    , Y - 1, TileType)) FinalType |= 2;
            //    if (CheckCondition(GameMap, X + 1, Y - 1, TileType)) FinalType |= 4;
            //    if (CheckCondition(GameMap, X - 1, Y    , TileType)) FinalType |= 8;
            //    if (CheckCondition(GameMap, X + 1, Y    , TileType)) FinalType |= 16;
            //    if (CheckCondition(GameMap, X - 1, Y + 1, TileType)) FinalType |= 32;
            //    if (CheckCondition(GameMap, X    , Y + 1, TileType)) FinalType |= 64;
            //    if (CheckCondition(GameMap, X + 1, Y + 1, TileType)) FinalType |= 128;
            //}

            SpriteBatch.Draw(
                _groundTexture, 
                DesRectangle, 
                new Rectangle(
                    _sourceRectangles[TileType].X,
                    _sourceRectangles[TileType].Y,
                    _sourceRectangles[TileType].Width,
                    _sourceRectangles[TileType].Height
                ),
                Color.White);
        }

        public Color GetTileColor(byte TileType)
        {
            throw new NotImplementedException();
        }

        public void LoadContent(ContentManager Content)
        {
            _groundTexture = Content.Load<Texture2D>("GRS2ROC");   
        }

        public void UnloadContent()
        {
            _groundTexture.Dispose();
        }
    }
}
