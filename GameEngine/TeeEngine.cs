using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameEngine.DataStructures;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Info;
using GameEngine.Interfaces;
using GameEngine.Shaders;
using GameEngine.Tiled;
using GameEngine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// The TeeEngine - the result of my sweat, blood and tears into this project. The TeeEngine is simply a 2D Tile Engine that
/// provides a number of powerful tools and properties to quickly create and design 2D games that rely on tiles as coordinate
/// systems. The Name Tee Engine came from the idea of a TileEngine, ie a TEngine. I have a personal obsession with Tea, so changing
/// the name of the engine to TeeEngine embeds a bit of my personality into this project.
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
/// </summary>
namespace GameEngine
{
    public class TeeEngine : GameComponent
    {
        #region Properties and Variables

        /// <summary>
        /// Graphics Device being used by this Engine to Render.
        /// </summary>
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
        /// List of all Entities on screen since the last DrawWorldViewPort call.
        /// </summary>
        public List<Entity> EntitiesOnScreen { get; private set; }

        /// <summary>
        /// Currently loaded TiledMap.
        /// </summary>
        public TiledMap Map { get; private set; }

        /// <summary>
        /// Returns a Collection of all the Entities that have been added to this Engine.
        /// </summary>
        public ICollection<Entity> Entities { get { return _entities.Values; } }

        /// <summary>
        /// List of currently registered GameShaders in use by the TeeEngine.
        /// </summary>
        public List<GameShader> GameShaders { get; private set; }

        /// <summary>
        /// Show Entity debug information when drawing the world view port to the screen.
        /// </summary>
        public bool ShowEntityDebugInfo { get; set; }

        /// <summary>
        /// Shows the QuadTrees bounding boxes when drawing the world viewport.
        /// </summary>
        public bool ShowQuadTree { get; set; }

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
        public DebugInfo DebugInfo { get; private set; }

        Dictionary<string, Entity> _entities;
        RenderTarget2D _inputBuffer;
        RenderTarget2D _outputBuffer;
        RenderTarget2D _dummyBuffer;

        //TODO: Might be smarter to create a class for handling these
        Stopwatch _watch1;              //primary diagnostic watch
        Stopwatch _watch2;              //secondary diagnostic watch
        Stopwatch _watch3;              //tertiary diagnostic watch
        
        int _entityIdCounter = 0;       //used for automatic assigning of IDs

        #endregion

        public TeeEngine(Game Game, int pxWidth, int pxHeight)
            :base(Game)
        {
            _watch1 = new Stopwatch();
            _watch2 = new Stopwatch();
            _watch3 = new Stopwatch();
            _entities = new Dictionary<string, Entity>();

            GraphicsDevice = Game.GraphicsDevice;

            ShowQuadTree = false;
            ShowEntityDebugInfo = false;
            ShowTileGrid = false;
            ShowBoundingBoxes = false;
            EntitiesOnScreen = new List<Entity>();

            DebugInfo = new DebugInfo();
            GameShaders = new List<GameShader>();

            SetResolution(pxWidth, pxHeight);
            Game.Components.Add(this);
        }

        public void LoadContent()
        {
            ContentManager Content = this.Game.Content;

            foreach (ILoadable loadableShader in GameShaders)
                loadableShader.LoadContent(Content);

            foreach (Entity entity in _entities.Values)
                entity.LoadContent(Content);
        }

