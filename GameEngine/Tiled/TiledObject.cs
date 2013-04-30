using System.Collections.Generic;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;

namespace GameEngine.Tiled
{
    public class TiledObject : PropertyBag
    {
        public int X { get; set; }
        public int Y { get; set; }

        public List<Point> Points { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int Gid { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }

        public TiledObject()
        {
            Gid = -1;
            X = -1;
            Y = -1;
        }

        public override string ToString()
        {
            return string.Format("TileObject: Name={0}, Type={1}, Pos=({2},{3})",
                Name, Type, X, Y);
        }
    }
}
