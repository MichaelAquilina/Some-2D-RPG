using Microsoft.Xna.Framework.Content;

namespace GameEngine.Interfaces
{
    /// <summary>
    /// ILoadable interface for loading Content.
    /// </summary>
    public interface ILoadable
    {
        void LoadContent(ContentManager content);
    }
}
