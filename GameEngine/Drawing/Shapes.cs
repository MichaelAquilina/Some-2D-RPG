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
        //NOTE: It is important to avoid initialising a new Texture each time
        //This is extremely memory intensive and will end up causing a large memory leak
        //use a cached copy of a texture each time one of the methods below are called
        private static Texture2D rectText;

        public static void DrawCross(this SpriteBatch SpriteBatch, Vector2 Center, int Size, Color Background, float layerDepth)
        {
            if (rectText == null)
            {
                rectText = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
                rectText.SetData<Color>(new Color[] { Color.White });
            }

            SpriteBatch.Draw(rectText,
                new Rectangle((int) (Center.X - Size / 2), (int) Center.Y, Size, 1),
                null, Background, 0, new Vector2(0, 0), SpriteEffects.None, layerDepth);

            SpriteBatch.Draw(rectText,
                new Rectangle((int)Center.X, (int) (Center.Y - Size / 2), 1, Size),
                null, Background, 0, new Vector2(0, 0), SpriteEffects.None, layerDepth);
        }

        public static void FillRectangle(this SpriteBatch SpriteBatch, Rectangle DestRectangle, Color Background, float layerDepth)
        {
            if(rectText == null)
            {
                rectText = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
                rectText.SetData<Color>(new Color[] {Color.White});
            }

            SpriteBatch.Draw(rectText, DestRectangle, Background);
        }

        public static void DrawRectangle(this SpriteBatch SpriteBatch, Rectangle DestRectangle, Color Background, float layerDepth)
        {
            if (rectText == null)
            {
                rectText = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
                rectText.SetData<Color>(new Color[] { Color.White });
            }

            SpriteBatch.Draw(rectText, 
                new Rectangle(DestRectangle.X, DestRectangle.Y, DestRectangle.Width, 1),
                null, Background, 0, new Vector2(0, 0), SpriteEffects.None, layerDepth);

            SpriteBatch.Draw(rectText,
                new Rectangle(DestRectangle.X, DestRectangle.Y, 1, DestRectangle.Height),
                null, Background, 0, new Vector2(0, 0), SpriteEffects.None, layerDepth);

            SpriteBatch.Draw(rectText,
                new Rectangle(DestRectangle.X + DestRectangle.Width, DestRectangle.Y, 1, DestRectangle.Height),
                null, Background, 0, new Vector2(0, 0), SpriteEffects.None, layerDepth);

            SpriteBatch.Draw(rectText,
                new Rectangle(DestRectangle.X, DestRectangle.Y + DestRectangle.Height, DestRectangle.Width, 1),
                null, Background, 0, new Vector2(0, 0), SpriteEffects.None, layerDepth);
        }
    }
}
