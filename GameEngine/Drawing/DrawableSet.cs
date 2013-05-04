using System;
using System.Collections.Generic;
using System.Reflection;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using GameEngine.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;

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
        private Dictionary<string, HashSet<GameDrawableInstance>> _stateDictionary = new Dictionary<string, HashSet<GameDrawableInstance>>();
        private Dictionary<string, HashSet<GameDrawableInstance>> _groupDictionary = new Dictionary<string, HashSet<GameDrawableInstance>>();

        public ICollection<string> GetStates()
        {
            return _stateDictionary.Keys;
        }

        public ICollection<string> GetGroups()
        {
            return _groupDictionary.Keys;
        }

        public HashSet<GameDrawableInstance> GetByState(string state)
        {
            return (state==null)? null: _stateDictionary[state];
        }

        public HashSet<GameDrawableInstance> GetByGroup(string group)
        {
            return (group==null)? null: _groupDictionary[group];
        }

        public GameDrawableInstance Add(string state, IGameDrawable drawable, string group="", int layer=0)
        {
            if (!_stateDictionary.ContainsKey(state))
                _stateDictionary.Add(state, new HashSet<GameDrawableInstance>());

            if (!_groupDictionary.ContainsKey(group))
                _groupDictionary.Add(group, new HashSet<GameDrawableInstance>());

            GameDrawableInstance instance = new GameDrawableInstance(drawable);
            instance.Layer = layer;

            instance._associatedGroup = group;
            instance._associatedState = state;

            _stateDictionary[state].Add(instance);
            _groupDictionary[group].Add(instance);

            return instance;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="drawable"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool Remove(IGameDrawable drawable, string state)
        {
            foreach(GameDrawableInstance instance in _stateDictionary[state])
                if (instance.Drawable == drawable)
                    return _stateDictionary[state].Remove(instance);

            return false;
        }

        public bool Remove(GameDrawableInstance gameDrawableInstance)
        {
            bool result = true;
            result &= _stateDictionary[gameDrawableInstance._associatedState].Remove(gameDrawableInstance);
            result &= _groupDictionary[gameDrawableInstance._associatedGroup].Remove(gameDrawableInstance);

            return result;
        }

        /// <summary>
        /// TODO: Improve this description.
        /// Copies all the IGameDrawable's found in the specified drawableSet into new GameDrawableInstances
        /// within the same States as those find in the specified drawableSet. Acts much like a custom set
        /// union operator.
        /// </summary>
        /// <param name="drawableSet"></param>
        public void Copy(DrawableSet drawableSet)
        {
            foreach (string state in drawableSet.GetStates())
            {
                foreach (GameDrawableInstance instance in drawableSet.GetByState(state))
                {
                    GameDrawableInstance copiedInstance = Add(state, instance.Drawable, instance._associatedGroup);
                    copiedInstance.Layer = instance.Layer;
                    copiedInstance.Offset = instance.Offset;
                    copiedInstance.Rotation = instance.Rotation;
                    copiedInstance.Visible = instance.Visible;
                }
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="drawableSet"></param>
        public void Remove(DrawableSet drawableSet)
        {
            foreach (string state in drawableSet.GetStates())
                foreach (GameDrawableInstance instance in drawableSet.GetByState(state))
                    Remove(instance.Drawable, state);
        }

        public void ClearAll()
        {
            _stateDictionary.Clear();
            _groupDictionary.Clear();
        }

        public void ResetState(string state, GameTime gameTime)
        {
            foreach (GameDrawableInstance instance in _stateDictionary[state])
                instance.Reset(gameTime);
        }

        public void ResetGroup(string group, GameTime gameTime)
        {
            foreach (GameDrawableInstance instance in _groupDictionary[group])
                instance.Reset(gameTime);
        }

        public void SetGroupProperty(string group, string property, object value)
        {
            PropertyInfo propertyInfo = typeof(GameDrawableInstance).GetProperty(property);

            if (property == null) throw new ArgumentException(string.Format("The Property '{0}' does not exist", property));

            foreach (GameDrawableInstance drawable in _groupDictionary[group])
                propertyInfo.SetValue(drawable, value, null);
        }

        public void SetStateProperty(string state, string property, object value)
        {
            PropertyInfo propertyInfo = typeof(GameDrawableInstance).GetProperty(property);

            if (property == null) throw new ArgumentException(string.Format("The Property '{0}' does not exist", property));

            foreach (GameDrawableInstance drawable in _stateDictionary[state])
                propertyInfo.SetValue(drawable, value, null);
        }

        public override string ToString()
        {
            return string.Format("DrawableSet: States={0}, Groups={1}", _stateDictionary.Keys.Count, _groupDictionary.Keys.Count);
        }

        /// <summary>
        /// Loads a DrawableSet file into a specified DrawableSet object.
        /// The method requires the string path to the xml file containing the drawable data and a reference to the
        /// ContentManager. An optional Layer value can be specified for the ordering of the drawables in the 
        /// DrawableSet. Currently only supports loading of Animation objects.
        /// </summary>
        /// <param name="drawableSet">DrawableSet object to load the animations into.</param>
        /// <param name="path">String path to the XML formatted .anim file</param>
        /// <param name="content">Reference to the ContentManager instance being used in the application</param>
        public static void LoadDrawableSetXml(DrawableSet drawableSet, string path, ContentManager content, double startTimeMS = 0)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);

            foreach (XmlNode animNode in document.SelectNodes("Animations/Animation"))
            {
                int frameDelay = XmlExtensions.GetAttributeValue<int>(animNode, "FrameDelay", 90);
                bool loop = XmlExtensions.GetAttributeValue<bool>(animNode, "Loop", true);
                int layer = XmlExtensions.GetAttributeValue<int>(animNode, "Layer", 0);

                string state = XmlExtensions.GetAttributeValue(animNode, "State");
                string group = XmlExtensions.GetAttributeValue(animNode, "Group", "");
                string spriteSheet = XmlExtensions.GetAttributeValue(animNode, "SpriteSheet");
                string[] offset = XmlExtensions.GetAttributeValue(animNode, "Offset", "0, 0").Split(',');
                string[] origin = XmlExtensions.GetAttributeValue(animNode, "Origin", "0.5, 1.0").Split(',');

                Vector2 offsetVector = new Vector2((float)Convert.ToDouble(offset[0]), (float)Convert.ToDouble(offset[1]));
                Vector2 originVector = new Vector2((float)Convert.ToDouble(origin[0]), (float)Convert.ToDouble(origin[1]));

                XmlNodeList frameNodes = animNode.SelectNodes("Frames/Frame");
                Rectangle[] frames = new Rectangle[frameNodes.Count];

                for (int i = 0; i < frameNodes.Count; i++)
                {
                    string[] tokens = frameNodes[i].InnerText.Split(',');
                    if (tokens.Length != 4)
                        throw new FormatException("Expected 4 Values for Frame Definition: X, Y, Width, Height");

                    int x = Convert.ToInt32(tokens[0]);
                    int y = Convert.ToInt32(tokens[1]);
                    int width = Convert.ToInt32(tokens[2]);
                    int height = Convert.ToInt32(tokens[3]);

                    frames[i] = new Rectangle(x, y, width, height);
                }

                Animation animation = new Animation(content.Load<Texture2D>(spriteSheet), frames, frameDelay, loop);
                animation.Origin = originVector;

                // TODO: Requires possible revision of code.
                // Allow support for specifying glob patterns in the case of state names.
                if (state.Contains("*"))
                {
                    // Use Glob patterns in favour of regular expressions.
                    state = Regex.Escape(state).Replace(@"\*", ".*").Replace(@"\?", ".");
                    Regex regexMatcher = new Regex(state);

                    foreach (string drawableSetState in drawableSet.GetStates())
                    {
                        if (regexMatcher.IsMatch(drawableSetState))
                        {
                            GameDrawableInstance instance = drawableSet.Add(drawableSetState, animation, group, layer);
                            instance.StartTimeMS = startTimeMS;
                            instance.Offset = offsetVector;
                        }
                    }
                }
                else
                {
                    GameDrawableInstance instance = drawableSet.Add(state, animation, group, layer);
                    instance.StartTimeMS = startTimeMS;
                    instance.Offset = offsetVector;
                }
            }
        }
    }
}
