using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameEngine.Tiled;
using GameEngine.Drawing;
using GameEngine;

namespace ShadowKill.GameObjects
{
    public class Bat : Entity
    {
        public Bat()
        {
            this.Origin = new Vector2(1.0f, 0.5f);
            this.Visible = true;
            this.rxWidth = 1.5f;
            this.rxHeight = 1.5f;
        }

        public override void Update(GameTime GameTime, TeeEngine Engine)
        {
            TX += (float)Math.Cos(GameTime.TotalGameTime.TotalSeconds) / 20;
        }

        public override void LoadContent(ContentManager Content)
        {
            Animation.LoadAnimationXML(Drawables, "Animations/Monsters/bat.anim", Content, null, 0);
            CurrentDrawable = "Down";
        }
    }
}
