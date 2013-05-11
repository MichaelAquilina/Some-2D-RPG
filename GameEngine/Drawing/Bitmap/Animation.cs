using System;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing.Bitmap
{
    /// <summary>
    /// Animation class that allows the user to specify metrics about animation frames from a spritesheet. Also allows
    /// specification of other meta properties such as the Delay between frames, whether the animation should loop
    /// and what methods to provide information about current frame information based on the Game Time.
    /// </summary>
    public class Animation : BitmapDrawable
    {
        internal const int FRAME_DELAY_DEFAULT = 100;

        public Texture2D SpriteSheet { get; set; }
        public Rectangle[] Frames { get; set; }
        public int FrameDelay { get; set; }
        public bool Loop { get; set; }

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

        // IGameDrawable interface method that returns the SpriteSheet associated with this animation.
        public override Texture2D GetSourceTexture(double elapsedMS)
        {
            return SpriteSheet;
        }

        // IGameDrawable interface method that returns the frame within the SpriteSheet that should currently be displayed.
        public override Rectangle? GetSourceRectangle(double elapsedMS)
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
        public override bool IsFinished(double elapsedMS)
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

        public override string ToString()
        {
            return string.Format("Animation: SpriteSheet={0}, Loop={1}, FrameDelay={2}",
                SpriteSheet.Name,
                Loop, FrameDelay);
        }
    }
}
