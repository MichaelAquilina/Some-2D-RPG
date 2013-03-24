using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Info
{
    /// <summary>
    /// Representation of the data currently found in the ViewPort being rendered by the TileEngine.
    /// This structure will be passed to any GameShaders that need to be applied to the world.
    /// </summary>
    public struct ViewPortInfo
    {
        public Rectangle pxViewPortBounds { get; set; }     //Current viewportr rectangular bounds in pixels
        public float pxTileWidth { get; set; }              //Width of each tile in pixels
        public float pxTileHeight { get; set; }             //Height of each tile in pixels
        public float pxTopLeftX { get; set; }                 //TopLeft X coordinate
        public float pxTopLeftY { get; set; }                 //TopLeft Y coordinate
        public float pxWidth { get; set; }
        public float pxHeight { get; set; }
        public int TileCountX { get; set; }                 //width count of Tiles on the screen
        public int TileCountY { get; set; }                 //height count of Tiles on the screen
        public float pxDispX { get; set; }                    //displacement X
        public float pxDispY { get; set; }                    //displacement Y
    }
}
