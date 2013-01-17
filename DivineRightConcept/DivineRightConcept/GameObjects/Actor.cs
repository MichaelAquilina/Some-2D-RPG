using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace DivineRightConcept.GameObjects
{
    public class Actor
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Texture2D Representation { get; set; }

        public Actor(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public void LoadContent(ContentManager Content)
        {
            this.Representation = Content.Load<Texture2D>("StickManTexture");
        }
    }
}
