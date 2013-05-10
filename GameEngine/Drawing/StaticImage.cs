using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing
{
   public class StaticImage : BitmapDrawable
   {
        public Texture2D SourceTexture { get; set; }
        public Rectangle? SourceRectangle { get; set; }

        public StaticImage(Texture2D sourceTexture, Rectangle? sourceRectangle)
        {
            this.SourceTexture = sourceTexture;
            this.SourceRectangle = sourceRectangle;
            this.Origin = new Vector2(0, 1);
        }

        public override Texture2D GetSourceTexture(double elapsedMS)
        {
            return SourceTexture;
        }

        public override Rectangle? GetSourceRectangle(double elapsedMS)
        {
            return SourceRectangle;
        }

        public override bool IsFinished(double elaspedMS)
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
