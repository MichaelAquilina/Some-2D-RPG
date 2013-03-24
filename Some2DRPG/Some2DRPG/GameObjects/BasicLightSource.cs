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
        public float PX { get; set; }
        public float PY { get; set; }

        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        public LightPositionType PositionType { get; set; }

        public Color Color { get; set; }

        public BasicLightSource()
        {
        }

        public BasicLightSource(float LightX, float LightY, float RadiusX, float RadiusY, Color LightColor, LightPositionType PositionType=LightPositionType.Relative)
        {
            this.PX = LightX;
            this.PY = LightY;
            this.RadiusX = RadiusX;
            this.RadiusY = RadiusY;
            this.Color = LightColor;
            this.PositionType = PositionType;
        }
    }
}
