using System;
using System.Collections.Generic;
using System.Reflection;
using GameEngine.Interfaces;

namespace GameEngine.Drawing
{
    /// <summary>
    /// A DrawableSet is a collection of IGameDrawable instances (each called a GameDrawableInstance), organised in a first-tier
    /// group of STATES. Whenever a STATE (e.g. Running, Walking) is given to a DrawableSet, it will return a list of all the GameDrawableInstances
    /// that were stored under that STATE. This provides a very quick and easy way of being able to retrieve items which are related
    /// at one go so that they may be drawn together on the screen. Optionally, a second tier grouping mechanism is given by exposing
    /// a GROUP variable in each GameDrawableInstance. This second tier grouping mechanism allows for items related to each other, but
    /// not necisseraly stored under the same STATE, to have their values set at one go in a batch operation.
    /// 
    /// GameDrawableInstances are important becuase they allow draw options to be set per Entity Drawable, without effecting the draw
    /// state of other Entities using the same drawable item. For this reason, properties such as Visibility, Layer, Rotation, Color and 
    /// SpriteEffects to use are all stored in a GameDrawableInstance rather than an IGameDrawable item.
    /// 
    /// A very brief diagram showing the overview of this structure can be seen below:
    /// 
    /// GameDrawableSet -----> [ STATE0 ] ------> {GameDrawableInstance1, GameDrawableInstance2, ..., GameDrawableInstanceN}
    ///                 -----> [ STATE1 ] ------> ...
    ///                 -----> ...
    ///                 -----> [ STATEN ] ------> ...
    ///                 
    /// Similiarly, the same applies for storing by GROUP. The access times of both these allocations is constant O(1) time due to the
    /// use of seperate dictionaries.
    /// 
    /// GameDrawableInstance -------> {IGameDrawable, Visible, Rotation, Color, SpriteEffects, Layer }
    /// 
    /// </summary>
    public class DrawableSet
    {
        private Dictionary<string, List<GameDrawableInstance>> _stateDictionary = new Dictionary<string, List<GameDrawableInstance>>();
        private Dictionary<string, List<GameDrawableInstance>> _groupDictionary = new Dictionary<string, List<GameDrawableInstance>>();

        public List<GameDrawableInstance> GetDrawablesByState(string State)
        {
            return _stateDictionary[State];
        }

        public List<GameDrawableInstance> GetDrawablesByGroup(string Group)
        {
            return _groupDictionary[Group];
        }

        public GameDrawableInstance Add(string State, IGameDrawable Drawable, string Group="", int Layer=0)
        {
            if (!_stateDictionary.ContainsKey(State))
                _stateDictionary.Add(State, new List<GameDrawableInstance>());

            if (!_groupDictionary.ContainsKey(Group))
                _groupDictionary.Add(Group, new List<GameDrawableInstance>());

            GameDrawableInstance instance = new GameDrawableInstance(Drawable);
            instance.Layer = Layer;

            instance._associatedGroup = Group;
            instance._associatedGroup = State;

            _stateDictionary[State].Add(instance);
            _groupDictionary[Group].Add(instance);

            return instance;
        }

        public bool Remove(GameDrawableInstance GameDrawableInstance)
        {
            bool result = true;
            result &= _stateDictionary.Remove(GameDrawableInstance._associatedState);
            result &= _groupDictionary.Remove(GameDrawableInstance._associatedGroup);

            return result;
        }

        public void ClearAll()
        {
            _stateDictionary.Clear();
            _groupDictionary.Clear();
        }

        public void SetGroupProperty(string Group, string Property, object Value)
        {
            PropertyInfo property = typeof(GameDrawableInstance).GetProperty(Property);

            if (property == null) throw new ArgumentException(string.Format("The Property '{0}' does not exist", Property));

            foreach (GameDrawableInstance drawable in _groupDictionary[Group])
                property.SetValue(drawable, Value, null);
        }

        public void SetStateProperty(string State, string Property, object Value)
        {
            PropertyInfo property = typeof(GameDrawableInstance).GetProperty(Property);

            if (property == null) throw new ArgumentException(string.Format("The Property '{0}' does not exist", Property));

            foreach (GameDrawableInstance drawable in _stateDictionary[State])
                property.SetValue(drawable, Value, null);
        }

        public override string ToString()
        {
            return string.Format("DrawableSet: States={0}, Groups={1}", _stateDictionary.Keys.Count, _groupDictionary.Keys.Count);
        }
    }
}
