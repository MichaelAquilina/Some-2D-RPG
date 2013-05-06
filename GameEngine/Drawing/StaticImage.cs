using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing
{
   public class StaticImage : IGameDrawable
   {
        public Texture2D SourceTexture { get; set; }
        public Rectangle SourceRectangle { get; set; }

        public Vector2 Origin { get; set; }

        public StaticImage(Texture2D sourceTexture, Rectangle sourceRectangle)
        {
            this.SourceTexture = sourceTexture;
            this.SourceRectangle = sourceRectangle;
            this.Origin = new Vector2(0, 1);
        }

        public Texture2D GetSourceTexture(double elapsedMS)
        {
            return SourceTexture;
        }

        public Rectangle GetSourceRectangle(double elapsedMS)
        {
            return SourceRectangle;
        }

        public bool IsFinished(double elaspedMS)
        {
            return true;
        }

        public override string ToString()
        {
            return string.Format("StaticImage: SourceTexture={0}, DrawOrigin={1}",
                SourceTexture.Name, Origin);
        }
    }
}
