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
        public Vector2 Offset { get; set; }

        public double StartTimeMS { get; set; }

        // Associated state and group when added to a DrawableSet.
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
            this.Offset = Vector2.Zero;
        }

        public double GetElapsedMS(GameTime gameTime)
        {
            return gameTime.TotalGameTime.TotalMilliseconds - StartTimeMS;
        }

        public bool IsFinished(GameTime gameTime)
        {
            return Drawable.IsFinished(GetElapsedMS(gameTime));
        }

        public void Reset(GameTime gameTime)
        {
            StartTimeMS = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public Rectangle GetSourceRectangle(GameTime gameTime)
        {
            return Drawable.GetSourceRectangle(GetElapsedMS(gameTime));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle destRectangle, float layerDepth, Vector2 origin)
        {
            Drawable.Draw(GetElapsedMS(gameTime), spriteBatch, destRectangle, Color, Rotation, origin, SpriteEffects, layerDepth);
        }

        public override string ToString()
        {
            return string.Format("GameDrawableInstance: Visible={0}, Layer={1}, Color={2}, Rotation={3}",
                Visible, Layer, Color, Rotation);
        }
    }
}
