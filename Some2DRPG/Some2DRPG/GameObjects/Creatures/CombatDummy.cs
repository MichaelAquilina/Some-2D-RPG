using System;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Some2DRPG.GameObjects.Characters;

namespace Some2DRPG.GameObjects.Creatures
{
    public class CombatDummy : NPC
    {
        Random _randomGenerator;
        SoundEffect[] _hitSounds;

        public const string ANIMATION = @"Animations/Monsters/combat_dummy.anim";

        public CombatDummy()
        {
            Construct();
        }

        void Construct()
        {
            this.Direction = Direction.Right;
            this._randomGenerator = new Random();
        }

        public override void LoadContent(ContentManager content)
        {
            _hitSounds = new SoundEffect[3];
            _hitSounds[0] = content.Load<SoundEffect>("Sounds/Hit/Hit_11");
            _hitSounds[1] = content.Load<SoundEffect>("Sounds/Hit/Hit_14");
            _hitSounds[2] = content.Load<SoundEffect>("Sounds/Hit/Hit_6");

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
            
                // Play random Hit Sound
                int index = _randomGenerator.Next(3);
                _hitSounds[index].Play();
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
