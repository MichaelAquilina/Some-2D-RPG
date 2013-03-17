using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Interfaces
{
    /// <summary>
    /// Defines an interface which is required by the TileEngine class to draw items on the screen.
    /// </summary>
    public interface IGameDrawable
    {
        Vector2 DrawOrigin { get; set; }

        Rectangle GetSourceRectangle(GameTime GameTime);
        Texture2D GetSourceTexture(GameTime GameTime);
    }
}
