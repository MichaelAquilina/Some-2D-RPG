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
using DivineRightConcept.WorldGenerators;

namespace DivineRightConcept
{
    public class DivineRightGame : Microsoft.Xna.Framework.Game
    {
        //Constant (Editable) Valuables
        const bool DEBUG = true;
        const int INPUT_DELAY = 30;

        const int WORLD_HEIGHT = 100;
        const int WORLD_WIDTH = 100;
        
        const int TILE_WIDTH = 30;
        const int TILE_HEIGHT = 30;

        const int VIEW_WIDTH = 450;
        const int VIEW_HEIGHT = 450;

        const float MOVEMENT_SPEED = 0.3f;

        Actor CurrentPlayer;

        //Graphic Related Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont _defaultSpriteFont;

        //Game Specific Variablies
        double prevGameTime = 0;
        GameWorld _world;
        IWorldGenerator _generator;

        //TO BE MOVED TO AN ACTOR CLASS
        Texture2D _spriteSheet;

        public DivineRightGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _generator = new RandomWorldGenerator();

            _world = new GameWorld(this);
            _world.WorldMap = _generator.Generate(WORLD_WIDTH, WORLD_HEIGHT);

            _world.Initialize();

            CurrentPlayer = new Actor(8, 8, 1.5f, 1.5f);
            _world.Actors.Add(CurrentPlayer);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _world.LoadContent();
            _spriteSheet = Content.Load<Texture2D>("KnucklesSheet");

            CurrentPlayer.LoadAnimationFile("Hero.anim", _spriteSheet);
            CurrentPlayer.SetCurrentAnimation("Idle");

            //LOAD THE DEFAULT FONT
            _defaultSpriteFont = Content.Load<SpriteFont>("DefaultSpriteFont");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState keyboardState = Keyboard.GetState();

            if (gameTime.TotalGameTime.TotalMilliseconds - prevGameTime > INPUT_DELAY)
            {
                CurrentPlayer.SetCurrentAnimation("Idle");

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    CurrentPlayer.SetCurrentAnimation("Running");
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.Y -= MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    CurrentPlayer.SetCurrentAnimation("Running");
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.Y += MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    CurrentPlayer.SetCurrentAnimation("Running");
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.X -= MOVEMENT_SPEED;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    CurrentPlayer.SetCurrentAnimation("Running");
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.X += MOVEMENT_SPEED;
                }

                //prevent from going out of range
                if (CurrentPlayer.X < 0) CurrentPlayer.X = 0;
                if (CurrentPlayer.Y < 0) CurrentPlayer.Y = 0;
                if (CurrentPlayer.X >= WORLD_WIDTH) CurrentPlayer.X = WORLD_WIDTH - 1;
                if (CurrentPlayer.Y >= WORLD_HEIGHT) CurrentPlayer.Y = WORLD_HEIGHT - 1;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                //ATTACK!
                CurrentPlayer.SetCurrentAnimation("Attack1");
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;

            spriteBatch.Begin();

                _world.DrawWorldViewPort(gameTime, spriteBatch, new Vector2(CurrentPlayer.X, CurrentPlayer.Y), TILE_WIDTH, TILE_HEIGHT, new Rectangle(110, 10, VIEW_WIDTH, VIEW_HEIGHT));
                _world.DrawMipMap(spriteBatch, new Rectangle(650, 0, 100, 100));

                //DRAW DEBUGGING INFORMATION
                spriteBatch.DrawString(_defaultSpriteFont, CurrentPlayer.X.ToString("0.0") + "," + CurrentPlayer.Y.ToString("0.0"), new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(_defaultSpriteFont, fps.ToString("0.0 FPS"), new Vector2(0, 20), Color.White);
                spriteBatch.DrawString(_defaultSpriteFont, "MapSize=" + WORLD_WIDTH + "x" + WORLD_HEIGHT, new Vector2(0, 40), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
