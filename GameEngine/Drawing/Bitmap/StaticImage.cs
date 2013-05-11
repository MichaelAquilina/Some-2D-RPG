using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing.Bitmap
{
   public class StaticImage : BitmapDrawable
   {
        public Texture2D SourceTexture { get; set; }
        public Rectangle? SourceRectangle { get; set; }

        public int ImageWidth { get { return SourceTexture.Width; } }
        public int ImageHeight { get { return SourceTexture.Height; } } 

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

        public Vector2 CalculateOrigin(int x, int y)
        {
            return new Vector2(((float)x) / SourceTexture.Width, ((float)y) / SourceTexture.Height);
        }

        public override string ToString()
        {
            return string.Format("StaticImage: SourceTexture={0}, DrawOrigin={1}",
                SourceTexture.Name, Origin);
        }
    }
}
