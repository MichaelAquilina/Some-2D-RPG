using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using GameEngine.DataStructures;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Info;
using GameEngine.Interfaces;
using GameEngine.Options;
using GameEngine.Shaders;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// The TeeEngine - the result of my sweat, blood and tears into this project. The TeeEngine is simply a 2D Tile Engine that
/// provides a number of powerful tools and properties to quickly create and design 2D games that rely on tiles as coordinate
/// systems. The Name Tee Engine came from the idea of a TileEngine, ie a TEngine. I have a personal obsession with Tea, so changing
/// the name of the engine to TeeEngine embeds a bit of my personality into this project.
/// </summary>
namespace GameEngine
{
    public class TeeEngine : GameComponent
    {
        #region Properties

        /// <summary>
        /// Graphics Device being used by this Engine to Render.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Width resolution of the TeeEngine buffer in pixels.
        /// </summary>
        public int PixelWidth { get; private set; }

        /// <summary>
        /// Height resolution of the TeeEngine buffer in pixels.
        /// </summary>
        public int PixelHeight { get; private set; }

        /// <summary>
        /// List of all Entities on screen since the last DrawWorldViewPort call.
        /// </summary>
        public List<Entity> EntitiesOnScreen { get; private set; }

        /// <summary>
        /// Currently loaded TiledMap.
        /// </summary>
        public TiledMap Map { get; private set; }

        /// <summary>
        /// Currently loaded MapModel that is associated with the currently loaded TiledMap.
        /// </summary>
        public IMapScript MapScript { get; private set; }

        /// <summary>
        /// Returns a Collection of all the Entities that have been added to this Engine.
        /// </summary>
        public ICollection<Entity> Entities { get { return _entities.Values; } }

        /// <summary>
        /// List of currently registered GameShaders in use by the TeeEngine.
        /// </summary>
        public ICollection<PostGameShader> GameShaders { get { return _postGameShaders.Values; } }

        /// <summary>
        /// Current Collision Data Structure being used to perform Collision Tests.
        /// </summary>
        public ICollider Collider { get; private set; }

        /// <summary>
        /// Last GameTime instance when the TeeEngine was Updated.
        /// </summary>
        public GameTime LastUpdateTime { get; private set; }

        /// <summary>
        /// Debug Information that can be accessed by the player.
        /// </summary>
        public DebugInfo DebugInfo { get; private set; }

        /// <summary>
        /// Class that allows the user to specify the settings for numerous drawing options.
        /// </summary>
        public DrawingOptions DrawingOptions { get; private set; }

        #endregion

        #region Internal Members

        // Map loading variables.
        bool _mapLoaded = false;
        MapEventArgs _mapLoadedEventArgs;

        // Entity creation and destroy lists.
        List<Entity> _entityCreate = new List<Entity>();               
        List<Entity> _entityDestroy = new List<Entity>();
        
        Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();        
        Dictionary<string, PostGameShader> _postGameShaders = new Dictionary<string,PostGameShader>();
        RenderTarget2D _inputBuffer;
        RenderTarget2D _outputBuffer;
        RenderTarget2D _dummyBuffer;

        // TODO: Might be smarter to create a class for handling these.
        Stopwatch _watch1;              // Primary diagnostic watch.
        Stopwatch _watch2;              // Secondary diagnostic watch.
        Stopwatch _watch3;              // Tertiary diagnostic watch.
                                           
        int _entityIdCounter = 0;       // Used for automatic assigning of IDs.

        #endregion

        public TeeEngine(Game game, int pixelWidth, int pixelHeight)
            :base(game)
        {
            _watch1 = new Stopwatch();
            _watch2 = new Stopwatch();
            _watch3 = new Stopwatch();

            GraphicsDevice = game.GraphicsDevice;

            DrawingOptions = new DrawingOptions();
            DrawingOptions.ShowQuadTree = false;
            DrawingOptions.ShowEntityDebugInfo = false;
            DrawingOptions.ShowTileGrid = false;
            DrawingOptions.ShowBoundingBoxes = false;
            DrawingOptions.ShowDrawableComponents = false;

            Collider = new HashList(64, 64);
            //Collider = new QuadTree();

            EntitiesOnScreen = new List<Entity>();

            DebugInfo = new DebugInfo();

            SetResolution(pixelWidth, pixelHeight);
            game.Components.Add(this);
        }

