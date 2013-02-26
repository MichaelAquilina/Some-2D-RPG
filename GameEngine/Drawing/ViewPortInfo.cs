using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Drawing
{
    /// <summary>
    /// Representation of the data currently found in the ViewPort being rendered by the TileEngine.
    /// This structure will be passed to any GameShaders that need to be applied to the world.
    /// </summary>
    public struct ViewPortInfo
    {
        public float PXTileWidth { get; set; }
        public float PXTileHeight { get; set; }
        public float TopLeftX { get; set; }
        public float TopLeftY { get; set; }
        public int TileCountX { get; set; }
        public int TileCountY { get; set; }
        public double dispX { get; set; }
        public double dispY { get; set; }
    }
}
