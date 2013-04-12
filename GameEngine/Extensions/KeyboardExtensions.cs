using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Extensions
{
    public static class KeyboardExtensions
    {
        private static Dictionary<object, List<Keys>> _lockedKeys = new Dictionary<object, List<Keys>>();

        private static void AddToLockedKeys(object callingObject, Keys value)
        {
            if (!_lockedKeys.ContainsKey(callingObject))
                _lockedKeys.Add(callingObject, new List<Keys>());

            _lockedKeys[callingObject].Add(value);
        }

        private static bool Remove(object callingObject, Keys value)
        {
            if (!_lockedKeys.ContainsKey(callingObject))
                return false;

            _lockedKeys[callingObject].Remove(value);
            if (_lockedKeys[callingObject].Count == 0) _lockedKeys.Remove(callingObject);

            return true;
        }

        /// <summary>
        /// TODO: Document this method since it has very specific functionality
        /// </summary>
        /// <param name="keyboardState"></param>
        /// <param name="key"></param>
        /// <param name="lockKey"></param>
        /// <returns></returns>
        public static bool GetKeyDownState(KeyboardState keyboardState, Keys key, object callingObject, bool lockKey)
        {
            bool result = false;

            if (keyboardState.IsKeyDown(key) && (!lockKey || !_lockedKeys.ContainsKey(callingObject) || !_lockedKeys[callingObject].Contains(key)))
            {
                result = true;
                if (lockKey) AddToLockedKeys(callingObject, key);
            }
            else
                result = false;

            if (!keyboardState.IsKeyDown(key) && lockKey)
                Remove(callingObject, key);

            return result;
        }
    }
}
