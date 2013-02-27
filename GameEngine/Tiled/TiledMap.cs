using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Tiled
{
    public enum Orientation { orthoganal, isometric }

    public class TiledMap : ILoadable
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public List<TileSet> TileSets { get; set; }
        public List<TileLayer> TileLayers { get; set; }

        public Dictionary<string, string> Properties
        {
            get;
            private set;
        }

        //needs to be converted to TiledObjects
        public List<Entity> Entities { get; set; }

        public TiledMap()
        {
            Properties = new Dictionary<string, string>();
            TileSets = new List<TileSet>();
            TileLayers = new List<TileLayer>();
            Entities = new List<Entity>();
        }

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
        }

        public void UnloadContent()
        {
            foreach (ILoadable entity in Entities)
                entity.UnloadContent();
        }

        //TODO: Make more robust, this can easily break
        //TODO: Support for zlib compression of tile data  (Zlib.NET)
        //http://stackoverflow.com/questions/6620655/compression-and-decompression-problem-with-zlib-net
        //TODO: Support for Objects
        public static TiledMap LoadTiledXML(string file, ContentManager Content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(file);

            XmlNode mapNode = document.SelectSingleNode("map");

            TiledMap map = new TiledMap();
            map.Width = Convert.ToInt32(mapNode.Attributes["width"].Value);
            map.Height = Convert.ToInt32(mapNode.Attributes["height"].Value);
            map.TileWidth = Convert.ToInt32(mapNode.Attributes["tilewidth"].Value);
            map.TileHeight = Convert.ToInt32(mapNode.Attributes["tileheight"].Value);

            XmlNode mapPropertyNode = mapNode.SelectSingleNode("properties");
            if(mapPropertyNode!=null)
            {
                foreach (XmlNode propertyNode in mapPropertyNode.SelectNodes("property"))
                {
                    string name = propertyNode.Attributes["name"].Value;
                    string value = propertyNode.Attributes["value"].Value;

                    map.Properties.Add(name, value);
                }
            }

            foreach (XmlNode tilesetNode in mapNode.SelectNodes("tileset"))
            {
                TileSet tileset = new TileSet();
                tileset.FirstGID = Convert.ToInt32(tilesetNode.Attributes["firstgid"].Value);
                tileset.Name = tilesetNode.Attributes["name"].Value;
                tileset.TileHeight = Convert.ToInt32(tilesetNode.Attributes["tileheight"].Value);
                tileset.TileWidth = Convert.ToInt32(tilesetNode.Attributes["tilewidth"].Value);

                XmlNode imageNode = tilesetNode.SelectSingleNode("image");
                tileset.Source = imageNode.Attributes["source"].Value;
                tileset.ImageHeight = Convert.ToInt32(imageNode.Attributes["height"].Value);
                tileset.ImageWidth = Convert.ToInt32(imageNode.Attributes["width"].Value);

                //TODO: Make this a abit smart since tile wont set the content names automatically for us
                tileset.SourceTexture = Content.Load<Texture2D>(tileset.Source);

                map.TileSets.Add(tileset);
            }

            foreach (XmlNode layerNode in mapNode.SelectNodes("layer"))
            {
                int width = Convert.ToInt32(layerNode.Attributes["width"].Value);
                int height = Convert.ToInt32(layerNode.Attributes["height"].Value);

                TileLayer tileLayer = new TileLayer(width, height);

                XmlNode dataNode = layerNode.SelectSingleNode("data");
                string[] tokens = dataNode.InnerText.Split(new char[]{'\n', ','}, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < tokens.Length; i++)
                {
                    int x = i % width;
                    int y = i / width;
                    tileLayer[x, y] = Convert.ToInt32(tokens[i]);
                }

                map.TileLayers.Add(tileLayer);
            }

            return map;
        }
    }
}
