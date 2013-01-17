using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightConcept.Generators
{
    public interface IWorldGenerator
    {
        int[][] Generate(int Width, int Height);
    }
}
