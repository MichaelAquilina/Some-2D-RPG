using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Tiled
{
    public class Tile : PropertyBag, IGameDrawable, ILoadable
    {
        internal Texture2D sourceTexture { get; set; }

        public string SourceTexturePath { get; internal set; }        
        public Rectangle SourceRectangle { get; internal set; }
        public int TileGid { get; internal set; }                    // Tile Global Identifier.
        public int TileId { get; internal set; }                     // Tile Local Indentifier (for within the same tileset).
        public TileSet TileSet { get; internal set; }                // Associated TileSet.

        // IGameDrawable Properties
        public Vector2 Origin { get; set; }

        public Tile()
        {
            this.Origin = new Vector2(0, 1);
        }

        public void LoadContent(ContentManager content)
        {
            sourceTexture = content.Load<Texture2D>(TileSet.ContentTexturePath);
        }

        public void UnloadContent()
        {
            sourceTexture.Dispose();
        }

        public Texture2D GetSourceTexture(double elapsedMS)
        {
            return sourceTexture;
        }

        public Rectangle GetSourceRectangle(double elapsedMS)
        {
            return SourceRectangle;
        }

        public override string ToString()
        {
            return string.Format("TileGid: {0}, TileSet: {1}", TileGid, TileSet.Name);
        }
    }
}
