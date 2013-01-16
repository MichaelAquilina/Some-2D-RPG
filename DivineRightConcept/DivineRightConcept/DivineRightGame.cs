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

        SpriteFont _defaultSpriteFont;
        Texture2D _groundTextures;
        int[][] _worldMap;

        //all this needs to move to a seperate class!
        int WORLD_HEIGHT = 500;
        int WORLD_WIDTH = 500;
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
            _worldMap = new int[WORLD_HEIGHT][];
            for (int i = 0; i < _worldMap.Length; i++)
            {
                _worldMap[i] = new int[WORLD_WIDTH];
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

                //prevent from going out of range
                if (x < 0) x = 0;
                if (y < 0) y = 0;
                if (x >= WORLD_WIDTH) x = WORLD_WIDTH - 1;
                if (y >= WORLD_HEIGHT) y = WORLD_HEIGHT - 1;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //DRAW THE WORLD MAP
            int pxTileWidth = MAP_WIDTH/TILE_WIDTH;
            int pxTileHeight = MAP_HEIGHT/TILE_HEIGHT;

            int centerX = TILE_WIDTH / 2;
            int centerY = TILE_WIDTH / 2;

            for (int i = 0; i < TILE_WIDTH; i++)
                for (int j = 0; j < TILE_HEIGHT; j++)
                {
                    spriteBatch.DrawGroundTexture(_groundTextures, _worldMap[i + x][j + y], new Rectangle(i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight));
                    //spriteBatch.DrawString(_defaultSpriteFont, (i + x) + "," + (j + y), new Vector2(i * pxTileWidth, j * pxTileHeight), Color.White);
                }
            
            //DRAW THE USERS CHARACTER
            spriteBatch.Draw(_stickManTexture, new Rectangle(centerX * pxTileWidth, centerY * pxTileHeight, pxTileWidth, pxTileHeight), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
