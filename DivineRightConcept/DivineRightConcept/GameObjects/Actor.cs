using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace DivineRightConcept.GameObjects
{
    //An Actor class that should ideally be inherited for more precise functionality
    public class Actor
    {
        public float X { get; set; }
        public float Y { get; set; }

        //the current representation of the Actor which should be appriopiatly updated depending on its state
        public Texture2D Representation { get; set; }

        public Actor(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public virtual void Update(GameTime gameTime)
        {
            //by default do nothing
            //classes that inhert should do something over here!

            //SOME NOTES
            //If each actor is going to handle its own updating, this might restrict how much its going to be capable of performing
            //certain actions, since it doesnt have much data avaialable to it. It might be smarter applying update logic from an
            //external class.
            
            //This also allows us to perform some neat stuff such as allowing the user to move his character, and then "Possesing"
            //another actor and moving that one instead! This is simply because we can change the CurrentPlayer reference and then
            //everything else is transparent

            //TODO: Perform some code refactoring to allow such changes
        }

        public virtual void LoadContent(ContentManager Content)
        {
            //Default Actor Representation shall be the infamous StickMan!
            this.Representation = Content.Load<Texture2D>("StickManTexture");
        }
    }
}
