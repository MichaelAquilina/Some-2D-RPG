using System;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing.Text
{
    public class PlainText : TextDrawable
    {
        public string Text { get; set; }

        public SpriteFont SpriteFont { get; set; }

        public PlainText()
        {
        }

        public PlainText(SpriteFont spriteFont, string text)
        {
            Construct(spriteFont, text);
        }

        void Construct(SpriteFont spriteFont, string text)
        {
            this.Text = text;
            this.SpriteFont = spriteFont;
        }

        public override int GetWidth(double elapsedMS)
        {
            return (int)Math.Ceiling(SpriteFont.MeasureString(Text).X);
        }

        public override int GetHeight(double elapsedMS)
        {
            return (int) Math.Ceiling(SpriteFont.MeasureString(Text).Y);
        }

        public override SpriteFont GetSpriteFont(double elapsedMS)
        {
            return SpriteFont;
        }

        public override string GetText(double elapsedMS)
        {
            return Text;
        }

        public override string ToString()
        {
            return string.Format("PlainText: Text={0}, SpriteFont={1}", Text, SpriteFont);
        }
    }
}
