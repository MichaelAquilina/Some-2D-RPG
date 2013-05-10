using System;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing
{
    /// <summary>
    /// Represents a GameDrawable Item that renders some form of bitmap Texture to the screen.
    /// Any IGameDrawable subclass that wishes to make use of drawing functionality capable
    /// of drawing textures to the screen, this abstract class provides all the drawing logic
    /// and just requires the implementation of GetSourceRectangle and GetSourceTexture 
    /// to be implemented.
    /// </summary>
    public abstract class BitmapDrawable : IGameDrawable
    {
        public Vector2 Origin { get; set; }

        public BitmapDrawable()
        {
            Origin = Vector2.Zero;
        }

        public int GetWidth(double elapsedMS)
        {
            Rectangle? sourceRectangle = GetSourceRectangle(elapsedMS);

            return (sourceRectangle.HasValue) ? sourceRectangle.Value.Width : GetSourceTexture(elapsedMS).Width;
        }

        public int GetHeight(double elapsedMS)
        {
            Rectangle? sourceRectangle = GetSourceRectangle(elapsedMS);

            return (sourceRectangle.HasValue) ? sourceRectangle.Value.Height : GetSourceTexture(elapsedMS).Height;
        }

        /// <summary>
        /// Returns a Rectangle object specifing the SourceRectangle
        /// to be used when drawing the SourceTexture specified with
        /// GetSourceTexture. Takes an elapsedMS parameter in the case
        /// where the Bitmap sublcass would return different SourceRectangles
        /// based on the current time (example Animation).
        /// </summary>
        public virtual Rectangle? GetSourceRectangle(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the SourceTexture from which all Draw operations will
        /// be performed from. Takes an elapsedMS parameter in the case where
        /// the Bitmap subclass would return different Textures based on
        /// the current time.
        /// </summary>
        public virtual Texture2D GetSourceTexture(double elapsedMS)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsFinished(double elapsedMS)
        {
            return true;
        }

        public void Draw(
            SpriteBatch spriteBatch,
            Rectangle destRectangle, 
            Color color, 
            float rotation, 
            Vector2 origin, 
            SpriteEffects spriteEffects, 
            float layerDepth,
            double elapsedMS
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
