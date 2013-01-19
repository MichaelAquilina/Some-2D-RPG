using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DivineRightConcept.Generators;
using Microsoft.Xna.Framework;

namespace DivineRightConcept
{
    //represents a game map that be used
    public class Map
    {
        public int Width { get { return _mapTiles.Length; } }
        public int Height { get { return _mapTiles[0].Length; } }

        public GraphicsDevice GraphicsDevice { get; set; }

        public Texture2D MiniMapTexture { get { return _miniMap; } }

        //provides access to Tile information through indexed properties
        public int this[int X, int Y]
        {
            get { return _mapTiles[X][Y]; }
        }

        private int[][] _mapTiles = null;
        private Texture2D _miniMap = null;

        public Map(int Width, int Height, GraphicsDevice GraphicsDevice)
        {
            this.GraphicsDevice = GraphicsDevice;
            this._mapTiles = new int[Width][];

            for( int i=0; i<Width; i++)
                this._mapTiles[i] = new int[Height];
        }

        public void Initialize(IWorldGenerator Generator)
        {
            _mapTiles = Generator.Generate(this.Width, this.Height);

            //GENERATE THE MINIMAP TEXTURE
            Color[] mapColors = new Color[this.Width * this.Height];
            _miniMap = new Texture2D(this.GraphicsDevice, this.Width, this.Height, false, SurfaceFormat.Color);

            for (int i = 0; i < this.Width; i++)
                for (int j = 0; j < this.Height; j++)
                    mapColors[j * this.Width + i] = Ground.TextureColors[this[i,j]];     //TODO: Try to remove Dependency on Ground class

            _miniMap.SetData<Color>(mapColors);
        }
    }
}
