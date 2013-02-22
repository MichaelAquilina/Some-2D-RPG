using GameEngine;
using GameEngine.Helpers;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShadowKill.GameObjects;
using ShadowKill.Shaders;
using ShadowKill.WorldGenerators;
using System;
using GameEngine.GameObjects;

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

        const float MOVEMENT_SPEED = 0.2f;

        double PrevGameTime = 0;
        LightShader LightShader;

        //Graphic Related Variables
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        SpriteFont DefaultSpriteFont;

        //Game Specific Variablies
        Hero CurrentPlayer;
        TileEngine World;
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
            Map loadedMap = WorldGenerator.Generate(Content, WORLD_WIDTH, WORLD_HEIGHT);

            World = new TileEngine(this, WINDOW_WIDTH, WINDOW_HEIGHT);
            World.LoadMap(loadedMap);

            CurrentPlayer = new Hero(8, 8);
            CurrentPlayer.Origin = new Vector2(0.5f, 1.0f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            LightShader = new LightShader(this.GraphicsDevice);
            LightShader.AmbientLight = new Color(30, 15, 15);
            LightShader.LightSources.Add(CurrentPlayer);

            World.RegisterGameShader(LightShader);
            World.Entities.Add(CurrentPlayer);

            World.LoadContent();

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            DefaultSpriteFont = Content.Load<SpriteFont>(@"Fonts\DefaultSpriteFont");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (gameTime.TotalGameTime.TotalMilliseconds - PrevGameTime > INPUT_DELAY)
            {
                bool moved = false;

                //MOVEMENT BASED KEYBOARD EVENTS
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    CurrentPlayer.CurrentAnimation = "Walk_Up";
                    CurrentPlayer.Direction = Direction.Up;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.Y -= MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    CurrentPlayer.CurrentAnimation = "Walk_Down";
                    CurrentPlayer.Direction = Direction.Down;
                    moved = true;
                    
                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.Y += MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    CurrentPlayer.CurrentAnimation = "Walk_Left";
                    CurrentPlayer.Direction = Direction.Left;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.X -= MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    CurrentPlayer.CurrentAnimation = "Walk_Right";
                    CurrentPlayer.Direction = Direction.Right;
                    moved = true;

                    PrevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.X += MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    CurrentPlayer.CurrentAnimation = "Slash_" + CurrentPlayer.Direction;
                    moved = true;
                }

                if (moved == false)
                    CurrentPlayer.CurrentAnimation = "Idle_" + CurrentPlayer.Direction;

                if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F1, true))
                    World.ShowBoundingBoxes = !World.ShowBoundingBoxes;

                if (KeyboardHelper.GetKeyDownState(keyboardState, Keys.F10, true))
                    Graphics.ToggleFullScreen();

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
            Rectangle destRectangle = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);

            //Draw the World View Port, Centered on the CurrentPlayer Actor
            World.DrawWorldViewPort(gameTime, SpriteBatch, new Vector2(CurrentPlayer.X, CurrentPlayer.Y), TILE_WIDTH, TILE_HEIGHT, destRectangle, Color.White);     
            
            //DRAW DEBUGGING INFORMATION
            SpriteBatch.Begin();
            {
                //DRAW THE LIGHT MAP OUTPUT TO THE SCREEN FOR DEBUGGING
                int lightMapHeight = 100;
                int lightMapWidth = (int) Math.Ceiling(100 * ((float) LightShader.LightMap.Width/LightShader.LightMap.Height));

                SpriteBatch.Draw(
                    LightShader.LightMap, 
                    new Rectangle(
                        WINDOW_WIDTH - lightMapWidth, 0, 
                        lightMapWidth, lightMapHeight
                    ), 
                    Color.White
                );

                double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;

                SpriteBatch.DrawString(DefaultSpriteFont, CurrentPlayer.X.ToString("0.0") + "," + CurrentPlayer.Y.ToString("0.0"), Vector2.Zero, Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, fps.ToString("0.0 FPS"), new Vector2(0, 20), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Resolution=" + World.Width + "x" + World.Height, new Vector2(0, 40), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "MapSize=" + WORLD_WIDTH + "x" + WORLD_HEIGHT, new Vector2(0, 60), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Total Map Objects = " + World.Entities.Count, new Vector2(0, 80), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Animations On Screen = " + World.AnimationsOnScreen, new Vector2(0, 100), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Light Sources On Screen = " + LightShader.LightSourcesOnScreen, new Vector2(0, 120), Color.White);
            }
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