        public void UnloadContent()
        {
            foreach (Entity entity in _entities.Values)
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

        public void LoadMap(TiledMap Map)
        {
            this.Map = Map;
            this.QuadTree = new QuadTree(Map.txWidth, Map.txHeight, Map.pxTileWidth, Map.pxTileHeight);
        }

        public void SetResolution(int pxWidth, int pxHeight)
        {
            this.pxWidth = pxWidth;
            this.pxHeight = pxHeight;

            if (Map != null)
                QuadTree = new QuadTree(Map.txWidth, Map.txHeight, Map.pxTileWidth, Map.pxTileHeight);

            if (_outputBuffer != null)
                _outputBuffer.Dispose();

            if (_inputBuffer != null)
                _inputBuffer.Dispose();

            _inputBuffer = new RenderTarget2D(GraphicsDevice, pxWidth, pxHeight, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);
            _outputBuffer = new RenderTarget2D(GraphicsDevice, pxWidth, pxHeight, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);

            //allow all game shaders to become aware of the change in resolution
            foreach (GameShader shader in GameShaders) shader.SetResolution(pxWidth, pxHeight);
        }

        #region Entity Related Functions

        public void AddEntity(Entity Entity)
        {
            AddEntity(null, Entity);
        }

        public void AddEntity(string Name, Entity Entity)
        {
            //Assign an automatic, unique entity name if none is supplied
            if (Name == null) Name = string.Format("Entity{0}", _entityIdCounter++);

            _entities.Add(Name, Entity);
            Entity.Name = Name;
            Entity.requiresAddition = true;
        }

        public Entity GetEntity(string Name)
        {
            return _entities[Name];
        }

        public void RemoveEntity(string Name)
        {
            if (_entities.ContainsKey(Name))
            {
                QuadTree.Root.Remove(_entities[Name], null);
                _entities[Name].Name = null;
                _entities.Remove(Name);
            }
        }

        #endregion

        #region Shader Related Functions

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

        public override void Update(GameTime GameTime)
        {
            LastUpdateTime = GameTime;
            _watch3.Reset();
            _watch1.Restart();

            foreach (string entityId in _entities.Keys)
            {
                Entity entity = _entities[entityId];
                entity.prevPxBoundingBox = entity.CurrentPxBoundingBox;

                //perform any per-entity update logic
                _watch2.Restart();
                {
                    entity.Update(GameTime, this);
                    entity.CurrentPxBoundingBox = entity.GetPxBoundingBox(GameTime);
                }
                DebugInfo.EntityUpdateTimes[entityId] = _watch2.Elapsed;

                if (entity.requiresAddition)
                {
                    QuadTree.Add(entity);
                    entity.prevPxBoundingBox = entity.CurrentPxBoundingBox;
                    entity.requiresAddition = false;
                }

                //reset the IsOnScreen variable before the next drawing operation
                entity.IsOnScreen = false;

                //if the entity has moved, then update his position in the QuadTree
                _watch3.Start();
                {
                    if (entity.CurrentPxBoundingBox != entity.prevPxBoundingBox)
                        QuadTree.Update(entity);
                }
                _watch3.Stop();
            }

            DebugInfo.TotalEntityUpdateTime = _watch1.Elapsed;
            DebugInfo.QuadTreeUpdateTime = _watch3.Elapsed;
        }

        #region Drawing Code

        private void DrawQuadTree(ViewPortInfo viewPort, SpriteBatch SpriteBatch, QuadTreeNode Node, Rectangle DestRectangle, SpriteFont SpriteFont)
        {
            if (Node == null) return;

            float absoluteX = Node.pxBounds.X - viewPort.pxTopLeftX;
            float absoluteY = Node.pxBounds.Y - viewPort.pxTopLeftY;

            int actualX = (int)Math.Ceiling(absoluteX * viewPort.ActualZoom);
            int actualY = (int)Math.Ceiling(absoluteY * viewPort.ActualZoom);

            //We need to calculate the 'Actual' width and height otherwise drawing might be innacurate when zoomed
            int actualWidth = (int)Math.Ceiling(((absoluteX + Node.pxBounds.Width) * viewPort.ActualZoom) - actualX);
            int actualHeight = (int)Math.Ceiling(((absoluteY + Node.pxBounds.Height) * viewPort.ActualZoom) - actualY);

            //Only draw leaf nodes which are within the viewport specified
            if (Node.Node1 == null 
                && new Rectangle(actualX, actualY, actualWidth, actualHeight).Intersects(DestRectangle))
            {
                string nodeText = Node.NodeID.ToString();

                SpriteBatch.DrawRectangle(new Rectangle(actualX, actualY, actualWidth, actualHeight), Color.Lime, 0);
                SpriteBatch.DrawString(
                    SpriteFont,
                    nodeText,
                    new Vector2(actualX + actualWidth / 2.0f, actualY + actualHeight / 2.0f) - SpriteFont.MeasureString(nodeText) / 2,
                    Color.Lime
                );
            }

            DrawQuadTree(viewPort, SpriteBatch, Node.Node1, DestRectangle, SpriteFont);
            DrawQuadTree(viewPort, SpriteBatch, Node.Node2, DestRectangle, SpriteFont);
            DrawQuadTree(viewPort, SpriteBatch, Node.Node3, DestRectangle, SpriteFont);
            DrawQuadTree(viewPort, SpriteBatch, Node.Node4, DestRectangle, SpriteFont);
        }
        
        public ViewPortInfo DrawWorldViewPort(SpriteBatch SpriteBatch, float pxCenterX, float pxCenterY, float Zoom, Rectangle pxDestRectangle, Color Color, SamplerState SamplerState, SpriteFont SpriteFont=null)
        {
            ViewPortInfo viewPortInfo = new ViewPortInfo();
            {
                viewPortInfo.pxTileWidth  = (int) Math.Round(Map.pxTileWidth * Zoom);
                viewPortInfo.pxTileHeight = (int) Math.Round(Map.pxTileHeight * Zoom);

                //Note about ActualZoom Property:
                //because there is a loss of data between to conversion from Map.pxTileWidth * Zoom -> (int)
                //we need to determine what was the actual level of zoom that was applied to the tiles and use that
                //this ensures that entities that will be drawn will be placed correctly on the screen
                viewPortInfo.ActualZoom = viewPortInfo.pxTileWidth / Map.pxTileWidth;

                viewPortInfo.pxWidth = pxDestRectangle.Width / viewPortInfo.ActualZoom;
                viewPortInfo.pxHeight = pxDestRectangle.Height / viewPortInfo.ActualZoom;

                viewPortInfo.pxTopLeftX = pxCenterX - viewPortInfo.pxWidth / 2.0f;
                viewPortInfo.pxTopLeftY = pxCenterY - viewPortInfo.pxHeight / 2.0f;

                viewPortInfo.TileCountX = (int) Math.Ceiling((double)viewPortInfo.pxWidth / Map.pxTileWidth) + 1;
                viewPortInfo.TileCountY = (int) Math.Ceiling((double)viewPortInfo.pxHeight / Map.pxTileHeight) + 1;

                //Prevent the View from going outisde of the WORLD coordinates
                if (viewPortInfo.pxTopLeftX < 0) viewPortInfo.pxTopLeftX = 0;
                if (viewPortInfo.pxTopLeftY < 0) viewPortInfo.pxTopLeftY = 0;

                if (viewPortInfo.pxTopLeftX + viewPortInfo.pxWidth >= Map.pxWidth)
                    viewPortInfo.pxTopLeftX = Map.pxWidth - viewPortInfo.pxWidth;
                if (viewPortInfo.pxTopLeftY + viewPortInfo.pxHeight >= Map.pxHeight)
                    viewPortInfo.pxTopLeftY = Map.pxHeight - viewPortInfo.pxHeight;

                //calculate any decimal displacement required (For Positions with decimal points)
                viewPortInfo.pxDispX = viewPortInfo.pxTopLeftX - ((int)viewPortInfo.pxTopLeftX / Map.pxTileWidth) * Map.pxTileWidth;
                viewPortInfo.pxDispY = viewPortInfo.pxTopLeftY - ((int)viewPortInfo.pxTopLeftY / Map.pxTileHeight) * Map.pxTileHeight;

                viewPortInfo.pxViewPortBounds = new Rectangle(
                    (int) Math.Ceiling(viewPortInfo.pxTopLeftX),
                    (int) Math.Ceiling(viewPortInfo.pxTopLeftY),
                    (int) Math.Ceiling(viewPortInfo.pxWidth),
                    (int) Math.Ceiling(viewPortInfo.pxHeight)
                );
            }

            //RENDER THE GAME WORLD TO THE VIEWPORT RENDER TARGET
            GraphicsDevice.SetRenderTarget(_inputBuffer);
            GraphicsDevice.Clear(Color.Black);

            //DRAW THE WORLD MAP
            _watch1.Restart();

            //Deferred Rendering should be fine for rendering tile as long as we draw tile layers one at a time
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState, null, null);
            {
                for (int layerIndex = 0; layerIndex < Map.TileLayers.Count; layerIndex++)
                {
                    //DRAW EACH LAYER
                    TileLayer tileLayer = Map.TileLayers[layerIndex];
                    float depth = 1 - (layerIndex / 10000.0f);

                    for (int i = 0; i < viewPortInfo.TileCountX; i++)
                    {
                        for (int j = 0; j < viewPortInfo.TileCountY; j++)
                        {
                            int tileX = (int)(i + viewPortInfo.pxTopLeftX / Map.pxTileWidth);
                            int tileY = (int)(j + viewPortInfo.pxTopLeftY / Map.pxTileHeight);

                            int tileGid = tileLayer[tileX, tileY];

                            Rectangle pxTileDestRect = new Rectangle(
                                (int) Math.Ceiling(i * viewPortInfo.pxTileWidth - viewPortInfo.pxDispX * viewPortInfo.ActualZoom),
                                (int) Math.Ceiling(j * viewPortInfo.pxTileHeight - viewPortInfo.pxDispY * viewPortInfo.ActualZoom),
                                (int) viewPortInfo.pxTileWidth,
                                (int) viewPortInfo.pxTileHeight
                            );

                            if (tileGid != 0 && tileGid != -1)   //NULL or INVALID Tile Gid is ignored
                            {
                                Tile tile = Map.Tiles[tileGid];

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

                            //DRAW THE TILE LAYER GRID IF ENABLE
                            if (ShowTileGrid && layerIndex == Map.TileLayers.Count - 1)
                                SpriteBatch.DrawRectangle(pxTileDestRect, Color.Black, 0);
                        }
                    }
                }
            }
            SpriteBatch.End();
            DebugInfo.TileRenderingTime = _watch1.Elapsed;

            //Calculate the entity Displacement caused by pxTopLeft at a global scale to prevent jittering
            float entityDispX = (int) Math.Ceiling(viewPortInfo.pxTopLeftX * viewPortInfo.ActualZoom);
            float entityDispY = (int) Math.Ceiling(viewPortInfo.pxTopLeftY * viewPortInfo.ActualZoom);

            //DRAW VISIBLE REGISTERED ENTITIES
            _watch1.Restart();
            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState, null, null);
            {
                EntitiesOnScreen = QuadTree.GetIntersectingEntites(viewPortInfo.pxViewPortBounds);

                foreach (Entity entity in EntitiesOnScreen)
                {
                    _watch2.Restart();

                    if (!entity.Visible) continue;
                    entity.IsOnScreen = true;

                    Vector2 pxAbsEntityPos = new Vector2(
                        entity.PX * viewPortInfo.ActualZoom - entityDispX,
                        entity.PY * viewPortInfo.ActualZoom - entityDispY
                    );

                    //BUG: Due to float nature of PX, PY, the bounding box can be very very innacurate when drawing on the screen
                    //We need to find a way round this because it is not ideal if we intend to allow entities to move using floating point accuracy
                    Rectangle pxAbsBoundingBox = new Rectangle(
                        (int) Math.Ceiling(entity.CurrentPxBoundingBox.X * viewPortInfo.ActualZoom - entityDispX),
                        (int) Math.Ceiling(entity.CurrentPxBoundingBox.Y * viewPortInfo.ActualZoom - entityDispY),
                        (int) Math.Ceiling(entity.CurrentPxBoundingBox.Width * viewPortInfo.ActualZoom),
                        (int) Math.Ceiling(entity.CurrentPxBoundingBox.Height * viewPortInfo.ActualZoom)
                    );

                    //DRAW ENTITY BOUNDING BOXES IF ENABLED
                    if (ShowBoundingBoxes)
                    {
                        SpriteBatch.DrawRectangle(new Rectangle(
                            pxAbsBoundingBox.X,
                            pxAbsBoundingBox.Y,
                            pxAbsBoundingBox.Width,
                            pxAbsBoundingBox.Height)
                            , Color.Red, 0f);
                        SpriteBatch.DrawCross(new Vector2(pxAbsEntityPos.X, pxAbsEntityPos.Y), 13, Color.Black, 0f);
                    }

                    foreach (GameDrawableInstance drawable in entity.Drawables[entity.CurrentDrawable])
                    {
                        if (!drawable.Visible) continue;

                        //The relative position of the object should always be (X,Y) - (viewPortInfo.TopLeftX,viewPortInfo.TopLeftY) where viewPortInfo.TopLeftX and
                        //viewPortInfo.TopLeftY have already been corrected in terms of the bounds of the WORLD map coordinates. This allows
                        //for panning at the edges.
                        Rectangle pxCurrentFrame = drawable.Drawable.GetSourceRectangle(LastUpdateTime);

                        int pxObjectWidth  = (int) Math.Ceiling(pxCurrentFrame.Width * entity.ScaleX * viewPortInfo.ActualZoom);
                        int pxObjectHeight = (int) Math.Ceiling(pxCurrentFrame.Height * entity.ScaleY * viewPortInfo.ActualZoom);

                        //Draw the Object based on the current Frame dimensions and the specified Object Width Height values
                        Rectangle objectDestRect = new Rectangle(
                                (int) Math.Ceiling(pxAbsEntityPos.X),
                                (int) Math.Ceiling(pxAbsEntityPos.Y),
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
                        float layerDepth = Math.Min(0.99f, 1 / (entity.PY + ((float)drawable.Layer / Map.pxHeight)));

                        SpriteBatch.Draw(
                            drawable.Drawable.GetSourceTexture(LastUpdateTime),
                            objectDestRect,
                            pxCurrentFrame,
                            drawableColor,
                            drawable.Rotation,
                            drawableOrigin,
                            drawable.SpriteEffects,
                            layerDepth);

                        //DRAW ENTITY DETAILS IF ENABLED (ENTITY DEBUG INFO)
                        if (ShowEntityDebugInfo)
                        {
                            string message = string.Format("{0}\nPX={1},PY={2}", objectDestRect, entity.PX, entity.PY);
                            SpriteBatch.DrawMultiLineString(
                                SpriteFont,
                                message,
                                new Vector2(
                                    objectDestRect.X - objectDestRect.Width * drawable.Drawable.rxDrawOrigin.X + objectDestRect.Width / 2, 
                                    objectDestRect.Y - objectDestRect.Height * drawable.Drawable.rxDrawOrigin.Y + objectDestRect.Height / 2
                                    ),
                                Color.Red);
                        }
                    }

                    DebugInfo.EntityRenderingTimes[entity.Name] = _watch2.Elapsed;
                }
            }
            SpriteBatch.End();
            DebugInfo.TotalEntityRenderingTime = _watch1.Elapsed;

            //DRAW THE QUAD TREE IF ENABLEd
            if (ShowQuadTree)
            {
                SpriteBatch.Begin();
                DrawQuadTree(viewPortInfo, SpriteBatch, QuadTree.Root, pxDestRectangle, SpriteFont);
                SpriteBatch.End();
            }

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
            DebugInfo.TotalGameShaderRenderTime = _watch1.Elapsed;

            //DRAW THE VIEWPORT TO THE STANDARD SCREEN
            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch.Begin();
            {
                SpriteBatch.Draw(_inputBuffer, pxDestRectangle, Color);
            }
            SpriteBatch.End();

            return viewPortInfo;
        }

        #endregion
    }
}
