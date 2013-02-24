using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Tiled
{
    public enum Orientation { orthoganal, isometric }

    public class TileLayer
    {
        public string Name { get; set; }

        public int Width {
            get { return _tiles.Length; }
        }
        public int Height { 
            get { return _tiles[0].Length; }
        }

        internal int[][] _tiles;

        public TileLayer(int Width, int Height)
        {
            _tiles = new int[Height][];
            for (int i = 0; i < Height; i++)
                _tiles[i] = new int[Width];
        }
    }

    public class TileSet
    {
        public string Name { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int FirstGID { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public string Source { get; set; }
    }

    public class TiledMap
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public List<TileSet> TileSets { get; set; }
        public List<TileLayer> TileLayers { get; set; }

        public TiledMap()
        {
            TileSets = new List<TileSet>();
            TileLayers = new List<TileLayer>();
        }
    }
}
