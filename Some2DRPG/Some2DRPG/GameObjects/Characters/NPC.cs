using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Some2DRPG.GameObjects.Misc;

namespace Some2DRPG.GameObjects.Characters
{
    // Could possibly be renamed to AiRpgEntity if is generic enough.
    public class NPC : RPGEntity
    {
        double _attackDelay = 1500;
        double _lastAttack = 0;

        float _attackDistance = 40f;
        float _agroDistance = 200;
        RPGEntity _target;

        public NPC()
        {
            this.AttackPriority = 6;
            this.HP = 200;
            this.Strength = 5;
            this.CollisionGroup = "Shadow";
            this.MovePrefix = "Walk";
            this.IdlePrefix = "Idle";
        }

        #region AI Methods

        public void AggressiveAI(GameTime gameTime, TeeEngine engine)
        {
            _target = GetHighestPriorityTarget(gameTime, engine, _agroDistance);
            
            // If the NPC has been assigned a target.
            if(_target != null && !IsAttacking(gameTime))
            {
                float distance = Vector2.Distance(_target.Pos, Pos);
                if (distance > _agroDistance || _target.HP <=0 )
                    _target = null;
                else
                {
                    if (distance > _attackDistance)
                        Approach(_target.Pos);
                    else
                    {
                        OnAttack(gameTime);

                        Vector2 difference = _target.Pos - Pos;

                        if (Math.Abs(difference.X) > Math.Abs(difference.Y))
                            Direction = (difference.X > 0) ? Direction.Right : Direction.Left;
                        else
                            Direction = (difference.Y > 0) ? Direction.Down : Direction.Up;
                    }
                }
            }
        }

        #endregion

        #region Interaction Methods

        public override bool IsFinishedAttacking(GameTime gameTime)
        {
            return Drawables.IsStateFinished(CurrentDrawableState, gameTime);
        }

        public override bool IsAttacking(GameTime gameTime)
        {
            return CurrentDrawableState.Contains("Slash");
        }

        public override void OnAttack(GameTime gameTime)
        {
            if (!IsAttacking(gameTime) && gameTime.TotalGameTime.TotalMilliseconds - _lastAttack > _attackDelay)
            {
                _lastAttack = gameTime.TotalGameTime.TotalMilliseconds;

                CurrentDrawableState = "Slash_" + Direction;
                Drawables.ResetState(CurrentDrawableState, gameTime);
            }
        }

        public override void OnInteract(Entity sender, GameTime gameTime, TeeEngine engine)
        {
            SpeechBubble speech = new SpeechBubble(this, "Hello there Adventurer! Whats you're name?");

            engine.AddEntity(speech);

            Vector2 distance = this.Pos - sender.Pos;
            if (distance.X > distance.Y)
                Direction = (distance.X > 0) ? Direction.Left : Direction.Right;
            else
                Direction = (distance.Y > 0) ? Direction.Up : Direction.Down;
        }

        #endregion

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (this.HP > 0)
            {
                AggressiveAI(gameTime, engine);

                base.Update(gameTime, engine);
            }
            else
            {
                // DIE.
            }
        }
    }
}
