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
            map.Width = Convert.ToInt32(mapNode.Attributes["width"].Value);
            map.Height = Convert.ToInt32(mapNode.Attributes["height"].Value);
            map.TileWidth = Convert.ToInt32(mapNode.Attributes["tilewidth"].Value);
            map.TileHeight = Convert.ToInt32(mapNode.Attributes["tileheight"].Value);
            map.LoadProperties(mapNode);

            //OBJECT LAYERS
            foreach (XmlNode objectLayerNode in mapNode.SelectNodes("objectgroup"))
            {
                ObjectLayer mapObjectLayer = new ObjectLayer();
                mapObjectLayer.Width = Convert.ToInt32(objectLayerNode.Attributes["width"].Value);
                mapObjectLayer.Height = Convert.ToInt32(objectLayerNode.Attributes["height"].Value);
                mapObjectLayer.Name = objectLayerNode.Attributes["name"].Value;

                foreach (XmlNode objectNode in objectLayerNode.SelectNodes("object"))
                {
                    MapObject mapObject = new MapObject();
                    mapObject.Name = objectNode.Attributes["name"].Value;
                    mapObject.Type = objectNode.Attributes["type"].Value;
                    mapObject.X = Convert.ToInt32(objectNode.Attributes["x"].Value);
                    mapObject.Y = Convert.ToInt32(objectNode.Attributes["y"].Value);
                    mapObject.LoadProperties(objectNode);

                    mapObjectLayer.Objects.Add(mapObject);
                }

                map.ObjectLayers.Add(mapObjectLayer);
            }

            //TILESETS
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

                    map.Tiles.Add(tile.TileGid, tile);
                    i++;
                }

                //add any individual properties to the tiles we have created
                foreach (XmlNode tileNode in tilesetNode.SelectNodes("tile"))
                {
                    int tileGid = firstGID + Convert.ToInt32(tileNode.Attributes["id"].Value);
                    Tile tile = map.Tiles[tileGid];
                    tile.LoadProperties(tileNode);
                }
            }

            //TILE LAYERS
            foreach (XmlNode layerNode in mapNode.SelectNodes("layer"))
            {
                int width = Convert.ToInt32(layerNode.Attributes["width"].Value);
                int height = Convert.ToInt32(layerNode.Attributes["height"].Value);

                TileLayer tileLayer = new TileLayer(width, height);
                tileLayer.Name = layerNode.Attributes["name"].Value;
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
