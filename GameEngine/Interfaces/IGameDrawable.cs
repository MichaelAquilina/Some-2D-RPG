using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Info;

namespace GameEngine.Interfaces
{
    /// <summary>
    /// Defines an interface with which the TeeEngine would be capable of drawing the object to
    /// the screen.
    /// </summary>
    public interface IGameDrawable
    {
        Vector2 Origin { get; set; }

        int GetWidth(double elapsedMS);

        int GetHeight(double elapsedMS);

        bool IsFinished(double elapsedMS);

        void Draw(
            SpriteBatch spriteBatch, 
            Rectangle destRectangle, 
            Color color,
            float rotation, 
            Vector2 origin,
            SpriteEffects spriteEffects,
            float layerDepth,
            double elapsedMS
            );
    }
}
