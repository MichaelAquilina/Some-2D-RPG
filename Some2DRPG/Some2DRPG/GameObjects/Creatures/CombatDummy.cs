using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Some2DRPG.GameObjects.Characters;

namespace Some2DRPG.GameObjects.Creatures
{
    public class CombatDummy : Entity
    {
        public const string ANIMATION = @"Animations/Monsters/combat_dummy.anim";
        public Direction Direction { get; set; }

        public CombatDummy()
        {
            Initialize(0, 0);
        }

        public CombatDummy(float x, float y) : base(x, y)
        {
            Initialize(x, y);
        }

        private void Initialize(float x, float y)
        {
            this.Direction = Direction.Right;
        }

        public override void LoadContent(ContentManager content)
        {
            // Load the animation
            DrawableSet.LoadDrawableSetXml(Drawables, ANIMATION, content);

            foreach (GameDrawableInstance instance in Drawables.GetByGroup("Body"))
            {
                if (instance.Drawable is Animation)
                {
                    Animation anim = (Animation)instance.Drawable;
                    //anim.onAnimationFinished += new Animation.AnimationFinishedEventHandler(anim_onAnimationFinished);
                }
            }
            

            CurrentDrawableState = "Idle_" + Direction;

            base.LoadContent(content);
        }

        private void anim_onAnimationFinished(Animation anim)
        {
            switch(Direction)
            {
                case Direction.Down:
                    Direction = Direction.Left;
                    break;
                case Direction.Left:
                    Direction = Direction.Up;
                    break;
                case Direction.Up:
                    Direction = Direction.Right;
                    break;
                case Direction.Right:
                    Direction = Direction.Down;
                    break;
            }

            CurrentDrawableState = "Idle_" + Direction;
            /*

            List<GameDrawableInstance> animations = Drawables.GetByState(CurrentDrawableState);

            foreach (GameDrawableInstance instance in animations)
            {
                if (instance.Drawable is Animation)
                {
                    Animation animation = (Animation)instance.Drawable;

                    if (animation.IsFinished(gameTime.TotalGameTime.Milliseconds - instance.StartTimeMS))
                    {
                        CurrentDrawableState = "Idle_" + Direction;
                    }
                }
            }
             */
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            if (CurrentDrawableState.Contains("Spin"))
            {
                
            }

            base.Update(gameTime, engine);
        }

        public void Spin(GameTime gameTime)
        {
            if(CurrentDrawableState.Contains("Idle"))
            {
                // Reset the animations
                Drawables.ResetGroup("Body", gameTime);
                CurrentDrawableState = "Spin_" + Direction;
            }
        }
    }
}
