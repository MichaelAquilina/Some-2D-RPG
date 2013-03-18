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

namespace ShadowKill
{
    public class ShadowKillGame : Microsoft.Xna.Framework.Game
    {
        //Constant (Editable) Valuables
        const bool DEBUG = true;

        const int CIRCLE_POINT_ACCURACY = 36;

        const int WINDOW_HEIGHT = 700;
        const int WINDOW_WIDTH = 1200;

        const int WORLD_HEIGHT = 500;
        const int WORLD_WIDTH = 500;
        
        const int TILE_WIDTH = 50;
        const int TILE_HEIGHT = 50;

        const int VIEW_WIDTH = 500;
        const int VIEW_HEIGHT = 480;

        bool helmetVisible = true;
        bool showDebugInfo = true;
        
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
        Hero CurrentPlayer;
        NPC FemaleNPC;
        Bat MonBat;

        TeeEngine Engine;

        public ShadowKillGame()
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

            CurrentPlayer = new Hero(15.39997f, 9.199986f);
            CurrentPlayer.Head = NPC.PLATE_ARMOR_HEAD;
            CurrentPlayer.Legs = NPC.PLATE_ARMOR_LEGS;
            CurrentPlayer.Feet = NPC.PLATE_ARMOR_FEET;
            CurrentPlayer.Shoulders = NPC.PLATE_ARMOR_SHOULDERS;
            CurrentPlayer.Torso = NPC.PLATE_ARMOR_TORSO;
            CurrentPlayer.Hands = NPC.PLATE_ARMOR_HANDS;
            CurrentPlayer.Weapon = NPC.WEAPON_DAGGER;

            FemaleNPC = new NPC(15, 9, NPC.FEMALE_HUMAN);
            //FemaleNPC.Legs = NPC.PLATE_ARMOR_LEGS;

            MonBat = new Bat();
            MonBat.TX = 19;
            MonBat.TY = 3;


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
                        entity.TX = (float)mapObject.X / Map.pxTileWidth;
                        entity.TY = (float)mapObject.Y / Map.pxTileHeight;
                        entity.rxWidth = mapObject.GetProperty<float>("Width", 1.0f);
                        entity.rxHeight = mapObject.GetProperty<float>("Height", 1.0f);
                        entity.Visible = true;
                        Animation.LoadAnimationXML(entity.Drawables, mapObject.GetProperty("AnimationSet"), Content, "");
                        entity.CurrentDrawable = mapObject.GetProperty("CurrentAnimation");
                        entity.Origin = new Vector2(0.5f, 1.0f);   //TODO: Load from Map Object Properties rather than hard code

