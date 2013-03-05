using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Tiled
{
    public class TileLayer : IPropertyBag
    {
        public string Name { get; set; }

        public int Width
        {
            get { return _width; }
        }
        public int Height
        {
            get { return _height; }
        }

        public int this[int x, int y]
        {
            get { return _tiles[x + y * _width ]; }
            set { _tiles[x + y * _width] = value; }
        }

        public Dictionary<string, string> Properties
        {
            get;
            private set;
        }

        private int _width;
        private int _height;
        internal int[] _tiles;

        public TileLayer(int Width, int Height)
        {
            Properties = new Dictionary<string, string>();

            _width = Width;
            _height = Height;
            _tiles = new int[Width * Height];
        }
    }
}
