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
        public string Group { get; set; }

        public GameDrawableInstance(IGameDrawable Drawable)
        {
            this.Drawable = Drawable;
            this.Visible = true;
            this.Rotation = 0;
            this.Color = Color.White;
            this.SpriteEffects = SpriteEffects.None;
            this.Layer = 0;
            this.Group = null;
        }

        public override string ToString()
        {
            return string.Format("GameDrawableInstance: Visible={0}, Group={1}, Layer={2}, Color={3}, Rotation={4}",
                Visible, Group, Layer, Color, Rotation);
        }
    }
}
