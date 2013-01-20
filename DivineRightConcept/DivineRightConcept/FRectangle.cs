using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightConcept
{
    /// <summary>
    /// Rectangle Structure that uses Float values as storage rather than integers such as the ones
    /// used in the standard XNA rectangle struct. The FRectangle structure can be used to specify
    /// texture ranges in for example the ground pallette.
    /// </summary>
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
