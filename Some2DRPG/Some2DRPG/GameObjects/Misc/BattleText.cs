using GameEngine.Drawing.Text;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Some2DRPG.GameObjects.Misc
{
    public class BattleText : Entity
    {
        public string Text { get; private set;}

        public Color TextColor { get; private set; }

        Vector2 _offset = Vector2.Zero;

        public BattleText(string text, Color color)
        {
            Construct(text, color);
        }

        void Construct(string text, Color color)
        {
            this.Text = text;
            this.TextColor = color;
        }

        public override void LoadContent(ContentManager content)
        {
            PlainText plainText = new PlainText(content.Load<SpriteFont>("Fonts/DefaultSpriteFont"), Text);
            plainText.Origin = new Vector2(0.5f, 1.0f);

            Drawables.Add("standard", plainText).Color = TextColor;
            CurrentDrawableState = "standard";
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            this.Opacity -= 0.02f;
            this.Drawables.SetStateProperty("standard", "Offset", this._offset);
            this._offset.Y -= 1;

            if (this.Opacity < 0)
                engine.RemoveEntity(this);

            base.Update(gameTime, engine);
        }
    }
}
