using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Tiled
{
    public class TileLayer
    {
        public string Name { get; set; }

        public int Width
        {
            get { return _tiles.Length; }
        }
        public int Height
        {
            get { return _tiles[0].Length; }
        }

        public int this[int x, int y]
        {
            get { return _tiles[x][y]; }
            set { _tiles[x][y] = value; }
        }

        internal int[][] _tiles;

        public TileLayer(int Width, int Height)
        {
            _tiles = new int[Width][];
            for (int i = 0; i < Width; i++)
                _tiles[i] = new int[Height];
        }
    }
}
