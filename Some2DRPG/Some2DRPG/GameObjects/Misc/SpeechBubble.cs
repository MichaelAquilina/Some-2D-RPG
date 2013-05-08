using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Some2DRPG.GameObjects.Misc
{
    public class SpeechBubble : Entity
    {
        public Entity Owner { get; set; }

        public TimeSpan CreateTime { get; internal set; }

        public TimeSpan LifeTime { get; set; }

        public SpeechBubble()
        {
            Construct(null, TimeSpan.FromSeconds(6));
        }

        public SpeechBubble(Entity owner, TimeSpan lifeTime)
        {
            Construct(owner, lifeTime);
        }

        void Construct(Entity owner, TimeSpan lifeTime)
        {
            this.Owner = owner;
            this.Pos = new Vector2(owner.CurrentBoundingBox.Left, owner.CurrentBoundingBox.Top);

            this.LifeTime = lifeTime;
            this.AlwaysOnTop = true;
            this.ScaleX = 0.5f;
            this.ScaleY = 0.5f;
        }

        public override void PostCreate(GameTime gameTime, TeeEngine engine)
        {
            CreateTime = gameTime.TotalGameTime;
        }

        public override void LoadContent(ContentManager content)
        {
            Texture2D speechText = content.Load<Texture2D>("Misc/speech-bubble");
            StaticImage bubbleImage = new StaticImage(speechText, speechText.Bounds);
            Drawables.Add("standard", bubbleImage);

            CurrentDrawableState = "standard";
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (gameTime.TotalGameTime >= CreateTime + LifeTime)
                engine.RemoveEntity(this);

            if(Owner != null)
                this.Pos = new Vector2(Owner.CurrentBoundingBox.Left, Owner.CurrentBoundingBox.Top);
        }
    }
}
