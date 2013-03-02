using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Tiled
{
    public class TiledMapObject : IPropertyBag
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public TiledMapObject()
        {
            this.Properties = new Dictionary<string, string>();
        }
    }
}
