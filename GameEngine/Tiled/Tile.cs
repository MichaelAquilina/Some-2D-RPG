using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Interfaces;

namespace GameEngine.Tiled
{
    public class Tile : PropertyBag, IGameDrawable
    {
        public Texture2D SourceTexture { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public int TileGid { get; set; }                    //Tile Global Identifier
        public string TileSetName { get; set; }

        //IGameDrawable Properties
        public Vector2 Origin { get; set; }

        public Tile()
        {
            this.Origin = new Vector2(0, 1);
        }

        public Texture2D GetSourceTexture(double ElapsedMS)
        {
            return SourceTexture;
        }

        public Rectangle GetSourceRectangle(double ElapsedMS)
        {
            return SourceRectangle;
        }

        public override string ToString()
        {
            return string.Format("TileGid: {0}, TileSet: {1}", TileGid, TileSetName);
        }
    }
}