        public void SetResolution(int pixelWidth, int pixelHeight)
        {
            this.PixelWidth = pixelWidth;
            this.PixelHeight = pixelHeight;

            if (Map != null)
                Collider.Construct(Map.txWidth, Map.txHeight, Map.TileWidth, Map.TileHeight);

            if (_outputBuffer != null)
                _outputBuffer.Dispose();

            if (_inputBuffer != null)
                _inputBuffer.Dispose();

            _inputBuffer = new RenderTarget2D(GraphicsDevice, pixelWidth, pixelHeight, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);
            _outputBuffer = new RenderTarget2D(GraphicsDevice, pixelWidth, pixelHeight, false, SurfaceFormat.Bgr565, DepthFormat.Depth24Stencil8);

            // Allow all game shaders to become aware of the change in resolution.
            foreach (PostGameShader shader in GameShaders) shader.SetResolution(pixelWidth, pixelHeight);
        }

        public void LoadContent()
        {
            // TODO.
        }

        #region Map Loading Methods

        public void LoadMap(TiledMap map, MapEventArgs mapLoadedEventArgs=null)
        {
            this.Map = map;
            this.Map.LoadContent(Game.Content);

            // Load Specified Map Scipt (If Any)
            if (this.Map.HasProperty("MapScript"))
            {
                string assemblyName = map.GetProperty("Assembly");
                string mapModel = map.GetProperty("MapScript");

                this.MapScript = (IMapScript)Activator.CreateInstance(assemblyName, mapModel).Unwrap();
            }
            else this.MapScript = null;

            // Convert TiledObjects into Entity objects.
            ConvertMapObjects(map);

            // Set the mapLoaded flag so that the MapLoaded event can be invoked at a later stage.
            _mapLoaded = true;
            _mapLoadedEventArgs = mapLoadedEventArgs;

            // unload previous map here.

            this.Collider.Construct(map.txWidth, map.txHeight, map.TileWidth, map.TileHeight);
        }

        public void LoadMap(string mapFilePath, MapEventArgs mapLoadedEventArgs=null)
        {
            LoadMap(TiledMap.FromTiledXml(mapFilePath), mapLoadedEventArgs);
        }

        #endregion

        #region Entity Related Functions

        public List<Entity> GetIntersectingEntities(FRectangle region)
        {
            return Collider.GetIntersectingEntites(region);
        }

        public void AddEntity(Entity entity)
        {
            AddEntity(null, entity);
        }

        public void AddEntity(string name, Entity entity)
        {
            // Assign an automatic, unique entity name if none is supplied.
            if (name == null) name = string.Format("{0}{1}", entity.GetType(), _entityIdCounter++);

            entity.Name = name;
            entity.LoadContent(Game.Content);
            _entityCreate.Add(entity);
        }

        public ICollection<string> GetEntityNames()
        {
            return _entities.Keys;
        }

        public ICollection<Entity> GetEntities()
        {
            return _entities.Values;
        }

        public Entity GetEntity(string name)
        {
            return _entities[name];
        }

        public bool RemoveEntity(string name)
        {
            if (_entities.ContainsKey(name))
            {
                // Deferred removal is important because
                // we cannot alter the update loops _entities.Values
                // or else a runtime error will occur if an entity
                // removes itself or someone else in an Update call.
                _entityDestroy.Add(_entities[name]);
                return true;
            }

            return false;
        }

        public bool RemoveEntity(Entity entity)
        {
            return RemoveEntity(entity.Name);
        }

        public void ClearEntities()
        {
            // Clear and then Re-Add all the entities.
            // We are deferring the deletion of all entities till after the games update loop.
            _entityDestroy.Clear();
            _entityDestroy.AddRange(_entities.Values);
        }

        // Destorys any pending entities in the Entity Destroy List.
        void DestroyEntities(GameTime gameTime)
        {
            DebugInfo.ColliderUpdateTime = _watch3.Elapsed;

            // REMOVE ANY ENTITIES FOUND IN THE ENTITY TRASH
            _watch2.Restart();
            for (int i = 0; i < _entityDestroy.Count; i++)
            {
                Entity entity = _entityDestroy[i];

                if (entity.PreDestroy(gameTime, this))
                {
                    _entities.Remove(entity.Name);

                    entity.Name = null;
                    Collider.Remove(entity);

                    entity.PostDestroy(gameTime, this);
                }
            }
            _entityDestroy.Clear();

            DebugInfo.TotalEntityRemovalTime = _watch2.Elapsed;
        }

