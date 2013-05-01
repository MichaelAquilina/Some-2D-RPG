using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    }
}
