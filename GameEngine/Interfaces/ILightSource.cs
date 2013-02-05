using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Interfaces
{
    public interface ILightSource
    {
        Texture2D GetLightSourceTexture(GameTime gameTime);

        Rectangle? GetLightSourceRectangle(GameTime gameTime);

        FRectangle GetRelativeDestRectangle(GameTime gameTime);

        Color GetLightColor(GameTime gameTime);
    }
}
