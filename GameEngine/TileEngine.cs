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

//In the Context of this Game Engine, the following Coordinate units are used:
//        PX: Pixels
//        TX: Tixels (Tile units)
//        RX: Rexels (Relative Units)
//The above can be renamed as necessary in the future if need be.
//
//any coordinate property should have one of the above units prepended to thei name
//      -example: txWidth, pxHeight
//any coordinate functions should follow the same convention:
//      -example: GetPxBoundingBox()
//
//As a General Rule of thumb:
//     tx + tx = tx
//     px + px = px
//     tx * px = px
//     tx + px = INVALID
//     rx * px = px
//     rx + rx = rx
//     rx * tx = INVALID                 
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

        public int pxWidth { get; private set; }

        public int pxHeight { get; private set; }

        public int EntitiesOnScreen { get; private set; }

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
            EntitiesOnScreen = 0;

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
            Shader.SetResolution(pxWidth, pxHeight);
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
        /// <param name="pxWidth">int Width in pixels.</param>
        /// <param name="pxHeight">int Height in pixels.</param>
        public void SetResolution(int pxWidth, int pxHeight)
        {
            this.pxWidth = pxWidth;
            this.pxHeight = pxHeight;

            if (_outputBuffer != null)
                _outputBuffer.Dispose();

            if (_inputBuffer != null)
                _inputBuffer.Dispose();

            _inputBuffer = new RenderTarget2D(this.Game.GraphicsDevice, pxWidth, pxHeight, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);
            _outputBuffer = new RenderTarget2D(this.Game.GraphicsDevice, pxWidth, pxHeight, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);

            //allow all game shaders to become aware of the change in resolution
            foreach (GameShader shader in GameShaders) shader.SetResolution(pxWidth, pxHeight);
        }
        
        /// <summary>
        /// Draws a viewport of the current game world at the specified CenterX, CenterY location. The Viewport size and location on the screen must be 
        /// specified in the DestRectangle parameter. The number of Tiles both Width-wise and Height-wise should be specified in the TileWidth and TileHeight
        /// parameters. All Miscallaneous items and actors will be drawn on the screen, in an animated state (which depends on the values in the parameter
        /// passed in GameTime). This can allow for rewinding of time in terms of animation if needs be.
        /// </summary>
        /// <param name="GameTime">GameTime object that would have been passed to the standard XNA Draw method.</param>
        /// <param name="SpriteBatch">SpriteBatch object with which to render the Viewport. Should have already been opened for rendering.</param>
        /// <param name="txCenter">X and Y Coordinates on the world map specifying where the viewport should be Centered.</param>
        /// <param name="pxTileWidth">Integer value specifying the Width in pixels of each Tile on the Map.</param>
        /// <param name="pxTileHeight">Integer value specifying the Height in pixels of each Tile on the Map.</param>
        /// <param name="pxDestRectangle">Rectangle object specifying the render destination for the viewport. Should specify location, width and height.</param>
        /// <param name="Color">Color object with which to blend the game world.</param>
        /// <param name="SamplerState">Specifies the type of sampler to use when drawing images with the SpriteBatch object.</param>
        public void DrawWorldViewPort(GameTime GameTime, SpriteBatch SpriteBatch, Vector2 txCenter, int pxTileWidth, int pxTileHeight, Rectangle pxDestRectangle, Color Color, SamplerState SamplerState)
        {
            GraphicsDevice GraphicsDevice = this.Game.GraphicsDevice;

            //reset counters
            EntitiesOnScreen = 0;

            ViewPortInfo viewPortInfo = new ViewPortInfo();             //Should be fast due to being a struct
            {
                viewPortInfo.TileCountX = (int)Math.Ceiling((double)pxDestRectangle.Width / pxTileWidth) + 1;
                viewPortInfo.TileCountY = (int)Math.Ceiling((double)pxDestRectangle.Height / pxTileHeight) + 1;

                viewPortInfo.txTopLeftX = (float)(txCenter.X - Math.Ceiling((double)viewPortInfo.TileCountX / 2));
                viewPortInfo.txTopLeftY = (float)(txCenter.Y - Math.Ceiling((double)viewPortInfo.TileCountY / 2));

                viewPortInfo.pxTileWidth = pxTileWidth;
                viewPortInfo.pxTileHeight = pxTileHeight;

                //Prevent the View from going outisde of the WORLD coordinates
                if (viewPortInfo.txTopLeftX < 0) viewPortInfo.txTopLeftX = 0;
                if (viewPortInfo.txTopLeftY < 0) viewPortInfo.txTopLeftY = 0;
                if (viewPortInfo.txTopLeftX + viewPortInfo.TileCountX >= Map.txWidth) viewPortInfo.txTopLeftX = Map.txWidth - viewPortInfo.TileCountX;
                if (viewPortInfo.txTopLeftY + viewPortInfo.TileCountY >= Map.txHeight) viewPortInfo.txTopLeftY = Map.txHeight - viewPortInfo.TileCountY;

                //calculate any decimal displacement required (For Positions with decimal points)
                viewPortInfo.txDispX = viewPortInfo.txTopLeftX - Math.Floor(viewPortInfo.txTopLeftX);
                viewPortInfo.txDispY = viewPortInfo.txTopLeftY - Math.Floor(viewPortInfo.txTopLeftY);
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
                            int tileX = (int)(i + viewPortInfo.txTopLeftX);
                            int tileY = (int)(j + viewPortInfo.txTopLeftY);

                            int tileGid = tileLayer[tileX, tileY];

                            if (tileGid != 0)   //NULL Tile Gid is ignored
                            {
                                Tile tile = Map.Tiles[tileGid];
                                Rectangle pxTileDestRect = new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight);

                                //traslate if there is any decimal displacement due to a Center with a floating point
                                pxTileDestRect.X -= (int)(viewPortInfo.txDispX * pxTileWidth);
                                pxTileDestRect.Y -= (int)(viewPortInfo.txDispY * pxTileHeight);

                                //automatic layering based on layerIndex
                                float depth = tileLayer.HasProperty("Foreground")? 0 : 1 - (layerIndex / 10000.0f);

                                SpriteBatch.Draw(
                                    tile.SourceTexture,
                                    pxTileDestRect,
                                    tile.SourceRectangle,
                                    Color.White,
                                    0, Vector2.Zero,
                                    SpriteEffects.None,
                                    depth    
                                );
                            }
                        }
                    }
                }

                //DRAW VISIBLE REGISTERED ENTITIES
                foreach (Entity entity in Entities)
                {
                    entity.IsOnScreen = false;

                    if (!entity.Visible) continue;

                    Vector2 pxEntityPos = new Vector2(
                        (int) Math.Ceiling((entity.TX - viewPortInfo.txTopLeftX) * pxTileWidth),
                        (int) Math.Ceiling((entity.TY - viewPortInfo.txTopLeftY) * pxTileHeight)
                    );

                    Rectangle pxBoundingBox = entity.GetPxBoundingBox(GameTime, pxTileWidth, pxTileHeight);
                    pxBoundingBox = new Rectangle(
                        (int) Math.Ceiling(pxBoundingBox.X - viewPortInfo.txTopLeftX * pxTileWidth),
                        (int) Math.Ceiling(pxBoundingBox.Y - viewPortInfo.txTopLeftY * pxTileHeight), 
                        pxBoundingBox.Width, pxBoundingBox.Height
                    );

                    if (ShowBoundingBoxes)
                    {
                        SpriteBatch.DrawRectangle(pxBoundingBox, Color.Red, 0f);
                        SpriteBatch.DrawCross(pxEntityPos, 13, Color.Black, 0f);
                    }

                    if (pxBoundingBox.Intersects(_inputBuffer.Bounds))
                    {
                        EntitiesOnScreen++;
                        entity.IsOnScreen = true;


                        foreach (GameDrawableInstance drawable in entity.Drawables[entity.CurrentDrawable])
                        {
                            if (!drawable.Visible) continue;

                            //The relative position of the object should always be (X,Y) - (viewPortInfo.TopLeftX,viewPortInfo.TopLeftY) where viewPortInfo.TopLeftX and
                            //viewPortInfo.TopLeftY have already been corrected in terms of the bounds of the WORLD map coordinates. This allows
                            //for panning at the edges.
                            Rectangle pxCurrentFrame = drawable.Drawable.GetSourceRectangle(GameTime);

                            int pxObjectWidth = (int)(pxCurrentFrame.Width * entity.rxWidth);
                            int pxObjectHeight = (int)(pxCurrentFrame.Height * entity.rxHeight);

                            //Draw the Object based on the current Frame dimensions and the specified Object Width Height values
                            Rectangle objectDestRect = new Rectangle(
                                    (int) pxEntityPos.X,
                                    (int) pxEntityPos.Y,
                                    pxObjectWidth,
                                    pxObjectHeight
                            );

                            Vector2 drawableOrigin = drawable.Drawable.rxDrawOrigin * new Vector2(pxCurrentFrame.Width, pxCurrentFrame.Height);

                            //FIXME: Bug related to when layerDepth becomes small and reaches 0.99 for all levels, causing depth information to be lost
                            //layer depth should depend how far down the object is on the map (Relative to Y)
                            //Important to also take into account the animation layers for the entity
                            float layerDepth = Math.Min(0.99f, 1 / (entity.TY + ((float)drawable.Layer / pxTileHeight)));

                            SpriteBatch.Draw(
                                drawable.Drawable.GetSourceTexture(GameTime),
                                objectDestRect,
                                pxCurrentFrame,
                                drawable.Color,
                                drawable.Rotation,
                                drawableOrigin,
                                drawable.SpriteEffects,
                                layerDepth);
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
                SpriteBatch.Draw(_inputBuffer, pxDestRectangle, Color);
            }
            SpriteBatch.End();
        }

        #endregion
    }
}
