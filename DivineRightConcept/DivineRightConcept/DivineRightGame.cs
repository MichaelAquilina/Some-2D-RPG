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

        //all this needs to move to a seperate class!
        int WORLD_HEIGHT = 200;
        int WORLD_WIDTH = 200;
        int TILE_WIDTH = 15;
        int TILE_HEIGHT = 15;
        int VIEW_WIDTH = 500;
        int VIEW_HEIGHT = 500;

        GameWorld _worldComponent;

        public DivineRightGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _worldComponent = new GameWorld(this, WORLD_WIDTH, WORLD_HEIGHT, TILE_WIDTH, TILE_HEIGHT);
            _worldComponent.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _worldComponent.StickManTexture = Content.Load<Texture2D>("StickManTexture");
            _worldComponent.GroundTextures = Content.Load<Texture2D>("GroundTextures");

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

            _worldComponent.DrawWorldViewPort(spriteBatch, x, y, new Rectangle(100, 0, VIEW_WIDTH, VIEW_HEIGHT));

            //DRAW DEBUGGING INFORMATION
            spriteBatch.DrawString(_defaultSpriteFont, x + "," + y, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
