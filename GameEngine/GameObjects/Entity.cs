using GameEngine.Drawing;
using GameEngine.Interfaces;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.GameObjects
{
    //An abstract Entity class that should be inherited by objects which are to be visible within the game world.
    //Any map objects, NPCs or playable characters should inherit from this class in order to be used by the
    //game engine.
    public class Entity : ILoadable
    {
        //X and Y Tile position on the Map
        public float X { get; set; }
        public float Y { get; set; }

        //Relative Width and Height for animations this Entity will render. 1.0f by Default.
        public float Width { get; set; }
        public float Height { get; set; }

        public bool Visible { get; set; }

        public bool BoundingBoxVisible { get; set; }

        //Relative Origin to the Width and height of each animation
        public Vector2 Origin { get; set; }

        public AnimationSet Animations { get; set; }
        public string CurrentAnimation { get; set; }

        public Entity()
        {
            Init();
        }

        public Entity(float X, float Y, float Width=1, float Height=1, bool Visible=true)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.Visible = Visible;

            Init();
        }

        private void Init()
        {
            this.Origin = Vector2.Zero;
            this.BoundingBoxVisible = false;
            this.Animations = new AnimationSet();
        }

        public void LoadAnimationXML(string FileName, ContentManager Content, int Layer=0)
        {
            AnimationSet.LoadAnimationXML(Animations, FileName, Content, Layer);
        }

        public virtual void Update(GameTime GameTime, TiledMap Map)
        {
        }

        public virtual void LoadContent(ContentManager Content)
        {
        }

        public virtual void UnloadContent()
        {
        }

        public override string ToString()
        {
            return string.Format("Entity: Pos=({0},{1}), Width={2}, Height={3}, Visible={4}", 
                X, Y, Width, Height, Visible);
        }
    }
}
