using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShadowKill.Shaders;

namespace ShadowKillGame.GameObjects
{
    public class BasicLightSource : ILightSource
    {
        public float X { get; set; }
        public float Y { get; set; }

        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        public LightPositionType PositionType { get; set; }

        public Color LightColor { get; set; }

        public BasicLightSource(float X, float Y, float RadiusX, float RadiusY, Color LightColor, LightPositionType PositionType=LightPositionType.Relative)
        {
            this.X = X;
            this.Y = Y;
            this.RadiusX = RadiusX;
            this.RadiusY = RadiusY;
            this.LightColor = LightColor;
            this.PositionType = PositionType;
        }
    }
}
