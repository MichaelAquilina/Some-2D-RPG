using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace GameEngine.GameObjects
{
    //An Actor class that should ideally be inherited for more precise functionality
    public class Actor
    {
        public float X { get; set; }
        public float Y { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public Dictionary<string, Animation> ActorAnimations { get; set; }

        //the current representation of the Actor which should be appriopiatly updated depending on its state
        public Animation CurrentAnimation { get; private set; }
        public string CurrentAnimationName { get; private set; }

        public Actor(float X, float Y, float Width=1, float Height=1)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;

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
        /// Loads a Dictionary of Animations for the current actor as specified in an XML formatted .anim file.
        /// The method requires the string path to the xml file containing the animation data, a reference to the
        /// ContentManager, and optionally, a boolean value specifing whether the current animations should be
        /// cleared for the actor before loading the new ones.
        /// </summary>
        /// <param name="Path">String path to the XML formatted .anim file</param>
        /// <param name="Content">Reference to the ContentManager instance being used in the application</param>
        /// <param name="Clear">optional bool value specifying whether to clear the actors current animation dictionary. True by Default</param>
        public void LoadAnimationXML(string Path, ContentManager Content, bool Clear = true)
        {
            if (Clear)
            {
                this.ActorAnimations.Clear();
                CurrentAnimation = null;
            }

            XmlDocument document = new XmlDocument();
            document.Load(Path);

            foreach (XmlNode animNode in document.SelectNodes("Animations/Animation"))
            {
                //optional attributes
                XmlAttribute frameDelayAttr = animNode.Attributes["FrameDelay"];
                XmlAttribute loopAttr = animNode.Attributes["Loop"];

                int FrameDelay = (frameDelayAttr == null)? Animation.FRAME_DELAY_DEFAULT: Convert.ToInt32(frameDelayAttr.Value);
                bool Loop = (loopAttr == null)? false : Convert.ToBoolean(loopAttr.Value);

                string Name = Convert.ToString(animNode.Attributes["Name"].Value);
                string SpriteSheet = Convert.ToString(animNode.Attributes["SpriteSheet"].Value);

                XmlNodeList frameNodes = animNode.SelectNodes("Frames/Frame");
                Rectangle[] frames = new Rectangle[frameNodes.Count];

                for( int i=0; i<frameNodes.Count; i++)
                {
                    string[] tokens = frameNodes[i].InnerText.Split(',');
                    if (tokens.Length != 4)
                        throw new FormatException("Expected 4 Values for Frame Definition: X, Y, Width, Height");

                    int X = Convert.ToInt32(tokens[0]);
                    int Y = Convert.ToInt32(tokens[1]);
                    int Width = Convert.ToInt32(tokens[2]);
                    int Height = Convert.ToInt32(tokens[3]);

                    frames[i] = new Rectangle(X, Y, Width, Height);
                }

                this.ActorAnimations[Name] = new Animation(Content.Load<Texture2D>(SpriteSheet), frames, FrameDelay, Loop);

                //if no current animation has been assigned, use the first animation found in the anim file (Considered Default)
                if (CurrentAnimation == null) CurrentAnimation = this.ActorAnimations[Name];
            }
        }
    }
}
