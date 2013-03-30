using GameEngine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Some2DRPG.Shaders
{
    public interface ILightSource
    {
        float PX { get; }

        float PY { get; }

        Color Color { get; }

        float RadiusX { get; }

        float RadiusY { get; }

        LightPositionType PositionType { get; }
    }
}
