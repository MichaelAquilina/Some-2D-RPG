using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing
{
    public abstract class BitmapDrawable : IGameDrawable
    {
        public virtual Vector2 Origin
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public virtual Rectangle GetSourceRectangle(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        public virtual Texture2D GetSourceTexture(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsFinished(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        public void Draw(
            double elapsedMS, SpriteBatch spriteBatch, 
            Rectangle destRectangle, Color color, 
            float rotation, Vector2 origin, 
            SpriteEffects spriteEffects, 
            float layerDepth
            )
        {
            spriteBatch.Draw(
                GetSourceTexture(elapsedMS),
                destRectangle,
                GetSourceRectangle(elapsedMS),
                color,
                rotation,
                origin,
                spriteEffects,
                layerDepth);
        }
    }
}
