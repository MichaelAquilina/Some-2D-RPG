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
        //TODO, IS THIS REALLLLY NEEDED? THIS SEEMS TO BE DEFAULT BEHAVIOUR IN XNA
        public static void DrawMultiLineString(this SpriteBatch SpriteBatch, SpriteFont SpriteFont, string Message, Vector2 Position, Color Color)
        {
            string[] messageLines = Message.Split('\n');

            for (int i = 0; i < messageLines.Length; i++)
            {
                string message = messageLines[i];

                Vector2 messageMetrics = SpriteFont.MeasureString(message);
                Vector2 messageVector = new Vector2(
                        Position.X - messageMetrics.X / 2,
                        Position.Y - messageMetrics.Y * messageLines.Length / 2 + i * messageMetrics.Y
                    );
                SpriteBatch.DrawString(SpriteFont, message, messageVector, Color);
            }
        }

        //NOTE: It is important to avoid initialising a new Texture each time
        //This is extremely memory intensive and will end up causing a large memory leak
        //use a cached copy of a texture each time one of the methods below are called
        private static Texture2D _rectText;

        public static void DrawCross(this SpriteBatch SpriteBatch, Vector2 Center, int Size, Color Background, float layerDepth)
        {
            if (_rectText == null)
            {
                _rectText = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
                _rectText.SetData<Color>(new Color[] { Color.White });
            }

            SpriteBatch.Draw(_rectText,
                new Rectangle((int) (Center.X - Size / 2), (int) Center.Y, Size, 1),
                null, Background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);

            SpriteBatch.Draw(_rectText,
                new Rectangle((int)Center.X, (int) (Center.Y - Size / 2), 1, Size),
                null, Background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        public static void FillRectangle(this SpriteBatch SpriteBatch, Rectangle DestRectangle, Color Background, float layerDepth)
        {
            if(_rectText == null)
            {
                _rectText = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
                _rectText.SetData<Color>(new Color[] {Color.White});
            }

            SpriteBatch.Draw(_rectText, DestRectangle, Background);
        }

        /// <summary>
        /// This can probably be vastly improved using vertices and a shader. The current method is extremely ineffecient for 
        /// what its intentional purpose is. (TODO)
        /// </summary>
        public static void DrawRectangle(this SpriteBatch SpriteBatch, Rectangle DestRectangle, Color Background, float layerDepth)
        {
            if (_rectText == null)
            {
                _rectText = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
                _rectText.SetData<Color>(new Color[] { Color.White });
            }

            SpriteBatch.Draw(_rectText, 
                new Rectangle(DestRectangle.X, DestRectangle.Y, DestRectangle.Width, 1),
                null, Background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);

            SpriteBatch.Draw(_rectText,
                new Rectangle(DestRectangle.X, DestRectangle.Y, 1, DestRectangle.Height),
                null, Background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);

            SpriteBatch.Draw(_rectText,
                new Rectangle(DestRectangle.X + DestRectangle.Width, DestRectangle.Y, 1, DestRectangle.Height),
                null, Background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);

            SpriteBatch.Draw(_rectText,
                new Rectangle(DestRectangle.X, DestRectangle.Y + DestRectangle.Height, DestRectangle.Width, 1),
                null, Background, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
        }
    }
}