                        Engine.Entities.Add(entity);
                    }
                    if (mapObject.Type != null && mapObject.Type.ToUpper() == "MAPOBJECT" || mapObject.Type == "")
                    {
                        Entity entity = new Entity();
                        entity.TX = (float)mapObject.X / Map.pxTileWidth;
                        entity.TY = (float)mapObject.Y / Map.pxTileHeight;
                        entity.rxWidth = 1.5f;
                        entity.rxHeight = 1.5f;
                        entity.Visible = true;

                        Tile SourceTile = Map.Tiles[mapObject.Gid];
                        entity.Drawables.Add("standard", SourceTile);
                        //entity.Drawables.SetNameProperty("standard", "Color", new Color(255, 255, 255, 200));

                        entity.CurrentDrawable = "standard";

                        Engine.Entities.Add(entity);
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

            LightShader.LightSources.Add(CurrentPlayer);
            LightShader.LightSources.Add(new BasicLightSource(1.0f, 1.0f, 29.0f, 29.0f, Color.CornflowerBlue, LightPositionType.Relative));

            LoadMapObjects(Engine.Map, Content);

            //Entity fireplace = new Entity(10.0f, 4.0f, 1.5f, 1.5f);
            //fireplace.LoadAnimationXML(@"Animations/Objects/fireplace.anim", Content);
            //fireplace.CurrentAnimation = "Burning";
            //fireplace.Origin = new Vector2(0.5f, 1.0f);

            //Engine.Entities.Add(fireplace);
            //LightShader.LightSources.Add(new BasicLightSource(fireplace.X, fireplace.Y, 7, 7, Color.OrangeRed));

            Engine.RegisterGameShader(LightShader);
            Engine.Entities.Add(MonBat);
            Engine.Entities.Add(CurrentPlayer);
            //Engine.Entities.Add(FemaleNPC);

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

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F10, true))
                Graphics.ToggleFullScreen();

            if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F11, true))
            {
                helmetVisible = !helmetVisible;
                CurrentPlayer.Drawables.SetGroupProperty("Head", "Visible", helmetVisible);
            }

            base.Update(gameTime);
        }

        private void DrawQuadTree(ViewPortInfo viewPort, SpriteBatch SpriteBatch, QuadTreeNode Node)
        {
            if( Node == null ) return;

            int PX = (int)Math.Ceiling((Node.TX - viewPort.txTopLeftX) * viewPort.pxTileWidth);
            int PY = (int)Math.Ceiling((Node.TY - viewPort.txTopLeftY) * viewPort.pxTileHeight);
            int pxWidth = (int)Math.Ceiling((Node.txWidth * viewPort.pxTileWidth));
            int pxHeight = (int)Math.Ceiling((Node.txHeight * viewPort.pxTileHeight));
            string nodeIdText = Node.NodeID.ToString();

            SpriteBatch.DrawRectangle(new Rectangle(PX, PY, pxWidth, pxHeight), Color.Lime, 0);
            SpriteBatch.DrawString(
                DefaultSpriteFont, 
                nodeIdText, 
                new Vector2(PX + pxWidth / 2.0f, PY + pxHeight / 2.0f) - DefaultSpriteFont.MeasureString(nodeIdText)/2, 
                Color.Lime
            );

            DrawQuadTree(viewPort, SpriteBatch, Node.Node1);
            DrawQuadTree(viewPort, SpriteBatch, Node.Node2);
            DrawQuadTree(viewPort, SpriteBatch, Node.Node3);
            DrawQuadTree(viewPort, SpriteBatch, Node.Node4);
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

            Rectangle pxDestRectangle = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);

            //Draw the World View Port, Centered on the CurrentPlayer Actor
            ViewPortInfo viewPort = Engine.DrawWorldViewPort(
                                            gameTime, 
                                            SpriteBatch, 
                                            new Vector2(CurrentPlayer.TX, CurrentPlayer.TY), 
                                            TILE_WIDTH, TILE_HEIGHT, 
                                            pxDestRectangle, 
                                            Color.White, 
                                            CurrentSampler);

            //DRAW DEBUGGING INFORMATION
            SpriteBatch.Begin();

            DrawQuadTree(viewPort, SpriteBatch, Engine.QuadTree.Root);

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

                SpriteBatch.DrawString(DefaultSpriteFont, CurrentPlayer.TX.ToString("0.0") + "," + CurrentPlayer.TY.ToString("0.0"), Vector2.Zero, Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, fps.ToString("0.0 FPS"), new Vector2(0, 20), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Resolution=" + Engine.pxWidth + "x" + Engine.pxHeight, new Vector2(0, 40), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "MapSize=" + Engine.Map.txWidth + "x" + Engine.Map.txHeight, new Vector2(0, 60), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, CurrentSampler.ToString(), new Vector2(0, 80), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Entities On Screen = " + Engine.EntitiesOnScreen.Count, new Vector2(0, 100), Color.White);
                //SpriteBatch.DrawString(DefaultSpriteFont, "Total Map Entities = " + Engine.Entities.Count, new Vector2(0, 80), Color.White);
                //SpriteBatch.DrawString(DefaultSpriteFont, "Light Sources = " + LightShader.LightSources.Count, new Vector2(0, 120), Color.White);
                //SpriteBatch.DrawString(DefaultSpriteFont, "Current Player Animation = " + CurrentPlayer.CurrentAnimation, new Vector2(0,140), Color.White);
                //SpriteBatch.DrawString(DefaultSpriteFont, "Circle Point Accuracy = " + LightShader.CirclePointAccurracy, new Vector2(0, 160), Color.White);
            }
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
