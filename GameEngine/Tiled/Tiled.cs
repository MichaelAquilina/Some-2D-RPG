using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Tiled
{
    public class Tiled
    {
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

                tileset.SourceTexture = Content.Load<Texture2D>(tileset.Source);

                map.TileSets.Add(tileset);
            }

            foreach (XmlNode layerNode in mapNode.SelectNodes("layer"))
            {
                int width = Convert.ToInt32(layerNode.Attributes["width"].Value);
                int height = Convert.ToInt32(layerNode.Attributes["height"].Value);

                TileLayer layer = new TileLayer(width, height);

                XmlNode dataNode = layerNode.SelectSingleNode("data");
                string[] line = dataNode.InnerText.Split('\n');

                int x = 0;
                int y = 0;

                for (int j = 0; j < line.Length; j++)
                {
                    x = 0;

                    //dummy lines should be skipped
                    if (line[j].Length == 0)
                        continue;

                    string[] items = line[j].Split(',');
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].Length == 0)
                            continue;

                        layer._tiles[y][x] = Convert.ToInt32(items[i]);
                        x = x + 1;
                    }

                    y = y + 1;
                }    

                map.TileLayers.Add(layer);
            }

            return map;
        }
    }
}
