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

    public class TiledMap : IPropertyBag
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public SortedList<int, Tile> Tiles { get; set; }
        public List<TileLayer> TileLayers { get; set; }
        public Dictionary<string, TiledMapObjectLayer> ObjectLayers { get; set; }

        public Dictionary<string, string> Properties
        {
            get;
            private set;
        }

        public TiledMap()
        {
            Properties = new Dictionary<string, string>();
            ObjectLayers = new Dictionary<string, TiledMapObjectLayer>();
            Tiles = new SortedList<int, Tile>();
            TileLayers = new List<TileLayer>();
        }

        public Tile GetTile(int X, int Y, int layerIndex)
        {
            TileLayer layer = TileLayers[layerIndex];

            return (layer[X, Y] == 0) ? null : Tiles[layer[X, Y]];
        }

        private static void LoadProperties(XmlNode SelectedNode, IPropertyBag Destination)
        {
            XmlNode propertiesNode = SelectedNode.SelectSingleNode("properties");

            if (propertiesNode == null) return;

            foreach (XmlNode propertyNode in propertiesNode.SelectNodes("property"))
            {
                string name = propertyNode.Attributes["name"].Value;
                string value = propertyNode.Attributes["value"].Value;

                Destination.Properties.Add(name, value);
            }
        }

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
            LoadProperties(mapNode, map);

            //OBJECT LAYERS
            foreach (XmlNode objectLayerNode in mapNode.SelectNodes("objectgroup"))
            {
                TiledMapObjectLayer mapObjectLayer = new TiledMapObjectLayer();
                mapObjectLayer.Width = Convert.ToInt32(objectLayerNode.Attributes["width"].Value);
                mapObjectLayer.Height = Convert.ToInt32(objectLayerNode.Attributes["height"].Value);
                mapObjectLayer.Name = objectLayerNode.Attributes["name"].Value;

                foreach (XmlNode objectNode in objectLayerNode.SelectNodes("object"))
                {
                    TiledMapObject mapObject = new TiledMapObject();
                    mapObject.Name = objectNode.Attributes["name"].Value;
                    mapObject.Type = objectNode.Attributes["type"].Value;
                    mapObject.X = Convert.ToInt32(objectNode.Attributes["x"].Value);
                    mapObject.Y = Convert.ToInt32(objectNode.Attributes["y"].Value);

                    LoadProperties(objectNode, mapObject);

                    mapObjectLayer.Objects.Add(mapObject);
                }

                map.ObjectLayers.Add(mapObjectLayer.Name, mapObjectLayer);
            }

            //TILESETS
            foreach (XmlNode tilesetNode in mapNode.SelectNodes("tileset"))
            {
                int firstGID = Convert.ToInt32(tilesetNode.Attributes["firstgid"].Value);
                string tilesetName = tilesetNode.Attributes["name"].Value;
                int tileHeight = Convert.ToInt32(tilesetNode.Attributes["tileheight"].Value);
                int tileWidth = Convert.ToInt32(tilesetNode.Attributes["tilewidth"].Value);
                Dictionary<string, string> tilesetProperties = new Dictionary<string, string>();

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

                //add any individual properites to the tiles we have created
                foreach (XmlNode tileNode in tilesetNode.SelectNodes("tile"))
                {
                    int tileGid = firstGID + Convert.ToInt32(tileNode.Attributes["id"].Value);
                    Tile tile = map.Tiles[tileGid];

                    LoadProperties(tileNode, tile);
                }
            }

            //TILE LAYERS
            foreach (XmlNode layerNode in mapNode.SelectNodes("layer"))
            {
                int width = Convert.ToInt32(layerNode.Attributes["width"].Value);
                int height = Convert.ToInt32(layerNode.Attributes["height"].Value);

                TileLayer tileLayer = new TileLayer(width, height);
                tileLayer.Name = layerNode.Attributes["name"].Value;

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

                LoadProperties(layerNode, tileLayer);

                map.TileLayers.Add(tileLayer);
            }

            return map;
        }
    }
}
