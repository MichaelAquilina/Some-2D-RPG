using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DivineRightConcept.WorldGenerators;
using Microsoft.Xna.Framework;
using DivineRightConcept.GroundPallettes;

namespace DivineRightConcept
{
    //represents a game map that be used
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
