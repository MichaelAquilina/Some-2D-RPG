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

namespace DivineRightConcept
{
    public class DivineRightGame : Microsoft.Xna.Framework.Game
    {
        const bool DEBUG = true;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameTime prevGameTime = new GameTime();
        int x = 5;
        int y = 5;
        Texture2D _stickManTexture;

        SpriteFont _defaultSpriteFont;
        Texture2D _groundTextures;
        int[][] _worldMap;

        //all this needs to move to a seperate class!
        int WORLD_HEIGHT = 20;
        int WORLD_WIDTH = 20;
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
                {
                    _worldMap[i][j] = random.Next(0, 4);
                }
            }

            if (DEBUG)
            {
                TextWriter writer = new StreamWriter("map_coord.txt");
                for (int i = 0; i < _worldMap.Length; i++)
                {
                    for (int j = 0; j < _worldMap.Length; j++)
                        writer.Write(_worldMap[i][j].ToString());

                    writer.WriteLine();
                }
                writer.Close();
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

            //calculate center position tile
            int centerX = TILE_WIDTH / 2;
            int centerY = TILE_WIDTH / 2;

            int topLeftX = x - centerX;
            int topLeftY = y - centerY;

            if(topLeftX<0) topLeftX = 0;
            if(topLeftY<0) topLeftY = 0;

            for (int i = 0; i < TILE_WIDTH; i++)
                for (int j = 0; j < TILE_HEIGHT; j++)
                {
                    int tileX = i + topLeftX;
                    int tileY = j + topLeftY;
                    spriteBatch.DrawGroundTexture(_groundTextures, _worldMap[tileX][tileY], new Rectangle( i * pxTileWidth, j * pxTileHeight, pxTileWidth, pxTileHeight));
                }
            
            //DRAW THE USERS CHARACTER
            //TODO: Documentation to explain how this is being calculcated!!!
            spriteBatch.Draw(_stickManTexture, new Rectangle((x - topLeftX) * pxTileWidth, (y - topLeftY) * pxTileHeight, pxTileWidth, pxTileHeight), Color.White);

            //DRAW DEBUGGING INFORMATION
            spriteBatch.DrawString(_defaultSpriteFont, x + "," + y, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
