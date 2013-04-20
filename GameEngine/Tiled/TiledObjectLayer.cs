using System.Collections.Generic;
using GameEngine.Interfaces;

namespace GameEngine.Tiled
{
    public class TiledObjectLayer : PropertyBag
    {
        public List<TiledObject> Objects { get; set; }

        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public TiledObjectLayer()
        {
            this.Objects = new List<TiledObject>();
        }

        public override string ToString()
        {
            return string.Format("TiledObjectLayer: Name={0}, Dimensions={1}x{2}", Name, Width, Height);
        }
    }
}
