using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using DivineRightConcept.Drawing;

namespace DivineRightConcept.GameObjects
{
    //An Actor class that should ideally be inherited for more precise functionality
    public class Actor
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Dictionary<string, Animation> ActorAnimations { get; set; }

        //the current representation of the Actor which should be appriopiatly updated depending on its state
        public Animation CurrentAnimation { get; private set; }
        public string CurrentAnimationName { get; private set; }

        public Actor(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
            this.ActorAnimations = new Dictionary<string, Animation>();
        }

        /// <summary>
        /// Sets the current animation to the specified animation name. The animation
        /// key name must exist in the ActorAnimations dictionary property for this actor
        /// or a KeyNotFoundException will be thrown. On succesful update, the CurrentAnimation
        /// property will be set to the specified animation.
        /// </summary>
        /// <param name="Name">String Name of the Animation to set the CurrentAnimation to.</param>
        public void SetCurrentAnimation(string Name)
        {
            CurrentAnimation = ActorAnimations[Name];
            CurrentAnimationName = Name;
        }
    }
}
