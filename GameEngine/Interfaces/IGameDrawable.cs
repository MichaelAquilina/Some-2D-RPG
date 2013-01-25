using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Interfaces
{
    public interface IGameDrawable
    {
        float X { get; }
        float Y { get; }
        float Width { get; }
        float Height { get; }

        bool Visible { get; }

        Color DrawColor { get; }

        Vector2 Origin { get; }

        Texture2D GetTexture(GameTime GameTime);
        Rectangle GetSourceRectangle(GameTime GameTime);
    }
}
