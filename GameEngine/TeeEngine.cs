using System;
using System.Collections.Generic;
using System.Reflection;
using GameEngine.DataStructures;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Info;
using GameEngine.Interfaces;
using GameEngine.Options;
using GameEngine.Pathfinding;
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
        /// Diagnostic Information that records the Overall Engine Performance.
        /// </summary>
        public DiagnosticInfo OverallPerformance { get; private set; }

        /// <summary>
        /// Diagnostic Information that records the performance of each entity's Update routine.
        /// </summary>
        public DiagnosticInfo EntityUpdatePerformance { get; private set; }

        /// <summary>
        /// Diagnostic Information that records the perofrmance of each entity's Render time.
        /// </summary>
        public DiagnosticInfo EntityRenderPerformance { get; private set; }

        /// <summary>
        /// Class that allows the user to specify the settings for numerous drawing options.
        /// </summary>
        public DrawingOptions DrawingOptions { get; private set; }

        /// <summary>
        /// Class that allows pathfinding on a loaded TiledMap
        /// </summary>
        public AStar Pathfinding { get; private set; }

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
                                           
        int _entityIdCounter = 0;       // Used for automatic assigning of IDs.

        #endregion

        public TeeEngine(Game game, int pixelWidth, int pixelHeight)
            :base(game)
        {
            GraphicsDevice = game.GraphicsDevice;

            DrawingOptions = new DrawingOptions();
            DrawingOptions.ShowColliderDebugInfo = false;
            DrawingOptions.ShowEntityDebugInfo = false;
            DrawingOptions.ShowTileGrid = false;
            DrawingOptions.ShowBoundingBoxes = false;
            DrawingOptions.ShowDrawableComponents = false;

            Collider = new HashList(16, 16);     // 16x16 is very fast but can be very memory intensive.
            //Collider = new QuadTree();

            EntitiesOnScreen = new List<Entity>();

            OverallPerformance = new DiagnosticInfo("Overall Game Performance");
            EntityUpdatePerformance = new DiagnosticInfo("Individual Entity Update Performance");
            EntityRenderPerformance = new DiagnosticInfo("Individual Entity Render Performance");

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

            // Setup the pathfinding with the new map
            this.Pathfinding = new AStar(this.Map);
        }

        public void LoadMap(string mapFilePath, MapEventArgs mapLoadedEventArgs=null)
        {
            LoadMap(TiledMap.LoadTmxFile(mapFilePath), mapLoadedEventArgs);
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

        // Destroys any pending entities in the Entity Destroy List.
        void DestroyEntities(GameTime gameTime)
        {
            // REMOVE ANY ENTITIES FOUND IN THE ENTITY TRASH
            OverallPerformance.RestartTiming("DestroyEntitiesTime");
            for (int i = 0; i < _entityDestroy.Count; i++)
            {
                Entity entity = _entityDestroy[i];

                if (entity.PreDestroy(gameTime, this))
                {
                    _entities.Remove(entity.Name);

                    EntityRenderPerformance.RemoveTiming(entity.Name);
                    EntityUpdatePerformance.RemoveTiming(entity.Name);
                    Collider.Remove(entity);

                    entity.Name = null;

                    entity.PostDestroy(gameTime, this);
                }
            }
            _entityDestroy.Clear();

            OverallPerformance.StopTiming("DestroyEntitiesTime");
        }

        // Creates any pending entities in the Entity Create List.
        void CreateEntities(GameTime gameTime)
        {
            // ADD ANY ENTITIES IN THE CREATION LIST
            OverallPerformance.RestartTiming("CreateEntitiesTime");
            for (int i = 0; i < _entityCreate.Count; i++)
            {
                Entity entity = _entityCreate[i];

                // The result of this call determines if the entity will be added or not.
                if (entity.PreCreate(gameTime, this))
                {
                    _entities.Add(entity.Name, entity);

                    entity.CurrentBoundingBox = entity.GetPxBoundingBox(gameTime);
                    entity.PreviousBoundingBox = entity.CurrentBoundingBox;
                    Collider.Add(entity);

                    entity.PostCreate(gameTime, this);
                }
                else entity.Name = null;
            }
            _entityCreate.Clear();

            OverallPerformance.StopTiming("CreateEntitiesTime");
        }

        // Automatic Conversion of TiledObjects in a .tmx file to TeeEngine Entities using C# Reflection.
        private void ConvertMapObjects(TiledMap map)
        {
            foreach (TiledObjectLayer objectLayer in map.TiledObjectLayers)
            {
                foreach (TiledObject tiledObject in objectLayer.TiledObjects)
                {
                    Entity entity = null;

                    // Load the Specified Entity type or use StaticEntity if none is specified.
                    if (tiledObject.Type != null)
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
                            entity = (Entity)createdObject;
                        else
                            throw new ArgumentException(string.Format("'{0}' is not an Entity object", tiledObject.Type));
                    }
                    else entity = new StaticEntity();

                    // Set Entity position to tiledObject location.
                    entity.Pos = new Vector2(tiledObject.X, tiledObject.Y);

                    // If a Tile is part of the definition, add it to the Entity's DrawableSet.
                    if (tiledObject.Gid != -1)
                    {
                        Tile sourceTile = map.Tiles[tiledObject.Gid];
                        StaticImage sourceImage = sourceTile.ToStaticImage();

                        entity.Drawables.Add("standard", sourceImage);
                        entity.CurrentDrawableState = "standard";
                        entity.Pos = new Vector2(tiledObject.X, tiledObject.Y);

                        // Cater for any difference in origin from Tiled's default Draw Origin of (0,1).
                        entity.Pos.X += (sourceImage.Origin.X - 0.0f) * sourceImage.GetSourceRectangle(0).Width;
                        entity.Pos.Y += (sourceImage.Origin.Y - 1.0f) * sourceImage.GetSourceRectangle(0).Height;
                    }

                    // If the entity implements the ISizedEntity interface, apply Width and Height.
                    if (entity is ISizedEntity)
                    {
                        ((ISizedEntity)entity).Width = tiledObject.Width;
                        ((ISizedEntity)entity).Height = tiledObject.Height;
                    }

                    // If the entity implements the IPolygonEntity interface, apply its Points.
                    if (entity is IPolygonEntity)
                        ((IPolygonEntity)entity).Points = tiledObject.Points;

                    // Method Queue to be invoked once the Entity has been assigned all its Properties.
                    SortedList<int, KeyValuePair<MethodInfo, object[]>> methodQueue = new SortedList<int, KeyValuePair<MethodInfo, object[]>>();

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
                            // Run a Method.
                            if (propertyKey.StartsWith("*"))
                            {
                                int start = 1;
                                int caretStart = propertyKey.IndexOf('<');
                                int caretEnd = propertyKey.IndexOf('>');

                                // Check Format.
                                if (caretEnd == -1 || caretStart == -1)
                                    throw new ArgumentException("A Method order needs to be specified. Example: MyMethod<2>");

                                // Check Method Order.
                                int methodOrder;
                                string orderStr = propertyKey.Substring(caretStart + 1, caretEnd - caretStart - 1);
                                if (!Int32.TryParse(orderStr, out methodOrder))
                                    throw new ArgumentException(string.Format("Invalid Method order specified: {0}", orderStr));

                                string methodName = propertyKey.Substring(start, caretStart - 1);
                                string[] methodParams = tiledObject.GetProperty(propertyKey, null).Split(',');
                                MethodInfo methodInfo = entity.GetType().GetMethod(methodName);

                                // Check Method Existance.
                                if (methodInfo == null)
                                    throw new ArgumentException(string.Format("The Method '{0}' does not exist or is Ambigious", methodName));

                                ParameterInfo[] paramInfo = methodInfo.GetParameters();
                                object[] parameters = new object[paramInfo.Length];

                                // Check Invalid Number of Parameters.
                                if (paramInfo.Length != methodParams.Length)
                                    throw new ArgumentException(string.Format(
                                        "The number of arguments passed is Invalid. Expected {0}, Specified {1}", paramInfo.Length, methodParams.Length)
                                        );

                                for (int i = 0; i < paramInfo.Length; i++)
                                    parameters[i] = ReflectionExtensions.SmartConvert(methodParams[i], paramInfo[i].ParameterType);

                                methodQueue.Add(methodOrder, new KeyValuePair<MethodInfo, object[]>(methodInfo, parameters));
                            }
                            else
                                // Bind Properties.
                                ReflectionExtensions.SmartSetProperty(entity, propertyKey, tiledObject.GetProperty(propertyKey));
                    }

                    // Invoke the methods found in the order specified.
                    foreach (int index in methodQueue.Keys)
                    {
                        MethodInfo methodInfo = methodQueue[index].Key;
                        object[] parameters = methodQueue[index].Value;

                        methodInfo.Invoke(entity, parameters);
                    }

                    if (entity != null) this.AddEntity(tiledObject.Name, entity);
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
            if (MapScript != null)
            {
                OverallPerformance.RestartTiming("MapScriptUpdateTime");
                MapScript.Update(this, gameTime);
                OverallPerformance.StopTiming("MapScriptUpdateTime");
            }

            OverallPerformance.ResetAll();
            OverallPerformance.RestartTiming("TotalEntityUpdateTime");
            
            foreach (string entityId in _entities.Keys)
            {
                Entity entity = _entities[entityId];
                entity.PreviousBoundingBox = entity.CurrentBoundingBox;

                EntityUpdatePerformance.RestartTiming(entityId);
                {
                    entity.Update(gameTime, this);
                }
                EntityUpdatePerformance.StopTiming(entityId);

                // Recalculate the Entities BoundingBox.
                entity.CurrentBoundingBox = entity.GetPxBoundingBox(gameTime);

                // Reset the IsOnScreen variable before the next drawing operation.
                entity.IsOnScreen = false;

                // If the entity has moved, then update his position in the QuadTree.
                OverallPerformance.StartTiming("ColliderUpdateTime");
                {
                    if (entity.CurrentBoundingBox != entity.PreviousBoundingBox)
                        Collider.Update(entity);
                }
                OverallPerformance.StopTiming("ColliderUpdateTime");
            }

            OverallPerformance.StopTiming("TotalEntityUpdateTime");

            DestroyEntities(gameTime);
            CreateEntities(gameTime);

            // If the Map Loaded flag has beem set, we need to invoke the MapScripts MapLoaded event hook.
            if (_mapLoaded)
            {
                EntityRenderPerformance.Clear();
                EntityUpdatePerformance.Clear();

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
            OverallPerformance.StartTiming("TileRenderingTime");

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

            OverallPerformance.StopTiming("TileRenderingTime");

            // Calculate the entity Displacement caused by pxTopLeft at a global scale to prevent jittering.
            // Each entity should be displaced by the *same amount* based on the pxTopLeftX/Y values
            // this is to prevent entities 'jittering on the screen' when moving the camera.
            float globalDispX = (int) Math.Ceiling(viewPortInfo.pxTopLeftX * viewPortInfo.ActualZoom);
            float globalDispY = (int) Math.Ceiling(viewPortInfo.pxTopLeftY * viewPortInfo.ActualZoom);

            // DRAW VISIBLE REGISTERED ENTITIES
            OverallPerformance.RestartTiming("TotalEntityRenderTime");
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, samplerState, null, null);
            {
                EntitiesOnScreen = Collider.GetIntersectingEntites(new FRectangle(viewPortInfo.pxViewPortBounds));

                // DRAW EACH ENTITIY THAT IS WITHIN THE SCREENS VIEWPORT
                foreach (Entity entity in EntitiesOnScreen)
                {
                    EntityRenderPerformance.RestartTiming(entity.Name);

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
                        spriteBatch.DrawRectangle(pxAbsBoundingBox.ToRectangle(), Color.Red, 1.0f);
                        SpriteBatchExtensions.DrawCross(
                            spriteBatch,
                            new Vector2(
                                (int) Math.Ceiling(pxAbsEntityPos.X), 
                                (int) Math.Ceiling(pxAbsEntityPos.Y)
                            ), 
                            13, Color.Black, 1.0f);
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
                    HashSet<DrawableInstance> drawableInstances = entity.Drawables.GetByState(entity.CurrentDrawableState);

                    if (drawableInstances != null)
                    {
                        foreach (DrawableInstance drawable in drawableInstances)
                        {
                            if (!drawable.Visible) continue;

                            // Layer depth should depend how far down the object is on the map (Relative to Y).
                            float layerDepth = (entity.Pos.Y - viewPortInfo.pxTopLeftY) / Map.pxHeight;

                            if (layerDepth < 0) layerDepth = 1.0f;

                            if (entity.AlwaysOnTop) layerDepth = float.MaxValue;

                            Rectangle objectDestRect = 
                                drawable.Draw(LastUpdateTime,
                                    spriteBatch,
                                    pxAbsEntityPos,
                                    layerDepth,
                                    entity.ScaleX,
                                    entity.ScaleY,
                                    entity.Opacity,
                                    Map.pxHeight,
                                    viewPortInfo,
                                    DrawingOptions.ShowDrawableComponents
                                );

                            // DRAW BOUNDING BOXES OF EACH INDIVIDUAL DRAWABLE COMPONENT
                            if (DrawingOptions.ShowDrawableComponents)
                            {
                                Rectangle drawableComponentRect = new Rectangle(
                                    (int)Math.Floor(objectDestRect.X - objectDestRect.Width * drawable.Drawable.Origin.X),
                                    (int)Math.Floor(objectDestRect.Y - objectDestRect.Height * drawable.Drawable.Origin.Y),
                                    objectDestRect.Width, objectDestRect.Height);

                                SpriteBatchExtensions.DrawRectangle(
                                    spriteBatch, drawableComponentRect, Color.Blue, 1.0f);
                            }
                        }
                    }

                    EntityRenderPerformance.StopTiming(entity.Name);
                }
            }
            spriteBatch.End();
            OverallPerformance.StopTiming("TotalEntityRenderTime");

            // APPLY GAME SHADERS TO THE RESULTANT IMAGE
            OverallPerformance.RestartTiming("TotalPostGameShaderRenderTime");
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
            OverallPerformance.StopTiming("TotalPostGameShaderRenderTime");

            // DRAW COLLIDER DEBUG INFORMATION IF ENABLED
            if (DrawingOptions.ShowColliderDebugInfo)
            {
                spriteBatch.Begin();
                Collider.DrawDebugInfo(viewPortInfo, spriteBatch, destRectangle, spriteFont, globalDispX, globalDispY);
                spriteBatch.End();
            }

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
