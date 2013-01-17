using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightConcept.Generators
{
    public class RandomWorldGenerator : IWorldGenerator
    {
        public int[][] Generate(int Width, int Height)
        {
            int[][] result = new int[Width][];
            Random randomGen = new Random();

            for (int i = 0; i < Width; i++)
                result[i] = new int[Height];

            //Generate Base
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    result[i][j] = Ground.GROUND_TEXTURE_GRASS;

            //Add Random Patches of Ground
            int thundraCount = randomGen.Next(0, Width * Height);
            for (int i = 0; i < thundraCount; i++)
            {
                int x = randomGen.Next(0, Width);
                int y = randomGen.Next(0, Height);

                result[x][y] = randomGen.Next(0, Ground.TextureColors.Length - 1);
            }
            
            return result;
        }
    }
}
