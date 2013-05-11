using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Drawing
{
    public class RegionText : TextDrawable
    {
        public SpriteFont SpriteFont { get; set; }

        public string Text { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public double TextDuration { get; set; }

        List<string> _processedText;

        public RegionText(SpriteFont spriteFont, string text, int width, int height)
        {
            this.SpriteFont = spriteFont;
            this.Text = text;
            this._processedText = new List<string>();
            this.TextDuration = 2000;

            SetText(text, width, height);
        }

        public override SpriteFont GetSpriteFont(double elapsedMS)
        {
            return SpriteFont;
        }

        public override bool IsFinished(double elapsedMS)
        {
            return GetTextIndex(elapsedMS) >= _processedText.Count;
        }

        public override string GetText(double elapsedMS)
        {
            return _processedText[Math.Min(GetTextIndex(elapsedMS), _processedText.Count -1)];
        }

        public void SetText(string text, int width, int height)
        {
            this.Text = text;
            this.Width = width;
            this.Height = height;

            ProcessText();
        }

        private int GetTextIndex(double elapsedMS)
        {
            return (int)(elapsedMS / TextDuration);
        }

        private void ProcessText()
        {
            StringBuilder builder = new StringBuilder();

            string[] words = Text.Split(' ');

            float currTextWidth = 0;
            float currTextHeight = 0;

            float lineSpaceHeight = SpriteFont.MeasureString("\r\n").Y;
            float whiteSpaceWidth = SpriteFont.MeasureString(" ").X;

            for (int i = 0; i < words.Length; i++)
            {
                Vector2 wordMetrics = SpriteFont.MeasureString(words[i]);

                if (currTextWidth + wordMetrics.X + whiteSpaceWidth < Width)
                {
                    builder.Append(words[i]);
                    builder.Append(' ');

                    currTextWidth += wordMetrics.X + whiteSpaceWidth;
                }
                else if (currTextHeight + lineSpaceHeight < Height)
                {
                    builder.AppendLine();
                    builder.Append(words[i]);
                    currTextHeight += wordMetrics.Y;
                }
                else
                { 
                    _processedText.Add(builder.ToString());

                    currTextHeight = 0;
                    currTextWidth = 0;
                    builder.Clear();
                    i = i - 1;
                }
            }

            if (builder.Length > 0) _processedText.Add(builder.ToString());
        }
    }
}
