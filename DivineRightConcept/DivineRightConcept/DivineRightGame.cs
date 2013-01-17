using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text;
using System.IO;
using DivineRightConcept.GameObjects;

namespace DivineRightConcept
{
    public class DivineRightGame : Microsoft.Xna.Framework.Game
    {
        //Constant (Editable) Valuables
        const bool DEBUG = true;
        const int INPUT_DELAY = 20;

        const int WORLD_HEIGHT = 50;
        const int WORLD_WIDTH = 50;
        
        const int TILE_WIDTH = 13;
        const int TILE_HEIGHT = 13;

        const int VIEW_WIDTH = 450;
        const int VIEW_HEIGHT = 450;

        Actor CurrentPlayer;

        //Graphic Related Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont _defaultSpriteFont;

        //Game Specific Variablies
        double prevGameTime = 0;
        GameWorld _world;

        public DivineRightGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _world = new GameWorld(this, WORLD_WIDTH, WORLD_HEIGHT);
            _world.Initialize();

            CurrentPlayer = new Actor(8, 8);
            _world.Actors.Add(CurrentPlayer);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _world.LoadContent();

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
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.Y -= 1;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.Y += 1;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.X -= 1;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    CurrentPlayer.X += 1;
                }

                //prevent from going out of range
                if (CurrentPlayer.X < 0) CurrentPlayer.X = 0;
                if (CurrentPlayer.Y < 0) CurrentPlayer.Y = 0;
                if (CurrentPlayer.X >= WORLD_WIDTH) CurrentPlayer.X = WORLD_WIDTH - 1;
                if (CurrentPlayer.Y >= WORLD_HEIGHT) CurrentPlayer.Y = WORLD_HEIGHT - 1;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;

            spriteBatch.Begin();

                _world.DrawWorldViewPort(spriteBatch, new Vector2(CurrentPlayer.X, CurrentPlayer.Y), TILE_WIDTH, TILE_HEIGHT, new Rectangle(100, 0, VIEW_WIDTH, VIEW_HEIGHT));
                spriteBatch.Draw(_world.MiniMapTexture, new Rectangle(650, 0, 100, 100), Color.White);

                //DRAW DEBUGGING INFORMATION
                spriteBatch.DrawString(_defaultSpriteFont, CurrentPlayer.X + "," + CurrentPlayer.Y, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(_defaultSpriteFont, fps.ToString("0.0 FPS"), new Vector2(0, 20), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
