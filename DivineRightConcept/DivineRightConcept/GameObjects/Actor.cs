using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace DivineRightConcept.GameObjects
{
    //An Actor class that should ideally be inherited for more precise functionality
    public class Actor
    {
        public int X { get; set; }
        public int Y { get; set; }

        //the current representation of the Actor which should be appriopiatly updated depending on its state
        public Texture2D Representation { get; set; }

        public Actor(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public void LoadContent(ContentManager Content)
        {
            //Default Actor Representation shall be the infamous StickMan!
            this.Representation = Content.Load<Texture2D>("StickManTexture");
        }
    }
}
