using System.Collections.Generic;
using GameEngine.Interfaces;

namespace GameEngine.Tiled
{
    public class TileLayer : PropertyBag
    {
        public string Name { get; set; }

        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }

        public int this[int index]
        {
            get { return _tiles[index]; }
            set { _tiles[index] = value; }
        }

        public int this[int x, int y]
        {
            get { return _tiles[x + y * Width ]; }
            set { _tiles[x + y * Width] = value; }
        }

        internal int[] _tiles;

        public TileLayer(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            _tiles = new int[Width * Height];
        }

        public override string ToString()
        {
            return string.Format("TileLayer: Name={0}, Dimensions={1}x{2}", Name, Width, Height);
        }
    }
}
