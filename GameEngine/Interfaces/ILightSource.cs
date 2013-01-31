using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Interfaces
{
    public interface ILightSource
    {
        Texture2D GetLightSourceTexture(GameTime gameTime);

        Rectangle GetLightSourceRectangle(GameTime gameTime);

        Rectangle GetReleativeDestRectangle(GameTime gameTime);
    }
}
