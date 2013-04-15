using System;
using System.Text;
using GameEngine;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Info;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShadowKillGame.GameObjects;
using Some2DRPG.GameObjects;
using Some2DRPG.Shaders;
using GameEngine.Drawing;
using System.Collections.Generic;

namespace Some2DRPG
{
    public class Some2DRPG : Microsoft.Xna.Framework.Game
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

        // Graphic Related Variables.
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        SpriteFont DefaultSpriteFont;

        // Game Specific Variablies.
        Entity FollowEntity;
        Hero CurrentPlayer;
        NPC FemaleNPC;

        TeeEngine Engine;

        public Some2DRPG()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            Graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            Graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
        }

        protected override void Initialize()
        {
            TiledMap tiledmap = TiledMap.LoadTiledXML("Content/example_map.tmx", Content);

            Engine = new TeeEngine(this, WINDOW_WIDTH, WINDOW_HEIGHT);
            Engine.LoadMap(tiledmap);

            CurrentPlayer = new Hero(32 * 4, 32 * 4);
            CurrentPlayer.CollisionDetection = true;
            CurrentPlayer.Head = NPC.PLATE_ARMOR_HEAD;
            CurrentPlayer.Legs = NPC.PLATE_ARMOR_LEGS;
            CurrentPlayer.Feet = NPC.PLATE_ARMOR_FEET;
            CurrentPlayer.Shoulders = NPC.PLATE_ARMOR_SHOULDERS;
            CurrentPlayer.Torso = NPC.PLATE_ARMOR_TORSO;
            CurrentPlayer.Hands = NPC.PLATE_ARMOR_HANDS;
            CurrentPlayer.Weapon = NPC.WEAPON_LONGSWORD;

            FollowEntity = CurrentPlayer;

            FemaleNPC = new NPC(15, 9, NPC.FEMALE_HUMAN);
            //FemaleNPC.Legs = NPC.PLATE_ARMOR_LEGS;

            base.Initialize();
        }

        // Loads objects in a tmx map file, created in tiled
        private void LoadMapObjects(TiledMap map, ContentManager content)
        {
            Random random = new Random();

            foreach (ObjectLayer layer in map.ObjectLayers)
            {
                foreach (MapObject mapObject in layer.Objects)
                {
                    if (mapObject.Type == "Chest")
                    {
                        Chest chest = new Chest(mapObject.X, mapObject.Y);

                        Engine.AddEntity(chest);
                    }
                    else
                    if (mapObject.Type == "CoinArea")
                    {
                        int density = mapObject.GetProperty<int>("Density", 0);
                        string coinTypeString = mapObject.GetProperty("CoinType", "Gold");

                        int coinx = mapObject.Width / density;
                        int coiny = mapObject.Height / density;

                        for (int i = 0; i < coinx; i++)
                        {
                            for (int j = 0; j < coiny; j++)
                            {
                                CoinType coinType;
                                if (coinTypeString == "Mixed")
                                    coinType = (CoinType)random.Next(3);
                                else
                                    coinType = (CoinType)Enum.Parse(typeof(CoinType), coinTypeString);

                                Coin coin = new Coin(mapObject.X + i * density, mapObject.Y + j * density, 100, coinType);
                                coin.Visible = true;

                                Engine.AddEntity(coin);
                            }
                        }
                    }
                    if (mapObject.Type == "Coin")
                    {
                        string coinTypeString = mapObject.GetProperty("CoinType", "Gold");
                        CoinType coinType = coinType = (CoinType)Enum.Parse(typeof(CoinType), coinTypeString);

                        Coin coin = new Coin(mapObject.X, mapObject.Y, 100, coinType);
                        coin.Visible = true;

                        Engine.AddEntity(coin);
                    }
                    else
                    if (mapObject.Gid != -1)
                    {
                        Entity entity = new Entity();
                        entity.Pos = new Vector2(mapObject.X, mapObject.Y);
                        entity.ScaleX = 1.0f;
                        entity.ScaleY = 1.0f;
                        entity.Visible = true;

                        Tile SourceTile = map.Tiles[mapObject.Gid];
                        GameDrawableInstance instance = entity.Drawables.Add("standard", SourceTile);
                        //entity.Drawables.SetNameProperty("standard", "Color", new Color(255, 255, 255, 200));

                        entity.CurrentDrawableState = "standard";

                        // Because tileds default draw origin is (0,1) - we need to update the entity positions based 
                        // on the custom position defined in the SourceTile.Origin property.
                        entity.Pos.X += (SourceTile.Origin.X - 0.0f) * SourceTile.GetSourceRectangle(0).Width;
                        entity.Pos.Y += (SourceTile.Origin.Y - 1.0f) * SourceTile.GetSourceRectangle(0).Height;

                        Engine.AddEntity(mapObject.Name, entity);
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            CurrentSampler = SamplerStates[SamplerIndex];

            LightShader = new LightShader(this.GraphicsDevice, CIRCLE_POINT_ACCURACY);
            LightShader.AmbientLight = new Color(30, 15, 15);
            LightShader.Enabled = false;

            LightShader.LightSources.Add(CurrentPlayer.LightSource);
            LightShader.LightSources.Add(new BasicLightSource(32, 32, 32 * 29.0f, 32 * 29.0f, Color.CornflowerBlue, LightPositionType.Relative));
            Engine.RegisterGameShader(LightShader);

            Engine.DrawingOptions.ShowEntityDebugInfo = false;
            Engine.DrawingOptions.ShowBoundingBoxes = false;
            Engine.DrawingOptions.ShowTileGrid = false;

            Random random = new Random();
            LoadMapObjects(Engine.Map, Content);
      
            Engine.AddEntity("Player", CurrentPlayer);

            for (int i = 0; i < 50; i++)
            {
                int px = (int) Math.Ceiling(random.NextDouble() * Engine.Map.pxWidth);
                int py = (int) Math.Ceiling(random.NextDouble() * Engine.Map.pxHeight);

                Bat bat = new Bat(px, py);
                Coin coin = new Coin(px, py, 100, (CoinType) random.Next(3));

                // Switch between adding bats and coins to the map.
                if (i % 2 == 0) Engine.AddEntity(bat);
                else
                    Engine.AddEntity(coin);
                //FollowEntity = bat;
            }

            Engine.LoadContent();

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            DefaultSpriteFont = Content.Load<SpriteFont>(@"Fonts\DefaultSpriteFont");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // F1 = Show/Hide Bounding Boxes
            // F2 = Show/Hide Debug Info
            // F3 = Enable/Disable LightShader
            // F4 = Change Current SamplerState
            // F5 = Show/Hide Tile Grid
            // F6 = Show/Hide Quad Tree
            // F7 = Show Performance Diagnostics
            // F8 = Show Entity Debug Info
            // F10 = Toggle Fullscreen Mode
            // F11 = Show/Hide Player Helmet
            // F12 = Disable Player Collision Detection

            KeyboardState keyboardState = Keyboard.GetState();

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F1, this, true))
                Engine.DrawingOptions.ShowDrawableComponents = !Engine.DrawingOptions.ShowDrawableComponents;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F2, this, true))
                showDebugInfo = !showDebugInfo;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F3, this, true))
                LightShader.Enabled = !LightShader.Enabled;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F4, this, true))
                CurrentSampler = SamplerStates[++SamplerIndex % SamplerStates.Length];

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F5, this, true))
                Engine.DrawingOptions.ShowTileGrid = !Engine.DrawingOptions.ShowTileGrid;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F6, this, true))
                Engine.DrawingOptions.ShowQuadTree = !Engine.DrawingOptions.ShowQuadTree;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F7, this, true))
                showDiagnostics = !showDiagnostics;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F8, this, true))
                Engine.DrawingOptions.ShowEntityDebugInfo = !Engine.DrawingOptions.ShowEntityDebugInfo;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F10, this, true))
                Graphics.ToggleFullScreen();

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
                CurrentPlayer.CollisionDetection = !CurrentPlayer.CollisionDetection;

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

        protected override void Draw(GameTime gameTime)
        {
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
                                            FollowEntity.Pos.X,
                                            FollowEntity.Pos.Y,
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
                        int lightMapWidth = (int)Math.Ceiling(100 * ((float)LightShader.LightMap.Width / LightShader.LightMap.Height));

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

                    float TX = CurrentPlayer.Pos.X / Engine.Map.pxTileWidth;
                    float TY = CurrentPlayer.Pos.Y / Engine.Map.pxTileHeight;

                    SpriteBatch.DrawString(DefaultSpriteFont, CurrentPlayer.Pos.ToString(), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, TX.ToString("0.0") + "," + TY.ToString("0.0"), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Coins=" + CurrentPlayer.Coins, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "HP=" + CurrentPlayer.HP, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, fps.ToString("0.0 FPS"), GeneratePos(textHeight), fpsColor);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Resolution=" + Engine.pxWidth + "x" + Engine.pxHeight, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "MapSize=" + Engine.Map.txWidth + "x" + Engine.Map.txHeight, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Sampler=" + CurrentSampler.ToString(), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Entities On Screen = " + Engine.EntitiesOnScreen.Count, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Total Entities = " + Engine.Entities.Count, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Latest Node Index = " + Engine.QuadTree.LatestNodeIndex, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Actual Zoom = " + viewPort.ActualZoom, GeneratePos(textHeight), Color.White);
                }

                if (showDiagnostics)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(Engine.DebugInfo.ToString());

                    builder.AppendLine("Entity Update Times");
                    Dictionary<string, TimeSpan> topUpdateTimes = Engine.DebugInfo.GetTop(Engine.DebugInfo.EntityUpdateTimes, 3);
                    foreach (string entityId in topUpdateTimes.Keys)
                        builder.AppendLine(string.Format("'{0}' = {1}", entityId, topUpdateTimes[entityId]));

                    builder.AppendLine("Entity Rendering Times");
                    Dictionary<string, TimeSpan> topRenderTimes = Engine.DebugInfo.GetTop(Engine.DebugInfo.EntityRenderingTimes, 3);
                    foreach (string entityId in topRenderTimes.Keys)
                        builder.AppendLine(string.Format("'{0}' = {1}", entityId, topRenderTimes[entityId]));

                    string textOutput = builder.ToString();
                    SpriteBatch.DrawString(DefaultSpriteFont, textOutput, new Vector2(0, WINDOW_HEIGHT - DefaultSpriteFont.MeasureString(textOutput).Y), Color.White);
                }
            }
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
