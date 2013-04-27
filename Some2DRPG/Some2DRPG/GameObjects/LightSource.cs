using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Some2DRPG.Shaders;

namespace Some2DRPG.GameObjects
{
    public class LightSource : Entity, ISizedEntity
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public float Pulse { get; set; }
        public double PulseStartTime { get; set; }
        public double PulseDuration { get; set; }

        public LightPositionType PositionType { get; set; }

        public Color Color { get; set; }

        public LightSource()
        {
            PulseDuration = 1000.0;
            PulseStartTime = 0;
            Pulse = 0.0f;
            Color = Color.White;
            PositionType = LightPositionType.Relative;
        }

        public override void PostInitialize(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            // Todo: this should technically NOT be here.
            this.Pos += new Vector2(Width/2.0f, Height/2.0f);

            LightShader lightShader = (LightShader)engine.GetPostGameShader("LightShader");
            lightShader.LightSources.Add(this);
        }

        public override void PostDestroy(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            LightShader lightShader = (LightShader)engine.GetPostGameShader("LightShader");
            lightShader.LightSources.Remove(this);
        }
    }
}
