using System;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing
{
    public abstract class TextDrawable : IGameDrawable
    {
        public Vector2 Origin { get; set; }
        
        public TextDrawable()
        {
            Origin = Vector2.Zero;
        }

        public int GetWidth(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        public int GetHeight(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        public virtual SpriteFont GetSpriteFont(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        public virtual string GetText(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        public bool IsFinished(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destRectangle, Color color, float rotation, Vector2 origin, SpriteEffects spriteEffects, float layerDepth, double elapsedMS)
        {
            float scale = destRectangle.Height / GetHeight(elapsedMS);

            spriteBatch.DrawString(
                GetSpriteFont(elapsedMS),
                GetText(elapsedMS),
                new Vector2(destRectangle.Left, destRectangle.Top),
                color,
                rotation,
                origin,
                scale,
                spriteEffects,
                layerDepth
            );
        }
    }
}
