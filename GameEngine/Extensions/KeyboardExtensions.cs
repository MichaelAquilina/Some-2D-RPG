using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Extensions
{
    public static class KeyboardExtensions
    {
        private static Dictionary<object, HashSet<Keys>> _lockedKeys = new Dictionary<object, HashSet<Keys>>();

        // adds the specified key to the specified lockObject's list of locked keys.
        private static void AddToLockedKeys(object lockObject, Keys value)
        {
            if (!_lockedKeys.ContainsKey(lockObject))
                _lockedKeys.Add(lockObject, new HashSet<Keys>());

            _lockedKeys[lockObject].Add(value);
        }

        // removes the specified key from the specified lockObject's list of locked keys.
        private static bool RemoveFromLockedKeys(object lockObject, Keys value)
        {
            if (!_lockedKeys.ContainsKey(lockObject))
                return false;

            _lockedKeys[lockObject].Remove(value);
            if (_lockedKeys[lockObject].Count == 0) _lockedKeys.Remove(lockObject);

            return true;
        }

        /// <summary>
        /// Returns a boolean value specifying whether the key specified in the parameter is currently down or not. If the key is down
        /// then the method will *lock* the key with the specified lockObject until it has been released and this method is called again.
        /// This method is very useful for performing 'one-time' actions for an Entity or Script that should not be repeated while the
        /// key is kept pressed by the user.
        /// </summary>
        /// <param name="keyboardState">The current KeyboardState retrieved by the calling method.</param>
        /// <param name="key">Key to check and lock if found to be down.</param>
        /// <param name="lockObject">The object with which to associate the lock. All other objects will still retrieve a true in the next method call.</param>
        /// <param name="lockKey">Boolean value specifying if the method should use the locking mechanism or not. True by default.</param>
        /// <returns>bool value specifying if the key in the paramater is down AND has not been locked yet. Returns false otherwise.</returns>
        public static bool GetKeyDownState(KeyboardState keyboardState, Keys key, object lockObject, bool lockKey=true)
        {
            bool result = false;

            if (keyboardState.IsKeyDown(key) && (!lockKey || !_lockedKeys.ContainsKey(lockObject) || !_lockedKeys[lockObject].Contains(key)))
            {
                result = true;
                if (lockKey) AddToLockedKeys(lockObject, key);
            }
            else
                result = false;

            if (!keyboardState.IsKeyDown(key) && lockKey)
                RemoveFromLockedKeys(lockObject, key);

            return result;
        }
    }
}
