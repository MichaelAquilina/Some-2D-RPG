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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DivineRightGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
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

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _groundTextures = Content.Load<Texture2D>("GroundTextures");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            int tileWidth = MAP_WIDTH/_worldMap.Length;
            int tileHeight = MAP_HEIGHT/_worldMap[0].Length;

            for (int i = 0; i < _worldMap.Length; i++)
                for (int j = 0; j < _worldMap[i].Length; j++)
                    spriteBatch.DrawGroundTexture(_groundTextures, _worldMap[i][j], new Rectangle(i*tileWidth,j*tileHeight,tileWidth,tileHeight));
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
