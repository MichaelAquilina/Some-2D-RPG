using System.Collections.Generic;
using GameEngine.Interfaces;

namespace GameEngine.Tiled
{
    public class ObjectLayer : PropertyBag
    {
        public List<MapObject> Objects { get; set; }

        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ObjectLayer()
        {
            this.Objects = new List<MapObject>();
        }

        public override string ToString()
        {
            return string.Format("ObjectLayer: Name={0}, Dimensions={1}x{2}", Name, Width, Height);
        }
    }
}
