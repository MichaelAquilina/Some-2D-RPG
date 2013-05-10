using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Drawing;

namespace GameEngine.Tiled
{
    public class Tile : PropertyBag, ILoadable
    {
        internal Texture2D sourceTexture { get; set; }

        public string SourceTexturePath { get; internal set; }        
        public Rectangle SourceRectangle { get; internal set; }
        public int TileGid { get; internal set; }                    // Tile Global Identifier.
        public int TileId { get; internal set; }                     // Tile Local Indentifier (for within the same tileset).
        public TileSet TileSet { get; internal set; }                // Associated TileSet.

        // IGameDrawable Properties
        public virtual Vector2 Origin { get; set; }

        public Tile()
        {
            this.Origin = new Vector2(0, 1);
        }

        public void LoadContent(ContentManager content)
        {
            sourceTexture = content.Load<Texture2D>(TileSet.ContentTexturePath);
        }

        public StaticImage ToStaticImage()
        {
            return new StaticImage(sourceTexture, SourceRectangle) { Origin = this.Origin };
        }


        public override string ToString()
        {
            return string.Format("TileGid: {0}, TileSet: {1}", TileGid, TileSet.Name);
        }
    }
}
