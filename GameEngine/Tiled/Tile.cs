using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Tiled
{
    public class Tile
    {
        public Texture2D SourceTexture { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public int TileGid { get; set; }                    //Tile Global Identifier
        public Dictionary<string, string> Properties { get; private set; }

        public Tile()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}
