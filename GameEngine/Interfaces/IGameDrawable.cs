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

        Rectangle GetSourceRectangle(double elapsedMS);
        Texture2D GetSourceTexture(double elapsedMS);

        bool IsFinished(double elapsedMS);

        void Draw(
            double elapsedMS, SpriteBatch spriteBatch,
            Rectangle destRectangle, Color color,
            float rotation, Vector2 origin,
            SpriteEffects spriteEffects,
            float layerDepth
            );
    }
}
