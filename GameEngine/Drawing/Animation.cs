using System;
using System.Xml;
using GameEngine.Extensions;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameEngine.Drawing
{
    /// <summary>
    /// Animation class that allows the user to specify metrics about animation frames from a spritesheet. Also allows
    /// specification of other meta properties such as the Delay between frames, whether the animation should loop
    /// and what methods to provide information about current frame information based on the Game Time.
    /// </summary>
    public class Animation : IGameDrawable
    {
        internal const int FRAME_DELAY_DEFAULT = 100;

        public Texture2D SpriteSheet { get; set; }
        public Rectangle[] Frames { get; set; }
        public int FrameDelay { get; set; }
        public bool Loop { get; set; }
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Initialises an Animation object specifies a SpriteSheet to us and the individual frame locations
        /// within the sheet the use. Optionally, the Delay between Frame changes and whether the animation
        /// should loop when complete can be passed as constructor parameters.
        /// </summary>
        /// <param name="spriteSheet">Texture2D object that represents the SpriteSheet to use for this animation.</param>
        /// <param name="frames">Array of Rectangle objects that specify the locations in the spritesheet to use as frames.</param>
        /// <param name="FrameChange">integer value specifying the amount of time in ms to delay between each frame change. Set to 100 by Default.</param>
        /// <param name="loop">bool value specifying wheter the animation should re-start at the end of the animation frames. Defaults to false.</param>
        /// <param name="Visible">bool value specifying whether the animation is visible on the screen. Defaults to false.</param>
        public Animation(Texture2D spriteSheet, Rectangle[] frames, int frameDelay = FRAME_DELAY_DEFAULT, bool loop=false)
        {
            this.SpriteSheet = spriteSheet;
            this.Frames = frames;
            this.FrameDelay = frameDelay;
            this.Loop = loop;
            this.Origin = Vector2.Zero;
        }

        public Texture2D GetSourceTexture(double elapsedMS)
        {
            return SpriteSheet;
        }

        public Rectangle GetSourceRectangle(double elapsedMS)
        {
            return GetFrame(elapsedMS);
        }

        /// <summary>
        /// Returns the current Frame Index as an integer value based on the GameTime
        /// parameters passed into this method.
        /// </summary>
        /// <param name="GameTime">GameTime object representing the current GameTime in the application.</param>
        /// <returns>Int index of the current frame in the Frames property.</returns>
        public int GetFrameIndex(double elapsedMS)
        {
            int index = (int) Math.Abs((elapsedMS) / FrameDelay);

            return (Loop)? index % Frames.Length : index;               //If looping, start from the beginning
        }

        /// <summary>
        /// Specifies whether the Animation has completed. If the Animation is of Looping type, then this
        /// method will always return a true. For non-looping animations, this method should return a true
        /// once it has passed its last frame. The GameTime parameter is required to determine its current
        /// position based on the current GameTime.
        /// </summary>
        /// <param name="GameTime">GameTime object specifying the current Game Time.</param>
        /// <returns>bool value specifying whether the animation has finished.</returns>
        public bool IsFinished(double elapsedMS)
        {
            return Loop || GetFrameIndex(elapsedMS) >= Frames.Length;
        }

        /// <summary>
        /// Returns the Current Frame to show in the sprite sheet based on the current
        /// games running time.
        /// </summary>
        /// <param name="GameTime">GameTime object representing the current state in time of the game.</param>
        /// <returns>Rectangle object representing the Frame in the spritesheet to show.</returns>
        public Rectangle GetFrame(double elapsedMS)
        {
            return Frames[Math.Min(GetFrameIndex(elapsedMS), Frames.Length-1)];
        }

        /// <summary>
        /// Loads Animations into a specified DrawableSet object from a specified in an XML formatted .anim file.
        /// The method requires the string path to the xml file containing the animation data and a reference to the
        /// ContentManager. An optional Layer value can be specified for the ordering of the animations in the 
        /// DrawableSet.
        /// </summary>
        /// <param name="drawableSet">DrawableSet object to load the animations into.</param>
        /// <param name="path">String path to the XML formatted .anim file</param>
        /// <param name="content">Reference to the ContentManager instance being used in the application</param>
        /// <param name="layer">(optional) integer layer value for y ordering on the same DrawableSet.</param>
        public static void LoadAnimationXML(DrawableSet drawableSet, string path, ContentManager content, int layer = 0, double startTimeMS=0)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);

            foreach (XmlNode animNode in document.SelectNodes("Animations/Animation"))
            {
                int frameDelay = XmlExtensions.GetAttributeValue<int>(animNode, "FrameDelay", 90);
                bool loop = XmlExtensions.GetAttributeValue<bool>(animNode, "Loop", true);

                string name = XmlExtensions.GetAttributeValue(animNode, "Name");
                string group = XmlExtensions.GetAttributeValue(animNode, "Group", "");
                string spriteSheet = XmlExtensions.GetAttributeValue(animNode, "SpriteSheet");
                string[] offset = XmlExtensions.GetAttributeValue(animNode, "Offset", "0, 0").Split(',');
                string[] origin = XmlExtensions.GetAttributeValue(animNode, "Origin", "0.5, 1.0").Split(',');

                XmlNodeList frameNodes = animNode.SelectNodes("Frames/Frame");
                Rectangle[] frames = new Rectangle[frameNodes.Count];

                for (int i = 0; i < frameNodes.Count; i++)
                {
                    string[] tokens = frameNodes[i].InnerText.Split(',');
                    if (tokens.Length != 4)
                        throw new FormatException("Expected 4 Values for Frame Definition: X, Y, Width, Height");

                    int X = Convert.ToInt32(tokens[0]);
                    int Y = Convert.ToInt32(tokens[1]);
                    int width = Convert.ToInt32(tokens[2]);
                    int height = Convert.ToInt32(tokens[3]);

                    frames[i] = new Rectangle(X, Y, width, height);
                }

                Animation animation = new Animation(content.Load<Texture2D>(spriteSheet), frames, frameDelay, loop);
                animation.Origin = new Vector2((float)Convert.ToDouble(origin[0]), (float)Convert.ToDouble(origin[1]));
                
                GameDrawableInstance instance = drawableSet.Add(name, animation, group, layer);
                instance.StartTimeMS = startTimeMS;
                instance.Offset = new Vector2((float)Convert.ToDouble(offset[0]), (float)Convert.ToDouble(offset[1]));
            }
        }

        public override string ToString()
        {
            return string.Format("Animation: SpriteSheet={0}, Loop={1}, FrameDelay={2}",
                SpriteSheet.Name,
                Loop, FrameDelay);
        }
    }
}
