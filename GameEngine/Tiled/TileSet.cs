using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameEngine.Tiled
{
    public class TileSet
    {
        public Texture2D SourceTexture { get; set; }

        public string Name { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int FirstGID { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public string Source { get; set; }

        public Dictionary<string, string> Properties
        {
            get;
            private set;
        }

        public TileSet()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}
