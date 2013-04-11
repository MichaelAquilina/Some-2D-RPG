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
        /// <param name="keyboardState"></param>
        /// <param name="key"></param>
        /// <param name="lockKey"></param>
        /// <returns></returns>
        public static bool GetKeyDownState(KeyboardState keyboardState, Keys key, bool lockKey)
        {
            bool result = false;

            if (keyboardState.IsKeyDown(key) && (!lockKey || !_lockedKeys.Contains(key)))
            {
                result = true;
                if (lockKey) _lockedKeys.Add(key);
            }
            else
                result = false;

            if (!keyboardState.IsKeyDown(key) && lockKey)
                _lockedKeys.Remove(key);

            return result;
        }
    }
}
