using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

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

        public int TileArea { get { return TileWidth * TileHeight; } }

        // The diagonal length of a tile in pixels.
        public double TileDiagonalLength { get; private set; }

        public Color Background { get; set; }

        public SortedList<int, Tile> Tiles { get; set; }                  // Provides a means of direct access with Global Indentifiers.
        public List<TileSet> TileSets { get; set; }                       // Individual TileSets loaded. Also contain references to Tiles sorted by their LOCAL id.
        public List<TileLayer> TileLayers { get; set; }                   // Tile Layers present within this map. Each one contains an array of tile global identifiers.
        public List<TiledObjectLayer> TiledObjectLayers { get; set; }     // Layers consisting of TileObjects.

        public TiledMap()
        {
            TiledObjectLayers = new List<TiledObjectLayer>();
            Tiles = new SortedList<int, Tile>();
            TileSets = new List<TileSet>();
            TileLayers = new List<TileLayer>();
        }

        public void LoadContent(ContentManager content)
        {
            foreach (Tile tile in Tiles.Values)
                tile.LoadContent(content);
        }

        public TiledObjectLayer GetObjectLayerByName(string name)
        {
            return TiledObjectLayers.Find(delegate(TiledObjectLayer t) { return t.Name.Equals(name); });
        }

        public TileLayer GetTileLayerByName(string name)
        {
            return TileLayers.Find(delegate(TileLayer t) { return t.Name.Equals(name); });
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

        public Tile GetPxTopMostTile(Vector2 pos)
        {
            return GetPxTopMostTile(pos.X, pos.Y);
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

        /// <summary>
        /// Returns the rectangular bounds for the tile at the specified coordinate. The
        /// coordinate passed should in tile coordinates (TX) rather than map pixel 
        /// coordinates (PX).
        /// </summary>
        public Rectangle GetTileBounds(Vector2 txPos)
        {
            Vector2 pxPos = TxToPx(txPos, false);

            return new Rectangle((int)pxPos.X, (int)pxPos.Y, TileWidth, TileHeight);
        }

        /// <summary>
        /// Converts the specfied global world (px) coordinates into its relevant tile coordinate.
        /// </summary>
        public Vector2 PxToTx(Vector2 position)
        {
            return new Vector2(
                (int) Math.Floor(position.X / TileWidth), 
                (int) Math.Floor(position.Y / TileHeight)
                );
        }

        /// <summary>
        /// Converts the specified tile (tx) coordinates into global world (px) coordinates. The result
        /// returned will be situated at the very center for the specified tile coordinate if the center
        /// parameter is specified as true.
        /// </summary>
        public Vector2 TxToPx(Vector2 position, bool center=true)
        {
            if (center)
                return position * new Vector2(TileWidth, TileHeight) + new Vector2(TileWidth / 2, TileHeight / 2);
            else
                return position * new Vector2(TileWidth, TileHeight);
        }

        // TODO: Document this.
        private static List<Point> ConvertToPointsList(string pointData)
        {
            if (pointData == null)
                return null;

            List<Point> points = new List<Point>();
            string[] tokenArray = pointData.Split(',', ' ');

            for (int i = 0; i < tokenArray.Length; i += 2)
                points.Add(
                    new Point(
                        Convert.ToInt32(tokenArray[i]), 
                        Convert.ToInt32(tokenArray[i+1])
                    )
                );

            return points;
        }

        // TODO: Support for zlib compression of tile data  (Zlib.NET)
        // http://stackoverflow.com/questions/6620655/compression-and-decompression-problem-with-zlib-net
        // Reads and converts and .tmx file to a TiledMap object. Doing so does NOT load appropriate tile textures into memory.
        // In order for textures to be loaded, the LoadContent(ContentManager) method must be called. This is usually automatically
        // performed by the TeeEngine when calling its LoadMap(TiledMap) method. However, it may be called independently if needs be.
        public static TiledMap LoadTmxFile(string file)
        {
            if (File.Exists(file))
            {
                // Find the working directory of this file so that any external files may use the same path.
                int dirIndex = file.LastIndexOfAny(new char[] { '/', '\\' });
                string workingDirectory = (dirIndex > 0) ? file.Substring(0, dirIndex) : "";

                XmlDocument document = new XmlDocument();
                document.Load(file);

                XmlNode mapNode = document.SelectSingleNode("map");

                TiledMap map = new TiledMap();
                map.txWidth = XmlExtensions.GetAttributeValue<int>(mapNode, "width", -1, true);
                map.txHeight = XmlExtensions.GetAttributeValue<int>(mapNode, "height", -1, true);
                map.TileWidth = XmlExtensions.GetAttributeValue<int>(mapNode, "tilewidth", -1, true);
                map.TileHeight = XmlExtensions.GetAttributeValue<int>(mapNode, "tileheight", -1, true);
                map.TileDiagonalLength = Math.Sqrt(Math.Pow(map.TileWidth, 2) + Math.Pow(map.TileHeight, 2));

                map.Background = ColorExtensions.ToColor(
                    XmlExtensions.GetAttributeValue(
                        mapNode, "backgroundcolor", "#000000"
                    )
                );
                map.LoadProperties(mapNode);

                // OBJECT LAYERS
                foreach (XmlNode objectLayerNode in mapNode.SelectNodes("objectgroup"))
                {
                    TiledObjectLayer mapObjectLayer = new TiledObjectLayer();
                    mapObjectLayer.Width = XmlExtensions.GetAttributeValue<int>(objectLayerNode, "width", 1);
                    mapObjectLayer.Height = XmlExtensions.GetAttributeValue<int>(objectLayerNode, "height", 1);
                    mapObjectLayer.Name = XmlExtensions.GetAttributeValue(objectLayerNode, "name");

                    foreach (XmlNode objectNode in objectLayerNode.SelectNodes("object"))
                    {
                        TiledObject mapObject = new TiledObject();
                        mapObject.Name = XmlExtensions.GetAttributeValue(objectNode, "name");
                        mapObject.Type = XmlExtensions.GetAttributeValue(objectNode, "type");
                        mapObject.X = XmlExtensions.GetAttributeValue<int>(objectNode, "x", 0);
                        mapObject.Y = XmlExtensions.GetAttributeValue<int>(objectNode, "y", 0);
                        mapObject.Width = XmlExtensions.GetAttributeValue<int>(objectNode, "width", 0);
                        mapObject.Height = XmlExtensions.GetAttributeValue<int>(objectNode, "height", 0);
                        mapObject.Gid = XmlExtensions.GetAttributeValue<int>(objectNode, "gid", -1);

                        XmlNode polygonNode = objectNode.SelectSingleNode("polygon");
                        if (polygonNode != null)
                            mapObject.Points = ConvertToPointsList(XmlExtensions.GetAttributeValue(polygonNode, "points"));

                        mapObject.LoadProperties(objectNode);

                        mapObjectLayer.TiledObjects.Add(mapObject);
                    }

                    map.TiledObjectLayers.Add(mapObjectLayer);
                }

                // TILESETS
                foreach (XmlNode tilesetNode in mapNode.SelectNodes("tileset"))
                {
                    XmlNode actualTilesetNode;
                    int firstGID = XmlExtensions.GetAttributeValue<int>(tilesetNode, "firstgid", -1, true);

                    // If the tileset comes from an external .tsx file, load the node from that file.
                    if (XmlExtensions.HasAttribute(tilesetNode, "source"))
                    {
                        XmlDocument tilesetDocument = new XmlDocument();
                        tilesetDocument.Load(
                            string.Format("{0}/{1}",
                                workingDirectory, XmlExtensions.GetAttributeValue(tilesetNode, "source")));

                        actualTilesetNode = tilesetDocument.SelectSingleNode("tileset");
                    }
                    else
                        actualTilesetNode = tilesetNode;

                    string tilesetName = XmlExtensions.GetAttributeValue(actualTilesetNode, "name");
                    int tileHeight = XmlExtensions.GetAttributeValue<int>(actualTilesetNode, "tileheight", -1, true);
                    int tileWidth = XmlExtensions.GetAttributeValue<int>(actualTilesetNode, "tilewidth", -1, true);

                    TileSet tileset = new TileSet();
                    tileset.LoadProperties(actualTilesetNode);

                    tileset.Name = tilesetName;
                    tileset.TileWidth = tileWidth;
                    tileset.TileHeight = tileHeight;
                    tileset.ContentTexturePath = tileset.GetProperty("Content");    // Content Texture Path for XNA. REQUIRED.

                    map.TileSets.Add(tileset);

                    XmlNode imageNode = actualTilesetNode.SelectSingleNode("image");
                    int imageWidth = XmlExtensions.GetAttributeValue<int>(imageNode, "width", -1, true);
                    int imageHeight = XmlExtensions.GetAttributeValue<int>(imageNode, "height", -1, true);
                    string sourceTexturePath = XmlExtensions.GetAttributeValue<string>(imageNode, "source", "", true);

                    // PreBuild the tiles from the tileset information.
                    int i = 0;
                    while (true)
                    {
                        int tx = (i * tileWidth) % imageWidth;
                        int ty = tileHeight * ((i * tileWidth) / imageWidth);

                        // This check is performed in the case where image width is not
                        // an exact multiple of the tile width specified.
                        if (tx + tileWidth > imageWidth)
                        {
                            tx = 0;
                            ty += tileHeight;
                        }

                        // If we have exceeded the image height, we are done.
                        if (ty + tileHeight > imageHeight)
                            break;

                        Tile tile = new Tile();
                        tile.SourceTexturePath = sourceTexturePath;                             // Path to the actual file being referred.
                        tile.SourceRectangle = new Rectangle(tx, ty, tileWidth, tileHeight);
                        tile.TileGid = i + firstGID;
                        tile.TileId = i;
                        tile.TileSet = tileset;

                        map.Tiles.Add(tile.TileGid, tile);
                        tileset.Tiles.Add(i, tile);
                        i++;
                    }

                    // Add any individual properties to the tiles we have created
                    foreach (XmlNode tileNode in actualTilesetNode.SelectNodes("tile"))
                    {
                        int tileGid = firstGID + XmlExtensions.GetAttributeValue<int>(tileNode, "id", -1, true);
                        Tile tile = map.Tiles[tileGid];
                        tile.LoadProperties(tileNode);

                        // BUILT INS
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

                    // SET BUILTIN PROPERTIES
                    tileLayer.Color = ColorExtensions.ToColor(
                        tileLayer.GetProperty<string>("Color", "#ffffff")
                        );
                    tileLayer.Opacity = tileLayer.GetProperty<float>("Opacity", 1.0f);

                    XmlNode dataNode = layerNode.SelectSingleNode("data");
                    string[] tokens = dataNode.InnerText.Split(
                        new char[] { '\n', ',', '\r' },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    for (int index = 0; index < tokens.Length; index++)
                        tileLayer[index] = Convert.ToInt32(tokens[index]);

                    map.TileLayers.Add(tileLayer);
                }

                return map;
            }
            else throw new IOException(string.Format("The Map File '{0}' does not exist.", file));
        }

        public override string ToString()
        {
            return string.Format("TiledMap: Dimensions={0}x{1}, TileDimensions={2}x{3}, TileLayers={4}, ObjectLayers={5}",
                txWidth, txHeight,
                TileWidth, TileHeight,
                TileLayers.Count,
                TiledObjectLayers.Count
                );
        }
    }
}
