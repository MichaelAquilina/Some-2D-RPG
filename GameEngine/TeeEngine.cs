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
using GameEngine.DataStructures;
using GameEngine.Info;
using System.Diagnostics;

/// <summary>
/// The TeeEngine - the result of my sweat, blood and tears into this project. The TeeEngine is simply a 2D Tile Engine that
/// provides a number of powerful tools and properties to quickly create and design 2D games that rely on tiles as coordinate
/// systems. The Name Tee Engine came from the idea of a TileEngine, ie a TEngine. I have a personal obsession with Tea, so changing
/// the name of the engine to TeeEngine places a bit of me into this project.
/// 
/// Using the TeeEngine is very simple:
/// 
/// TeeEngine engine = new TeeEngine(Game, 1024, 768);      //1024x768 resolution
/// engine.LoadMap("some_tiled_map.tmx");
/// 
/// engine.Entities.Add(entity1);
/// ...
/// 
/// engine.RegisterGameShader(shader1);
/// ...
///
///In the Context of this Game Engine, the following Coordinate units are used:
///        PX: Pixels
///        TX: Tixels (Tile units)
///        RX: Rexels (Relative Units)
///The above can be renamed as necessary in the future if need be.
///
///any coordinate property should have one of the above units prepended to thei name
///      -example: txWidth, pxHeight
///any coordinate functions should follow the same convention:
///      -example: GetPxBoundingBox()
///
///As a General Rule of thumb:
///     tx + tx = tx
///     px + px = px
///     tx * px = px
///     tx + px = INVALID
///     rx * px = px
///     rx + rx = rx
///     rx * tx = INVALID
/// </summary>
namespace GameEngine
{
    public class TeeEngine : GameComponent
    {
        #region Properties and Variables

        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Width resolution of the TeeEngine buffer in pixels.
        /// </summary>
        public int pxWidth { get; private set; }

        /// <summary>
        /// Height resolution of the TeeEngine buffer in pixels.
        /// </summary>
        public int pxHeight { get; private set; }

        /// <summary>
        /// Width of the tiles to render in Pixels.
        /// </summary>
        public int pxTileWidth { get; private set; }

        /// <summary>
        /// Height of the tiles to render in Pixels.
        /// </summary>
        public int pxTileHeight { get; private set; }

        /// <summary>
        /// Dictionary of all Entities currently active in the current Game World. Entities are 
        /// stored by their unique name
        /// </summary>
        public Dictionary<string, Entity> Entities { get; set; }

        /// <summary>
        /// List of all Entities on screen since the last DrawWorldViewPort call.
        /// </summary>
        public List<Entity> EntitiesOnScreen { get; private set; }

        /// <summary>
        /// Currently loaded TiledMap.
        /// </summary>
        public TiledMap Map { get; private set; }

        /// <summary>
        /// List of currently registered GameShaders in use by the TeeEngine.
        /// </summary>
        public List<GameShader> GameShaders { get; private set; }

        /// <summary>
        /// bool value specifying if the tile grid should be shown during render calls.
        /// </summary>
        public bool ShowTileGrid { get; set; }

        /// <summary>
        /// bool value specifying if the bounding boxes for entities should be shown during render calls.
        /// </summary>
        public bool ShowBoundingBoxes { get; set; }

        /// <summary>
        /// Current QuadTree built during the latest Update call.
        /// </summary>
        public QuadTree QuadTree { get; set; }

        /// <summary>
        /// Last GameTime instance when the TeeEngine was Updated.
        /// </summary>
        public GameTime LastUpdateTime { get; private set; }

        /// <summary>
        /// Debug Information that can be accessed by the player.
        /// </summary>
        public DebugInfo DebugInfo { get { return _debugInfo; } }

        RenderTarget2D _inputBuffer;
        RenderTarget2D _outputBuffer;
        RenderTarget2D _dummyBuffer;
        Stopwatch _watch1;              //primary diagnostic watch
        Stopwatch _watch2;              //secondary diagnostic watch
        DebugInfo _debugInfo;

        #endregion

