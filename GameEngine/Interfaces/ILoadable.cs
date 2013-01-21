using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Interfaces
{
    public interface ILoadable
    {
        void LoadContent(ContentManager Content);

        void UnloadContent();
    }
}
