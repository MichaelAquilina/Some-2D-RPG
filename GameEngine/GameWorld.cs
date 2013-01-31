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
using Microsoft.Xna.Framework.Content;

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
        #region Properties

        public Map WorldMap {
            get { return _worldMap; }
            set
            {
                //clear cached copy
                if (_miniMapTex != null)
                    _miniMapTex.Dispose();
                _miniMapTex = null;
                _worldMap = value;
            }
        }

        public Texture2D LightMap
        {
            get { return (Texture2D)_lightRenderTarget; }
        }

        public Texture2D ViewPort
        {
            get { return (Texture2D)_viewPortTarget; }
        }

        public float LightValue { get; set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int ObjectsOnScreen { get; private set; }

        public List<IGameDrawable> DrawableObjects { get; private set; }

        public bool ShowBoundingBoxes { get; set; }

        #endregion

        #region Variables

        private Map _worldMap;                  //World Map Instance
        private Texture2D _miniMapTex;           //Cached copy of the MipMapTexture

        //TEMP (TO REMOVE)
        Effect _alphaShader;
        Texture2D _lightSource;

        RenderTarget2D _lightRenderTarget;
        RenderTarget2D _viewPortTarget;

        #endregion

        public GameWorld(Game Game, int Width, int Height)
            :base(Game)
        {
            LightValue = 0.8f;
            ShowBoundingBoxes = false;
            SetResolution(Width, Height);
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
            ContentManager Content = this.Game.Content;
            GraphicsDevice GraphicsDevice = this.Game.GraphicsDevice;

            _alphaShader = Content.Load<Effect>("Alpha");
            _lightSource = Content.Load<Texture2D>(@"MapObjects/LightSource2");

            this.WorldMap.GroundPallette.LoadContent(Game.Content);
        }

        public void UnloadContent()
        {
            this.WorldMap.GroundPallette.UnloadContent();
            if (_miniMapTex != null)
                _miniMapTex.Dispose();

            if (_lightRenderTarget != null)
                _lightRenderTarget.Dispose();

            if (_viewPortTarget != null)
                _viewPortTarget.Dispose();

            _miniMapTex = null;
            _lightRenderTarget = null;
            _viewPortTarget = null;
        }

        /// <summary>
        /// Sets the Resolution for Rendering the Game World. This is inately tied to the resolution the game
        /// will be rendered at. Internally, new render targets are created for both the viewport and the
        /// light map that will be used by DrawViewPort.
        /// </summary>
        /// <param name="Width">int Width in pixels.</param>
        /// <param name="Height">int Height in pixels.</param>
        public void SetResolution(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;

            if (_lightRenderTarget != null)
                _lightRenderTarget.Dispose();

            if (_viewPortTarget != null)
                _viewPortTarget.Dispose();

            _lightRenderTarget = new RenderTarget2D(this.Game.GraphicsDevice, Width, Height);
            _viewPortTarget = new RenderTarget2D(this.Game.GraphicsDevice, Width, Height);
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
            if (_miniMapTex == null)
                _miniMapTex = GenerateMipMapTexture(this.WorldMap);

            SpriteBatch.Begin();
            SpriteBatch.Draw(_miniMapTex, DestRectangle, Color.White);
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
        /// <param name="Color">Color object with which to blend the game world.</param>
        public void DrawWorldViewPort(GameTime GameTime, SpriteBatch SpriteBatch, Vector2 Center, int pxTileWidth, int pxTileHeight, Rectangle DestRectangle, Color Color)
        {
            GraphicsDevice GraphicsDevice = this.Game.GraphicsDevice;

            //RENDER THE LIGHT MAP TO A DESTINATION TEXTURE
            GraphicsDevice.SetRenderTarget(_lightRenderTarget);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            {
                SpriteBatch.Draw(_lightSource, new Rectangle(10, 50, 100, 100), Color.White);
                SpriteBatch.Draw(_lightSource, new Rectangle(10, 50, 200, 200), Color.White);
                SpriteBatch.Draw(_lightSource, new Rectangle(100, 160, 400, 400), Color.White);
            }
            SpriteBatch.End();

            //RENDER THE GAME WORLD TO THE VIEWPORT RENDER TARGET
            GraphicsDevice.SetRenderTarget(_viewPortTarget);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            {
                //determine the amount of tiles to be draw on the viewport
                int TileCountX = (int)Math.Ceiling((double)DestRectangle.Width / pxTileWidth) + 1;
                int TileCountY = (int)Math.Ceiling((double)DestRectangle.Height / pxTileHeight) + 1;

                //determine the topleft world coordinate in the view
                float topLeftX = (float)(Center.X - Math.Ceiling((double)TileCountX / 2));
                float topLeftY = (float)(Center.Y - Math.Ceiling((double)TileCountY / 2));

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
                        int tileX = (int)(i + topLeftX);
                        int tileY = (int)(j + topLeftY);

                        Rectangle tileDestRect = new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight);

                        //traslate if there is any decimal displacement due to a Center with a floating point
                        tileDestRect.X -= (int)(dispX * pxTileWidth);
                        tileDestRect.Y -= (int)(dispY * pxTileHeight);

                        SpriteBatch.Draw(
                            this.WorldMap.GroundPallette.GetTileSourceTexture(this.WorldMap[tileX, tileY]),
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
                            objectX,
                            objectY,
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
                    if (drawObject.Visible && ObjectBoundingBox.Intersects(new Rectangle(0,0,Width,Height)) )
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

                SpriteBatch.End();

                //DRAW THE VIEWPORT TO THE STANDARD SCREEN
                GraphicsDevice.SetRenderTarget(null);
                SpriteBatch.Begin();
                {
                    SpriteBatch.Draw((Texture2D)_viewPortTarget, DestRectangle, Color);
                }
                SpriteBatch.End();

                //DRAW THE LIGHT MAP TO THE STANDARD SCREEN
                _alphaShader.Parameters["LightValue"].SetValue(LightValue);
                SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, _alphaShader);
                {
                    SpriteBatch.Draw((Texture2D)_lightRenderTarget, DestRectangle, Color.White);
                }
                SpriteBatch.End();

            }
        }
    }
}
