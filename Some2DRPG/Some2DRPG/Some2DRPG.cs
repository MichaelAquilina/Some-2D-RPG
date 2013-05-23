using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using GameEngine.Extensions;
using GameEngine.Info;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Some2DRPG.GameObjects.Characters;
using Some2DRPG.Shaders;
using Some2DRPG.Items;
using Some2DRPG.GameObjects;
using GameEngine.Pathfinding;

namespace Some2DRPG
{
    public class Some2DRPG : Game
    {
        // Constant (Editable) Valuables.
        const bool DEBUG = true;

        const int CIRCLE_POINT_ACCURACY = 36;

        const int WINDOW_HEIGHT = 700;
        const int WINDOW_WIDTH = 1200;

        const int WORLD_HEIGHT = 500;
        const int WORLD_WIDTH = 500;

        const int VIEW_WIDTH = 500;
        const int VIEW_HEIGHT = 480;

        bool helmetVisible = true;
        bool showDebugInfo = true;
        bool showDiagnostics = false;

        float Zoom = 1.6f;

        int TextCounter = 0;
        int SamplerIndex = 0;
        SamplerState CurrentSampler;
        SamplerState[] SamplerStates = new SamplerState[] { 
            SamplerState.PointWrap,
            SamplerState.PointClamp,
            SamplerState.LinearWrap,
            SamplerState.LinearClamp,
            SamplerState.AnisotropicWrap,
            SamplerState.AnisotropicClamp 
        };

        LightShader LightShader;

        Texture2D mouseCursor;
        Texture2D mouseCursorDown;
        Texture2D mouseCursorFist;
        Texture2D mouseCursorOpen;

        // Graphic Related Variables.
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        SpriteFont DefaultSpriteFont;

        // Game Specific Variablies.
        Random Random = new Random();

        TeeEngine Engine;

        Hero CurrentPlayer { get { return (Hero) Engine.GetEntity("Player"); } }

        public Some2DRPG()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            Graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            Graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
        }

        private bool PathfindingValidator(Vector2 txPos, TeeEngine engine)
        {
            Tile tile = engine.Map.GetTxTopMostTile((int)txPos.X, (int)txPos.Y);

            return tile != null && !tile.HasProperty("Impassable");
        }

        protected override void Initialize()
        {
            Engine = new TeeEngine(this, WINDOW_WIDTH, WINDOW_HEIGHT);
            Engine.Pathfinding.Validator = PathfindingValidator;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ItemRepository.LoadRepositoryXml("Items/ItemRepository.xml", Content);

            LightShader = new LightShader(this.GraphicsDevice, CIRCLE_POINT_ACCURACY);
            LightShader.AmbientLight = new Color(30, 15, 15);
            LightShader.Enabled = false;
            Engine.RegisterGameShader("LightShader", LightShader);

            mouseCursor = Content.Load<Texture2D>("Misc/Pointers/cursor");
            mouseCursorDown = Content.Load<Texture2D>("Misc/Pointers/cursor_down");
            mouseCursorFist = Content.Load<Texture2D>("Misc/Pointers/cursor_fist");
            mouseCursorOpen = Content.Load<Texture2D>("misc/Pointers/cursor_open");

            MapEventArgs mapArgs = new MapEventArgs();
            mapArgs.SetProperty("Target", "Bedroom");
            Engine.LoadMap("Content/Maps/inside_example.tmx", mapArgs);

            CurrentSampler = SamplerStates[SamplerIndex];

            Engine.DrawingOptions.ShowEntityDebugInfo = false;
            Engine.DrawingOptions.ShowBoundingBoxes = false;
            Engine.DrawingOptions.ShowTileGrid = false;

            Engine.LoadContent();

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            DefaultSpriteFont = Content.Load<SpriteFont>(@"Fonts\DefaultSpriteFont");
        }