        // Creates any pending entities in the Entity Create List.
        void CreateEntities(GameTime gameTime)
        {
            // ADD ANY ENTITIES IN THE CREATION LIST
            _watch2.Restart();
            for (int i = 0; i < _entityCreate.Count; i++)
            {
                Entity entity = _entityCreate[i];

                // The result of this call determines if the entity will be added or not.
                if (entity.PreInitialize(gameTime, this))
                {
                    _entities.Add(entity.Name, entity);

                    entity.CurrentBoundingBox = entity.GetPxBoundingBox(gameTime);
                    entity.prevBoundingBox = entity.CurrentBoundingBox;
                    Collider.Add(entity);

                    entity.PostInitialize(gameTime, this);
                }
                else entity.Name = null;
            }
            _entityCreate.Clear();

            DebugInfo.TotalEntityAdditionTime = _watch2.Elapsed;
        }

        // Automatic Conversion of TiledObjects in a .tmx file to TeeEngine Entities using C# Reflection.
        void ConvertMapObjects(TiledMap map)
        {
            foreach (TiledObjectLayer objectLayer in map.TiledObjectLayers)
            {
                foreach (TiledObject tiledObject in objectLayer.TiledObjects)
                {
                    Entity entity = null;

                    // Special (Static) Tiled-Object when a Gid is specified.
                    if (tiledObject.Type == null && tiledObject.Gid != -1)
                    {
                        Tile sourceTile = map.Tiles[tiledObject.Gid];

                        entity = new Entity();
                        entity.Drawables.Add("standard", sourceTile);
                        entity.CurrentDrawableState = "standard";
                        entity.Pos = new Vector2(tiledObject.X, tiledObject.Y);

                        // Cater for any difference in origin from Tiled's default Draw Origin of (0,1).
                        entity.Pos.X += (sourceTile.Origin.X - 0.0f) * sourceTile.GetSourceRectangle(0).Width;
                        entity.Pos.Y += (sourceTile.Origin.Y - 1.0f) * sourceTile.GetSourceRectangle(0).Height;
                    }
                    else
                    {
                        // Try and load Entity types from both the Assembly specified in MapProperties and within the GameEngine.
                        Assembly userAssembly = (map.HasProperty("Assembly")) ? Assembly.Load(map.GetProperty("Assembly")) : null;
                        Assembly engineAssembly = Assembly.GetExecutingAssembly();

                        // Try for user Assembly first - allows default Objects to be overriden if absoluately necessary.
                        object createdObject = null;
                        if (userAssembly != null)
                            createdObject = userAssembly.CreateInstance(tiledObject.Type);

                        if (createdObject == null)
                            createdObject = engineAssembly.CreateInstance(tiledObject.Type);

                        if (createdObject == null)
                            throw new ArgumentException(string.Format("'{0}' does not exist in any of the loaded Assemblies", tiledObject.Type));

                        if (createdObject is Entity)
                        {
                            // Convert to Entity object and assign values.
                            entity = (Entity)createdObject;
                            entity.Pos = new Vector2(tiledObject.X, tiledObject.Y);

                            // If the entity implements the ISizedEntity interface, apply Width and Height.
                            if (entity is ISizedEntity)
                            {
                                ((ISizedEntity)entity).Width = tiledObject.Width;
                                ((ISizedEntity)entity).Height = tiledObject.Height;
                            }

                            foreach (string propertyKey in tiledObject.PropertyKeys)
                            {
                                // Ignore all properties starting with '.'
                                if (propertyKey.StartsWith("."))
                                    continue;

                                // Bind Events.
                                if (propertyKey.StartsWith("$"))
                                {
                                    string methodName = tiledObject.GetProperty(propertyKey);
                                    string eventName = propertyKey.Substring(1, propertyKey.Length - 1);

                                    MethodInfo methodInfo = MapScript.GetType().GetMethod(methodName);
                                    EventInfo eventInfo = entity.GetType().GetEvent(eventName);
                                    Delegate delegateMethod = Delegate.CreateDelegate(eventInfo.EventHandlerType, MapScript, methodInfo);

                                    eventInfo.AddEventHandler(entity, delegateMethod);
                                }
                                else
                                    // Bind Properties.
                                    ReflectionExtensions.SmartSetProperty(entity, propertyKey, tiledObject.GetProperty(propertyKey));
                            }
                        }
                        else throw new ArgumentException(string.Format("'{0}' is not an Entity object", tiledObject.Type));
                    }

                    this.AddEntity(tiledObject.Name, entity);
                }
            }
        }

