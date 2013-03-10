using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Interfaces;

namespace GameEngine.Tiled
{
    public class Tile : PropertyBag
    {
        public Texture2D SourceTexture { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public int TileGid { get; set; }                    //Tile Global Identifier
    }
}
