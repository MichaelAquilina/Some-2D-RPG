using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Interfaces;

namespace GameEngine.Drawing
{
   public class StaticImage : IGameDrawable
   {
        public Texture2D SourceTexture { get; set; }
        public Rectangle SourceRectangle { get; set; }

        public Vector2 Origin { get; set; }

        public StaticImage(Texture2D SourceTexture, Rectangle SourceRectangle)
        {
            this.SourceTexture = SourceTexture;
            this.SourceRectangle = SourceRectangle;
            this.Origin = new Vector2(0, 1);
        }

        public Texture2D GetSourceTexture(double ElapsedMS)
        {
            return SourceTexture;
        }

        public Rectangle GetSourceRectangle(double ElapsedMS)
        {
            return SourceRectangle;
        }

        public override string ToString()
        {
            return string.Format("StaticImage: SourceTexture={0}, DrawOrigin={1}",
                SourceTexture.Name, Origin);
        }
    }
}
