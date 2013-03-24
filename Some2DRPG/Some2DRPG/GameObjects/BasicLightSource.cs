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
        public float TX { get; set; }
        public float TY { get; set; }

        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        public LightPositionType PositionType { get; set; }

        public Color Color { get; set; }

        public BasicLightSource()
        {
        }

        public BasicLightSource(float LightX, float LightY, float RadiusX, float RadiusY, Color LightColor, LightPositionType PositionType=LightPositionType.Relative)
        {
            this.TX = LightX;
            this.TY = LightY;
            this.RadiusX = RadiusX;
            this.RadiusY = RadiusY;
            this.Color = LightColor;
            this.PositionType = PositionType;
        }
    }
}
