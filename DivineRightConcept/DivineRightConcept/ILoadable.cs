using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace DivineRightConcept
{
    public interface ILoadable
    {
        void LoadContent(ContentManager Content);

        void UnloadContent();
    }
}
