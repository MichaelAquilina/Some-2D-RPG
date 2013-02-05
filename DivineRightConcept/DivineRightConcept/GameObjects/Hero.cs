using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace DivineRightConcept.GameObjects
{
    public class Hero : Actor, ILoadable
    {
        public Hero(float X, float Y) :
            base(X, Y, 1.5f, 1.5f)
        {
        }

        public void LoadContent(ContentManager Content)
        {
            this.LoadAnimationXML(@"Animations/Knuckles.anim", Content);

            SetCurrentAnimation("Idle");
        }

        public void UnloadContent()
        {
            throw new NotImplementedException();
        }
    }
}
