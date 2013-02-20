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
    public enum Direction { Left, Right, Up, Down };

    //An Actor class that should ideally be inherited for more precise functionality
    public class Actor : IGameDrawable
    {
        public float X { get; set; }
        public float Y { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public float Rotation { get; set; }

        public bool Visible { get; set; }

        public bool BoundingBoxVisible { get; set; }

        public Vector2 Origin { get; set; }

        public Direction Direction { get; set; }

        public Color DrawColor { get; set; }

        public SpriteEffects CurrentSpriteEffect { get; set; }

        public AnimationSet ActorAnimations { get; set; }

        //the current representation of the Actor which should be appriopiatly updated depending on its state
        public Animation CurrentAnimation { get; private set; }
        public string CurrentAnimationName { get; private set; }

        public Actor(float X, float Y, float Width=1, float Height=1, bool Visible=true)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.Visible = Visible;
            this.DrawColor = Color.White;
            this.Origin = Vector2.Zero;
            this.BoundingBoxVisible = false;
            this.Rotation = 0;
            this.CurrentSpriteEffect = SpriteEffects.None;
            this.Direction = Direction.Right;

            this.ActorAnimations = new AnimationSet();
        }

        public virtual Texture2D GetTexture(GameTime GameTime)
        {
            return CurrentAnimation.SpriteSheet;
        }

        public virtual Rectangle GetSourceRectangle(GameTime GameTime)
        {
            return CurrentAnimation.GetCurrentFrame(GameTime);
        }

        /// <summary>
        /// Sets the current animation to the specified animation name. The animation
        /// key name must exist in the ActorAnimations dictionary property for this actor
        /// or a KeyNotFoundException will be thrown. On succesful update, the CurrentAnimation
        /// property will be set to the specified animation.
        /// </summary>
        /// <param name="Name">String Name of the Animation to set the CurrentAnimation to.</param>
        public void SetCurrentAnimation(string Name)
        {
            CurrentAnimation = ActorAnimations[Name];
            CurrentAnimationName = Name;
        }

        public void LoadAnimationXML(string FileName, ContentManager Content, bool Clear = false)
        {
            if (Clear)
            {
                ActorAnimations.Clear();
                CurrentAnimation = null;
            }

            AnimationSet.LoadAnimationXML(ActorAnimations, FileName, Content);
        }
    }
}
