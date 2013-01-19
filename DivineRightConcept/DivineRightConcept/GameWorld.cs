using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using DivineRightConcept.WorldGenerators;
using DivineRightConcept.GameObjects;

namespace DivineRightConcept
{
    /// <summary>
    /// Class that represents the current state of the game world, including the Actors residing in it. Provides functions
    /// to draw/render the current state of the world, as well as other draw functions such as drawing a MiniMap version
    /// of the current WorldMap.
    /// </summary>
    public class GameWorld : GameComponent
    {
        #region Variables

        public Map WorldMap {
            get { return _worldMap; }
            set { _mipMapTex = null; _worldMap = value; }
        }
        public List<Actor> Actors { get; private set; }

        private Map _worldMap;              //World Map Instance
        private Texture2D _mipMapTex;       //Cached copy of the MipMapTexture

        #endregion

        public GameWorld(Game Game)
            :base(Game)
        {
        }

        private Texture2D GenerateMipMapTexture(Map Map)
        {
            //GENERATE THE MINIMAP TEXTURE
            Color[] mapColors = new Color[Map.Width * Map.Height];
            Texture2D resultTexture = new Texture2D(this.Game.GraphicsDevice, Map.Width, Map.Height, false, SurfaceFormat.Color);

            for (int i = 0; i < Map.Width; i++)
                for (int j = 0; j < Map.Height; j++)
                    mapColors[j * Map.Width + i] = Map.GroundPallette.GetTileColor(Map[i, j]);

            resultTexture.SetData<Color>(mapColors);

            return resultTexture;
        }

        public override void Initialize()
        {
            Actors = new List<Actor>();

            base.Initialize();
        }

        /// <summary>
        /// Provide an interace which allows the GameWorld to load the necessary content. Should ask all its Actors, Items or anything else
        /// to also Load their Content so that they may also be rendered correctly when it comes to being drawn.
        /// </summary>
        public void LoadContent()
        {
            this.WorldMap.GroundPallette.LoadContent(Game.Content);

            foreach (Actor actor in Actors)
                actor.LoadContent(Game.Content);
        }

        public void DrawMipMap(SpriteBatch SpriteBatch, Rectangle DestRectangle)
        {
            //CHECK CACHED COPY
            if (_mipMapTex == null)
                _mipMapTex = GenerateMipMapTexture(this.WorldMap);

            SpriteBatch.Draw(_mipMapTex, DestRectangle, Color.White);
        }

        /// <summary>
        /// Draws a viewport of the current game world at the specified CenterX, CenterY location. The Viewport size and location on the screen must be 
        /// specified in the DestRectangle parameter. The number of Tiles both Width-wise and Height-wise should be specified in the TileWidth and TileHeight
        /// parameters. 
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
            if (topLeftX + TileWidth >= WorldMap.Width) topLeftX = WorldMap.Width - TileWidth;
            if (topLeftY + TileHeight >= WorldMap.Height) topLeftY = WorldMap.Height - TileHeight;

            //draw each tile
            for (int i = 0; i < TileWidth; i++)
                for (int j = 0; j < TileHeight; j++)
                {
                    int tileX = i + topLeftX;
                    int tileY = j + topLeftY;

                    Rectangle tileDestRect = new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight);
                    tileDestRect.X += DestRectangle.X;
                    tileDestRect.Y += DestRectangle.Y;

                    this.WorldMap.GroundPallette.DrawGroundTexture(SpriteBatch, WorldMap[tileX, tileY], tileDestRect);
                }

            //DRAW THE ACTORS
            foreach (Actor actor in Actors)
            {
                //The relative position of the character should always be (X,Y) - (topLeftX,TopLeftY) where topLeftX and
                //topLeftY have already been corrected in terms of the bounds of the WORLD map coordinates. This allows
                //for panning at the edges.
                Rectangle actorDestRect = new Rectangle(
                        (actor.X - topLeftX) * pxTileWidth + DestRectangle.X,
                        (actor.Y - topLeftY) * pxTileHeight + DestRectangle.Y,
                        pxTileWidth, pxTileHeight);

                //only render the actor if he is within the specified viewport
                if (DestRectangle.Contains(actorDestRect))
                    SpriteBatch.Draw(actor.Representation, actorDestRect, Color.White);
            }
        }
    }
}
