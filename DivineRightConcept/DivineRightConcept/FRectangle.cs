using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightConcept
{
    //A Rectangle structure capable of holding float values
    public class FRectangle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public FRectangle(float X, float Y, float Width, float Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }
    }
}
