using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Interfaces;

namespace GameEngine.Tiled
{
    public class ObjectLayer : IPropertyBag
    {
        public List<MapObject> Objects { get; set; }

        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public ObjectLayer()
        {
            this.Properties = new Dictionary<string, string>();
            this.Objects = new List<MapObject>();
        }
    }
}
