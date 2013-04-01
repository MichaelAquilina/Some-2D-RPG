using System;
using System.Collections.Generic;
using System.Xml;
using GameEngine.Extensions;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace GameEngine.Drawing
{
    //TODO: Refine Definition of NAME and GROUP
    /// <summary>
    /// A DrawableSet is a collection of IGameDrawable instances (each called a GameDrawableInstance), organised in a first-tier
    /// group of a give NAME. Whenever a NAME is given to a DrawableSet, it will return a list of all the GameDrawableInstances
    /// that were stored under that NAME. This provides a very quick and easy way of being able to retrieve items which are related
    /// at one go so that they may be drawn together on the screen. Optionally, a second tier grouping mechanism is given by exposing
    /// a GROUP variable in each GameDrawableInstance. This second tier grouping mechanism allows for items related to each other, but
    /// not necisseraly stored under the same name, to have their values set at one go in a batch operation.
    /// 
    /// GameDrawableInstances are important becuase they allow draw options to be set per Entity Drawable, without effecting the draw
    /// state of other Entities using the same drawable item. For this reason, properties such as Visibility, Layer, Rotation, Color and 
    /// SpriteEffects to use are all stored in a GameDrawableInstance rather than an IGameDrawable item.
    /// 
    /// A very brief diagram showing the overview of this structure can be seen below:
    /// 
    /// GameDrawableSet -----> [ NAME0 ] ------> {GameDrawableInstance1, GameDrawableInstance2, ..., GameDrawableInstanceN}
    ///                 -----> [ NAME1 ] ------> ...
    ///                 -----> ...
    ///                 -----> [ NAMEN ] ------> ...
    /// 
    /// GameDrawableInstance -------> {IGameDrawable, Visible, Rotation, Color, SpriteEffects, Group, Layer }
    /// 
    /// </summary>
    public class DrawableSet
    {
        private Dictionary<string, List<GameDrawableInstance>> _drawables = new Dictionary<string, List<GameDrawableInstance>>();

        public List<GameDrawableInstance> this[string Name]
        {
            get { return _drawables[Name]; }
        }

        /// <summary>
        /// Adds an IGameDrawable to the DrawableSet under the give NAME. An IGameDrawable added to a DrawableSet will be placed in
        /// a GameDrawableInstance which allows the user to set per Instance values for Color, Rotation, Visibility etc...
        /// </summary>
        /// <param name="Name">string NAME to place the IGameDrawable item under.</param>
        /// <param name="Drawable">IGameDrawable item to add to the DrawableSet.</param>
        /// <param name="Group">(optional) Associate a string Group name with the IGameDrawable for batch setting of related items.</param>
        /// <param name="Layer">(optional) Assign a Z layer value to this IGameDrawable in relation to drawables in the same set.</param>
        public void Add(string Name, IGameDrawable Drawable, string Group=null, int Layer=0)
        {
            if (!_drawables.ContainsKey(Name))
                _drawables.Add(Name, new List<GameDrawableInstance>());

            GameDrawableInstance gameDrawableInstance = new GameDrawableInstance(Drawable);
            gameDrawableInstance.Group = Group;
            gameDrawableInstance.Layer = Layer;

            _drawables[Name].Add(gameDrawableInstance);
        }

        public void Clear(string Name)
        {
            _drawables.Remove(Name);
        }

        public void ClearAll()
        {
            _drawables.Clear();
        }

        public void SetGroupProperty(string Group, string Property, object Value)
        {
            PropertyInfo property = typeof(GameDrawableInstance).GetProperty(Property);

            if (property == null) throw new ArgumentException(string.Format("The Property '{0}' does not exist", Property));

            foreach (List<GameDrawableInstance> drawables in _drawables.Values)
                foreach (GameDrawableInstance drawing in drawables)
                    if (drawing.Group == Group) property.SetValue(drawing, Value, null);
        }

        public void SetNameProperty(string Name, string Property, object Value)
        {
            PropertyInfo property = typeof(GameDrawableInstance).GetProperty(Property);

            if (property == null) throw new ArgumentException(string.Format("The Property '{0}' does not exist", Property));

            foreach (GameDrawableInstance drawable in _drawables[Name])
                property.SetValue(drawable, Value, null);
        }

        public override string ToString()
        {
            return string.Format("DrawableSet: NAME groups={0}", _drawables.Keys.Count);
        }
    }
}
