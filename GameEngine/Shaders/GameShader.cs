using System;
using GameEngine.Drawing;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Shaders
{
    /// <summary>
    /// Abstract Plugin Shader class that can be applied to a TileEngine. The shader provides an easy way to output a RenderTarget based
    /// on some input RenderTarget that would have been rendered by the Engine previously. This allows sequantial application
    /// of Game Shaders
    /// </summary>
    public abstract class GameShader : ILoadable
    {
        public bool Enabled { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }

        public GameShader(GraphicsDevice GraphicsDevice)
        {
            this.GraphicsDevice = GraphicsDevice;
            this.Enabled = true;
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

        public virtual void ApplyShader(SpriteBatch SpriteBatch, ViewPortInfo ViewPortInfo,  GameTime GameTime, RenderTarget2D InputBuffer, RenderTarget2D OutputBuffer )
        {
            throw new NotImplementedException();            
        }

        public override string ToString()
        {
            return string.Format("GameShader: {0}, Enabled={1}", this.GetType().Name, Enabled);
        }
    }
}
