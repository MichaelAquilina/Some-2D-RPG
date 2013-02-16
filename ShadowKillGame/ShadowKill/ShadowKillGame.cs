using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ShadowKill.WorldGenerators;
using ShadowKill.GameObjects;
using GameEngine.Shaders;
using ShadowKill.Shaders;

namespace ShadowKill
{
    public class ShadowKillGame : Microsoft.Xna.Framework.Game
    {
        //Constant (Editable) Valuables
        const bool DEBUG = true;
        const int INPUT_DELAY = 30;

        const int WINDOW_HEIGHT = 500;
        const int WINDOW_WIDTH = 900;

        const int WORLD_HEIGHT = 500;
        const int WORLD_WIDTH = 500;
        
        const int TILE_WIDTH = 50;
        const int TILE_HEIGHT = 50;

        const int VIEW_WIDTH = 500;
        const int VIEW_HEIGHT = 480;

        const float MOVEMENT_SPEED = 0.3f;

        double PrevGameTime = 0;
        bool F1IsDown = false;
        bool AIsDown = false;
        int Combo = 0;
        int ComboMax = 4;

        LightShader LightShader;

        //Graphic Related Variables
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        SpriteFont DefaultSpriteFont;

        //Game Specific Variablies
        Hero CurrentPlayer;
        GameWorld World;
        IWorldGenerator WorldGenerator;

        public ShadowKillGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            Graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            Graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
        }

        protected override void Initialize()
        {
            WorldGenerator = new RandomWorldGenerator();

            World = new GameWorld(this, VIEW_WIDTH, VIEW_HEIGHT);
            World.WorldMap = WorldGenerator.Generate(WORLD_WIDTH, WORLD_HEIGHT);

            CurrentPlayer = new Hero(8, 8);
            CurrentPlayer.Origin = new Vector2(0.5f, 1.0f);

            LightShader = new LightShader(this.GraphicsDevice);
            LightShader.AmbientLight = new Color(50,40,30);
            LightShader.LightSources.Add(CurrentPlayer);

            World.RegisterObject(CurrentPlayer);
            World.RegisterObject(LightShader);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            World.LoadContent();

            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Rectangle[] mapObjectSrcRectangles = new Rectangle[] { 
                new Rectangle(3, 13, 113, 103)
                ,new Rectangle(124, 4, 72, 109)
                ,new Rectangle(280,40, 30, 57)
                ,new Rectangle(320,80, 40, 32)
                //,new Rectangle(203,40, 73, 80)
            };

            //GENERATION AND STORAGE OF MAP OBJECTS SHOULD BE WITHIIN THE MAP FILE ITSELF (TODO)
            Random random = new Random();
            for (int i = 0; i < WORLD_WIDTH * WORLD_WIDTH / 7; i++)
            {
                float treeX = (float) (random.NextDouble() * WORLD_WIDTH);
                float treeY = (float) (random.NextDouble() * WORLD_HEIGHT);

                MapObject mapObject = new MapObject(treeX, treeY, 1.0f, 1.0f);

                mapObject.SourceRectangle = mapObjectSrcRectangles[random.Next(0, mapObjectSrcRectangles.Length)];

                mapObject.SourceTexture = Content.Load<Texture2D>(@"MapObjects\OBJECTS");
                mapObject.Origin = new Vector2(0.5f, 1.0f);

                World.DrawableObjects.Add(mapObject);
            }

            //LOAD THE DEFAULT FONT
            DefaultSpriteFont = Content.Load<SpriteFont>(@"Fonts\DefaultSpriteFont");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState keyboardState = Keyboard.GetState();

            if (gameTime.TotalGameTime.TotalMilliseconds - PrevGameTime > INPUT_DELAY)
            {
                //If the current animation has finished, then revert to default
                if (CurrentPlayer.CurrentAnimation.IsFinished(gameTime))
                {
                    Combo = 0;
                    CurrentPlayer.SetCurrentAnimation("Idle");
                }

                //MOVEMENT BASED KEYBOARD EVENTS
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    CurrentPlayer.SetCurrentAnimation("Running");
                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.Y -= MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    CurrentPlayer.SetCurrentAnimation("Running");
                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.Y += MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    CurrentPlayer.SetCurrentAnimation("Running");
                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.X -= MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    CurrentPlayer.SetCurrentAnimation("Running");
                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.X += MOVEMENT_SPEED;
                }

                //ACTION BASED KEYBOARD EVENTS
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    if (!AIsDown && Combo < ComboMax)
                    {
                        //ATTACK!
                        AIsDown = true;
                        Combo++;

                        CurrentPlayer.SetCurrentAnimation("Attack"+Combo);
                        CurrentPlayer.CurrentAnimation.ResetAnimation(gameTime);
                    }
                }
                else
                {
                    AIsDown = false;
                }

                if (keyboardState.IsKeyDown(Keys.F1))
                {
                    if (!F1IsDown && Combo < ComboMax)
                    {
                        F1IsDown = true;
                        World.ShowBoundingBoxes = !World.ShowBoundingBoxes;
                    }
                }
                else
                {
                    F1IsDown = false;
                }

                //prevent from going out of range
                if (CurrentPlayer.X < 0) CurrentPlayer.X = 0;
                if (CurrentPlayer.Y < 0) CurrentPlayer.Y = 0;
                if (CurrentPlayer.X >= WORLD_WIDTH -1) CurrentPlayer.X = WORLD_WIDTH - 1;
                if (CurrentPlayer.Y >= WORLD_HEIGHT -1) CurrentPlayer.Y = WORLD_HEIGHT - 1;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Rectangle DestRectangle = new Rectangle(180, 10, VIEW_WIDTH, VIEW_HEIGHT);

            World.DrawWorldViewPort(gameTime, SpriteBatch, new Vector2(CurrentPlayer.X, CurrentPlayer.Y), TILE_WIDTH, TILE_HEIGHT, DestRectangle, Color.White);     
            
            //DRAW DEBUGGING INFORMATION
            SpriteBatch.Begin();
            {
                SpriteBatch.Draw(LightShader.LightMap, new Rectangle(690, 120, 100, 100), Color.White);

                double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;

                SpriteBatch.DrawString(DefaultSpriteFont, CurrentPlayer.X.ToString("0.0") + "," + CurrentPlayer.Y.ToString("0.0"), Vector2.Zero, Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, fps.ToString("0.0 FPS"), new Vector2(0, 20), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "MapSize=" + WORLD_WIDTH + "x" + WORLD_HEIGHT, new Vector2(0, 40), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Total Map Objects = " + World.DrawableObjects.Count, new Vector2(0, 60), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Objects On Screen = " + World.ObjectsOnScreen, new Vector2(0, 80), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Light Sources On Screen = " + LightShader.LightSourcesOnScreen, new Vector2(0, 100), Color.White);
            }
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
