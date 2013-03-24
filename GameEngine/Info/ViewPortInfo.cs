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
        public float txTopLeftX { get; set; }               //TopLeft tile X coordinate
        public float txTopLeftY { get; set; }               //TopLeft tile Y coordinate
        public int TileCountX { get; set; }                 //width count of Tiles on the screen
        public int TileCountY { get; set; }                 //height count of Tiles on the screen
        public double txDispX { get; set; }                 //decimal displacement X
        public double txDispY { get; set; }                 //decimal displacement Y
    }
}
