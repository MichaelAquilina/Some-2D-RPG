using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Helpers
{
    public static class KeyboardHelper
    {
        private static HashSet<Keys> _lockedKeys = new HashSet<Keys>();

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
