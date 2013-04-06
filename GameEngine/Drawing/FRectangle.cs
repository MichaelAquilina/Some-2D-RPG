using Microsoft.Xna.Framework;
using System;

namespace GameEngine.Drawing
{
    //TODO: Might be smarter to make use of integers internally. These would have a precision of say 1000 decimals
    //and when retrieved from a property would be divided by 1000 and returned. Internally however, any operations that
    //would occur would be with integers which are faster on the CPU to perform. Have to think about *benefit* vs *complexity*
    //though as this would possibly introduce new bugs.
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

        public FRectangle(Rectangle Rectangle)
        {
            _x = Rectangle.X;
            _y = Rectangle.Y;
            _width = Rectangle.Width;
            _height = Rectangle.Height;
        }

        public FRectangle(float X, float Y, float Width, float Height)
        {
            _x = X;
            _y = Y;
            _width = Width;
            _height = Height;
        }

        public bool Intersects(FRectangle FRectangle)
        {
            return !(FRectangle.Right < this.Left
                      || FRectangle.Left > this.Right
                      || FRectangle.Bottom < this.Top
                      || FRectangle.Top > this.Bottom );
        }

        public bool Contains(FRectangle FRectangle)
        {
            return !(FRectangle.Right < this.Left || FRectangle.Right > this.Right
                     || FRectangle.Left < this.Left || FRectangle.Left > this.Right
                     || FRectangle.Top < this.Top || FRectangle.Top > this.Bottom
                     || FRectangle.Bottom < this.Top || FRectangle.Bottom > this.Bottom);
        }

        public bool Intersects(Rectangle Rectangle)
        {
            return Intersects(new FRectangle(Rectangle));
        }

        public bool Contains(Rectangle Rectangle)
        {
            return Contains(new FRectangle(Rectangle));
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

        public static FRectangle operator +(FRectangle FRectangle, Vector2 Vector)
        {
            return new FRectangle(
                FRectangle.X + Vector.X,
                FRectangle.Y + Vector.Y,
                FRectangle.Width,
                FRectangle.Height
            );
        }

        public static FRectangle operator -(FRectangle FRectangle, Vector2 Vector)
        {
            return FRectangle + (-1 * Vector);
        }

        public static bool operator ==(FRectangle FRect1, FRectangle FRect2)
        {
            return
                FRect1.X == FRect2.X &&
                FRect1.Y == FRect2.Y &&
                FRect1.Width == FRect2.Width &&
                FRect1.Height == FRect2.Height;
        }

        public static bool operator !=(FRectangle FRect1, FRectangle FRect2)
        {
            return !(FRect1 == FRect2);
        }

        #endregion
    }
}
