using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing
{
    public class GameDrawableInstance
    {
        public IGameDrawable Drawable { get; set; }
        public bool Visible { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; }
        public SpriteEffects SpriteEffects { get; set; }
        public int Layer { get; set; }

        public double StartTimeMS { get; set; }

        // Associated state and group when added to a DrawableSet
        // a GameDrawableInstance should be associated with one and only one DrawableSet.
        internal string _associatedState = null;
        internal string _associatedGroup = null;

        public GameDrawableInstance(IGameDrawable drawable)
        {
            this.StartTimeMS = 0;
            this.Drawable = drawable;
            this.Visible = true;
            this.Rotation = 0;
            this.Color = Color.White;
            this.SpriteEffects = SpriteEffects.None;
            this.Layer = 0;
        }

        public void Reset(GameTime gameTime)
        {
            StartTimeMS = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public Rectangle GetSourceRectangle(GameTime gameTime)
        {
            return Drawable.GetSourceRectangle(gameTime.TotalGameTime.TotalMilliseconds - StartTimeMS);
        }

        public Texture2D GetSourceTexture(GameTime gameTime)
        {
            return Drawable.GetSourceTexture(gameTime.TotalGameTime.TotalMilliseconds - StartTimeMS);
        }

        public override string ToString()
        {
            return string.Format("GameDrawableInstance: Visible={0}, Layer={1}, Color={2}, Rotation={3}",
                Visible, Layer, Color, Rotation);
        }
    }
}
