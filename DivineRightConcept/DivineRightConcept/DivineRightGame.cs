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

namespace DivineRightConcept
{
    public class DivineRightGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameTime prevGameTime = new GameTime();
        int x = 0;
        int y = 0;
        Texture2D _stickManTexture;

        Texture2D _groundTextures;
        int[][] _worldMap;

        int TILE_WIDTH = 8;
        int TILE_HEIGHT = 8;
        int MAP_WIDTH = 500;
        int MAP_HEIGHT = 500;

        public DivineRightGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Random random = new Random();
            _worldMap = new int[TILE_WIDTH][];
            for (int i = 0; i < _worldMap.Length; i++)
            {
                _worldMap[i] = new int[TILE_HEIGHT];
                for (int j = 0; j < _worldMap[i].Length; j++)
                    _worldMap[i][j] = random.Next(0, 4);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _groundTextures = Content.Load<Texture2D>("GroundTextures");
            _stickManTexture = Content.Load<Texture2D>("StickManTexture");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    prevGameTime = gameTime;
                    y = y - 1;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    prevGameTime = gameTime;
                    y = y + 1;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    prevGameTime = gameTime;
                    x = x - 1;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    prevGameTime = gameTime;
                    x = x + 1;
                }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //DRAW THE WORLD MAP
            int tileWidth = MAP_WIDTH/_worldMap.Length;
            int tileHeight = MAP_HEIGHT/_worldMap[0].Length;

            for (int i = 0; i < _worldMap.Length; i++)
                for (int j = 0; j < _worldMap[i].Length; j++)
                    spriteBatch.DrawGroundTexture(_groundTextures, _worldMap[i][j], new Rectangle(i*tileWidth,j*tileHeight,tileWidth,tileHeight));
            
            //DRAW THE USERS CHARACTER
            spriteBatch.Draw(_stickManTexture, new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