        #endregion

        #region Shader Related Functions

        public bool IsRegistered(string shaderName)
        {
            return _postGameShaders.ContainsKey(shaderName);
        }

        public bool IsRegistered(PostGameShader shader)
        {
            return _postGameShaders.ContainsValue(shader);
        }

        public PostGameShader GetPostGameShader(string shaderName)
        {
            return _postGameShaders[shaderName];
        }

        public void RegisterGameShader(string shaderName, PostGameShader shader)
        {
            _postGameShaders.Add(shaderName, shader);
            shader.LoadContent(this.Game.Content);
            shader.SetResolution(PixelWidth, PixelHeight);
        }

        public bool UnregisterGameShader(string shaderName)
        {
            if (_postGameShaders.ContainsKey(shaderName))
            {
                PostGameShader shader = _postGameShaders[shaderName];

                return _postGameShaders.Remove(shaderName);
            }
            else return false;
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            LastUpdateTime = gameTime;

            // Allow Map to Perform Update Routine
            if(MapScript != null)
                MapScript.Update(this, gameTime);

            // Clear previous entity update times.
            DebugInfo.Reset();
            _watch3.Reset();

            foreach (string entityId in _entities.Keys)
            {
                Entity entity = _entities[entityId];
                entity.prevBoundingBox = entity.CurrentBoundingBox;

                // Perform any per-entity update logic.
                _watch2.Restart(); 
                entity.Update(gameTime, this);
                DebugInfo.EntityUpdateTimes[entityId] = _watch2.Elapsed;
                DebugInfo.TotalEntityUpdateTime += _watch2.Elapsed;

                // Recalculate the Entities BoundingBox.
                _watch2.Restart();
                entity.CurrentBoundingBox = entity.GetPxBoundingBox(gameTime);
                DebugInfo.TotalEntityUpdateBoundingBoxTime += _watch2.Elapsed;

                // Reset the IsOnScreen variable before the next drawing operation.
                entity.IsOnScreen = false;

                // If the entity has moved, then update his position in the QuadTree.
                _watch3.Start();
                {
                    if (entity.CurrentBoundingBox != entity.prevBoundingBox)
                        Collider.Update(entity);
                }
                _watch3.Stop();
            }

            DestroyEntities(gameTime);
            CreateEntities(gameTime);

            // If the Map Loaded flag has beem set, we need to invoke the MapScripts MapLoaded event hook.
            if (_mapLoaded)
            {
                if (MapScript != null)
                    MapScript.MapLoaded(this, Map, _mapLoadedEventArgs);

                _mapLoadedEventArgs = null;
                _mapLoaded = false;

                DestroyEntities(gameTime);
                CreateEntities(gameTime);
            }
        }
 
