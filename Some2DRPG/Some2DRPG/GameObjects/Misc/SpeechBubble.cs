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

        public SpeechBubble()
        {
            Construct(null);
        }

        public SpeechBubble(Entity owner)
        {
            Construct(owner);
        }

        void Construct(Entity owner)
        {
            this.Owner = owner;
            this.Pos = CalculatePosition();

            this.AlwaysOnTop = true;
        }

        public override void PostCreate(GameTime gameTime, TeeEngine engine)
        {
            CreateTime = gameTime.TotalGameTime;
            Drawables.ResetState("standard", gameTime);
        }

        public override void LoadContent(ContentManager content)
        {
            Texture2D speechText = content.Load<Texture2D>("Misc/speech-bubble");
            StaticImage bubbleImage = new StaticImage(speechText, null);
            bubbleImage.Origin = bubbleImage.CalculateOrigin(24, 47);

            RegionText textBox = new RegionText(
                content.Load<SpriteFont>("Fonts/SpeechFont"), 
                "Hello there Adventurer! Fancy some Ale?",
                speechText.Width - 4,
                34
                );
            textBox.TextDuration = 3000;

            DrawableInstance bubbleInstance = Drawables.Add("standard", bubbleImage);
            DrawableInstance textInstance = Drawables.Add("standard", textBox);

            bubbleInstance.Layer = 0;
            textInstance.Layer = 1;
            textInstance.Color = Color.Black;
            textInstance.Offset = new Vector2(-20, -1 * speechText.Height + 2);

            CurrentDrawableState = "standard";
        }

        public Vector2 CalculatePosition()
        {
            return new Vector2(Owner.CurrentBoundingBox.Left + (Owner.CurrentBoundingBox.Width / 2.0f), Owner.CurrentBoundingBox.Top - 5);
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (Drawables.IsStateFinished("standard", gameTime))
                engine.RemoveEntity(this);

            if(Owner != null)
                this.Pos = CalculatePosition();
        }
    }
}
