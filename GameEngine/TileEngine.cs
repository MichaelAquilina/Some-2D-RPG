using System;
using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using GameEngine.Shaders;
using GameEngine.Tiled;
using GameEngine.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    /// <summary>
    /// Class that represents the current state of the game world, including the Entities residing in it. Provides functions
    /// to draw/render the current state of the world, as well as other draw functions such as drawing a MiniMap version
    /// of the current WorldMap.
    /// </summary>
    public class TileEngine : GameComponent
    {
        #region Properties

        public int PixelWidth { get; private set; }

        public int PixelHeight { get; private set; }

        public int DrawablesOnScreen { get; private set; }

        public TiledMap Map { get; private set; }

        public List<GameShader> GameShaders { get; private set; }

        //needs to be converted to TiledObjects
        public List<Entity> Entities { get; set; }

        public bool ShowBoundingBoxes { get; set; }

        #endregion

        #region Variables

        RenderTarget2D _inputBuffer;
        RenderTarget2D _outputBuffer;
        RenderTarget2D _dummyBuffer;

        #endregion

        #region Initialisation

        public TileEngine(Game Game, int PixelWidth, int PixelHeight)
            :base(Game)
        {
            ShowBoundingBoxes = false;
            DrawablesOnScreen = 0;

            GameShaders = new List<GameShader>();
            Entities = new List<Entity>();

            SetResolution(PixelWidth, PixelHeight);
            Game.Components.Add(this);
        }

        public void LoadContent()
        {
            ContentManager Content = this.Game.Content;

            foreach (ILoadable loadableShader in GameShaders)
                loadableShader.LoadContent(Content);

            foreach (Entity entity in Entities)
                entity.LoadContent(Content);
        }

        public void UnloadContent()
        {
            foreach (Entity entity in Entities)
                entity.UnloadContent();

            if (_inputBuffer != null)
                _inputBuffer.Dispose();

            if (_outputBuffer != null)
                _outputBuffer.Dispose();

            _inputBuffer = null;
            _outputBuffer = null;

            foreach (ILoadable loadableShader in GameShaders)
                loadableShader.UnloadContent();
        }

        #endregion

        #region Register/Unregister methods

        public bool IsRegistered(GameShader Shader)
        {
            return GameShaders.Contains(Shader);
        }

        public void RegisterGameShader(GameShader Shader)
        {
            GameShaders.Add(Shader);
            Shader.LoadContent(this.Game.Content);
            Shader.SetResolution(PixelWidth, PixelHeight);
        }

        public bool UnregisterGameShader(GameShader Shader)
        {
            Shader.UnloadContent();
            return GameShaders.Remove(Shader);
        }

        #endregion

        #region Update Methods

        public override void Update(GameTime GameTime)
        {
            foreach (Entity entity in Entities)
                entity.Update(GameTime, Map);
        }

        #endregion

        #region Public API Methods

        public void LoadMap(TiledMap Map)
        {
            this.Map = Map;
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
            PixelWidth = Width;
            PixelHeight = Height;

            if (_outputBuffer != null)
                _outputBuffer.Dispose();

            if (_inputBuffer != null)
                _inputBuffer.Dispose();

            _inputBuffer = new RenderTarget2D(this.Game.GraphicsDevice, Width, Height, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);
            _outputBuffer = new RenderTarget2D(this.Game.GraphicsDevice, Width, Height, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);

            //allow all game shaders to become aware of the change in resolution
            foreach (GameShader shader in GameShaders) shader.SetResolution(Width, Height);
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
        /// <param name="pxTileWidth">Integer value specifying the Width in pixels of each Tile on the Map.</param>
        /// <param name="pxTileHeight">Integer value specifying the Height in pixels of each Tile on the Map.</param>
        /// <param name="DestRectangle">Rectangle object specifying the render destination for the viewport. Should specify location, width and height.</param>
        /// <param name="Color">Color object with which to blend the game world.</param>
        /// <param name="SamplerState">Specifies the type of sampler to use when drawing images with the SpriteBatch object.</param>
        public void DrawWorldViewPort(GameTime GameTime, SpriteBatch SpriteBatch, Vector2 Center, int pxTileWidth, int pxTileHeight, Rectangle DestRectangle, Color Color, SamplerState SamplerState)
        {
            GraphicsDevice GraphicsDevice = this.Game.GraphicsDevice;

            //reset counters
            DrawablesOnScreen = 0;

            ViewPortInfo viewPortInfo = new ViewPortInfo();             //Should be fast due to being a struct
            {
                viewPortInfo.TileCountX = (int)Math.Ceiling((double)DestRectangle.Width / pxTileWidth) + 1;
                viewPortInfo.TileCountY = (int)Math.Ceiling((double)DestRectangle.Height / pxTileHeight) + 1;

                viewPortInfo.TopLeftX = (float)(Center.X - Math.Ceiling((double)viewPortInfo.TileCountX / 2));
                viewPortInfo.TopLeftY = (float)(Center.Y - Math.Ceiling((double)viewPortInfo.TileCountY / 2));

                viewPortInfo.PXTileWidth = pxTileWidth;
                viewPortInfo.PXTileHeight = pxTileHeight;

                //Prevent the View from going outisde of the WORLD coordinates
                if (viewPortInfo.TopLeftX < 0) viewPortInfo.TopLeftX = 0;
                if (viewPortInfo.TopLeftY < 0) viewPortInfo.TopLeftY = 0;
                if (viewPortInfo.TopLeftX + viewPortInfo.TileCountX >= Map.Width) viewPortInfo.TopLeftX = Map.Width - viewPortInfo.TileCountX;
                if (viewPortInfo.TopLeftY + viewPortInfo.TileCountY >= Map.Height) viewPortInfo.TopLeftY = Map.Height - viewPortInfo.TileCountY;

                //calculate any decimal displacement required (For Positions with decimal points)
                viewPortInfo.dispX = viewPortInfo.TopLeftX - Math.Floor(viewPortInfo.TopLeftX);
                viewPortInfo.dispY = viewPortInfo.TopLeftY - Math.Floor(viewPortInfo.TopLeftY);
            }

            //RENDER THE GAME WORLD TO THE VIEWPORT RENDER TARGET
            GraphicsDevice.SetRenderTarget(_inputBuffer);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState, null, null);
            {
                //DRAW THE WORLD MAP
                for (int layerIndex = 0; layerIndex < Map.TileLayers.Count; layerIndex++)
                {
                    //DRAW EACH LAYER
                    TileLayer tileLayer = Map.TileLayers[layerIndex];

                    for (int i = 0; i < viewPortInfo.TileCountX; i++)
                    {
                        for (int j = 0; j < viewPortInfo.TileCountY; j++)
                        {
                            int tileX = (int)(i + viewPortInfo.TopLeftX);
                            int tileY = (int)(j + viewPortInfo.TopLeftY);

                            int tileGid = tileLayer[tileX, tileY];

                            if (tileGid != 0)   //NULL Tile Gid is ignored
                            {
                                Tile tile = Map.Tiles[tileGid];
                                Rectangle tileDestRect = new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight);

                                //traslate if there is any decimal displacement due to a Center with a floating point
                                tileDestRect.X -= (int)(viewPortInfo.dispX * pxTileWidth);
                                tileDestRect.Y -= (int)(viewPortInfo.dispY * pxTileHeight);

                                float depth = tileLayer.HasProperty("Foreground")? 0 : 1 - (layerIndex / 10000.0f);

                                SpriteBatch.Draw(
                                    tile.SourceTexture,
                                    tileDestRect,
                                    tile.SourceRectangle,
                                    Color.White,
                                    0, Vector2.Zero,
                                    SpriteEffects.None,
                                    depth    //automatic layering based on layerIndex
                                );
                            }
                        }
                    }
                }

                //DRAW VISIBLE REGISTERED ENTITIES
                foreach (Entity entity in Entities)
                {
                    if (!entity.Visible) continue;

                    foreach (GameDrawableInstance drawable in entity.Drawables[entity.CurrentDrawable])
                    {
                        if(!drawable.Visible) continue;

                        //The relative position of the object should always be (X,Y) - (viewPortInfo.TopLeftX,viewPortInfo.TopLeftY) where viewPortInfo.TopLeftX and
                        //viewPortInfo.TopLeftY have already been corrected in terms of the bounds of the WORLD map coordinates. This allows
                        //for panning at the edges.
                        Rectangle currentFrame = drawable.Drawable.GetSourceRectangle(GameTime);

                        int objectX = (int)Math.Ceiling((entity.X - viewPortInfo.TopLeftX) * pxTileWidth);
                        int objectY = (int)Math.Ceiling((entity.Y - viewPortInfo.TopLeftY) * pxTileHeight);

                        int objectWidth = (int)(currentFrame.Width * entity.Width);
                        int objectHeight = (int)(currentFrame.Height * entity.Height);

                        //Draw the Object based on the current Frame dimensions and the specified Object Width Height values
                        Rectangle objectDestRect = new Rectangle(
                                objectX,
                                objectY,
                                objectWidth,
                                objectHeight
                        );

                        Vector2 drawableOrigin = drawable.Drawable.DrawOrigin * new Vector2(currentFrame.Width, currentFrame.Height);

                        //Calculate the Origin of the Object, as well as its Bounding Box
                        Rectangle objectBoundingBox = new Rectangle(
                            (int)Math.Ceiling(objectDestRect.X - drawableOrigin.X * entity.Width),
                            (int)Math.Ceiling(objectDestRect.Y - drawableOrigin.Y * entity.Height),
                            objectDestRect.Width,
                            objectDestRect.Height
                        );

                        //only render the object if the objects BoundingBox it is within the specified viewport
                        if (objectBoundingBox.Intersects(_inputBuffer.Bounds))
                        {
                            DrawablesOnScreen++;

                            //Draw the Bounding Box and a Cross indicating the Origin
                            //TODO, this should technically be in Game Space, not Engine Space
                            if (entity.BoundingBoxVisible || this.ShowBoundingBoxes)
                            {
                                SpriteBatch.DrawCross(new Vector2(objectDestRect.X, objectDestRect.Y), 7, Color.Black, 0);
                                SpriteBatch.DrawRectangle(objectBoundingBox, Color.Red, 0.001f);
                            }

                            //FIXME: Bug related to when layerDepth becomes small and reaches 0.99 for all levels, causing depth information to be lost
                            //layer depth should depend how far down the object is on the map (Relative to Y)
                            //Important to also take into account the animation layers for the entity
                            float layerDepth = Math.Min(0.99f, 1 / (entity.Y + ((float) drawable.Layer/pxTileHeight)));

                            SpriteBatch.Draw(
                                drawable.Drawable.GetSourceTexture(GameTime),
                                objectDestRect,
                                currentFrame,
                                drawable.Color,
                                drawable.Rotation,
                                drawableOrigin,
                                drawable.SpriteEffects,
                                layerDepth );        
                        }
                    }
                }
            }
            SpriteBatch.End();

            //APPLY GAME SHADERS TO THE RESULTANT IMAGE
            for (int i = 0; i < GameShaders.Count; i++)
            {
                if (GameShaders[i].Enabled)
                {
                    GameShaders[i].ApplyShader(SpriteBatch, viewPortInfo, GameTime, _inputBuffer, _outputBuffer);

                    //swap buffers after each render
                    _dummyBuffer = _inputBuffer;
                    _inputBuffer = _outputBuffer;
                    _outputBuffer = _dummyBuffer;
                }
            }

            //DRAW THE VIEWPORT TO THE STANDARD SCREEN
            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch.Begin();
            {
                SpriteBatch.Draw(_inputBuffer, DestRectangle, Color);
            }
            SpriteBatch.End();
        }

        #endregion
    }
}
