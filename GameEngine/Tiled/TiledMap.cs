using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.Extensions;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Tiled
{
    public class TiledMap : PropertyBag, ILoadable
    {
        public int pxWidth { get { return txWidth * TileWidth; } }
        public int pxHeight { get { return txHeight * TileHeight; } }

        public int txWidth { get; set; }
        public int txHeight { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public Color Background { get; set; }

        public SortedList<int, Tile> Tiles { get; set; }
        public List<TileLayer> TileLayers { get; set; }
        public List<ObjectLayer> ObjectLayers { get; set; }

        public TiledMap()
        {
            ObjectLayers = new List<ObjectLayer>();
            Tiles = new SortedList<int, Tile>();
            TileLayers = new List<TileLayer>();
        }

        public void LoadContent(ContentManager content)
        {
            foreach (Tile tile in Tiles.Values)
                tile.LoadContent(content);
        }

        public void UnloadContent()
        {
            foreach (Tile tile in Tiles.Values)
                tile.UnloadContent();
        }

        /// <summary>
        /// Retrieves the Topmost tile at the specified (X,Y) location on the map. By top most, the function
        /// refers to the draw order, i.e. the tile which would be shown on top. The first non-zero tile found
        /// from the top will be returned by this function. Tiles found in Tile layers marked as "Foreground" 
        /// are not included. If no non-zero tile is found in any layer, then a null value is returned.
        /// </summary>
        /// <param name="tx">integer X value.</param>
        /// <param name="ty">integer Y value.</param>
        /// <returns>Topmost Tile object found at the specified location. Null if none exists.</returns>
        public Tile GetTxTopMostTile(int tx, int ty)
        {
            for (int layerIndex = TileLayers.Count - 1; layerIndex >= 0; layerIndex--)
            {
                TileLayer layer = TileLayers[layerIndex];
                int tileGid = layer[tx, ty];

                if(  tileGid != 0 && tileGid != -1 )
                    return Tiles[layer[tx, ty]];
            }

            return null;
        }
        
        // Same as GetTxTopMostFile but using pixel coordinates
        public Tile GetPxTopMostTile(float px, float py)
        {
            return GetTxTopMostTile(
                (int) (px / TileWidth), 
                (int) (py / TileHeight)
            );
        }

        // Get the specified tile from the specified layer index using tx coordinates
        public Tile GetTxTile(int tx, int ty, int layerIndex)
        {
            TileLayer layer = TileLayers[layerIndex];
            int tileID = layer[tx, ty];

            return (tileID == 0) ? null : Tiles[tileID];
        }

        // Same as GetTxTile but specified in pixels
        public Tile GetPxTile(float px, float py, int layerIndex)
        {
            return GetTxTile(
                (int)(px / TileWidth),
                (int)(py / TileHeight),
                layerIndex
            );
        }

        // TODO: Support for zlib compression of tile data  (Zlib.NET)
        // http://stackoverflow.com/questions/6620655/compression-and-decompression-problem-with-zlib-net
        // Reads and converts and .tmx file to a TiledMap object. Doing so does NOT load appropriate tile textures into memory.
        // In order for textures to be loaded, the LoadContent(ContentManager) method must be called. This is usually automatically
        // performed by the TeeEngine when calling its LoadMap(TiledMap) method. However, it may be called independently if needs be.
        public static TiledMap FromTiledXml(string file)
        {
            XmlDocument document = new XmlDocument();
            document.Load(file);

            XmlNode mapNode = document.SelectSingleNode("map");

            TiledMap map = new TiledMap();
            map.txWidth = XmlExtensions.GetAttributeValue<int>(mapNode, "width", -1, true);
            map.txHeight = XmlExtensions.GetAttributeValue<int>(mapNode, "height", -1, true);
            map.TileWidth = XmlExtensions.GetAttributeValue<int>(mapNode, "tilewidth", -1, true);
            map.TileHeight = XmlExtensions.GetAttributeValue<int>(mapNode, "tileheight", -1, true);
            map.Background = ColorExtensions.ToColor(
                XmlExtensions.GetAttributeValue(
                    mapNode, "backgroundcolor", "#000000"
                )
            );

            // OBJECT LAYERS
            foreach (XmlNode objectLayerNode in mapNode.SelectNodes("objectgroup"))
            {
                ObjectLayer mapObjectLayer = new ObjectLayer();
                mapObjectLayer.Width = XmlExtensions.GetAttributeValue<int>(objectLayerNode, "width", 1);
                mapObjectLayer.Height = XmlExtensions.GetAttributeValue<int>(objectLayerNode, "height", 1);
                mapObjectLayer.Name = XmlExtensions.GetAttributeValue(objectLayerNode, "name");

                foreach (XmlNode objectNode in objectLayerNode.SelectNodes("object"))
                {
                    MapObject mapObject = new MapObject();
                    mapObject.Name = XmlExtensions.GetAttributeValue(objectNode, "name");
                    mapObject.Type = XmlExtensions.GetAttributeValue(objectNode, "type");
                    mapObject.X = XmlExtensions.GetAttributeValue<int>(objectNode, "x", 0);
                    mapObject.Y = XmlExtensions.GetAttributeValue<int>(objectNode, "y", 0);
                    mapObject.Width = XmlExtensions.GetAttributeValue<int>(objectNode, "width", 0);
                    mapObject.Height = XmlExtensions.GetAttributeValue<int>(objectNode, "height", 0);
                    mapObject.Gid = XmlExtensions.GetAttributeValue<int>(objectNode, "gid", -1);
                    mapObject.LoadProperties(objectNode);

                    mapObjectLayer.Objects.Add(mapObject);
                }

                map.ObjectLayers.Add(mapObjectLayer);
            }

            // TILESETS
            foreach (XmlNode tilesetNode in mapNode.SelectNodes("tileset"))
            {
                int firstGID = XmlExtensions.GetAttributeValue<int>(tilesetNode, "firstgid", -1, true);
                string tilesetName = XmlExtensions.GetAttributeValue(tilesetNode, "name");
                int tileHeight = XmlExtensions.GetAttributeValue<int>(tilesetNode, "tileheight", -1, true);
                int tileWidth = XmlExtensions.GetAttributeValue<int>(tilesetNode, "tilewidth", -1, true);

                XmlNode imageNode = tilesetNode.SelectSingleNode("image");
                int imageWidth = XmlExtensions.GetAttributeValue<int>(imageNode, "width", -1, true);
                int imageHeight = XmlExtensions.GetAttributeValue<int>(imageNode, "height", -1, true);
                string sourceTexturePath = XmlExtensions.GetAttributeValue<string>(imageNode, "source", "", true);
                
                // TEMPORARY FIX which ignores file extensions so that the content pipeline can use tmx files
                sourceTexturePath = sourceTexturePath.Substring(0, sourceTexturePath.LastIndexOf('.'));

                // PreBuild the tiles from the tileset information
                int i = 0;
                while (true)
                {
                    int tx = (i * tileWidth) % imageWidth;
                    int ty = tileHeight * ((i * tileWidth) / imageWidth);

                    //If we have exceeded the image height, we are done
                    if (ty >= imageHeight)
                        break;

                    Tile tile = new Tile();
                    tile.sourceTexturePath = sourceTexturePath;
                    tile.SourceRectangle = new Rectangle(tx, ty, tileWidth, tileHeight);
                    tile.TileGid = i + firstGID;
                    tile.TileSetName = tilesetName;

                    map.Tiles.Add(tile.TileGid, tile);
                    i++;
                }

                // Add any individual properties to the tiles we have created
                foreach (XmlNode tileNode in tilesetNode.SelectNodes("tile"))
                {
                    int tileGid = firstGID + XmlExtensions.GetAttributeValue<int>(tileNode, "id", -1, true);
                    Tile tile = map.Tiles[tileGid];
                    tile.LoadProperties(tileNode);

                    // Adjust the draw origin based on the tile property 'DrawOrigin'
                    string[] drawOrigin = tile.GetProperty("DrawOrigin", "0, 1").Split(',');
                    tile.Origin = new Vector2(
                        (float)Convert.ToDouble(drawOrigin[0]),
                        (float)Convert.ToDouble(drawOrigin[1])
                        );
                }
            }

            // TILE LAYERS
            foreach (XmlNode layerNode in mapNode.SelectNodes("layer"))
            {
                int width = XmlExtensions.GetAttributeValue<int>(layerNode, "width", 0);
                int height = XmlExtensions.GetAttributeValue<int>(layerNode, "height", 0);

                TileLayer tileLayer = new TileLayer(width, height);
                tileLayer.Name = XmlExtensions.GetAttributeValue(layerNode, "name");
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
                TileWidth, TileHeight,
                TileLayers.Count,
                ObjectLayers.Count
                );
        }
    }
}
