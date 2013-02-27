using GameEngine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowKill.Shaders
{
    public interface ILightSource
    {
        float X { get; }

        float Y { get; }

        Color LightColor { get; }

        float RadiusX { get; }

        float RadiusY { get; }
    }
}
