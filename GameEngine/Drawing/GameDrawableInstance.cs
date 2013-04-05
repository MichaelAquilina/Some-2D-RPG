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

        //associated state and group when added to a DrawableSet
        //a GameDrawableInstance should be associated with one and only one DrawableSet
        internal string _associatedState = null;
        internal string _associatedGroup = null;

        public GameDrawableInstance(IGameDrawable Drawable)
        {
            this.Drawable = Drawable;
            this.Visible = true;
            this.Rotation = 0;
            this.Color = Color.White;
            this.SpriteEffects = SpriteEffects.None;
            this.Layer = 0;
        }

        public override string ToString()
        {
            return string.Format("GameDrawableInstance: Visible={0}, Layer={1}, Color={2}, Rotation={3}",
                Visible, Layer, Color, Rotation);
        }
    }
}
