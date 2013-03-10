using System.Collections.Generic;
using GameEngine.Interfaces;

namespace GameEngine.Tiled
{
    public class MapObject : PropertyBag
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
    }
}
