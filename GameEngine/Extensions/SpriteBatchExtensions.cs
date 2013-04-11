using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Extensions
{
    public static class SpriteBatchExtensions
    {
        // TODO, IS THIS REALLLLY NEEDED? THIS SEEMS TO BE DEFAULT BEHAVIOUR IN XNA.
        public static void DrawMultiLineString(SpriteBatch spriteBatch, SpriteFont spriteFont, string message, Vector2 position, Color color)
        {
            string[] messageLines = message.Split('\n');

            for (int i = 0; i < messageLines.Length; i++)
            {
                string line = messageLines[i];

                Vector2 messageMetrics = spriteFont.MeasureString(line);
                Vector2 messageVector = new Vector2(
                        position.X - messageMetrics.X / 2,
                        position.Y - messageMetrics.Y * messageLines.Length / 2 + i * messageMetrics.Y
                    );
                spriteBatch.DrawString(spriteFont, line, messageVector, color);
            }
        }

        // NOTE: It is important to avoid initialising a new Texture each time
        // This is extremely memory intensive and will end up causing a large memory leak
        // use a cached copy of a texture each time one of the methods below are called.
        private static Texture2D _rectText;

        public static void DrawCross(SpriteBatch spriteBatch, Vector2 center, int size, Color background, float layerDepth)
        {
            if (_rectText == null)
            {
                _rectText = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _rectText.SetData<Color>(new Color[] { Color.White });
            }

            spriteBatch.Draw(_rectText,
                new Rectangle((int) (center.X - size / 2), (int) center.Y, size, 1),
                null, background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);

            spriteBatch.Draw(_rectText,
                new Rectangle((int)center.X, (int) (center.Y - size / 2), 1, size),
                null, background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        public static void FillRectangle(SpriteBatch spriteBatch, Rectangle destRectangle, Color background, float layerDepth)
        {
            if(_rectText == null)
            {
                _rectText = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _rectText.SetData<Color>(new Color[] {Color.White});
            }

            spriteBatch.Draw(_rectText, destRectangle, background);
        }

        /// <summary>
        /// This can probably be vastly improved using vertices and a shader. The current method is extremely ineffecient for 
        /// what its intentional purpose is. (TODO)
        /// </summary>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle destRectangle, Color background, float layerDepth)
        {
            if (_rectText == null)
            {
                _rectText = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _rectText.SetData<Color>(new Color[] { Color.White });
            }

            spriteBatch.Draw(_rectText, 
                new Rectangle(destRectangle.X, destRectangle.Y, destRectangle.Width, 1),
                null, background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);

            spriteBatch.Draw(_rectText,
                new Rectangle(destRectangle.X, destRectangle.Y, 1, destRectangle.Height),
                null, background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);

            spriteBatch.Draw(_rectText,
                new Rectangle(destRectangle.X + destRectangle.Width, destRectangle.Y, 1, destRectangle.Height),
                null, background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);

            spriteBatch.Draw(_rectText,
                new Rectangle(destRectangle.X, destRectangle.Y + destRectangle.Height, destRectangle.Width, 1),
                null, background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
        }
    }
}
