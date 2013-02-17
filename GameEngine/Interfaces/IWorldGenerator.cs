using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Interfaces
{
    public interface IWorldGenerator
    {
        Map Generate(ContentManager Content, int Width, int Height);
    }
}
