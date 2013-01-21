using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GameObjects
{
    //represents a game map that be used. Contains tile information as well as 
    //the type of ground pallette used for this particular map
    public class Map
    {
        public int Width { get { return _mapTiles.Length; } }
        public int Height { get { return _mapTiles[0].Length; } }
        public IGroundPallette GroundPallette { get; private set; }

        //provides access to Tile information through indexed properties
        public int this[int X, int Y]
        {
            get { return _mapTiles[X][Y]; }
            set { _mapTiles[X][Y] = value; }
        }

        private int[][] _mapTiles = null;

        public Map(int Width, int Height, IGroundPallette GroundPallette)
        {
            this._mapTiles = new int[Width][];
            this.GroundPallette = GroundPallette;

            for( int i=0; i<Width; i++)
                this._mapTiles[i] = new int[Height];
        }
    }
}
