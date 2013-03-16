using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.Helpers;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing
{
    /// <summary>
    /// An animation set represents a number of animations grouped together by an animation name. For example, a character
    /// may have a Move_Left and Move_Right animation. He may also have multiple layers of animations that correspond to the
    /// animation such as a helmet, his body, his torso, his legs etc.... These are collectively an animation NAME. Animations
    /// can also be part of a GROUP. A group name represents a relation between animations. For example, there may be 2 seperate
    /// animations for Move_Left and Move_Right, but there are a group of animations that relate to the 'Head' of the character.
    /// </summary>
    public class DrawableSet
    {
        private Dictionary<string, List<IGameDrawable>> _drawablesDictionary = new Dictionary<string, List<IGameDrawable>>();

        public List<IGameDrawable> this[string Name]
        {
            get { return _drawablesDictionary[Name]; }
        }

        /// <summary>
        /// Adds an IGameDrawable to the DrawableSet under the give NAME.
        /// </summary>
        /// <param name="Name">string NAME to place the IGameDrawable item under.</param>
        /// <param name="Drawable">IGameDrawbale item to add to the DrawableSet.</param>
        public void Add(string Name, IGameDrawable Drawable)
        {
            if (!_drawablesDictionary.ContainsKey(Name))
                _drawablesDictionary.Add(Name, new List<IGameDrawable>());

            _drawablesDictionary[Name].Add(Drawable);
        }

        /// <summary>
        /// Sets all animations within the specified group to a specified Visibility state
        /// (true being Visible and false being not Visible). This will override any per
        /// animation settings in the group. All animations with their group set to null will
        /// always be ignored by this method.
        /// </summary>
        /// <param name="Group">Name of the group to apply the setting to.</param>
        /// <param name="Visible">bool value specifying what to set the Visibility of the Animations to.</param>
        public void SetGroupVisibility(string Group, bool Visible)
        {
            foreach (List<IGameDrawable> drawables in _drawablesDictionary.Values)
                foreach (IGameDrawable drawing in drawables)
                    if (drawing.Group == Group) drawing.Visible = Visible;
        }
    }
}
