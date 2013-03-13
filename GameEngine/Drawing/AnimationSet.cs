using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing
{
    public class AnimationSet : Dictionary<string, List<Animation>>
    {
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
            foreach (List<Animation> animations in this.Values)
                foreach (Animation anim in animations)
                    if (anim.Group == Group) anim.Visible = Visible;
        }

        private static string GetAttributeValue(XmlNode Node, string Name, string Default = null)
        {
            return Node.Attributes[Name] == null ? Default : Node.Attributes[Name].Value;
        }

        /// <summary>
        /// Loads an AnimationSet object from a specified in an XML formatted .anim file.
        /// The method requires the string path to the xml file containing the animation data, a reference to the
        /// ContentManager, and optionally, a boolean value specifing whether the current animations should be
        /// cleared for the actor before loading the new ones.
        /// </summary>
        /// <param name="Path">String path to the XML formatted .anim file</param>
        /// <param name="Content">Reference to the ContentManager instance being used in the application</param>
        public static AnimationSet LoadAnimationXML(AnimationSet AnimationSet, string Path, ContentManager Content, int Layer=0)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Path);

            foreach (XmlNode animNode in document.SelectNodes("Animations/Animation"))
            {
                int frameDelay = Convert.ToInt32(GetAttributeValue(animNode, "FrameDelay", "90"));
                bool loop = Convert.ToBoolean(GetAttributeValue(animNode, "Loop", "true"));
                string group = GetAttributeValue(animNode, "Group");

                string name = GetAttributeValue(animNode, "Name");
                string spriteSheet = GetAttributeValue(animNode, "SpriteSheet");
                string[] origin = GetAttributeValue(animNode, "Origin", "0.5, 1.0").Split(',');

                XmlNodeList frameNodes = animNode.SelectNodes("Frames/Frame");
                Rectangle[] frames = new Rectangle[frameNodes.Count];

                for (int i = 0; i < frameNodes.Count; i++)
                {
                    string[] tokens = frameNodes[i].InnerText.Split(',');
                    if (tokens.Length != 4)
                        throw new FormatException("Expected 4 Values for Frame Definition: X, Y, Width, Height");

                    int X = Convert.ToInt32(tokens[0]);
                    int Y = Convert.ToInt32(tokens[1]);
                    int width = Convert.ToInt32(tokens[2]);
                    int height = Convert.ToInt32(tokens[3]);

                    frames[i] = new Rectangle(X, Y, width, height);
                }

                if (!AnimationSet.ContainsKey(name))
                    AnimationSet.Add(name, new List<Animation>());

                Animation animation = new Animation(Content.Load<Texture2D>(spriteSheet), frames, frameDelay, loop, true, Layer);
                animation.Group = group;
                animation.Origin = new Vector2((float)Convert.ToDouble(origin[0]), (float)Convert.ToDouble(origin[1]));
                AnimationSet[name].Add(animation);
            }

            return AnimationSet;
        }
    }
}
