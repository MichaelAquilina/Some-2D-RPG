using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.Helpers;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Tiled
{
    public class TiledMap : PropertyBag
    {
        public int pxWidth { get { return txWidth * pxTileWidth; } }
        public int pxHeight { get { return txHeight * pxTileHeight; } }

        public int txWidth { get; set; }
        public int txHeight { get; set; }

        public int pxTileWidth { get; set; }
        public int pxTileHeight { get; set; }

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
        /// <param name="TX">integer X value.</param>
        /// <param name="TY">integer Y value.</param>
        /// <returns>Topmost Tile object found at the specified location. Null if none exists.</returns>
        public Tile GetTxTopMostTile(int TX, int TY)
        {
            for (int layerIndex = TileLayers.Count - 1; layerIndex >= 0; layerIndex--)
            {
                TileLayer layer = TileLayers[layerIndex];
                int tileGid = layer[TX, TY];

                if(  tileGid != 0 && tileGid != -1 )
                    return Tiles[layer[TX, TY]];
            }

            return null;
        }
        
        //same as GetTxTopMostFile but using pixel coordinates
        public Tile GetPxTopMostTile(float PX, float PY)
        {
            return GetTxTopMostTile(
                (int) (PX / pxTileWidth), 
                (int) (PY / pxTileHeight)
            );
        }

        //Get the specified tile from the specified layer index using tx coordinates
        public Tile GetTxTile(int TX, int TY, int layerIndex)
        {
            TileLayer layer = TileLayers[layerIndex];
            int tileID = layer[TX, TY];

            return (tileID == 0) ? null : Tiles[tileID];
        }

        //same as GetTxTile but specified in pixels
        public Tile GetPxTile(int PX, int PY, int layerIndex)
        {
            return GetTxTile(
                (int)(PX / pxTileWidth),
                (int)(PY / pxTileHeight),
                layerIndex
            );
        }

        //TODO: Support for zlib compression of tile data  (Zlib.NET)
        //http://stackoverflow.com/questions/6620655/compression-and-decompression-problem-with-zlib-net
        public static TiledMap LoadTiledXML(string file, ContentManager Content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(file);

            XmlNode mapNode = document.SelectSingleNode("map");

            TiledMap map = new TiledMap();
            map.txWidth = mapNode.GetAttributeValue<int>("width", -1, true);
            map.txHeight = mapNode.GetAttributeValue<int>("height", -1, true);
            map.pxTileWidth = mapNode.GetAttributeValue<int>("tilewidth", -1, true);
            map.pxTileHeight = mapNode.GetAttributeValue<int>("tileheight", -1, true);
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
                    mapObject.Gid = objectNode.GetAttributeValue<int>("gid", 0);
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

        public override string ToString()
        {
            return string.Format("TiledMap: Dimensions={0}x{1}, TileDimensions={2}x{3}, TileLayers={4}, ObjectLayers={5}",
                txWidth, txHeight,
                pxTileWidth, pxTileHeight,
                TileLayers.Count,
                ObjectLayers.Count
                );
        }
    }
}
