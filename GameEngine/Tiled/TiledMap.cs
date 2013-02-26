using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Interfaces;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Tiled
{
    public enum Orientation { orthoganal, isometric }

    public class TileLayer
    {
        public string Name { get; set; }

        public int Width {
            get { return _tiles.Length; }
        }
        public int Height { 
            get { return _tiles[0].Length; }
        }

        internal int[][] _tiles;

        public int this[int x, int y] {
            get { return _tiles[x][y]; }
            set { _tiles[x][y] = value; }
        }

        public TileLayer(int Width, int Height)
        {
            _tiles = new int[Height][];
            for (int i = 0; i < Height; i++)
                _tiles[i] = new int[Width];
        }
    }

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
    }

    public class TiledMap : ILoadable
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public List<TileSet> TileSets { get; set; }
        public List<TileLayer> TileLayers { get; set; }

        //needs to be thrown away TODO
        public List<Entity> Entities { get; set; }

        public TiledMap()
        {
            TileSets = new List<TileSet>();
            TileLayers = new List<TileLayer>();
            Entities = new List<Entity>();
        }

        //TODO reduce complexity!!!
        public TileSet GetTileSetByGid(int Gid)
        {
            TileSet result = null;
            int minGid = Int32.MaxValue;

            //find the tileset with the closest positive distance
            foreach(TileSet tileSet in TileSets)
            {
                int diff = Gid - tileSet.FirstGID;
                if (diff >= 0 && diff < minGid)
                    result = tileSet;
            }

            return result;
        }

        public void LoadContent(ContentManager Content)
        {
            foreach (ILoadable entity in Entities)
                entity.LoadContent(Content);

            //TODO: Implementation or remove
            //throw new NotImplementedException();
        }

        public void UnloadContent()
        {
            foreach (ILoadable entity in Entities)
                entity.UnloadContent();

            //TODO: Implementation or remove
            //throw new NotImplementedException();
        }
    }
}
