using System;
using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameEngine.Shaders;

namespace GameEngine
{
    public struct ViewPortInfo
    {
        public float PXTileWidth { get; set; }
        public float PXTileHeight { get; set; }
        public float TopLeftX { get; set; }
        public float TopLeftY { get; set; }
        public int TileCountX { get; set; }
        public int TileCountY { get; set; }
    }

    /// <summary>
    /// Class that represents the current state of the game world, including the Actors residing in it. Provides functions
    /// to draw/render the current state of the world, as well as other draw functions such as drawing a MiniMap version
    /// of the current WorldMap.
    /// </summary>
    public class GameWorld : GameComponent
    {
        #region Properties

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int ObjectsOnScreen { get; private set; }

        public List<ILoadable> LoadableContent { get; private set; }

        public List<IGameDrawable> DrawableObjects { get; private set; }

        public List<GameShader> GameShaders { get; private set; }

        public bool ShowBoundingBoxes { get; set; }

        #endregion

        #region Variables

        private Map _worldMap;                   //World Map Instance
        private Texture2D _miniMapTex;           //Cached copy of the MipMapTexture

        RenderTarget2D _inputBuffer;
        RenderTarget2D _outputBuffer;

        #endregion

        #region Initialisation

        public GameWorld(Game Game, int Width, int Height)
            :base(Game)
        {
            ShowBoundingBoxes = false;

            ObjectsOnScreen = 0;

            LoadableContent = new List<ILoadable>();
            DrawableObjects = new List<IGameDrawable>();
            GameShaders = new List<GameShader>();

            SetResolution(Width, Height);
        }

        public void LoadContent()
        {
            ContentManager Content = this.Game.Content;

            foreach (ILoadable loadableObject in LoadableContent)
                loadableObject.LoadContent(Content);

            foreach (ILoadable loadableShader in GameShaders)
                loadableShader.LoadContent(Content);

            if(_worldMap!=null)
                _worldMap.LoadContent(Game.Content);
        }

        public void UnloadContent()
        {
            if( _worldMap != null )
               _worldMap.UnloadContent();
    
            if (_miniMapTex != null)
                _miniMapTex.Dispose();

            if (_inputBuffer != null)
                _inputBuffer.Dispose();

            if (_outputBuffer != null)
                _outputBuffer.Dispose();

            _miniMapTex = null;
            _inputBuffer = null;
            _outputBuffer = null;

            foreach (ILoadable loadableObject in LoadableContent)
                loadableObject.UnloadContent();

            foreach (ILoadable loadableShader in GameShaders)
                loadableShader.UnloadContent();
        }

        #endregion

        #region Register/Unregister methods

        /// <summary>
        /// Registers an arbitrary object to the GameWorld based on the interfaces and abstract classes
        /// it inherits. Once the object has been registered in the game world, it will act according
        /// to how its behaviour was specified in its interfaces. If the object does not inherit anything
        /// then this method does absolutely nothing. 
        /// </summary>
        /// <param name="SomeObject">Object that inherts from GameEngine interfaces or classes</param>
        public void RegisterObject(Object SomeObject)
        {
            if (SomeObject is ILoadable) RegisterLoadableContent((ILoadable)SomeObject);
            if (SomeObject is IGameDrawable) RegisterDrawableObject((IGameDrawable)SomeObject);
            if (SomeObject is GameShader) RegisterGameShader((GameShader)SomeObject);
        }

        public void RegisterLoadableContent(ILoadable Loadable)
        {
            LoadableContent.Add(Loadable);
        }

        public void RegisterDrawableObject(IGameDrawable GameDrawable)
        {
            DrawableObjects.Add(GameDrawable);
        }

        public void RegisterGameShader(GameShader Shader)
        {
            GameShaders.Add(Shader);
            Shader.SetResolution(Width, Height);
        }

        public bool UnregisterObject(Object SomeObject)
        {
            bool result = true;

            if (SomeObject is ILoadable) result &= UnregisterLoadableContent((ILoadable)SomeObject);
            if (SomeObject is IGameDrawable) result &= UnregisterDrawableObject((IGameDrawable)SomeObject);
            if (SomeObject is GameShader) result &= UnregisterGameShader((GameShader)SomeObject);

            return result;
        }

        public bool UnregisterLoadableContent(ILoadable Loadable)
        {
            Loadable.UnloadContent();
            return LoadableContent.Remove(Loadable);
        }

        public bool UnregisterDrawableObject(IGameDrawable GameDrawable)
        {
            return DrawableObjects.Remove(GameDrawable);
        }

        public bool UnregisterGameShader(GameShader Shader)
        {
            Shader.UnloadContent();
            return GameShaders.Remove(Shader);
        }

        #endregion

        #region Public API Methods

