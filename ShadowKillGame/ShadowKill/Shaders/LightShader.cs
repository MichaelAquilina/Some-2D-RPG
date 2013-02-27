using System;
using System.Collections.Generic;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using GameEngine.Shaders;
using GameEngine;

namespace ShadowKill.Shaders
{
    public enum LightPositionType { Fixed, Relative };

    //Can Probably be extended to allow Custom Shaped Polygons rather than only circles
    public class LightShader : GameShader
    {
        private Effect _colorShader;
        private Effect _lightShader;
        private RenderTarget2D _lightTarget;

        public List<ILightSource> LightSources { get; private set; }

        public Texture2D LightMap { get { return (Texture2D)_lightTarget; } }

        public int LightSourcesOnScreen { get; private set; }

        public int CirclePointAccurracy { get; set; }

        public Color AmbientLight { get; set; }

        public LightShader(GraphicsDevice GraphicsDevice, int CirclePointAccuracy)
            :base(GraphicsDevice)
        {
            this.LightSources = new List<ILightSource>();
            this.AmbientLight = Color.White;
            this.LightSourcesOnScreen = 0;
            this.CirclePointAccurracy = CirclePointAccuracy;
        }

        private VertexPositionColor[] SetUpCircle(float radiusX, float radiusY, Vector3 center, Color color, int points, Vector2? Range)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[points * 3];

            float angle = MathHelper.TwoPi / points;

            for (int i = 0; i < points; i++)
            {
                vertices[i * 3].Position = center;
                vertices[i * 3].Color = color;

                vertices[i * 3 + 1].Position = new Vector3(
                    (float)Math.Sin(angle * i) * radiusX + center.X,
                    (float)Math.Cos(angle * i) * radiusY + center.Y,
                    0.0f
                );
                vertices[i * 3 + 1].Color = Color.Black;

                vertices[i * 3 + 2].Position = new Vector3(
                    (float)Math.Sin(angle * (i + 1)) * radiusX + center.X,
                    (float)Math.Cos(angle * (i + 1)) * radiusY + center.Y,
                    0.0f
                );
                vertices[i * 3 + 2].Color = Color.Black;
            }

            return vertices;
        }

        public override void LoadContent(ContentManager Content)
        {
            _lightShader = Content.Load<Effect>(@"Shaders\LightShader");
            _colorShader = Content.Load<Effect>(@"Shaders\ColorShader");
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

        //TODO: HIGHLY UNOPTIMIZED, DO NOT INITIALISE VERTICES EACH ROUND, USE VERTEX BUFFERS AND INDICES
        public override void ApplyShader(SpriteBatch SpriteBatch, ViewPortInfo ViewPortInfo,  GameTime GameTime, RenderTarget2D InputBuffer, RenderTarget2D OutputBuffer )
        {
            GraphicsDevice.SetRenderTarget(_lightTarget);
            GraphicsDevice.Clear(AmbientLight);
            GraphicsDevice.BlendState = BlendState.Additive;

            LightSourcesOnScreen = 0;
            foreach (ILightSource lightSource in LightSources)
            {
                float x = lightSource.X;
                float y = lightSource.Y;

                if (lightSource.PositionType == LightPositionType.Relative)
                {
                    x -= ViewPortInfo.TopLeftX;
                    y -= ViewPortInfo.TopLeftY;
                    x *= ViewPortInfo.PXTileWidth;
                    y *= ViewPortInfo.PXTileHeight;
                    x /= _lightTarget.Width;
                    y /= _lightTarget.Height;
                    x = -1.0f + x * 2;
                    y = 1.0f - y * 2;
                }

                float radiusX = lightSource.RadiusX * ViewPortInfo.PXTileWidth;
                float radiusY = lightSource.RadiusY * ViewPortInfo.PXTileHeight;

                radiusX /= _lightTarget.Width;
                radiusY /= _lightTarget.Height;

                VertexPositionColor[] vertexCircle = SetUpCircle(
                    radiusX, radiusY,
                    new Vector3(x, y, 0),
                    lightSource.LightColor,
                    CirclePointAccurracy, null
                );

                _colorShader.CurrentTechnique = _colorShader.Techniques["Pretransformed"];

                foreach (EffectPass pass in _colorShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives(
                        PrimitiveType.TriangleList, 
                        vertexCircle, 
                        0, CirclePointAccurracy, 
                        VertexPositionColor.VertexDeclaration
                    );
                }
            }

            GraphicsDevice.SetRenderTarget(OutputBuffer);
            _lightShader.Parameters["LightMap"].SetValue(_lightTarget);

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, _lightShader);
            {
                SpriteBatch.Draw( InputBuffer, InputBuffer.Bounds, Color.White);
            }
            SpriteBatch.End();
        }
    }
}
