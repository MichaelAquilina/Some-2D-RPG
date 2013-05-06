using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Some2DRPG.GameObjects.Characters;

namespace Some2DRPG.GameObjects.Creatures
{
    public class CombatDummy : NPC
    {
        public const string ANIMATION = @"Animations/Monsters/combat_dummy.anim";
        public Direction Direction { get; set; }

        public CombatDummy()
        {
            Initialize(0, 0);
        }

        private void Initialize(float x, float y)
        {
            this.Direction = Direction.Right;
        }

        public override void LoadContent(ContentManager content)
        {
            // Load the animation
            DrawableSet.LoadDrawableSetXml(Drawables, ANIMATION, content);
            CurrentDrawableState = "Idle_" + Direction;
        }

        public override void Hit(Entity sender, GameTime gameTime)
        {
            if (!CurrentDrawableState.Contains("Spin"))
            {
                CurrentDrawableState = "Spin_" + Direction;
                Drawables.ResetState(CurrentDrawableState, gameTime);
            }
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            if (CurrentDrawableState.Contains("Spin") && Drawables.IsStateFinished(CurrentDrawableState, gameTime))
            {
                Direction = (Direction) ((int)(Direction + 1) % 4);
                CurrentDrawableState = "Idle_" + Direction;
            }
        }
    }
}
