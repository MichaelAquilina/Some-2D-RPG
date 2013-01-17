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
        //Constant (Editable) Valuables
        const bool DEBUG = true;
        const int WORLD_HEIGHT = 200;
        const int WORLD_WIDTH = 200;
        const int TILE_WIDTH = 15;
        const int TILE_HEIGHT = 15;
        const int VIEW_WIDTH = 450;
        const int VIEW_HEIGHT = 450;

        //Graphic Related Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont _defaultSpriteFont;

        //Game Specific Variablies
        double prevGameTime = 0;
        GameWorld _worldComponent;
        int PlayerX = 0;
        int PlayerY = 0;

        public DivineRightGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _worldComponent = new GameWorld(this, WORLD_WIDTH, WORLD_HEIGHT);
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

            if (gameTime.TotalGameTime.TotalMilliseconds - prevGameTime > 50)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    PlayerY = PlayerY - 1;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    PlayerY = PlayerY + 1;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    PlayerX = PlayerX - 1;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    prevGameTime = gameTime.TotalGameTime.TotalMilliseconds;
                    PlayerX = PlayerX + 1;
                }

                //prevent from going out of range
                if (PlayerX < 0) PlayerX = 0;
                if (PlayerY < 0) PlayerY = 0;
                if (PlayerX >= WORLD_WIDTH) PlayerX = WORLD_WIDTH - 1;
                if (PlayerY >= WORLD_HEIGHT) PlayerY = WORLD_HEIGHT - 1;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            _worldComponent.DrawWorldViewPort(spriteBatch, PlayerX, PlayerY, TILE_WIDTH, TILE_HEIGHT, new Rectangle(100, 0, VIEW_WIDTH, VIEW_HEIGHT));

            //DRAW DEBUGGING INFORMATION
            spriteBatch.DrawString(_defaultSpriteFont, PlayerX + "," + PlayerY, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
