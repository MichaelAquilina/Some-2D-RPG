using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DivineRightConcept.Drawing
{
    /// <summary>
    /// Animation class that allows the user to specify an animation from a texture
    /// </summary>
    public class Animation
    {
        public Texture2D SpriteSheet { get; set; }
        public Rectangle[] Frames { get; set; }
        public int FrameChange { get; set; }

        public Animation(Texture2D SpriteSheet, Rectangle[] Frames, int FrameChange=100)
        {
            this.SpriteSheet = SpriteSheet;
            this.Frames = Frames;
            this.FrameChange = FrameChange;
        }

        public Rectangle GetCurrentFrame(GameTime GameTime)
        {
            int index = (int)(GameTime.TotalGameTime.TotalMilliseconds / FrameChange);
            return Frames[index % Frames.Length];
        }
    }
}
