using Microsoft.Xna.Framework.Content;

namespace GameEngine.Interfaces
{
    /// <summary>
    /// Defines an interface with which the TeeEngine is capable of loading resource content from
    /// the content manager when it is required from the object.
    /// </summary>
    public interface ILoadable
    {
        void LoadContent(ContentManager content);
    }
}
