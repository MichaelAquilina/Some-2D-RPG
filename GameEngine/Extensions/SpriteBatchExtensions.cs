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
        /// <summary>
        /// This method takes a string and automatically attempts to fit the string within the specified max
        /// line length parameter by splitting it into multiple lines when possible. This is done by taking
        /// each token in the string that is divided by the whitespace character ' '. It is still possible for one
        /// of the lines in the string to exceed the pxMaxLineLength paramater value if a token is longer than the
        /// specified amount. A padding value is required to be passed to specify how much pixel space should be
        /// used between each line outputted. The Vector2 value passed to the function specifies the center where
        /// the string will be drawn to the screen.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch option with which to perform the string draw operation.</param>
        /// <param name="spriteFont">SpriteFont instance which will be used to draw the string.</param>
        /// <param name="message">string message to draw to the screen and automatically split.</param>
        /// <param name="pxMaxLineLength">integer value specifying the ideal max possible length for each line in pixels.</param>
        /// <param name="padding">integer value specifying the amount of padding to use between each line in pixels.</param>
        /// <param name="position">Vector2 position specifying the center of where the string should be drawn.</param>
        /// <param name="color">Color instance specifying the color with which to draw the text.</param>
        public static void DrawMultiLineString(
            SpriteBatch spriteBatch, SpriteFont spriteFont, 
            string message, int pxMaxLineLength, int padding, 
            Vector2 position, Color color)
        {
            string[] wordTokens = message.Split(' ');
            StringBuilder builder = new StringBuilder();
            List<string> lines = new List<string>();
            int maxHeight = Int32.MinValue;

            // Determine how we are going to split each line.
            foreach (string token in wordTokens)
            {
                if (spriteFont.MeasureString(builder.ToString()).X +
                    spriteFont.MeasureString(token).X +
                    spriteFont.MeasureString(" ").X > pxMaxLineLength)
                {
                    string line = builder.ToString();
                    lines.Add(line);
                    maxHeight = Math.Max(maxHeight, (int) spriteFont.MeasureString(line).Y);
                    builder.Clear();
                }

                builder.Append(token);
            }

            if (builder.Length > 0) lines.Add(builder.ToString());

            // Draw the lines to the screen and center them.
            for(int i=0; i<lines.Count; i++)
            {
                string line = lines[i];
                Vector2 lineMeasurements = spriteFont.MeasureString(line);

                spriteBatch.DrawString(
                    spriteFont,
                    line,
                    new Vector2(
                        position.X - lineMeasurements.X / 2,
                        position.Y - (maxHeight + padding * 2) / 2 * (lines.Count - i)
                        ),
                    color);
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