        protected void CheckItemActions(GameTime gameTime, KeyboardState keyboardState)
        {
            // Swap HeadGear
            if(KeyboardExtensions.GetKeyDownState(keyboardState, Keys.D1, this, true))
            {
                if(CurrentPlayer.IsEquiped(ItemRepository.GameItems["PlateHelmet"]))
                    CurrentPlayer.Equip(ItemRepository.GameItems["RobeHood"]);
                else
                    CurrentPlayer.Equip(ItemRepository.GameItems["PlateHelmet"]);
            }

            // Swap Vest
            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.D2, this, true))
            {
                if (CurrentPlayer.IsEquiped(ItemRepository.GameItems["PlateChest"]))
                    CurrentPlayer.Equip(ItemRepository.GameItems["RobeShirt"]);
                else
                    CurrentPlayer.Equip(ItemRepository.GameItems["PlateChest"]);
            }

            // Swap Gloves
            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.D3, this, true))
            {
                if (CurrentPlayer.IsEquiped(ItemRepository.GameItems["PlateGloves"]))
                    CurrentPlayer.Unequip(ItemRepository.GameItems["PlateGloves"]);
                else
                    CurrentPlayer.Equip(ItemRepository.GameItems["PlateGloves"]);
            }

            // Swap Pants
            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.D4, this, true))
            {
                if (CurrentPlayer.IsEquiped(ItemRepository.GameItems["PlatePants"]))
                    CurrentPlayer.Equip(ItemRepository.GameItems["RobeSkirt"]);
                else
                    CurrentPlayer.Equip(ItemRepository.GameItems["PlatePants"]);
            }

            // Swap Boots
            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.D5, this, true))
            {
                if (CurrentPlayer.IsEquiped(ItemRepository.GameItems["PlateBoots"]))
                    CurrentPlayer.Equip(ItemRepository.GameItems["PlainShoes"]);
                else
                    CurrentPlayer.Equip(ItemRepository.GameItems["PlateBoots"]);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // F1 = Show/Hide Bounding Boxes
            // F2 = Show/Hide Debug Info
            // F3 = Enable/Disable LightShader
            // F4 = Change Current SamplerState
            // F5 = Show/Hide Tile Grid
            // F6 = Show/Hide Collider Debug Info
            // F7 = Show Performance Diagnostics
            // F8 = Show Entity Debug Info
            // F10 = Show Drawable Components
            // F11 = Show/Hide Player Helmet
            // F12 = Disable Player Collision Detection

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            CheckItemActions(gameTime, keyboardState);

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F1, this, true))
                Engine.DrawingOptions.ShowBoundingBoxes = !Engine.DrawingOptions.ShowBoundingBoxes;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F2, this, true))
                showDebugInfo = !showDebugInfo;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F3, this, true))
                LightShader.Enabled = !LightShader.Enabled;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F4, this, true))
                CurrentSampler = SamplerStates[++SamplerIndex % SamplerStates.Length];

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F5, this, true))
                Engine.DrawingOptions.ShowTileGrid = !Engine.DrawingOptions.ShowTileGrid;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F6, this, true))
                Engine.DrawingOptions.ShowColliderDebugInfo = !Engine.DrawingOptions.ShowColliderDebugInfo;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F7, this, true))
                showDiagnostics = !showDiagnostics;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F8, this, true))
                Engine.DrawingOptions.ShowEntityDebugInfo = !Engine.DrawingOptions.ShowEntityDebugInfo;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F10, this, true))
                Engine.DrawingOptions.ShowDrawableComponents = !Engine.DrawingOptions.ShowDrawableComponents;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F11, this, true))
            {
                helmetVisible = !helmetVisible;
                //CurrentPlayer.Drawables.SetGroupProperty("Head", "Visible", helmetVisible);
                if( helmetVisible)
                    CurrentPlayer.Drawables.SetGroupProperty("Head", "Offset", Vector2.Zero);
                else
                    CurrentPlayer.Drawables.SetGroupProperty("Head", "Offset", new Vector2(0, -40));
            }

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F12, this, true))
                CurrentPlayer.TerrainCollisionEnabled = !CurrentPlayer.TerrainCollisionEnabled;

            // INCREASE ZOOM LEVEL
            if(KeyboardExtensions.GetKeyDownState(keyboardState, Keys.OemPlus, this, true))
                Zoom += 0.1f;

            // DECREASE ZOOM LEVEL
            if(KeyboardExtensions.GetKeyDownState(keyboardState, Keys.OemMinus, this, true))
                Zoom -= 0.1f;

            base.Update(gameTime);
        }

        private Vector2 GeneratePos(float textHeight)
        {
            return new Vector2(0, TextCounter++ * textHeight);
        }

        private Texture2D GetMousePointer(MouseState mouseState)
        {
            Texture2D drawPointer;

            if (mouseState.LeftButton == ButtonState.Pressed)
                drawPointer = mouseCursorDown;
            else
                drawPointer = mouseCursor;

            return drawPointer;
        }

        protected override void Draw(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            GraphicsDevice.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = WINDOW_WIDTH,
                Height = WINDOW_HEIGHT,
                MinDepth = 0,
                MaxDepth = 1
            };
            GraphicsDevice.Clear(Color.Black);

            TextCounter = 0;
            float textHeight = DefaultSpriteFont.MeasureString("d").Y;

            Rectangle pxDestRectangle = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);

            // Draw the World View Port, Centered on the CurrentPlayer Actor.
            ViewPortInfo viewPort = Engine.DrawWorldViewPort(
                                            SpriteBatch,
                                            CurrentPlayer.Pos,
                                            Zoom,
                                            pxDestRectangle,
                                            Color.White,
                                            CurrentSampler,
                                            DefaultSpriteFont);

            // DRAW DEBUGGING INFORMATION
            SpriteBatch.Begin();
            {
                if (showDebugInfo)
                {
                    // DRAW THE LIGHT MAP OUTPUT TO THE SCREEN FOR DEBUGGING
                    if (LightShader.Enabled)
                    {
                        int lightMapHeight = 100;
                        int lightMapWidth = (int) Math.Ceiling(100 * ((float)LightShader.LightMap.Width / LightShader.LightMap.Height));

                        SpriteBatch.Draw(
                            LightShader.LightMap,
                            new Rectangle(
                                WINDOW_WIDTH - lightMapWidth, 0,
                                lightMapWidth, lightMapHeight
                            ),
                            Color.White
                        );
                    }

                    double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;
                    Color fpsColor = (Math.Ceiling(fps) < 60) ? Color.Red : Color.White;

                    float TX = CurrentPlayer.Pos.X / Engine.Map.TileWidth;
                    float TY = CurrentPlayer.Pos.Y / Engine.Map.TileHeight;

                    Vector2 worldCoord = viewPort.GetWorldCoordinates(new Point(mouseState.X, mouseState.Y));
                    Vector2 tileCoord = worldCoord / (new Vector2(Engine.Map.TileWidth, Engine.Map.TileHeight));;
                    tileCoord.X = (int) Math.Floor(tileCoord.X);
                    tileCoord.Y = (int) Math.Floor(tileCoord.Y);

                    SpriteBatch.DrawString(DefaultSpriteFont, CurrentPlayer.Pos.ToString(), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, TX.ToString("0.0") + "," + TY.ToString("0.0"), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Coins=" + CurrentPlayer.Coins, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "HP=" + CurrentPlayer.HP, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, fps.ToString("0.0 FPS"), GeneratePos(textHeight), fpsColor);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Resolution=" + Engine.PixelWidth + "x" + Engine.PixelHeight, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "MapSize=" + Engine.Map.txWidth + "x" + Engine.Map.txHeight, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Sampler=" + CurrentSampler.ToString(), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Entities On Screen = " + Engine.EntitiesOnScreen.Count, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Total Entities = " + Engine.Entities.Count, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Actual Zoom = " + viewPort.ActualZoom, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Mouse World Coordinates = " + worldCoord, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Mouse Tile Coordinates = " + tileCoord, GeneratePos(textHeight), Color.White);
                }

                if (showDiagnostics)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(Engine.OverallPerformance.Description);
                    builder.AppendLine(Engine.OverallPerformance.ShowAll(false));

                    builder.AppendLine(Engine.EntityUpdatePerformance.Description);
                    builder.AppendLine(Engine.EntityUpdatePerformance.ShowTop(3));

                    builder.AppendLine(Engine.UserPerformance.Description);
                    builder.AppendLine(Engine.UserPerformance.ShowTop(3));

                    string textOutput = builder.ToString();
                    SpriteBatch.DrawString(DefaultSpriteFont, textOutput, new Vector2(0, WINDOW_HEIGHT - DefaultSpriteFont.MeasureString(textOutput).Y), Color.White);
                }

                // Draw the Mouse Pointer to the screen based on the current state.
                Texture2D pointer = GetMousePointer(mouseState);
                SpriteBatch.Draw(pointer, new Rectangle(mouseState.X, mouseState.Y, pointer.Width, pointer.Height), Color.White);
            }
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
