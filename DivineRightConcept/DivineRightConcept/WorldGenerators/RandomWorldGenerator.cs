using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ShadowKill.GroundPallettes;
using GameEngine.GameObjects;
using GameEngine.Interfaces;

namespace ShadowKill.WorldGenerators
{
    //Purely Experimental Generator class used for debugging purposes
    public class RandomWorldGenerator : IWorldGenerator
    {
        public Map Generate(int Width, int Height)
        {
            GroundV2 ground = new GroundV2();
            Map result = new Map(Width, Height, ground);
            Random randomGen = new Random();

            //Generate Base
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    result[i, j] = GroundV2.GROUND_TEXTURE_GRASS;

            //Add Random Patches of Ground
            int thundraCount = randomGen.Next( Width * Height / 2, Width * Height);
            for (int i = 0; i < thundraCount; i++)
            {
                int x = randomGen.Next(0, Width);
                int y = randomGen.Next(0, Height);

                result[x, y] = Convert.ToByte(randomGen.Next(0, ground.TileCount - 1));
            }

            //DUMP MAP COORDINATES FOR DEBUGGING
            TextWriter writer = new StreamWriter("map_coord.txt");
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                    writer.Write(result[i, j].ToString());

                writer.WriteLine();
            }
            writer.Close();
            
            return result;
        }
    }
}
