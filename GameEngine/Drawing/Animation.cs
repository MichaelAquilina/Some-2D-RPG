using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Drawing
{
    /// <summary>
    /// Animation class that allows the user to specify an animation from a texture
    /// </summary>
    public class Animation
    {
        public Texture2D SpriteSheet { get; set; }
        public Rectangle[] Frames { get; set; }
        public int FrameDelay { get; set; }
        public bool Loop { get; set; }

        private double _startTime = 0;
        private const int FRAME_DELAY_DEFAULT = 100;

        /// <summary>
        /// Initialises an Animation object specifies a SpriteSheet to us and the individual frame locations
        /// within the sheet the use. Optionally, the Delay between Frame changes and whether the animation
        /// should loop when complete can be passed as constructor parameters.
        /// </summary>
        /// <param name="SpriteSheet">Texture2D object that represents the SpriteSheet to use for this animation.</param>
        /// <param name="Frames">Array of Rectangle objects that specify the locations in the spritesheet to use as frames.</param>
        /// <param name="FrameChange">integer value specifying the amount of time in ms to delay between each frame change. Set to 100 by Default.</param>
        /// <param name="Loop">bool value specifying wheter the animation should re-start at the end of the animation frames.</param>
        public Animation(Texture2D SpriteSheet, Rectangle[] Frames, int FrameDelay = FRAME_DELAY_DEFAULT, bool Loop = false)
        {
            this.SpriteSheet = SpriteSheet;
            this.Frames = Frames;
            this.FrameDelay = FrameDelay;
            this.Loop = Loop;
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
        /// <param name="GameTime"></param>
        /// <returns>integer index of the current frame</returns>
        public int GetCurrentFrameIndex(GameTime GameTime)
        {
            int index = (int)((GameTime.TotalGameTime.TotalMilliseconds - _startTime) / FrameDelay);

            if (Loop)
                return index % Frames.Length;
            else         
                return Math.Min(index, Frames.Length - 1);
        }

        /// <summary>
        /// Returns the Current Frame to show in the sprite sheet based on the current
        /// games running time.
        /// </summary>
        /// <param name="GameTime">GameTime object representing the current state in time of the game.</param>
        /// <returns>Rectangle object representing the Frame in the spritesheet to show.</returns>
        public Rectangle GetCurrentFrame(GameTime GameTime)
        {
            return Frames[GetCurrentFrameIndex(GameTime)];
        }
    }
}