        public void LoadMap(Map Map, bool Clear=true)
        {
            if (_worldMap != null)
                _worldMap.UnloadContent();

            _worldMap = Map;

            if( Clear ) DrawableObjects.Clear();

            foreach (MapObject mapObject in _worldMap.MapObjects)
                RegisterObject(mapObject);

            foreach (Entity entity in _worldMap.MapEntities)
                RegisterObject(entity);
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
        /// <param name="TileWidth">Integer value specifying the Width in pixels of each Tile on the Map.</param>
        /// <param name="TileHeight">Integer value specifying the Height in pixels of each Tile on the Map.</param>
        /// <param name="DestRectangle">Rectangle object specifying the render destination for the viewport. Should specify location, width and height.</param>
        /// <param name="Color">Color object with which to blend the game world.</param>
        public void DrawWorldViewPort(GameTime GameTime, SpriteBatch SpriteBatch, Vector2 Center, int pxTileWidth, int pxTileHeight, Rectangle DestRectangle, Color Color)
        {
            GraphicsDevice GraphicsDevice = this.Game.GraphicsDevice;

            ViewPortInfo viewPortInfo = new ViewPortInfo();
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
                if (viewPortInfo.TopLeftX + viewPortInfo.TileCountX >= _worldMap.Width) viewPortInfo.TopLeftX = _worldMap.Width - viewPortInfo.TileCountX;
                if (viewPortInfo.TopLeftY + viewPortInfo.TileCountY >= _worldMap.Height) viewPortInfo.TopLeftY = _worldMap.Height - viewPortInfo.TileCountY;
            }

            //calculate any decimal displacement required (For Positions with decimal points)
            double dispX = viewPortInfo.TopLeftX - Math.Floor(viewPortInfo.TopLeftX);
            double dispY = viewPortInfo.TopLeftY - Math.Floor(viewPortInfo.TopLeftY);

            //RENDER THE GAME WORLD TO THE VIEWPORT RENDER TARGET
            GraphicsDevice.SetRenderTarget(_inputBuffer);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            {
                //DRAW THE WORLD MAP TILES
                for (int i = 0; i < viewPortInfo.TileCountX; i++)
                    for (int j = 0; j < viewPortInfo.TileCountY; j++)
                    {
                        int tileX = (int)(i + viewPortInfo.TopLeftX);
                        int tileY = (int)(j + viewPortInfo.TopLeftY);

                        Rectangle tileDestRect = new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight);

                        //traslate if there is any decimal displacement due to a Center with a floating point
                        tileDestRect.X -= (int)(dispX * pxTileWidth);
                        tileDestRect.Y -= (int)(dispY * pxTileHeight);

                        SpriteBatch.Draw(
                            _worldMap.GroundPallette.GetTileSourceTexture(this._worldMap[tileX, tileY]),
                            tileDestRect,
                            _worldMap.GroundPallette.GetTileSourceRectangle(this._worldMap[tileX, tileY]),
                            Color.White,
                            0, Vector2.Zero,
                            SpriteEffects.None,
                            1
                        );
                    }

                ObjectsOnScreen = 0;

                //DRAW THE IGAMEDRAWABLE COMPONENTS (Actors, MapObjects, etc...)
                foreach (IGameDrawable drawObject in DrawableObjects)
                {
                    //The relative position of the object should always be (X,Y) - (viewPortInfo.TopLeftX,viewPortInfo.TopLeftY) where viewPortInfo.TopLeftX and
                    //viewPortInfo.TopLeftY have already been corrected in terms of the bounds of the WORLD map coordinates. This allows
                    //for panning at the edges.
                    Rectangle ObjectSrcRect = drawObject.GetSourceRectangle(GameTime);

                    int objectX = (int)Math.Ceiling((drawObject.X - viewPortInfo.TopLeftX) * pxTileWidth);
                    int objectY = (int)Math.Ceiling((drawObject.Y - viewPortInfo.TopLeftY) * pxTileHeight);

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
                    if (drawObject.Visible && ObjectBoundingBox.Intersects(_inputBuffer.Bounds))
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
                            drawObject.CurrentSpriteEffect,
                            Math.Min(0.99f, 1 / drawObject.Y));        //layer depth should depend how far down the object is on the map (Relative to Y)
                    }
                }
            }
            SpriteBatch.End();

            //TODO: Can possibly improve performance by setting render target to the back buffer for the last shader pass
            for (int i = 0; i < GameShaders.Count; i++)
                GameShaders[i].ApplyShader(SpriteBatch, viewPortInfo, GameTime, _inputBuffer, _outputBuffer );

            //DRAW THE VIEWPORT TO THE STANDARD SCREEN
            GraphicsDevice.SetRenderTarget(null);
            SpriteBatch.Begin();
            {
                SpriteBatch.Draw(_outputBuffer, DestRectangle, Color);
            }
            SpriteBatch.End();
        }

        #endregion
    }
}
