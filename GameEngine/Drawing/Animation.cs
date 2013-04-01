using System;
using System.Xml;
using GameEngine.Extensions;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        public Vector2 rxDrawOrigin { get; set; }

        private double _startTime = 0;

        /// <summary>
        /// Initialises an Animation object specifies a SpriteSheet to us and the individual frame locations
        /// within the sheet the use. Optionally, the Delay between Frame changes and whether the animation
        /// should loop when complete can be passed as constructor parameters.
        /// </summary>
        /// <param name="SpriteSheet">Texture2D object that represents the SpriteSheet to use for this animation.</param>
        /// <param name="Frames">Array of Rectangle objects that specify the locations in the spritesheet to use as frames.</param>
        /// <param name="FrameChange">integer value specifying the amount of time in ms to delay between each frame change. Set to 100 by Default.</param>
        /// <param name="Loop">bool value specifying wheter the animation should re-start at the end of the animation frames. Defaults to false.</param>
        /// <param name="Visible">bool value specifying whether the animation is visible on the screen. Defaults to false.</param>
        public Animation(Texture2D SpriteSheet, Rectangle[] Frames, int FrameDelay = FRAME_DELAY_DEFAULT, bool Loop=false)
        {
            this.SpriteSheet = SpriteSheet;
            this.Frames = Frames;
            this.FrameDelay = FrameDelay;
            this.Loop = Loop;
            this.rxDrawOrigin = Vector2.Zero;
        }

        public Texture2D GetSourceTexture(GameTime GameTime)
        {
            return SpriteSheet;
        }

        public Rectangle GetSourceRectangle(GameTime GameTime)
        {
            return GetCurrentFrame(GameTime);
        }

        /// <summary>
        /// Resets the Animation to the first frame. Requires the GameTime as an input
        /// parameters so that the animation may know at point in time the game is at.
        /// This shouldnt be a problem since most of the logic involved with animations
        /// will occur in Draw and Update methods - both of which are passed GameTime
        /// parameters.
        /// </summary>
        /// <param name="GameTime">Current GameTime that the game is at.</param>
        public void ResetAnimation(GameTime GameTime)
        {
            _startTime = GameTime.TotalGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Returns the current Frame Index as an integer value based on the GameTime
        /// parameters passed into this method.
        /// </summary>
        /// <param name="GameTime">GameTime object representing the current GameTime in the application.</param>
        /// <returns>Int index of the current frame in the Frames property.</returns>
        public int GetCurrentFrameIndex(GameTime GameTime)
        {
            int index = (int)((GameTime.TotalGameTime.TotalMilliseconds - _startTime) / FrameDelay);

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
        public bool IsFinished(GameTime GameTime)
        {
            return Loop || GetCurrentFrameIndex(GameTime) >= Frames.Length;
        }

        /// <summary>
        /// Returns the Current Frame to show in the sprite sheet based on the current
        /// games running time.
        /// </summary>
        /// <param name="GameTime">GameTime object representing the current state in time of the game.</param>
        /// <returns>Rectangle object representing the Frame in the spritesheet to show.</returns>
        public Rectangle GetCurrentFrame(GameTime GameTime)
        {
            return Frames[Math.Min(GetCurrentFrameIndex(GameTime), Frames.Length-1)];
        }

        /// <summary>
        /// Loads Animations into a specified DrawableSet object from a specified in an XML formatted .anim file.
        /// The method requires the string path to the xml file containing the animation data and a reference to the
        /// ContentManager. An optional Layer value can be specified for the ordering of the animations in the 
        /// DrawableSet.
        /// </summary>
        /// <param name="DrawableSet">DrawableSet object to load the animations into.</param>
        /// <param name="Path">String path to the XML formatted .anim file</param>
        /// <param name="Content">Reference to the ContentManager instance being used in the application</param>
        /// <param name="Layer">(optional) integer layer value for y ordering on the same DrawableSet.</param>
        public static void LoadAnimationXML(DrawableSet DrawableSet, string Path, ContentManager Content, string Group="", int Layer = 0)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Path);

            foreach (XmlNode animNode in document.SelectNodes("Animations/Animation"))
            {
                int frameDelay = animNode.GetAttributeValue<int>("FrameDelay", 90);
                bool loop = animNode.GetAttributeValue<bool>("Loop", true);

                string name = animNode.GetAttributeValue("Name");
                string spriteSheet = animNode.GetAttributeValue("SpriteSheet");
                string[] origin = animNode.GetAttributeValue("Origin", "0.5, 1.0").Split(',');

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

                Animation animation = new Animation(Content.Load<Texture2D>(spriteSheet), frames, frameDelay, loop);
                animation.rxDrawOrigin = new Vector2((float)Convert.ToDouble(origin[0]), (float)Convert.ToDouble(origin[1]));
                DrawableSet.Add(name, animation, Group, Layer);
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
