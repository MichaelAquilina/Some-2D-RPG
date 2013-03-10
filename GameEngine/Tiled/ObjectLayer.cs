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
    }
}
