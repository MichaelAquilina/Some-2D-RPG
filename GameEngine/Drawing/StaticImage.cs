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

        public Color Color { get; set; }
        public float Rotation { get; set; }
        public int Layer { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public bool Visible { get; set; }
        public Vector2 Origin { get; set; }
        public string Group { get; set; }

        public StaticImage(Texture2D SourceTexture, Rectangle SourceRectangle)
        {
            this.SourceTexture = SourceTexture;
            this.SourceRectangle = SourceRectangle;
            this.Color = Color.White;
            this.Rotation = 0;
            this.Layer = 0;
            this.SpriteEffect = SpriteEffects.None;
            this.Visible = true;
            this.Origin = new Vector2(0, 1);
            this.Group = null;
        }

        public Texture2D GetSourceTexture(GameTime GameTime)
        {
            return SourceTexture;
        }

        public Rectangle GetSourceRectangle(GameTime GameTime)
        {
            return SourceRectangle;
        }
    }
}
