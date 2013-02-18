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
        bool F10IsDown = false;
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

        HashSet<Keys> _lockedKeys = new HashSet<Keys>();

        public ShadowKillGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            Graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            Graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
        }

        private bool GetKeyDownState(KeyboardState KeyboardState, Keys Key, bool Lock)
        {
            bool result = false;

            if (KeyboardState.IsKeyDown(Key) && (!Lock || !_lockedKeys.Contains(Key)))
            {
                result = true;
                if (Lock) _lockedKeys.Add(Key);
            }
            else
                result = false;

            if (!KeyboardState.IsKeyDown(Key) && Lock)
                _lockedKeys.Remove(Key);

            return result;
        }

        protected override void Initialize()
        {
            WorldGenerator = new RandomWorldGenerator();

            World = new GameWorld(this, WINDOW_WIDTH, WINDOW_HEIGHT);
            World.LoadMap(WorldGenerator.Generate(Content, WORLD_WIDTH, WORLD_HEIGHT));

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
                //TODO: Move this within the actor class?
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
                if (GetKeyDownState(keyboardState, Keys.A, true) && Combo < ComboMax)
                {
                    Combo++;
                    CurrentPlayer.SetCurrentAnimation("Attack"+Combo);
                    CurrentPlayer.CurrentAnimation.ResetAnimation(gameTime);
                }

                if (GetKeyDownState(keyboardState, Keys.F1, true))
                    World.ShowBoundingBoxes = !World.ShowBoundingBoxes;

                if (GetKeyDownState(keyboardState, Keys.F10, true))
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
            Rectangle DestRectangle = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);

            World.DrawWorldViewPort(gameTime, SpriteBatch, new Vector2(CurrentPlayer.X, CurrentPlayer.Y), TILE_WIDTH, TILE_HEIGHT, DestRectangle, Color.White);     
            
            //DRAW DEBUGGING INFORMATION
            SpriteBatch.Begin();
            {
                SpriteBatch.Draw(LightShader.LightMap, new Rectangle(690, 120, 100, 100), Color.White);

                double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;

                SpriteBatch.DrawString(DefaultSpriteFont, CurrentPlayer.X.ToString("0.0") + "," + CurrentPlayer.Y.ToString("0.0"), Vector2.Zero, Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, fps.ToString("0.0 FPS"), new Vector2(0, 20), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Resolution=" + World.Width + "x" + World.Height, new Vector2(0, 40), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "MapSize=" + WORLD_WIDTH + "x" + WORLD_HEIGHT, new Vector2(0, 60), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Total Map Objects = " + World.DrawableObjects.Count, new Vector2(0, 80), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Objects On Screen = " + World.ObjectsOnScreen, new Vector2(0, 100), Color.White);
                SpriteBatch.DrawString(DefaultSpriteFont, "Light Sources On Screen = " + LightShader.LightSourcesOnScreen, new Vector2(0, 120), Color.White);
            }
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