        public TeeEngine(Game Game, int pxWidth, int pxHeight, int pxTileWidth, int pxTileHeight)
            :base(Game)
        {
            _debugInfo = new DebugInfo();
            _watch1 = new Stopwatch();
            _watch2 = new Stopwatch();

            GraphicsDevice = Game.GraphicsDevice;

            ShowTileGrid = false;
            ShowBoundingBoxes = false;
            EntitiesOnScreen = new List<Entity>();

            GameShaders = new List<GameShader>();
            Entities = new Dictionary<string, Entity>();

            SetResolution(pxWidth, pxHeight, pxTileWidth, pxTileHeight);
            Game.Components.Add(this);
        }

        public void LoadContent()
        {
            ContentManager Content = this.Game.Content;

            foreach (ILoadable loadableShader in GameShaders)
                loadableShader.LoadContent(Content);

            foreach (Entity entity in Entities.Values)
                entity.LoadContent(Content);
        }

        public void UnloadContent()
        {
            foreach (Entity entity in Entities.Values)
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

        public void LoadMap(TiledMap Map)
        {
            this.Map = Map;
            this.QuadTree = new QuadTree(Map.txWidth, Map.txHeight, pxTileWidth, pxTileHeight);
        }

        /// <summary>
        /// Sets the Resolution for Rendering the Game World. This is inately tied to the resolution the game
        /// will be rendered at. Internally, new render targets are created for both the viewport and the
        /// light map that will be used by DrawViewPort.
        /// </summary>
        /// <param name="pxWidth">int Width in pixels.</param>
        /// <param name="pxHeight">int Height in pixels.</param>
        /// <param name="pxTileHeight">int Height of Tiles to render in Pixels.</param>
        /// <param name="pxTileWidth">int Width of Tiles to render in Pixels.</param>
        public void SetResolution(int pxWidth, int pxHeight, int pxTileWidth, int pxTileHeight)
        {
            this.pxWidth = pxWidth;
            this.pxHeight = pxHeight;
            this.pxTileWidth = pxTileWidth;
            this.pxTileHeight = pxTileHeight;

            if (Map != null)
                QuadTree = new QuadTree(Map.txWidth, Map.txHeight, pxTileWidth, pxTileHeight);

            if (_outputBuffer != null)
                _outputBuffer.Dispose();

            if (_inputBuffer != null)
                _inputBuffer.Dispose();

            _inputBuffer = new RenderTarget2D(GraphicsDevice, pxWidth, pxHeight, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);
            _outputBuffer = new RenderTarget2D(GraphicsDevice, pxWidth, pxHeight, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);

            //allow all game shaders to become aware of the change in resolution
            foreach (GameShader shader in GameShaders) shader.SetResolution(pxWidth, pxHeight);
        }

        public override void Update(GameTime GameTime)
        {
            LastUpdateTime = GameTime;

            _watch1.Restart();
            foreach (string entityId in Entities.Keys)
            {
                Entity entity = Entities[entityId];

                _watch2.Restart();
                entity.Update(GameTime, this);
                DebugInfo.EntityUpdateTime[entityId] = _watch2.Elapsed;

                entity.CurrentPxBoundingBox = entity.GetPxBoundingBox(GameTime, pxTileWidth, pxTileHeight);  
            }
            _debugInfo.TotalEntityUpdateTime = _watch1.Elapsed;

            _watch1.Restart();
            QuadTree.Build(Entities.Values);
            _debugInfo.QuadTreeBuildTime = _watch1.Elapsed;
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
        public ViewPortInfo DrawWorldViewPort(SpriteBatch SpriteBatch, Vector2 txCenter, Rectangle pxDestRectangle, Color Color, SamplerState SamplerState)
        {
            //reset counters
            EntitiesOnScreen.Clear();

            //Should be fast to create due to being a struct
            ViewPortInfo viewPortInfo = new ViewPortInfo();             
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

            //DRAW THE WORLD MAP
            _watch1.Restart();
            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState, null, null);
            {
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
                                //TODO: Investigate this - this should belong in GameSpace
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

                                //DEBUGGING
                                //TODO: THIS IS VERY INEFFECIENT (mulitple draws in each layer for no reason)
                                if(ShowTileGrid) SpriteBatch.DrawRectangle(pxTileDestRect, Color.Black, 0);
                            }
                        }
                    }
                }
            }
            SpriteBatch.End();
            _debugInfo.TileRenderingTime = _watch1.Elapsed;


            //DRAW VISIBLE REGISTERED ENTITIES
            _watch1.Restart();
            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState, null, null);
            {
                foreach (string entityId in Entities.Keys)
                {
                    _watch2.Restart();

                    Entity entity = Entities[entityId];
                    entity.IsOnScreen = false;

                    if (!entity.Visible) continue;

                    Vector2 pxEntityPos = new Vector2(
                        (int) Math.Ceiling((entity.TX - viewPortInfo.txTopLeftX) * pxTileWidth),
                        (int) Math.Ceiling((entity.TY - viewPortInfo.txTopLeftY) * pxTileHeight)
                    );

                    Rectangle pxBoundingBox = entity.CurrentPxBoundingBox;
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
                        EntitiesOnScreen.Add(entity);
                        entity.IsOnScreen = true;

                        foreach (GameDrawableInstance drawable in entity.Drawables[entity.CurrentDrawable])
                        {
                            if (!drawable.Visible) continue;

                            //The relative position of the object should always be (X,Y) - (viewPortInfo.TopLeftX,viewPortInfo.TopLeftY) where viewPortInfo.TopLeftX and
                            //viewPortInfo.TopLeftY have already been corrected in terms of the bounds of the WORLD map coordinates. This allows
                            //for panning at the edges.
                            Rectangle pxCurrentFrame = drawable.Drawable.GetSourceRectangle(LastUpdateTime);

                            int pxObjectWidth = (int)(pxCurrentFrame.Width * entity.rxWidth);
                            int pxObjectHeight = (int)(pxCurrentFrame.Height * entity.rxHeight);

                            //Draw the Object based on the current Frame dimensions and the specified Object Width Height values
                            Rectangle objectDestRect = new Rectangle(
                                    (int)pxEntityPos.X,
                                    (int)pxEntityPos.Y,
                                    pxObjectWidth,
                                    pxObjectHeight
                            );

                            Vector2 drawableOrigin = drawable.Drawable.rxDrawOrigin * new Vector2(pxCurrentFrame.Width, pxCurrentFrame.Height);
                            Color drawableColor = new Color()
                            {
                                R = drawable.Color.R,
                                G = drawable.Color.G,
                                B = drawable.Color.B,
                                A = (byte)(drawable.Color.A * entity.Opacity)
                            };

                            //FIXME: Bug related to when layerDepth becomes small and reaches 0.99 for all levels, causing depth information to be lost
                            //layer depth should depend how far down the object is on the map (Relative to Y)
                            //Important to also take into account the animation layers for the entity
                            float layerDepth = Math.Min(0.99f, 1 / (entity.TY + ((float)drawable.Layer / pxTileHeight)));

                            SpriteBatch.Draw(
                                drawable.Drawable.GetSourceTexture(LastUpdateTime),
                                objectDestRect,
                                pxCurrentFrame,
                                drawableColor,
                                drawable.Rotation,
                                drawableOrigin,
                                drawable.SpriteEffects,
                                layerDepth);
                        }
                    }

                    _debugInfo.EntityRenderingTime[entityId] = _watch2.Elapsed;
                }
            }
            SpriteBatch.End();
            _debugInfo.TotalEntityRenderingTime = _watch1.Elapsed;

            _watch1.Restart();
            //APPLY GAME SHADERS TO THE RESULTANT IMAGE
            for (int i = 0; i < GameShaders.Count; i++)
            {
                if (GameShaders[i].Enabled)
                {
                    GameShaders[i].ApplyShader(SpriteBatch, viewPortInfo, LastUpdateTime, _inputBuffer, _outputBuffer);

                    //swap buffers after each render
                    _dummyBuffer = _inputBuffer;
                    _inputBuffer = _outputBuffer;
                    _outputBuffer = _dummyBuffer;
                }
            }
            _debugInfo.TotalGameShaderRenderTime = _watch1.Elapsed;

            //DRAW THE VIEWPORT TO THE STANDARD SCREEN
            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch.Begin();
            {
                SpriteBatch.Draw(_inputBuffer, pxDestRectangle, Color);
            }
            SpriteBatch.End();

            return viewPortInfo;
        }
    }
}
