using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Extensions
{
    public static class KeyboardExtensions
    {
        private static HashSet<Keys> _lockedKeys = new HashSet<Keys>();

        /// <summary>
        /// TODO: Document this method since it has very specific functionality
        /// </summary>
        /// <param name="KeyboardState"></param>
        /// <param name="Key"></param>
        /// <param name="Lock"></param>
        /// <returns></returns>
        public static bool GetKeyDownState(KeyboardState KeyboardState, Keys Key, bool Lock)
        {
            bool result = false;

            if (KeyboardState.IsKeyDown(Key) && (!Lock || !_lockedKeys.Contains(Key)))
            {
                result = true;
                if (Lock) _lockedKeys.Add(Key);
            }
            else
                result = false;

            if (!KeyboardState.IsKeyDown(Key) && Lock)
                _lockedKeys.Remove(Key);

            return result;
        }
    }
}
