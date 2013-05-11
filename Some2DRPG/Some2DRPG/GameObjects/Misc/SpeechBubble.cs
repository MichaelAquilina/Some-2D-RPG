using System;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.Drawing.Bitmap;
using GameEngine.Drawing.Text;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Some2DRPG.GameObjects.Misc
{
    public class SpeechBubble : Entity
    {
        public Entity Owner { get; set; }

        public TimeSpan CreateTime { get; internal set; }

        public string Text { get; private set; }

        RegionText _textBox;
        StaticImage _backgroundImage;

        public SpeechBubble()
        {
            Construct(null, "");
        }

        public SpeechBubble(Entity owner, string text)
        {
            Construct(owner, text);
        }

        void Construct(Entity owner, string text)
        {
            this.Owner = owner;
            this.Pos = CalculatePosition();
            this.Text = text;

            this.AlwaysOnTop = true;
        }

        public void SetText(string text)
        {
            this.Text = text;
            _textBox.SetText(text, _backgroundImage.ImageWidth - 4, 34);
        }

        public override void PostCreate(GameTime gameTime, TeeEngine engine)
        {
            CreateTime = gameTime.TotalGameTime;
            Drawables.ResetState("standard", gameTime);
        }

        public override void LoadContent(ContentManager content)
        {
            Texture2D speechText = content.Load<Texture2D>("Misc/speech-bubble");
            StaticImage _backgroundImage = new StaticImage(speechText, null);
            _backgroundImage.Origin = _backgroundImage.CalculateOrigin(24, 47);

            RegionText _textBox = new RegionText(
                content.Load<SpriteFont>("Fonts/SpeechFont"), 
                Text,
                _backgroundImage.ImageWidth - 4,
                34
                );
            _textBox.TextDuration = 3000;

            DrawableInstance bubbleInstance = Drawables.Add("standard", _backgroundImage);
            DrawableInstance textInstance = Drawables.Add("standard", _textBox);

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
