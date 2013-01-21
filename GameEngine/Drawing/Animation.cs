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
        public int FrameChange { get; set; }

        private double _startTime = 0;

        public Animation(Texture2D SpriteSheet, Rectangle[] Frames, int FrameChange=100)
        {
            this.SpriteSheet = SpriteSheet;
            this.Frames = Frames;
            this.FrameChange = FrameChange;
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
        /// Returns the Current Frame to show in the sprite sheet based on the current
        /// games running time.
        /// </summary>
        /// <param name="GameTime">GameTime object representing the current state in time of the game.</param>
        /// <returns>Rectangle object representing the Frame in the spritesheet to show.</returns>
        public Rectangle GetCurrentFrame(GameTime GameTime)
        {
            int index = (int)((GameTime.TotalGameTime.TotalMilliseconds - _startTime) / FrameChange);
            return Frames[index % Frames.Length];
        }
    }
}
