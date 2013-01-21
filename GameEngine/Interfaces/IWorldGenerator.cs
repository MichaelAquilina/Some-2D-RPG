using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;

namespace GameEngine.Interfaces
{
    public interface IWorldGenerator
    {
        Map Generate(int Width, int Height);
    }
}
