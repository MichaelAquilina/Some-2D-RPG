using System;
using System.Collections.Generic;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using GameEngine.Shaders;
using GameEngine;

//SHOULD BE MOVED OUTSIDE OF THE GAMEENGINE PROJECT DUE TO 
//DEPENDENCY ON CONTENT PROJECT (COUPLES THE THREE PROJECTS LIKE THIS)
namespace DivineRightConcept.Shaders
{
    public class LightShader : GameShader
    {
        private Effect _lightShader;
        private RenderTarget2D _lightTarget;

        public List<ILightSource> LightSources { get; private set; }

        public Texture2D LightMap { get { return (Texture2D)_lightTarget; } }

        public int LightSourcesOnScreen { get; private set; }

        public Color AmbientLight { get; set; }

        public LightShader(GraphicsDevice GraphicsDevice)
            :base(GraphicsDevice)
        {
            LightSources = new List<ILightSource>();
            AmbientLight = Color.White;
            LightSourcesOnScreen = 0;
        }

        public override void LoadContent(ContentManager Content)
        {
            _lightShader = Content.Load<Effect>(@"Shaders\LightShader");
        }

        public override void UnloadContent()
        {
            if( _lightShader != null ) _lightShader.Dispose();
            if( _lightTarget != null ) _lightTarget.Dispose();

            _lightShader = null;
            _lightTarget = null;
        }

        public override void SetResolution(int Width, int Height)
        {
            if (_lightTarget != null) _lightTarget.Dispose();

            _lightTarget = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
        }

        public override void ApplyShader(SpriteBatch SpriteBatch, ViewPortInfo ViewPortInfo,  GameTime GameTime, RenderTarget2D InputBuffer, RenderTarget2D OutputBuffer )
        {
            //RENDER THE LIGHT MAP TO A DESTINATION TEXTURE
            GraphicsDevice.SetRenderTarget(_lightTarget);
            GraphicsDevice.Clear(AmbientLight);

            LightSourcesOnScreen = 0;

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            {
                foreach (ILightSource lightSource in LightSources)
                {
                    FRectangle RelativeDestRectangle = lightSource.GetRelativeDestRectangle(GameTime);
                    Rectangle LightDestRectangle = new Rectangle(
                        (int)Math.Ceiling((RelativeDestRectangle.X - ViewPortInfo.TopLeftX) * ViewPortInfo.PXTileWidth),
                        (int)Math.Ceiling((RelativeDestRectangle.Y - ViewPortInfo.TopLeftY) * ViewPortInfo.PXTileHeight),
                        (int)Math.Ceiling(RelativeDestRectangle.Width * ViewPortInfo.PXTileWidth),
                        (int)Math.Ceiling(RelativeDestRectangle.Height * ViewPortInfo.PXTileHeight)
                    );

                    if (LightDestRectangle.Intersects(InputBuffer.Bounds))
                    {
                        LightSourcesOnScreen++;

                        SpriteBatch.Draw(
                            lightSource.GetLightSourceTexture(GameTime),
                            LightDestRectangle,
                            lightSource.GetLightSourceRectangle(GameTime),
                            lightSource.GetLightColor(GameTime));
                    }
                }
            }
            SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(OutputBuffer);
            _lightShader.Parameters["LightMap"].SetValue((Texture2D)_lightTarget);

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, _lightShader);
            {
                SpriteBatch.Draw( InputBuffer, InputBuffer.Bounds, Color.White);
            }
            SpriteBatch.End();
        }
    }
}
