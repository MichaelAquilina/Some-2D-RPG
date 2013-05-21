using System;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Some2DRPG.GameObjects.Characters;
using GameEngine;

namespace Some2DRPG.GameObjects.Creatures
{
    public class CombatDummy : RPGEntity
    {
        Random _randomGenerator;
        SoundEffect[] _hitSounds;

        public CombatDummy()
        {
            Construct();
        }

        void Construct()
        {
            this.HP = 2000;
            this.BaseRace = RPGEntity.CREATURES_DUMMY;
            this.Direction = Direction.Right;
            this.CollisionGroup = "collision";
            this.Immovable = true;
            this.AttackPriority = 1;
            this._randomGenerator = new Random();
            this.Hit += new RPGEntityEventHandler(CombatDummy_Hit);
        }

        public override void LoadContent(ContentManager content)
        {
            _hitSounds = new SoundEffect[3];
            _hitSounds[0] = content.Load<SoundEffect>("Sounds/Hit/Hit_11");
            _hitSounds[1] = content.Load<SoundEffect>("Sounds/Hit/Hit_14");
            _hitSounds[2] = content.Load<SoundEffect>("Sounds/Hit/Hit_6");

            CurrentDrawableState = "Idle_" + Direction;

            base.LoadContent(content);
        }


        private void CombatDummy_Hit(RPGEntity sender, Entity invoker, GameTime gameTime, TeeEngine engine)
        {
            if (!CurrentDrawableState.Contains("Spin"))
            {
                CurrentDrawableState = "Spin_" + Direction;
                Drawables.ResetState(CurrentDrawableState, gameTime);
            }

            // Play random Hit Sound.
            int index = _randomGenerator.Next(3);
            _hitSounds[index].Play();

            this.HP = 2000;
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
