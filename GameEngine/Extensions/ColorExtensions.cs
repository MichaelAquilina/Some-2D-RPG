using System;
using System.Globalization;
using Microsoft.Xna.Framework;

//Code from http://thedeadpixelsociety.com/2012/01/hex-colors-in-xna/
namespace GameEngine.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Creates an ARGB hex string representation of the <see cref="Color"/> value.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> value to parse.</param>
        /// <param name="includeHash">Determines whether to include the hash mark (#) character in the string. Defaults to true if not specified.</param>
        /// <returns>A hex string representation of the specified <see cref="Color"/> value.</returns>
        public static string ToHex(this Color color, bool includeHash=true)
        {
            string[] argb = {
                color.A.ToString("X2"),
                color.R.ToString("X2"),
                color.G.ToString("X2"),
                color.B.ToString("X2"),
            };
            return (includeHash ? "#" : string.Empty) + string.Join(string.Empty, argb);
        }

        /// Creates a <see cref="Color"/> value from an ARGB or RGB hex string.  The string may
        /// begin with or without the hash mark (#) character.
        /// </summary>
        /// <param name="hexString">The ARGB hex string to parse.</param>
        /// <returns>
        /// A <see cref="Color"/> value as defined by the ARGB or RGB hex string.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the string is not a valid ARGB or RGB hex value.</exception>
        public static Color ToColor(this string hexString)
        {
            if (hexString.StartsWith("#"))
                hexString = hexString.Substring(1);
            uint hex = uint.Parse(hexString, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Color color = Color.White;
            if (hexString.Length == 8)
            {
                color.A = (byte)(hex >> 24);
                color.R = (byte)(hex >> 16);
                color.G = (byte)(hex >> 8);
                color.B = (byte)(hex);
            }
            else if (hexString.Length == 6)
            {
                color.R = (byte)(hex >> 16);
                color.G = (byte)(hex >> 8);
                color.B = (byte)(hex);
            }
            else
            {
                throw new InvalidOperationException("Invald hex representation of an ARGB or RGB color value.");
            }
            return color;
        }
    }
}
