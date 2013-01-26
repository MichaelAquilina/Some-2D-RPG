using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Interfaces;

namespace GameEngine.GameObjects
{
    public class MapObject : IGameDrawable
    {
        public float X { get; set; }
        public float Y { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public float Rotation { get; set; }

        public bool Visible { get; set; }

        public bool BoundingBoxVisible { get; set; }

        public Color DrawColor { get; set; }

        public Vector2 Origin { get; set; }

        public Texture2D SourceTexture { get; set; }
        public Rectangle SourceRectangle { get; set; }

        public MapObject(float X, float Y, float Width, float Height, bool Visible=true)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.Visible = Visible;
            this.DrawColor = Color.White;
            this.Origin = new Vector2(0, 0);
            this.BoundingBoxVisible = false;
            this.Rotation = 0;
        }

        public Texture2D GetTexture(GameTime GameTime)
        {
            return SourceTexture;
        }

        public Rectangle GetSourceRectangle(GameTime GameTime)
        {
            return SourceRectangle;
        }
    }
}