        public ViewPortInfo DrawWorldViewPort(SpriteBatch spriteBatch, Vector2 center, float zoom, Rectangle destRectangle, Color color, SamplerState samplerState, SpriteFont spriteFont=null)
        {
            ViewPortInfo viewPortInfo = new ViewPortInfo();
            {
                viewPortInfo.pxTileWidth  = (int) Math.Ceiling(Map.TileWidth * zoom);
                viewPortInfo.pxTileHeight = (int) Math.Ceiling(Map.TileHeight * zoom);

                // Note about ActualZoom Property:
                // because there is a loss of data between to conversion from Map.pxTileWidth * Zoom -> (int)
                // we need to determine what was the actual level of zoom that was applied to the tiles and use that
                // this ensures that entities that will be drawn will be placed correctly on the screen.
                viewPortInfo.ActualZoom = viewPortInfo.pxTileWidth / Map.TileWidth;

                viewPortInfo.pxWidth = destRectangle.Width / viewPortInfo.ActualZoom;
                viewPortInfo.pxHeight = destRectangle.Height / viewPortInfo.ActualZoom;

                viewPortInfo.pxTopLeftX = center.X - viewPortInfo.pxWidth / 2.0f;
                viewPortInfo.pxTopLeftY = center.Y - viewPortInfo.pxHeight / 2.0f;

                viewPortInfo.TileCountX = (int) Math.Ceiling((double)viewPortInfo.pxWidth / Map.TileWidth) + 1;
                viewPortInfo.TileCountY = (int) Math.Ceiling((double)viewPortInfo.pxHeight / Map.TileHeight) + 1;

                // Prevent the View from going outisde of the WORLD coordinates.
                if (viewPortInfo.pxTopLeftX < 0) viewPortInfo.pxTopLeftX = 0;
                if (viewPortInfo.pxTopLeftY < 0) viewPortInfo.pxTopLeftY = 0;

                if (viewPortInfo.pxTopLeftX + viewPortInfo.pxWidth >= Map.pxWidth)
                    viewPortInfo.pxTopLeftX = Map.pxWidth - viewPortInfo.pxWidth;
                if (viewPortInfo.pxTopLeftY + viewPortInfo.pxHeight >= Map.pxHeight)
                    viewPortInfo.pxTopLeftY = Map.pxHeight - viewPortInfo.pxHeight;

                // Calculate any decimal displacement required (For Positions with decimal points).
                viewPortInfo.pxDispX = viewPortInfo.pxTopLeftX - ((int)viewPortInfo.pxTopLeftX / Map.TileWidth) * Map.TileWidth;
                viewPortInfo.pxDispY = viewPortInfo.pxTopLeftY - ((int)viewPortInfo.pxTopLeftY / Map.TileHeight) * Map.TileHeight;

                viewPortInfo.pxViewPortBounds = new Rectangle(
                    (int) Math.Ceiling(viewPortInfo.pxTopLeftX),
                    (int) Math.Ceiling(viewPortInfo.pxTopLeftY),
                    (int) Math.Ceiling(viewPortInfo.pxWidth),
                    (int) Math.Ceiling(viewPortInfo.pxHeight)
                );
            }

            // RENDER THE GAME WORLD TO THE VIEWPORT RENDER TARGET
            GraphicsDevice.SetRenderTarget(_inputBuffer);
            GraphicsDevice.Clear(Map.Background);

            // DRAW THE WORLD MAP
            _watch1.Restart();

            // Deferred Rendering should be fine for rendering tile as long as we draw tile layers one at a time
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState, null, null);
            {
                for (int layerIndex = 0; layerIndex < Map.TileLayers.Count; layerIndex++)
                {
                    // DRAW EACH LAYER
                    TileLayer tileLayer = Map.TileLayers[layerIndex];

                    if (tileLayer.Visible)
                    {
                        Color layerColor = new Color()
                        {
                            R = tileLayer.Color.R,
                            G = tileLayer.Color.G,
                            B = tileLayer.Color.B,
                            A = (byte)(tileLayer.Color.A * tileLayer.Opacity)
                        };

                        float depth = 1 - (layerIndex / 10000.0f);

                        for (int i = 0; i < viewPortInfo.TileCountX; i++)
                        {
                            for (int j = 0; j < viewPortInfo.TileCountY; j++)
                            {
                                int tileX = (int)(i + viewPortInfo.pxTopLeftX / Map.TileWidth);
                                int tileY = (int)(j + viewPortInfo.pxTopLeftY / Map.TileHeight);

                                int tileGid = tileLayer[tileX, tileY];

                                Rectangle pxTileDestRect = new Rectangle(
                                    (int) Math.Ceiling(i * viewPortInfo.pxTileWidth - viewPortInfo.pxDispX * viewPortInfo.ActualZoom),
                                    (int) Math.Ceiling(j * viewPortInfo.pxTileHeight - viewPortInfo.pxDispY * viewPortInfo.ActualZoom),
                                    (int) viewPortInfo.pxTileWidth,
                                    (int) viewPortInfo.pxTileHeight
                                );

                                if (tileGid != 0 && tileGid != -1)   // NULL or INVALID Tile Gid is ignored
                                {
                                    Tile tile = Map.Tiles[tileGid];

                                    spriteBatch.Draw(
                                        tile.sourceTexture,
                                        pxTileDestRect,
                                        tile.SourceRectangle,
                                        layerColor,
                                        0, Vector2.Zero,
                                        SpriteEffects.None,
                                        depth
                                    );
                                }

                                // DRAW THE TILE LAYER GRID IF ENABLE
                                if (DrawingOptions.ShowTileGrid && layerIndex == Map.TileLayers.Count - 1)
                                    spriteBatch.DrawRectangle(pxTileDestRect, Color.Black, 0);
                            }
                        }
                    }
                }
            }
            spriteBatch.End();
            DebugInfo.TileRenderingTime = _watch1.Elapsed;

