using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Tiled
{
    public class TiledMapObjectLayer : IPropertyBag
    {
        public List<TiledMapObject> Objects { get; set; }

        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public TiledMapObjectLayer()
        {
            this.Properties = new Dictionary<string, string>();
            this.Objects = new List<TiledMapObject>();
        }
    }
}
