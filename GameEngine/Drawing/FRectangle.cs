using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Drawing
{
    /// <summary>
    /// Rectangle Structure that uses Float values as storage rather than integers such as the ones
    /// used in the standard XNA rectangle struct. The FRectangle structure can be used to specify
    /// texture ranges in for example the ground pallette.
    /// </summary>
    public struct FRectangle
    {
        public float X { get { return _x; } set { _x = value; } }
        public float Y { get { return _y; } set { _y = value; } }
        public float Width { get { return _width; } set { _width = value; } }
        public float Height { get { return _height; } set { _height = value; } }

        private float _x;
        private float _y;
        private float _width;
        private float _height;

        public FRectangle(float X, float Y, float Width, float Height)
        {
            _x = X;
            _y = Y;
            _width = Width;
            _height = Height;
        }
    }
}
