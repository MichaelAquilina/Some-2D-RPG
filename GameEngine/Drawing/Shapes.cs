using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Drawing
{
    /// <summary>
    /// Class that provides static extension methods for drawing Geometrical shapes on the Display
    /// </summary>
    public static class Shapes
    {
        public static void DrawRectangle(this SpriteBatch SpriteBatch, Rectangle DestRectangle, Color Background)
        {
            Texture2D rectText = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
            rectText.SetData<Color>(new Color[] { Background });

            SpriteBatch.Draw(rectText, DestRectangle, Color.White);
        }
    }
}
