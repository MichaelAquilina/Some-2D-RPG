using GameEngine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowKill.Shaders
{
    public interface ILightSource
    {
        float TX { get; }

        float TY { get; }

        Color Color { get; }

        float RadiusX { get; }

        float RadiusY { get; }

        LightPositionType PositionType { get; }
    }
}
