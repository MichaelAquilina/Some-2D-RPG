using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using DivineRightConcept.Drawing;
using System.IO;

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

        /// <summary>
        /// Attempts to load an animation file (*.anim) with a list of animations.
        /// TODO Allow specifaction of Sprite Sheet within the animation file
        /// </summary>
        /// <param name="Path">String Path to the animation file to load.</param>
        /// <param name="SpriteSheet">Texture2D representing the SpriteSheet to use for the animation</param>
        /// <param name="Clear">Boolean parameter specifying whether to clear the current actor animation list. True by default.</param>
        public void LoadAnimationFile(string Path, Texture2D SpriteSheet, bool Clear=true)
        {
            if (Clear) ActorAnimations.Clear();

            TextReader reader = new StreamReader(Path);
            string data =reader.ReadToEnd();
            string currentAnimation = null;
            List<Rectangle> frames = new List<Rectangle>();

            foreach( string raw_line in data.Split('\n'))
            {
                string line;
                line = raw_line.TrimStart();
                line = line.TrimEnd();

                //ignore comments
                if (line.StartsWith("#"))
                    continue;
                else
                //recognize animation names
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        if (currentAnimation != null)
                        {
                            Animation animation = new Animation(SpriteSheet, frames.ToArray());
                            this.ActorAnimations[currentAnimation] = animation;
                            frames.Clear();
                        }

                        currentAnimation = line.Substring(1, line.Length - 2);
                    }
                    else
                        if (currentAnimation != null && line != "")
                        {
                            string[] values = line.Split(',');
                            if (values.Length != 4)
                                throw new FormatException("Expected 4 Values - X,Y,Width,Height");
                            else
                            {
                                int X = Convert.ToInt32(values[0]);
                                int Y = Convert.ToInt32(values[1]);
                                int Width = Convert.ToInt32(values[2]);
                                int Height = Convert.ToInt32(values[3]);

                                frames.Add(new Rectangle(X, Y, Width, Height));
                            }
                        }
            }

            if (currentAnimation != null)
            {
                Animation animation = new Animation(SpriteSheet, frames.ToArray());
                this.ActorAnimations[currentAnimation] = animation;
            }
        }
    }
}
