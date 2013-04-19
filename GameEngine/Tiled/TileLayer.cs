using System.Collections.Generic;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;

namespace GameEngine.Tiled
{
    public class TileLayer : PropertyBag
    {
        public string Name { get; set; }

        public bool Visible { get; set; }

        public float Opacity { get; set; }

        public Color Color { get; set; }

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
            get {
                if (x <0 || y < 0 || x >= txWidth || y >= txHeight) return -1;
                return _tiles[x + y * txWidth ]; 
            }
            set { _tiles[x + y * txWidth] = value; }
        }

        internal int[] _tiles;

        public TileLayer(int width, int height)
        {
            this.txWidth = width;
            this.txHeight = height;
            this.Visible = true;
            this.Opacity = 1.0f;
            this.Color = Color.White;

            _tiles = new int[width * height];
        }

        public override string ToString()
        {
            return string.Format("TileLayer: Name={0}, Dimensions={1}x{2}", Name, txWidth, txHeight);
        }
    }
}
