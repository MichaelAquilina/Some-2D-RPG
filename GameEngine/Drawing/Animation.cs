using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Drawing
{
    /// <summary>
    /// Animation class that allows the user to specify metrics about animation frames from a spritesheet. Also allows
    /// specification of other meta properties such as the Delay between frames, whether the animation should loop
    /// and what methods to provide information about current frame information based on the Game Time.
    /// </summary>
    public class Animation
    {
        internal const int FRAME_DELAY_DEFAULT = 100;

        public Texture2D SpriteSheet { get; set; }
        public Rectangle[] Frames { get; set; }
        public int FrameDelay { get; set; }
        public bool Loop { get; set; }
        public Color Color { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public float Rotation { get; set; }
        public int Layer { get; set; }
        public bool Visible { get; set; }
        public string Group { get; set; }
        public Vector2 Origin { get; set; }

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
        /// <param name="Layer">integer value specifying at which layer should the animation reside on the entity (0 being the lowest layer)/</param>
        public Animation(Texture2D SpriteSheet, Rectangle[] Frames, int FrameDelay = FRAME_DELAY_DEFAULT, bool Loop=false, bool Visible=false, int Layer=0)
        {
            this.SpriteSheet = SpriteSheet;
            this.Frames = Frames;
            this.FrameDelay = FrameDelay;
            this.Loop = Loop;
            this.Color = Color.White;
            this.SpriteEffect = SpriteEffects.None;
            this.Rotation = 0;
            this.Visible = Visible;
            this.Layer = Layer;
            this.Origin = Vector2.Zero;
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
    }
}
