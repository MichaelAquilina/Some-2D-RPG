using System.Collections.Generic;
using GameEngine.Interfaces;

namespace GameEngine.Tiled
{
    public class TileLayer : PropertyBag
    {
        public string Name { get; set; }

        public int txWidth
        {
            get;
            private set;
        }
        public int txHeight
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
            get { return _tiles[x + y * txWidth ]; }
            set { _tiles[x + y * txWidth] = value; }
        }

        internal int[] _tiles;

        public TileLayer(int Width, int Height)
        {
            this.txWidth = Width;
            this.txHeight = Height;
            _tiles = new int[Width * Height];
        }

        public override string ToString()
        {
            return string.Format("TileLayer: Name={0}, Dimensions={1}x{2}", Name, txWidth, txHeight);
        }
    }
}
