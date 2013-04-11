using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Interfaces
{
    /// <summary>
    /// ILoadable interface for loading and unloading unmanaged Content.
    /// (TODO) This interface should be where development of an appropriate AssetManager should start
    /// </summary>
    public interface ILoadable
    {
        void LoadContent(ContentManager content);

        void UnloadContent();
    }
}
