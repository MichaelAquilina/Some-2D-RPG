using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Drawing
{
    /// <summary>
    /// Representation of the data currently found in the ViewPort being rendered by the TileEngine.
    /// This structure will be passed to any GameShaders that need to be applied to the world.
    /// </summary>
    public struct ViewPortInfo
    {
        public float pxTileWidth { get; set; }
        public float pxTileHeight { get; set; }
        public float txTopLeftX { get; set; }
        public float txTopLeftY { get; set; }
        public int TileCountX { get; set; }
        public int TileCountY { get; set; }
        public double txDispX { get; set; }
        public double txDispY { get; set; }
    }
}
