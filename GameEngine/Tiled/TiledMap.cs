using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Tiled
{
    public enum Orientation { orthoganal, isometric }

    public class TiledMap : ILoadable
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public SortedList<int, Tile> Tiles { get; set; }
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
            Tiles = new SortedList<int, Tile>();
            TileLayers = new List<TileLayer>();
            Entities = new List<Entity>();
        }

        public Tile GetTile(int X, int Y, int layerIndex)
        {
            TileLayer layer = TileLayers[layerIndex];

            return (layer[X, Y] == 0) ? null : Tiles[layer[X, Y]];
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
                int firstGID = Convert.ToInt32(tilesetNode.Attributes["firstgid"].Value);
                string tilesetName = tilesetNode.Attributes["name"].Value;
                int tileHeight = Convert.ToInt32(tilesetNode.Attributes["tileheight"].Value);
                int tileWidth = Convert.ToInt32(tilesetNode.Attributes["tilewidth"].Value);

                XmlNode imageNode = tilesetNode.SelectSingleNode("image");
                string source = imageNode.Attributes["source"].Value;
                int imageHeight = Convert.ToInt32(imageNode.Attributes["height"].Value);
                int imageWidth = Convert.ToInt32(imageNode.Attributes["width"].Value);

                //TODO: Make this a abit smart since tile wont set the content names automatically for us
                Texture2D sourceTexture = Content.Load<Texture2D>(source.Substring(0,source.LastIndexOf('.')));     //TEMP WORKAROUND FOR TILED SAVING FORMAT

                //Build the tiles from the tileset information
                int i = 0;
                while (true)
                {
                    int tx = (i * tileWidth) % imageWidth;
                    int ty = tileHeight * ((i * tileWidth) / imageWidth);

                    //if we have exceeded the image height, we are done
                    if (ty >= imageHeight)
                        break;

                    Tile tile = new Tile();
                    tile.SourceTexture = sourceTexture;
                    tile.SourceRectangle = new Rectangle(tx, ty, tileWidth, tileHeight);
                    tile.TileGid = i + firstGID;

                    map.Tiles.Add(tile.TileGid, tile);
                    i++;
                }

                //add any properites to the tiles we have created
                foreach (XmlNode tileNode in tilesetNode.SelectNodes("tile"))
                {
                    int tileGid = firstGID + Convert.ToInt32(tileNode.Attributes["id"].Value);

                    XmlNode tilePropertyNode = tileNode.SelectSingleNode("properties");

                    if(tilePropertyNode!=null)
                    {
                        foreach (XmlNode propertyNode in tilePropertyNode.SelectNodes("property"))
                        {
                            string name = propertyNode.Attributes["name"].Value;
                            string value = propertyNode.Attributes["value"].Value;

                            map.Tiles[tileGid].Properties.Add(name, value);
                        }
                    }
                }
            }

            foreach (XmlNode layerNode in mapNode.SelectNodes("layer"))
            {
                int width = Convert.ToInt32(layerNode.Attributes["width"].Value);
                int height = Convert.ToInt32(layerNode.Attributes["height"].Value);

                TileLayer tileLayer = new TileLayer(width, height);

                XmlNode dataNode = layerNode.SelectSingleNode("data");
                string[] tokens = dataNode.InnerText.Split(
                    new char[]{'\n',',','\r'}, 
                    StringSplitOptions.RemoveEmptyEntries
                );

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
