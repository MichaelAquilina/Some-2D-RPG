using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Interfaces
{
    public interface IGameDrawable
    {
        Color Color { get; set; }
        Rectangle GetSourceRectangle(GameTime GameTime);
        Texture2D GetSourceTexture(GameTime GameTime);
        string Group { get; set; }
        int Layer { get; set; }
        Vector2 Origin { get; set; }
        float Rotation { get; set; }
        SpriteEffects SpriteEffect { get; set; }
        bool Visible { get; set; }
    }
}
