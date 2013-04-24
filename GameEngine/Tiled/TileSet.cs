using System.Collections.Generic;
using GameEngine.GameObjects;

namespace GameEngine.Tiled
{
    public class TileSet : PropertyBag
    {
        public string ContentTexturePath { get; set; }
        public string Name { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public SortedList<int, Tile> Tiles { get; set; }

        public TileSet()
        {
            Tiles = new SortedList<int, Tile>();
        }
    }
}
