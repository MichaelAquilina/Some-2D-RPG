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
        public float LightX { get; set; }
        public float LightY { get; set; }

        public float LightRadiusX { get; set; }
        public float LightRadiusY { get; set; }

        public LightPositionType PositionType { get; set; }

        public Color LightColor { get; set; }

        public BasicLightSource()
        {
        }

        public BasicLightSource(float LightX, float LightY, float RadiusX, float RadiusY, Color LightColor, LightPositionType PositionType=LightPositionType.Relative)
        {
            this.LightX = LightX;
            this.LightY = LightY;
            this.LightRadiusX = RadiusX;
            this.LightRadiusY = RadiusY;
            this.LightColor = LightColor;
            this.PositionType = PositionType;
        }
    }
}
