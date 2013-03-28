using System;
using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Helpers;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShadowKill.GameObjects;
using ShadowKill.Shaders;
using ShadowKillGame.GameObjects;
using GameEngine.Drawing;
using GameEngine.Geometry;
using GameEngine.DataStructures;
using GameEngine.Info;
using System.Diagnostics;
using System.Text;

namespace ShadowKill
{
    public class Some2DRPG : Microsoft.Xna.Framework.Game
    {
        //Constant (Editable) Valuables
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
        bool showQuadTree = false;
        bool showDiagnostics = false;

        float Zoom = 1.8f;

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

        //Graphic Related Variables
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        SpriteFont DefaultSpriteFont;

        //Game Specific Variablies
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

        private void LoadMapObjects(TiledMap Map, ContentManager Content)
        {
            foreach (ObjectLayer layer in Map.ObjectLayers)
            {
                foreach (MapObject mapObject in layer.Objects)
                {
                    if (mapObject.Type != null && mapObject.Type.ToUpper() == "ENTITY")
                    {
                        Entity entity = new Entity();
                        entity.PX = mapObject.X;
                        entity.PY = mapObject.Y;
                        entity.ScaleX = mapObject.GetProperty<float>("Width", 1.0f);
                        entity.ScaleY = mapObject.GetProperty<float>("Height", 1.0f);
                        entity.Visible = true;
                        Animation.LoadAnimationXML(entity.Drawables, mapObject.GetProperty("AnimationSet"), Content, "");
                        entity.CurrentDrawable = mapObject.GetProperty("CurrentAnimation");
                        entity.Origin = new Vector2(0.5f, 1.0f);   //TODO: Load from Map Object Properties rather than hard code

                        Engine.AddEntity(mapObject.Name, entity);
                    }
                    if (mapObject.Type != null && mapObject.Type.ToUpper() == "MAPOBJECT" || mapObject.Type == "")
                    {
                        Entity entity = new Entity();
                        entity.PX = mapObject.X;
                        entity.PY = mapObject.Y;
                        entity.ScaleX = 1.0f;
                        entity.ScaleY = 1.0f;
                        entity.Visible = true;

                        Tile SourceTile = Map.Tiles[mapObject.Gid];
                        entity.Drawables.Add("standard", SourceTile);
                        //entity.Drawables.SetNameProperty("standard", "Color", new Color(255, 255, 255, 200));

                        entity.CurrentDrawable = "standard";

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

            Engine.ShowBoundingBoxes = false;
            Engine.ShowTileGrid = false;

            Random random = new Random();
            LoadMapObjects(Engine.Map, Content);
      
            Engine.AddEntity("Player", CurrentPlayer);

            for (int i = 0; i < 200; i++)
            {
                int px = (int) Math.Ceiling(random.NextDouble() * Engine.Map.pxWidth);
                int py = (int) Math.Ceiling(random.NextDouble() * Engine.Map.pxHeight);

                Bat bat = new Bat(px, py);
                Engine.AddEntity(bat);
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
            //F1 = Show/Hide Bounding Boxes
            //F2 = Show/Hide Debug Info
            //F3 = Enable/Disable LightShader
            //F4 = Change Current SamplerState
            //F5 = Show/Hide Tile Grid
            //F6 = Show/Hide Quad Tree
            //F10 = Toggle Fullscreen Mode
            //F11 = Show/Hide Player Helmet

            KeyboardState keyboardState = Keyboard.GetState();

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F1, true))
                Engine.ShowBoundingBoxes = !Engine.ShowBoundingBoxes;

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F2, true))
                showDebugInfo = !showDebugInfo;

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F3, true))
                LightShader.Enabled = !LightShader.Enabled;

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F4, true))
                CurrentSampler = SamplerStates[++SamplerIndex % SamplerStates.Length];

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F5, true))
                Engine.ShowTileGrid = !Engine.ShowTileGrid;

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F6, true))
                showQuadTree = !showQuadTree;

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F7, true))
                showDiagnostics = !showDiagnostics;

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F10, true))
                Graphics.ToggleFullScreen();

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F11, true))
            {
                helmetVisible = !helmetVisible;
                CurrentPlayer.Drawables.SetGroupProperty("Head", "Visible", helmetVisible);
            }

            if(KeyboardHelper.GetKeyDownState(keyboardState, Keys.OemPlus, true))
                Zoom += 0.1f;

            if(KeyboardHelper.GetKeyDownState(keyboardState, Keys.OemMinus, true))
                Zoom -= 0.1f;

            base.Update(gameTime);
        }

        private void DrawQuadTree(ViewPortInfo viewPort, SpriteBatch SpriteBatch, QuadTreeNode Node, Rectangle DestRectangle)
        {
            if( Node == null ) return;

            float PX = Node.pxBounds.X - viewPort.pxTopLeftX;
            float PY = Node.pxBounds.Y - viewPort.pxTopLeftY;
            int pxWidth = Node.pxBounds.Width;
            int pxHeight = Node.pxBounds.Height;
            string nodeIdText = Node.NodeID.ToString();

            if (new Rectangle((int)PX, (int)PY, pxWidth, pxHeight).Intersects(DestRectangle))
            {
                PX = (int)Math.Ceiling(PX * viewPort.ActualZoom);
                PY = (int)Math.Ceiling(PY * viewPort.ActualZoom);
                pxWidth = (int)Math.Ceiling(pxWidth * viewPort.ActualZoom);
                pxHeight = (int)Math.Ceiling(pxHeight * viewPort.ActualZoom);

                SpriteBatch.DrawRectangle(new Rectangle((int)PX, (int)PY, pxWidth, pxHeight), Color.Lime, 0);
                SpriteBatch.DrawString(
                    DefaultSpriteFont,
                    nodeIdText,
                    new Vector2(PX + pxWidth / 2.0f, PY + pxHeight / 2.0f) - DefaultSpriteFont.MeasureString(nodeIdText) / 2,
                    Color.Lime
                );

                DrawQuadTree(viewPort, SpriteBatch, Node.Node1, DestRectangle);
                DrawQuadTree(viewPort, SpriteBatch, Node.Node2, DestRectangle);
                DrawQuadTree(viewPort, SpriteBatch, Node.Node3, DestRectangle);
                DrawQuadTree(viewPort, SpriteBatch, Node.Node4, DestRectangle);
            }
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

            //Draw the World View Port, Centered on the CurrentPlayer Actor
            ViewPortInfo viewPort = Engine.DrawWorldViewPort(
                                            SpriteBatch,
                                            FollowEntity.PX,
                                            FollowEntity.PY,
                                            Zoom,
                                            pxDestRectangle,
                                            Color.White,
                                            CurrentSampler);

            //DRAW DEBUGGING INFORMATION
            SpriteBatch.Begin();
            {
                if (showQuadTree) DrawQuadTree(viewPort, SpriteBatch, Engine.QuadTree.Root, pxDestRectangle);

                if (showDebugInfo)
                {
                    //DRAW THE LIGHT MAP OUTPUT TO THE SCREEN FOR DEBUGGING
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

                    float TX = CurrentPlayer.PX / Engine.Map.pxTileWidth;
                    float TY = CurrentPlayer.PY / Engine.Map.pxTileHeight;

                    SpriteBatch.DrawString(DefaultSpriteFont, CurrentPlayer.PX.ToString("0.0") + "," + CurrentPlayer.PY.ToString("0.0"), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, TX.ToString("0.0") + "," + TY.ToString("0.0"), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, fps.ToString("0.0 FPS"), GeneratePos(textHeight), fpsColor);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Resolution=" + Engine.pxWidth + "x" + Engine.pxHeight, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "MapSize=" + Engine.Map.txWidth + "x" + Engine.Map.txHeight, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Sampler=" + CurrentSampler.ToString(), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Entities On Screen = " + Engine.EntitiesOnScreen.Count, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Total Entities = " + Engine.Entities.Count, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Latest Node Index = " + Engine.QuadTree.LatestNodeIndex, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Zoom = " + viewPort.ActualZoom, GeneratePos(textHeight), Color.White);
                }

                if (showDiagnostics)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(Engine.DebugInfo.ToString());

                    builder.AppendLine("Entity Update Times");
                    foreach (string entityId in Engine.DebugInfo.GetTop(Engine.DebugInfo.EntityUpdateTimes, 3).Keys)
                        builder.AppendLine(string.Format("'{0}' = {1}", entityId, Engine.DebugInfo.EntityUpdateTimes[entityId]));

                    builder.AppendLine("Entity Rendering Times");
                    foreach (string entityId in Engine.DebugInfo.GetTop(Engine.DebugInfo.EntityRenderingTimes, 3).Keys)
                        builder.AppendLine(string.Format("'{0}' = {1}", entityId, Engine.DebugInfo.EntityUpdateTimes[entityId]));

                    string textOutput = builder.ToString();
                    SpriteBatch.DrawString(DefaultSpriteFont, textOutput, new Vector2(0, WINDOW_HEIGHT - DefaultSpriteFont.MeasureString(textOutput).Y), Color.White);
                }
            }
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
