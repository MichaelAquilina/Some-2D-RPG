using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using DivineRightConcept.WorldGenerators;
using DivineRightConcept.GameObjects;
using DivineRightConcept.Drawing;

namespace DivineRightConcept
{
    /// <summary>
    /// Class that represents the current state of the game world, including the Actors residing in it. Provides functions
    /// to draw/render the current state of the world, as well as other draw functions such as drawing a MiniMap version
    /// of the current WorldMap.
    /// </summary>
    /// TODO: Possibly convert this class from a GameComponent into an ILoadable
    public class GameWorld : GameComponent
    {
        #region Variables

        public Map WorldMap {
            get { return _worldMap; }
            set { _mipMapTex = null; _worldMap = value; }       //clear the cached mipmap
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

        public void UnloadContent()
        {
            this.WorldMap.GroundPallette.UnloadContent();
            this._mipMapTex.Dispose();
            this._mipMapTex = null;
        }

        /// <summary>
        /// Draws a Minitaure version of the current WorldMap as a Texture on the screen, specified by the DestRectangle parameter. The minimap
        /// will have 1 pixel for each tile on the Worldmap. The color should be roughly represantative of what texture the tile woudld show
        /// on the location of the map - although this is entirely dependant on the GroundPallette being used (influenced by the GetTileColor
        /// interface method). On first map load, the MipMap will have to be generated, but subsequent calls to this method will use a cached
        /// version of the mimimap to prevent excess overhead during draw time.
        /// </summary>
        /// <param name="SpriteBatch">An open SpriteBatch object with which to Draw the MiniMap.</param>
        /// <param name="DestRectangle">A Rectangle specifying the Destination on the screen where the MiniMap should be drawn.</param>
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
        /// <param name="Center">X and Y Coordinates on the world map specifying where the viewport should be Centered.</param>
        /// <param name="TileWidth">Integer value specifying the Width in pixels of each Tile on the Map.</param>
        /// <param name="TileHeight">Integer value specifying the Height in pixels of each Tile on the Map.</param>
        /// <param name="DestRectangle">Rectangle object specifying the render destination for the viewport. Should specify location, width and height.</param>
        public void DrawWorldViewPort(SpriteBatch SpriteBatch, Vector2 Center, int pxTileWidth, int pxTileHeight, Rectangle DestRectangle)
        {
            //DEBUGGING: draw backgrond underneath viewport so we know when it is out of position
            SpriteBatch.DrawRectangle(DestRectangle, Color.Red);

            //DRAW THE WORLD MAP
            int TileCountX = (int) Math.Ceiling( (double) DestRectangle.Width / pxTileWidth ) + 1;
            int TileCountY = (int) Math.Ceiling( (double) DestRectangle.Height / pxTileHeight ) + 1;

            //determine the topleft world coordinate in the view
            float topLeftX = (float) (Center.X - Math.Ceiling((double)TileCountX/2));
            float topLeftY = (float) (Center.Y - Math.Ceiling((double)TileCountY/2));

            //Prevent the View from going outisde of the WORLD coordinates
            if (topLeftX < 0) topLeftX = 0;
            if (topLeftY < 0) topLeftY = 0;
            if (topLeftX + TileCountX >= WorldMap.Width) topLeftX = WorldMap.Width - TileCountX;
            if (topLeftY + TileCountY >= WorldMap.Height) topLeftY = WorldMap.Height - TileCountY;

            //calculate any decimal displacement required (For Positions with decimal points)
            double dispX = topLeftX - Math.Floor(topLeftX);
            double dispY = topLeftY - Math.Floor(topLeftY);

            //draw each tile
            for (int i = 0; i < TileCountX; i++)
                for (int j = 0; j < TileCountY; j++)
                {
                    int tileX = (int) (i + topLeftX);
                    int tileY = (int) (j + topLeftY);

                    Rectangle tileDestRect = new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight);
                    FRectangle tileSrcRect = new FRectangle(0.0f, 0.0f, 1.0f, 1.0f);

                    //translate according to the destination rectangle
                    tileDestRect.X += DestRectangle.X;
                    tileDestRect.Y += DestRectangle.Y;

                    //traslate if there is any decimal displacement due to a Center with a floating point
                    tileDestRect.X -= (int)(dispX * pxTileWidth);
                    tileDestRect.Y -= (int)(dispY * pxTileHeight);

                    //HANDLE BORDER EDGE CULLING
                    //TODO MAKE THIS NEATER, THIS IS VERY HACKISH
                    //CONDITIONAL BRANCHES SHOULD BE AVOIDED, KEEP EVERYTHING MATHEMATICAL WHERE POSSIBLE!
                    if (i == 0)
                    {
                        int prevWidth = tileDestRect.Width;
                        tileDestRect.Width = (int) Math.Ceiling(tileDestRect.Width * (1.0f - dispX));
                        tileDestRect.X += (prevWidth - tileDestRect.Width);
                        tileSrcRect.Width = (float) (1.0f - dispX);
                        tileSrcRect.X += (1.0f - tileSrcRect.Width);
                    }

                    if (j == 0)
                    {
                        int prevHeight = tileDestRect.Height;
                        tileDestRect.Height = (int) Math.Ceiling(tileDestRect.Height * (1.0f - dispY));
                        tileDestRect.Y += (prevHeight - tileDestRect.Height);
                        tileSrcRect.Height = (float)(1.0f - dispY);
                        tileSrcRect.Y += (1.0f - tileSrcRect.Height);
                    }

                    if (i == TileCountX - 1)
                    {
                        tileDestRect.Width = (int)Math.Ceiling(tileDestRect.Width * dispX);
                        tileSrcRect.Width = (float)(dispX);
                    }

                    if (j == TileCountY - 1)
                    {
                        tileDestRect.Height = (int)Math.Ceiling(tileDestRect.Height * dispY);
                        tileSrcRect.Height = (float)(dispY);
                    }

                    this.WorldMap.GroundPallette.DrawGroundTexture(SpriteBatch, WorldMap[tileX, tileY], tileDestRect, tileSrcRect);
                }

            //DRAW THE ACTORS
            foreach (Actor actor in Actors)
            {
                //The relative position of the character should always be (X,Y) - (topLeftX,TopLeftY) where topLeftX and
                //topLeftY have already been corrected in terms of the bounds of the WORLD map coordinates. This allows
                //for panning at the edges.
                int actorX = (int) ((actor.X - topLeftX) * pxTileWidth);
                int actorY = (int) ((actor.Y - topLeftY) * pxTileHeight);

                Rectangle actorDestRect = new Rectangle(
                        actorX + DestRectangle.X,
                        actorY + DestRectangle.Y,
                        pxTileWidth, pxTileHeight);

                //only render the actor if he is within the specified viewport
                if (DestRectangle.Contains(actorDestRect))
                    SpriteBatch.Draw(actor.Representation, actorDestRect, Color.White);
            }
        }
    }
}
