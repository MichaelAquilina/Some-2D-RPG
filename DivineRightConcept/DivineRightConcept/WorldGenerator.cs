using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightConcept
{
    public class WorldGenerator
    {
        public int[][] GenerateWorld(int Width, int Height)
        {
            int[][] result = new int[Width][];
            Random randomGen = new Random();

            for (int i = 0; i < Width; i++)
                result[i] = new int[Height];

            //Generate Base
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    result[i][j] = Ground.GROUND_TEXTURE_GRASS;

            //Add Random Patches of Thundra
            int thundraCount = randomGen.Next(5, 8);
            for (int i = 0; i < thundraCount; i++)
            {
                int x = randomGen.Next(0, Width - 1);
                int y = randomGen.Next(0, Height - 1);

                result[x][y] = Ground.GROUND_TEXTURE_THUNDRA;
                result[x + 1][y + 1] = Ground.GROUND_TEXTURE_THUNDRA;
            }
            
            return result;
        }
    }
}
