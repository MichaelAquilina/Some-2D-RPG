using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ShadowKill.GroundPallettes;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShadowKill.WorldGenerators
{
    //Purely Experimental Generator class used for debugging purposes
    public class RandomWorldGenerator : IWorldGenerator
    {
        public Map Generate(ContentManager Content, int Width, int Height)
        {
            GroundV2 ground = new GroundV2();
            Map result = new Map(Width, Height, ground);
            Random randomGen = new Random();

            //Rectangle[] mapObjectSrcRectangles = new Rectangle[] { 
            //    new Rectangle(3, 13, 113, 103)
            //    ,new Rectangle(124, 4, 72, 109)
            //    ,new Rectangle(280,40, 30, 57)
            //    ,new Rectangle(320,80, 40, 32)
            //    //,new Rectangle(203,40, 73, 80)
            //};

            ////GENERATION AND STORAGE OF MAP OBJECTS SHOULD BE WITHIIN THE MAP FILE ITSELF (TODO)
            //Random random = new Random();
            //for (int i = 0; i < Width * Height / 7; i++)
            //{
            //    float treeX = (float)(random.NextDouble() * Width);
            //    float treeY = (float)(random.NextDouble() * Height);

            //    MapObject mapObject = new MapObject(treeX, treeY, 1.0f, 1.0f);

            //    mapObject.SourceRectangle = mapObjectSrcRectangles[random.Next(0, mapObjectSrcRectangles.Length)];

            //    mapObject.SourceTexture = Content.Load<Texture2D>(@"MapObjects\OBJECTS");
            //    mapObject.Origin = new Vector2(0.5f, 1.0f);

            //    result.MapObjects.Add(mapObject);
            //}

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
