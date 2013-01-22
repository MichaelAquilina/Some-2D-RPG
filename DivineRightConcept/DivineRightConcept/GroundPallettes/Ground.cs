using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameEngine.Drawing;
using GameEngine.Interfaces;
using GameEngine.GameObjects;

namespace DivineRightConcept.GroundPallettes
{
    //Standard Ground class containing all information on the Ground Texture and extension methods for Drawing
    //Ground Textures on Sprite Batches
    //Can be thought of as the Ground Rendering Engine
    public class Ground : IGroundPallette
    {
        public const int GROUND_TEXTURE_WIDTH = 128;
        public const int GROUND_TEXTURE_HEIGHT = 128;

        public const int GROUND_TEXTURE_SOIL = 0;
        public const int GROUND_TEXTURE_ROCK = 1;
        public const int GROUND_TEXTURE_PEBBLES = 2;
        public const int GROUND_TEXTURE_GRASS = 3;
        public const int GROUND_TEXTURE_THUNDRA = 4;
        public const int GROUND_TEXTURE_NONE = 5;
        public const int GROUND_TEXTURE_WATER = 6;

        //texture colors that correspond to each type. The index for each color should corrspond to the assigned const value defined above
        public static Color[] TextureColors = new Color[] {Color.Brown, Color.Gray, Color.DarkGray, Color.Green, Color.Yellow, Color.Black, Color.LightBlue};

        public Texture2D GroundTexture { get; private set; }
        public int TileCount { get { return TextureColors.Length; } }

        public Color GetTileColor(byte TileType)
        {
            return Ground.TextureColors[TileType];
        }

        //Load the Required Textures for the Ground Rendering Engine
        public void LoadContent(ContentManager Content)
        {
            this.GroundTexture = Content.Load<Texture2D>("GroundTextures");
        }

        public void UnloadContent()
        {
            this.GroundTexture.Dispose();
        }

        //CONSIDER REMOVING STATIC EXTENSION AND USING THIS AS A CLASS
        public void DrawGroundTexture(SpriteBatch SpriteBatch, Map GameMap, int X, int Y, Rectangle DestinationRectangle, FRectangle SourceRectangle)
        {
            //int i = (GroundType % 3);
            //int j = (GroundType / 3);

            //SpriteBatch.Draw(
            //    GroundTexture,
            //    DestinationRectangle,
            //    new Rectangle(
            //        i * GROUND_TEXTURE_WIDTH + (int) (SourceRectangle.X * GROUND_TEXTURE_WIDTH),
            //        j * GROUND_TEXTURE_HEIGHT + (int) (SourceRectangle.Y * GROUND_TEXTURE_HEIGHT),
            //        (int) (SourceRectangle.Width * GROUND_TEXTURE_WIDTH),
            //        (int) (SourceRectangle.Height * GROUND_TEXTURE_HEIGHT)
            //    ),
            //    Color.White
            //);
        }
    }
}
