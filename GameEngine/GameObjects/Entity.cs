using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using GameEngine.Drawing;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GameObjects
{
    //An Actor class that should ideally be inherited for more precise functionality
    //TODO: Merge MapObject and Entity into one
    //technically they should be representing the same thing. If more functionality is needed, then extend it accordingly
    //TODO: Add the ability to provide custom Update functionality PER entity using overrides
    //TODO: Probably Remove the IGameDrawable Interface. Entity should be the interface for drawing!!!!
    public class Entity
    {
        public float X { get; set; }
        public float Y { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }


        public bool Visible { get; set; }

        public bool BoundingBoxVisible { get; set; }

        public Vector2 Origin { get; set; }

        public AnimationSet Animations { get; set; }
        public string CurrentAnimationSetName { get; set; }

        public Entity(float X, float Y, float Width=1, float Height=1, bool Visible=true)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.Visible = Visible;
            this.Origin = Vector2.Zero;
            this.BoundingBoxVisible = false;

            this.Animations = new AnimationSet();
        }

        public void LoadAnimationXML(string FileName, ContentManager Content, bool Clear = false)
        {
            if (Clear)
            {
                Animations.Clear();
            }

            AnimationSet.LoadAnimationXML(Animations, FileName, Content);
        }
    }
}
