using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using GameEngine.Helpers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Tiled
{
    public class TiledMap : PropertyBag
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public SortedList<int, Tile> Tiles { get; set; }
        public List<TileLayer> TileLayers { get; set; }
        public List<ObjectLayer> ObjectLayers { get; set; }

        public TiledMap()
        {
            ObjectLayers = new List<ObjectLayer>();
            Tiles = new SortedList<int, Tile>();
            TileLayers = new List<TileLayer>();
        }

        /// <summary>
        /// Retrieves the Topmost tile at the specified (X,Y) location on the map. By top most, the function
        /// refers to the draw order, i.e. the tile which would be shown on top. The first non-zero tile found
        /// from the top will be returned by this function. Tiles found in Tile layers marked as "Foreground" 
        /// are not included. If no non-zero tile is found in any layer, then a null value is returned.
        /// </summary>
        /// <param name="X">integer X value.</param>
        /// <param name="Y">integer Y value.</param>
        /// <returns>Topmost Tile object found at the specified location. Null if none exists.</returns>
        public Tile GetTopMostTile(int X, int Y)
        {
            for (int layerIndex = TileLayers.Count - 1; layerIndex >= 0; layerIndex--)
            {
                TileLayer layer = TileLayers[layerIndex];
                if (layer.HasProperty("Foreground")) continue;      //ignore foreground layers

                if( layer[X, Y] != 0 )
                    return Tiles[layer[X, Y]];
            }

            return null;
        }

        public override string ToString()
        {
            return string.Format("TiledMap: Dimensions={0}x{1}, TileDimensions={2}x{3}, TileLayers={4}, ObjectLayers={5}",
                Width, Height,
                TileWidth, TileHeight,
                TileLayers.Count,
                ObjectLayers.Count
                );
        }

        /// <summary>
        /// Retrieves the Tile object found in the specified location of the map, in a specified
        /// Layer (by Index). Will return null if no tile was found at the layer and specified
        /// location.
        /// </summary>
        /// <param name="X">integer X value.</param>
        /// <param name="Y">integer Y value.</param>
        /// <param name="layerIndex">Index of the layer to search In.</param>
        /// <returns>Tile object found at the specified location. Null if none exists.</returns>
        public Tile GetTile(int X, int Y, int layerIndex)
        {
            TileLayer layer = TileLayers[layerIndex];
            int tileID = layer[X, Y];

            return (tileID == 0) ? null : Tiles[tileID];
        }

        //TODO: Support for zlib compression of tile data  (Zlib.NET)
        //http://stackoverflow.com/questions/6620655/compression-and-decompression-problem-with-zlib-net
        public static TiledMap LoadTiledXML(string file, ContentManager Content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(file);

            XmlNode mapNode = document.SelectSingleNode("map");

            TiledMap map = new TiledMap();
            map.Width = mapNode.GetAttributeValue<int>("width", -1, true);
            map.Height = mapNode.GetAttributeValue<int>("height", -1, true);
            map.TileWidth = mapNode.GetAttributeValue<int>("tilewidth", -1, true);
            map.TileHeight = mapNode.GetAttributeValue<int>("tileheight", -1, true);
            map.LoadProperties(mapNode);

            //OBJECT LAYERS
            foreach (XmlNode objectLayerNode in mapNode.SelectNodes("objectgroup"))
            {
                ObjectLayer mapObjectLayer = new ObjectLayer();
                mapObjectLayer.Width = objectLayerNode.GetAttributeValue<int>("width", 1);
                mapObjectLayer.Height = objectLayerNode.GetAttributeValue<int>("height", 1);
                mapObjectLayer.Name = objectLayerNode.GetAttributeValue("name");

                foreach (XmlNode objectNode in objectLayerNode.SelectNodes("object"))
                {
                    MapObject mapObject = new MapObject();
                    mapObject.Name = objectNode.GetAttributeValue("name");
                    mapObject.Type = objectNode.GetAttributeValue("type");
                    mapObject.X = objectNode.GetAttributeValue<int>("x", 0);
                    mapObject.Y = objectNode.GetAttributeValue<int>("y", 0);
                    mapObject.LoadProperties(objectNode);

                    mapObjectLayer.Objects.Add(mapObject);
                }

                map.ObjectLayers.Add(mapObjectLayer);
            }

            //TILESETS
            foreach (XmlNode tilesetNode in mapNode.SelectNodes("tileset"))
            {
                int firstGID = tilesetNode.GetAttributeValue<int>("firstgid", -1, true);
                string tilesetName = tilesetNode.GetAttributeValue("name");
                int tileHeight = tilesetNode.GetAttributeValue<int>("tileheight", -1, true);
                int tileWidth = tilesetNode.GetAttributeValue<int>("tilewidth", -1, true);

                XmlNode imageNode = tilesetNode.SelectSingleNode("image");
                string source = imageNode.GetAttributeValue<string>("source", "", true);
                int imageWidth = imageNode.GetAttributeValue<int>("width", -1, true);
                int imageHeight = imageNode.GetAttributeValue<int>("height", -1, true);

                //TODO: Make this a abit smart since tile wont set the content names automatically for us
                Texture2D sourceTexture = Content.Load<Texture2D>(source.Substring(0,source.LastIndexOf('.')));     //TEMP WORKAROUND FOR TILED SAVING FORMAT

                //PreBuild the tiles from the tileset information
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
                    tile.TileSetName = tilesetName;

                    map.Tiles.Add(tile.TileGid, tile);
                    i++;
                }

                //add any individual properties to the tiles we have created
                foreach (XmlNode tileNode in tilesetNode.SelectNodes("tile"))
                {
                    int tileGid = firstGID + tileNode.GetAttributeValue<int>("id", -1, true);
                    Tile tile = map.Tiles[tileGid];
                    tile.LoadProperties(tileNode);
                }
            }

            //TILE LAYERS
            foreach (XmlNode layerNode in mapNode.SelectNodes("layer"))
            {
                int width = layerNode.GetAttributeValue<int>("width", 0);
                int height = layerNode.GetAttributeValue<int>("height", 0);

                TileLayer tileLayer = new TileLayer(width, height);
                tileLayer.Name = layerNode.GetAttributeValue("name");
                tileLayer.LoadProperties(layerNode);

                XmlNode dataNode = layerNode.SelectSingleNode("data");
                string[] tokens = dataNode.InnerText.Split(
                    new char[]{'\n',',','\r'}, 
                    StringSplitOptions.RemoveEmptyEntries
                );

                for (int index = 0; index < tokens.Length; index++)
                    tileLayer[index] = Convert.ToInt32(tokens[index]);

                map.TileLayers.Add(tileLayer);
            }

            return map;
        }
    }
}
