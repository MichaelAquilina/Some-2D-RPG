using GameEngine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowKill.Shaders
{
    public interface ILightSource
    {
        float LightX { get; }

        float LightY { get; }

        Color LightColor { get; }

        float LightRadiusX { get; }

        float LightRadiusY { get; }

        LightPositionType PositionType { get; }
    }
}
