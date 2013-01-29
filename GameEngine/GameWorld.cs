using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
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

        public List<IGameDrawable> DrawableObjects { get; private set; }

        public bool ShowBoundingBoxes { get; set; }

        private Map _worldMap;                  //World Map Instance
        private Texture2D _mipMapTex;           //Cached copy of the MipMapTexture

        #endregion

        public GameWorld(Game Game)
            :base(Game)
        {
            ShowBoundingBoxes = false;
        }

        /// <summary>
        /// Generates a minitiure version of the specified Map into a texture and returns it. Each pixel
        /// within the returned texture would correspond to one tile on the Map (based on the information
        /// returned by the GroundPallette being used by the Map).
        /// </summary>
        /// <param name="Map">Map with which to generate the MipMap.</param>
        /// <returns>Generated MiniMap Texture.</returns>
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
            DrawableObjects = new List<IGameDrawable>();

            base.Initialize();
        }

        /// <summary>
        /// Provide an interace which allows the GameWorld to load the necessary content. Should ask all its Actors, Items or anything else
        /// to also Load their Content so that they may also be rendered correctly when it comes to being drawn.
        /// </summary>
        public void LoadContent()
        {
            this.WorldMap.GroundPallette.LoadContent(Game.Content);
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

            SpriteBatch.Begin();
            SpriteBatch.Draw(_mipMapTex, DestRectangle, Color.White);
            SpriteBatch.End();
        }

        /// <summary>
        /// Draws a viewport of the current game world at the specified CenterX, CenterY location. The Viewport size and location on the screen must be 
        /// specified in the DestRectangle parameter. The number of Tiles both Width-wise and Height-wise should be specified in the TileWidth and TileHeight
        /// parameters. All Miscallaneous items and actors will be drawn on the screen, in an animated state (which depends on the values in the parameter
        /// passed in GameTime). This can allow for rewinding of time in terms of animation if needs be.
        /// </summary>
        /// <param name="GameTime">GameTime object that would have been passed to the standard XNA Draw method.</param>
        /// <param name="SpriteBatch">SpriteBatch object with which to render the Viewport. Should have already been opened for rendering.</param>
        /// <param name="Center">X and Y Coordinates on the world map specifying where the viewport should be Centered.</param>
        /// <param name="TileWidth">Integer value specifying the Width in pixels of each Tile on the Map.</param>
        /// <param name="TileHeight">Integer value specifying the Height in pixels of each Tile on the Map.</param>
        /// <param name="DestRectangle">Rectangle object specifying the render destination for the viewport. Should specify location, width and height.</param>
        /// <param name="ObjectsOnScreen">output variable that returns the number of objects that are currently being drawn on screen. Mainly used for debugging purposes</param>
        public void DrawWorldViewPort(GameTime GameTime, SpriteBatch SpriteBatch, Vector2 Center, int pxTileWidth, int pxTileHeight, Rectangle DestRectangle, out int ObjectsOnScreen)
        {
            

            Rectangle PrevScissorRectangle = SpriteBatch.GraphicsDevice.ScissorRectangle;
            SpriteBatch.GraphicsDevice.ScissorRectangle = DestRectangle;

            SpriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null, null,
                new RasterizerState() { ScissorTestEnable = true });
            {
                //determine the amount of tiles to be draw on the viewport
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

                //DRAW THE WORLD MAP TILES
                for (int i = 0; i < TileCountX; i++)
                    for (int j = 0; j < TileCountY; j++)
                    {
                        int tileX = (int) (i + topLeftX);
                        int tileY = (int) (j + topLeftY);

                        Rectangle tileDestRect = new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight);

                        //translate according to the destination rectangle
                        tileDestRect.X += DestRectangle.X;
                        tileDestRect.Y += DestRectangle.Y;

                        //traslate if there is any decimal displacement due to a Center with a floating point
                        tileDestRect.X -= (int)(dispX * pxTileWidth);
                        tileDestRect.Y -= (int)(dispY * pxTileHeight);

                        SpriteBatch.Draw(
                            this.WorldMap.GroundPallette.SourceTexture,
                            tileDestRect,
                            this.WorldMap.GroundPallette.GetTileSourceRectangle(this.WorldMap[tileX, tileY]), 
                            Color.White,
                            0, Vector2.Zero,
                            SpriteEffects.None,
                            1
                        );
                    }

                ObjectsOnScreen = 0;

                //DRAW THE IGAMEDRAWABLE COMPONENTS (Actors, MapObjects, etc...)
                //TODO: Probably Possible to reduce the complexity of this method (Due to the way BoundingBox is calculated)
                foreach (IGameDrawable drawObject in DrawableObjects)
                {
                    //The relative position of the object should always be (X,Y) - (topLeftX,TopLeftY) where topLeftX and
                    //topLeftY have already been corrected in terms of the bounds of the WORLD map coordinates. This allows
                    //for panning at the edges.
                    Rectangle ObjectSrcRect = drawObject.GetSourceRectangle(GameTime);                    
                    
                    int objectX = (int)Math.Ceiling((drawObject.X - topLeftX) * pxTileWidth);
                    int objectY = (int)Math.Ceiling((drawObject.Y - topLeftY) * pxTileHeight);

                    int objectWidth = (int)(ObjectSrcRect.Width * drawObject.Width);
                    int objectHeight = (int)(ObjectSrcRect.Height * drawObject.Height);

                    //Draw the Object based on the current Frame dimensions and the specified Object Width Height values
                    Rectangle ObjectDestRect = new Rectangle(
                            objectX + DestRectangle.X,
                            objectY + DestRectangle.Y,
                            objectWidth,
                            objectHeight
                    );

                    //Calculate the Origin of the Object, as well as its Bounding Box
                    Vector2 objectOrigin = drawObject.Origin * new Vector2(ObjectSrcRect.Width, ObjectSrcRect.Height);
                    Rectangle ObjectBoundingBox = new Rectangle(
                        (int)Math.Ceiling(ObjectDestRect.X - objectOrigin.X * drawObject.Width),
                        (int)Math.Ceiling(ObjectDestRect.Y - objectOrigin.Y * drawObject.Height),
                        ObjectDestRect.Width,
                        ObjectDestRect.Height
                    );

                    //only render the object if the objects BoundingBox it is within the specified viewport
                    if (drawObject.Visible && DestRectangle.Intersects(ObjectBoundingBox))
                    {
                        ObjectsOnScreen++;

                        //Draw the Bounding Box and a Cross indicating the Origin
                        if (drawObject.BoundingBoxVisible || this.ShowBoundingBoxes)
                        {
                            SpriteBatch.DrawCross(new Vector2(ObjectDestRect.X, ObjectDestRect.Y), 7, Color.Black, 0);
                            SpriteBatch.DrawRectangle(ObjectBoundingBox, Color.Red, 0.001f);
                        }

                        SpriteBatch.Draw(
                            drawObject.GetTexture(GameTime),
                            ObjectDestRect,
                            ObjectSrcRect,
                            drawObject.DrawColor,
                            drawObject.Rotation,
                            objectOrigin,
                            SpriteEffects.None,
                            Math.Min(0.99f, 1 / drawObject.Y));        //layer depth should depend how far down the object is on the map (Relative to Y)
                    }
                }

                SpriteBatch.End();                                                      //End Sprite Batch using custom settings
                SpriteBatch.GraphicsDevice.ScissorRectangle = PrevScissorRectangle;     //Restore Previously applied Scissor Rectangle
            }
        }
    }
}
