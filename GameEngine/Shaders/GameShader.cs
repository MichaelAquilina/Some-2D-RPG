using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Shaders
{
    public abstract class GameShader : ILoadable
    {
        public GraphicsDevice GraphicsDevice { get; set; }

        public GameShader(GraphicsDevice GraphicsDevice)
        {
            this.GraphicsDevice = GraphicsDevice;
        }

        public virtual void LoadContent(ContentManager Content)
        {
            throw new NotImplementedException();
        }

        public virtual void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public virtual void SetResolution(int Width, int Height)
        {
            throw new NotImplementedException();
        }

        public virtual void ApplyShader(SpriteBatch SpriteBatch, RenderTarget2D InputBuffer, RenderTarget2D OutputBuffer, GameTime GameTime, ViewPortInfo ViewPortInfo)
        {
            throw new NotImplementedException();            
        }
    }
}
