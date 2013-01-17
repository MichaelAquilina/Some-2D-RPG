using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using DivineRightConcept.Generators;
using DivineRightConcept.GameObjects;

namespace DivineRightConcept
{
    /// <summary>
    /// Class that represents the current state of the game world, including the Actors residing in it. Provides functions
    /// to draw/render the current state of the world. No Game Logic should occur within this state class apart from 
    /// initial generation (POSSIBLY).
    /// </summary>
    public class GameWorld : GameComponent
    {
        #region Variables

        public int WorldWidth { get; private set; }
        public int WorldHeight { get; private set; }

        public Texture2D GroundTextures { get; set; }

        //interface for retreiving the generated minimap
        public Texture2D MiniMapTexture { get { return _miniMap; }}
        public List<Actor> Actors { get; private set; }

        //contains actual tile information about the world map
        private int[][] _worldMap;

        //minimap texture generated
        private Texture2D _miniMap;

        //ground rendering class
        private Ground _ground;

        #endregion

        public GameWorld(Game Game, int WorldWidth, int WorldHeight)
            :base(Game)
        {
            this.WorldWidth = WorldWidth;
            this.WorldHeight = WorldHeight;
        }

        public override void Initialize()
        {
            Actors = new List<Actor>();

            _ground = new Ground();

            RandomWorldGenerator generator = new RandomWorldGenerator();
            _worldMap = generator.Generate(WorldWidth, WorldHeight);

            //create a small scale map for the user (The MiniMap)
            Color[] mapColors = new Color[WorldWidth * WorldHeight];
            _miniMap = new Texture2D(Game.GraphicsDevice, WorldWidth, WorldHeight, false, SurfaceFormat.Color);
            for (int i = 0; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                    mapColors[j * _worldMap.Length + i] = Ground.TextureColors[_worldMap[i][j]];
            _miniMap.SetData<Color>(mapColors);

            base.Initialize();
        }

        /// <summary>
        /// Provide an interace which allows the GameWorld to load the necessary content. Should ask all its Actors, Items or anything else
        /// to also Load their Content so that they may also be rendered correctly when it comes to being drawn.
        /// </summary>
        public void LoadContent()
        {
            _ground.LoadContent(Game.Content);

            foreach (Actor actor in Actors)
                actor.LoadContent(Game.Content);
        }

        /// <summary>
        /// Draws a viewport of the current game world at the specified CenterX, CenterY location. The Viewport size and location on the screen must be 
        /// specified in the DestRectangle parameter. The number of Tiles both Width-wise and Height-wise should be specified in the TileWidth and TileHeight
        /// parameters. TEMP, PlayerX and PlayerY coordinates are shown. These should be removed at a later stage when such a value would be inbuilt within this
        /// class and rendered according to whether or not it is within the viewport.
        /// </summary>
        /// <param name="SpriteBatch">SpriteBatch object with which to render the Viewport. Should have already been opened for rendering.</param>
        /// <param name="Center">X and Y Coordinates on the world map specifying where the to be drawn viewport should be rendered.</param>
        /// <param name="TileWidth">Number of Tiles, Width-wise that should be shown within the viewport.</param>
        /// <param name="TileHeight">Number of Tiles, Height-wise that should be shown within the viewport.</param>
        /// <param name="DestRectangle">Rectangle object specifying the render destination for the viewport. Should specify location, width and height.</param>
        public void DrawWorldViewPort(SpriteBatch SpriteBatch, Vector2 Center, int TileWidth, int TileHeight, Rectangle DestRectangle)
        {
            //DRAW THE WORLD MAP
            int pxTileWidth = DestRectangle.Width / TileWidth;
            int pxTileHeight = DestRectangle.Height / TileHeight;

            //determine the topleft world coordinate in the view
            int topLeftX = (int) (Center.X - Math.Ceiling((double)TileWidth/2));
            int topLeftY = (int) (Center.Y - Math.Ceiling((double)TileHeight/2));

            //Prevent the View from going outisde of the WORLD coordinates
            if (topLeftX < 0) topLeftX = 0;
            if (topLeftY < 0) topLeftY = 0;
            if (topLeftX + TileWidth >= WorldWidth) topLeftX = WorldWidth - TileWidth;
            if (topLeftY + TileHeight >= WorldHeight) topLeftY = WorldHeight - TileHeight;

            //draw each tile
            for (int i = 0; i < TileWidth; i++)
                for (int j = 0; j < TileHeight; j++)
                {
                    int tileX = i + topLeftX;
                    int tileY = j + topLeftY;

                    Rectangle tileDestRect = new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight);
                    tileDestRect.X += DestRectangle.X;
                    tileDestRect.Y += DestRectangle.Y;

                    _ground.DrawGroundTexture(SpriteBatch, _worldMap[tileX][tileY], tileDestRect);
                }

            foreach (Actor actor in Actors)
            {
                //DRAW THE ACTOR
                //The relative position of the character should always be (X,Y) - (topLeftX,TopLeftY) where topLeftX and topLeftY have already been corrected
                //in terms of the bounds of the WORLD map coordinates. This allows for panning at the edges.
                SpriteBatch.Draw(actor.Representation, new Rectangle(
                    (actor.X - topLeftX) * pxTileWidth + DestRectangle.X,
                    (actor.Y - topLeftY) * pxTileHeight + DestRectangle.Y,
                    pxTileWidth, pxTileHeight),
                    Color.White);
            }
        }
    }
}
