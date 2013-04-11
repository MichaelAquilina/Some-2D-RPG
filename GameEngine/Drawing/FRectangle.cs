using Microsoft.Xna.Framework;
using System;

namespace GameEngine.Drawing
{
    // TODO: Might be smarter to make use of integers internally. These would have a precision of say 1000 decimals
    // and when retrieved from a property would be divided by 1000 and returned. Internally however, any operations that
    // would occur would be with integers which are faster on the CPU to perform. Have to think about *benefit* vs *complexity*
    // though as this would possibly introduce new bugs.
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

        public float Top { get { return _y; } }
        public float Left { get { return _x; } }
        public float Bottom { get { return _y + _height; } }
        public float Right { get { return _x + _width; } }

        private float _x;
        private float _y;
        private float _width;
        private float _height;

        public FRectangle(Rectangle rectangle)
        {
            _x = rectangle.X;
            _y = rectangle.Y;
            _width = rectangle.Width;
            _height = rectangle.Height;
        }

        public FRectangle(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public bool Intersects(FRectangle frectangle)
        {
            return !(frectangle.Right < this.Left
                      || frectangle.Left > this.Right
                      || frectangle.Bottom < this.Top
                      || frectangle.Top > this.Bottom );
        }

        public bool Contains(FRectangle frectangle)
        {
            return !(frectangle.Right < this.Left || frectangle.Right > this.Right
                     || frectangle.Left < this.Left || frectangle.Left > this.Right
                     || frectangle.Top < this.Top || frectangle.Top > this.Bottom
                     || frectangle.Bottom < this.Top || frectangle.Bottom > this.Bottom);
        }

        public bool Intersects(Rectangle rectangle)
        {
            return Intersects(new FRectangle(rectangle));
        }

        public bool Contains(Rectangle rectangle)
        {
            return Contains(new FRectangle(rectangle));
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(
                (int) Math.Floor(this.X),
                (int) Math.Floor(this.Y),
                (int) (Math.Ceiling(this.Width) + Math.Round(this.X -X)),
                (int) (Math.Ceiling(this.Height) + Math.Round(this.Y -Y))
            );
        }

        public override bool Equals(object obj)
        {
            if (obj is FRectangle)
                return this == (FRectangle)obj;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Width.GetHashCode() + Height.GetHashCode();
        }

        private static string FORMAT_STRING = "X={0}, Y={1}, Width={2}, Height={3}";

        public override string ToString()
        {
            return string.Format(FORMAT_STRING, X, Y, Width, Height);
        }

        public string ToString(string format)
        {
            return string.Format(
                FORMAT_STRING, 
                X.ToString(format), Y.ToString(format), 
                Width.ToString(format), Height.ToString(format));
        }

        #region Static Operators

        public static FRectangle operator +(FRectangle frectangle, Vector2 vector)
        {
            return new FRectangle(
                frectangle.X + vector.X,
                frectangle.Y + vector.Y,
                frectangle.Width,
                frectangle.Height
            );
        }

        public static FRectangle operator -(FRectangle frectangle, Vector2 vector)
        {
            return frectangle + (-1 * vector);
        }

        public static bool operator ==(FRectangle frectangle1, FRectangle frectangle2)
        {
            return
                frectangle1.X == frectangle2.X &&
                frectangle1.Y == frectangle2.Y &&
                frectangle1.Width == frectangle2.Width &&
                frectangle1.Height == frectangle2.Height;
        }

        public static bool operator !=(FRectangle frectangle1, FRectangle frectangle2)
        {
            return !(frectangle1 == frectangle2);
        }

        #endregion
    }
}
