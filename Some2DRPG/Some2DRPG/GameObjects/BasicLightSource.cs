using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Some2DRPG.Shaders;

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

        public BasicLightSource(float lightX, float lightY, float radiusX, float radiusY, Color lightColor, LightPositionType positionType=LightPositionType.Relative)
        {
            this.PX = lightX;
            this.PY = lightY;
            this.RadiusX = radiusX;
            this.RadiusY = radiusY;
            this.Color = lightColor;
            this.PositionType = positionType;
        }
    }
}
