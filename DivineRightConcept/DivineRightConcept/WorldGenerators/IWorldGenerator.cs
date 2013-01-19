using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightConcept.WorldGenerators
{
    public interface IWorldGenerator
    {
        Map Generate(int Width, int Height);
    }
}