            // Calculate the entity Displacement caused by pxTopLeft at a global scale to prevent jittering.
            // Each entity should be displaced by the *same amount* based on the pxTopLeftX/Y values
            // this is to prevent entities 'jittering on the screen' when moving the camera.
            float globalDispX = (int) Math.Ceiling(viewPortInfo.pxTopLeftX * viewPortInfo.ActualZoom);
            float globalDispY = (int) Math.Ceiling(viewPortInfo.pxTopLeftY * viewPortInfo.ActualZoom);

            // DRAW VISIBLE REGISTERED ENTITIES
            _watch1.Restart();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, samplerState, null, null);
            {
                EntitiesOnScreen = Collider.GetIntersectingEntites(new FRectangle(viewPortInfo.pxViewPortBounds));

                // DRAW EACH ENTITIY THAT IS WITHIN THE SCREENS VIEWPORT
                foreach (Entity entity in EntitiesOnScreen)
                {
                    _watch2.Restart();

                    if (!entity.Visible) continue;
                    entity.IsOnScreen = true;

                    // Determine the absolute position on the screen for entity position and the bounding box.
                    Vector2 pxAbsEntityPos = new Vector2(
                        entity.Pos.X * viewPortInfo.ActualZoom - globalDispX,
                        entity.Pos.Y * viewPortInfo.ActualZoom - globalDispY
                    );

                    FRectangle pxAbsBoundingBox = new FRectangle(
                        entity.CurrentBoundingBox.X * viewPortInfo.ActualZoom - globalDispX,
                        entity.CurrentBoundingBox.Y * viewPortInfo.ActualZoom - globalDispY,
                        entity.CurrentBoundingBox.Width * viewPortInfo.ActualZoom,
                        entity.CurrentBoundingBox.Height * viewPortInfo.ActualZoom
                    );

                    // DRAW ENTITY BOUNDING BOXES IF ENABLED
                    if (DrawingOptions.ShowBoundingBoxes)
                    {
                        spriteBatch.DrawRectangle(pxAbsBoundingBox.ToRectangle(), Color.Red, 0.0001f);
                        SpriteBatchExtensions.DrawCross(
                            spriteBatch,
                            new Vector2(
                                (int) Math.Ceiling(pxAbsEntityPos.X), 
                                (int) Math.Ceiling(pxAbsEntityPos.Y)
                            ), 
                            13, Color.Black, 0f);
                    }

                    // DRAW ENTITY DETAILS IF ENABLED (ENTITY DEBUG INFO)
                    if (DrawingOptions.ShowEntityDebugInfo)
                    {
                        SpriteBatchExtensions.DrawMultiLineString(
                            spriteBatch,
                            spriteFont,
                            entity.GetDebugInfo(),
                            (int) entity.CurrentBoundingBox.Width * 2,
                            4,
                            new Vector2(
                                pxAbsBoundingBox.X + pxAbsBoundingBox.Width/2,
                                pxAbsBoundingBox.Y + pxAbsBoundingBox.Height/2
                                ),
                            Color.Purple);
                    }

                    // DRAW EVERY GAMEDRAWABLE INSTANCE CURRENTLY ACTIVE IN THE ENTITIES DRAWABLE SET.
                    List<GameDrawableInstance> drawableInstances = entity.Drawables.GetByState(entity.CurrentDrawableState);

                    if (drawableInstances != null)
                    {
                        foreach (GameDrawableInstance drawable in drawableInstances)
                        {
                            if (!drawable.Visible) continue;

                            // The relative position of the object should always be (X,Y) - (globalDispX, globalDispY). globalDispX and globalDispY
                            // are based on viewPortInfo.TopLeftX and viewPortInfo.TopLeftY. viewPortInfo.TopLeftX and viewPortInfo.TopLeftY have 
                            // already been corrected in terms of the bounds of the WORLD map coordinates. This allows for panning at the edges.
                            Rectangle pxCurrentFrame = drawable.GetSourceRectangle(LastUpdateTime);

                            int pxObjectWidth = (int)Math.Ceiling(pxCurrentFrame.Width * entity.ScaleX * viewPortInfo.ActualZoom);
                            int pxObjectHeight = (int)Math.Ceiling(pxCurrentFrame.Height * entity.ScaleY * viewPortInfo.ActualZoom);

                            // Draw the Object based on the current Frame dimensions and the specified Object Width Height values.
                            Rectangle objectDestRect = new Rectangle(
                                    (int)Math.Ceiling(pxAbsEntityPos.X) + (int)Math.Ceiling(drawable.Offset.X * viewPortInfo.ActualZoom),
                                    (int)Math.Ceiling(pxAbsEntityPos.Y) + (int)Math.Ceiling(drawable.Offset.Y * viewPortInfo.ActualZoom),
                                    pxObjectWidth,
                                    pxObjectHeight
                            );

                            Vector2 drawableOrigin = new Vector2(
                                (float)Math.Ceiling(drawable.Drawable.Origin.X * pxCurrentFrame.Width),
                                (float)Math.Ceiling(drawable.Drawable.Origin.Y * pxCurrentFrame.Height)
                                );

                            Color drawableColor = new Color()
                            {
                                R = drawable.Color.R,
                                G = drawable.Color.G,
                                B = drawable.Color.B,
                                A = (byte)(drawable.Color.A * entity.Opacity)
                            };

                            // Layer depth should depend how far down the object is on the map (Relative to Y).
                            // Important to also take into account the animation layers for the entity.
                            float layerDepth = Math.Min(0.99f, 1 / (entity.Pos.Y + ((float)drawable.Layer / Map.pxHeight)));

                            // FINALLY ... DRAW
                            spriteBatch.Draw(
                                drawable.GetSourceTexture(LastUpdateTime),
                                objectDestRect,
                                pxCurrentFrame,
                                drawableColor,
                                drawable.Rotation,
                                drawableOrigin,
                                drawable.SpriteEffects,
                                layerDepth);

                            // DRAW BOUNDING BOXES OF EACH INDIVIDUAL DRAWABLE COMPONENT
                            if (DrawingOptions.ShowDrawableComponents)
                            {
                                Rectangle drawableComponentRect = new Rectangle(
                                    (int)Math.Floor(objectDestRect.X - objectDestRect.Width * drawable.Drawable.Origin.X),
                                    (int)Math.Floor(objectDestRect.Y - objectDestRect.Height * drawable.Drawable.Origin.Y),
                                    objectDestRect.Width, objectDestRect.Height);

                                SpriteBatchExtensions.DrawRectangle(
                                    spriteBatch, drawableComponentRect, Color.Blue, 0);
                            }
                        }
                    }

                    DebugInfo.EntityRenderingTimes[entity.Name] = _watch2.Elapsed;
                }
            }
            spriteBatch.End();
            DebugInfo.TotalEntityRenderingTime = _watch1.Elapsed;

            // DRAW COLLIDER DEBUG INFORMATION IF ENABLED
            if (DrawingOptions.ShowQuadTree)
            {
                spriteBatch.Begin();
                Collider.DrawDebugInfo(viewPortInfo, spriteBatch, destRectangle, spriteFont, globalDispX, globalDispY);
                spriteBatch.End();
            }

            _watch1.Restart();
            // APPLY GAME SHADERS TO THE RESULTANT IMAGE
            foreach(PostGameShader postGameShader in GameShaders)
            {
                if (postGameShader.Enabled)
                {
                    postGameShader.ApplyShader(spriteBatch, viewPortInfo, LastUpdateTime, _inputBuffer, _outputBuffer);

                    // Swap buffers after each render.
                    _dummyBuffer = _inputBuffer;
                    _inputBuffer = _outputBuffer;
                    _outputBuffer = _dummyBuffer;
                }
            }
            DebugInfo.TotalGameShaderRenderTime = _watch1.Elapsed;

            // DRAW THE VIEWPORT TO THE STANDARD SCREEN
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            {
                spriteBatch.Draw(_inputBuffer, destRectangle, color);
            }
            spriteBatch.End();

            return viewPortInfo;
        }
    }
}
