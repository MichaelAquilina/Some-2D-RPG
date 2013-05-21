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
            this.CurrentState = EntityStates.Idle;
            this.AttackPriority = 6;
            this.HP = 200;
            this.Strength = 5;
            this.CollisionGroup = "Shadow";
            this.Interacted += new RPGEntityEventHandler(NPC_Interacted);
        }

        private void NPC_Interacted(RPGEntity sender, Entity invoker, GameTime gameTime, TeeEngine engine)
        {
            SpeechBubble speech = new SpeechBubble(this, "Hello there Adventurer! Whats you're name?");

            engine.AddEntity(speech);

            Vector2 distance = this.Pos - invoker.Pos;
            if (distance.X > distance.Y)
                Direction = (distance.X > 0) ? Direction.Left : Direction.Right;
            else
                Direction = (distance.Y > 0) ? Direction.Up : Direction.Down;
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            _target = GetHighestPriorityTarget(gameTime, engine, _agroDistance);

            if (CurrentState == EntityStates.Idle)
            {
                if (_target != null) CurrentState = EntityStates.Alert;
            }
            else if (CurrentState == EntityStates.Alert)
            {
                if (_target == null) CurrentState = EntityStates.Idle;
                else
                {
                    if (Vector2.Distance(this.Pos, _target.Pos) < _attackDistance)
                    {
                        // Only Attack during specific delayed intervals.
                        if (gameTime.TotalGameTime.TotalMilliseconds - _lastAttack > _attackDelay)
                        {
                            _lastAttack = gameTime.TotalGameTime.TotalMilliseconds;

                            CurrentDrawableState = "Slash_" + Direction;
                            CurrentState = EntityStates.Attacking;

                            ClearHitList();
                            Drawables.ResetState(CurrentDrawableState, gameTime);
                        }
                    }
                    else Approach(_target.Pos);
                }
            }
            else if (CurrentState == EntityStates.Attacking)
            {
                PerformHitCheck(gameTime, engine);
                if (Drawables.IsStateFinished(CurrentDrawableState, gameTime))
                    CurrentState = EntityStates.Alert;
            }

            // Update Idle or Walking animations.
            if (CurrentState == EntityStates.Alert || CurrentState == EntityStates.Idle)
            {
                if (prevPos != Pos)
                {
                    Vector2 difference = Pos - prevPos;
                    if (Math.Abs(difference.X) > Math.Abs(difference.Y))
                        Direction = (difference.X > 0) ? Direction.Right : Direction.Left;
                    else
                        Direction = (difference.Y > 0) ? Direction.Down : Direction.Up;

                    CurrentDrawableState = "Walk_" + Direction;
                }
                else CurrentDrawableState = "Idle_" + Direction;
            }

            base.Update(gameTime, engine);
        }
    }
}
