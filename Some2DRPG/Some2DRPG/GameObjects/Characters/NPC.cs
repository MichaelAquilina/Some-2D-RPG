using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;

namespace Some2DRPG.GameObjects.Characters
{
    // Could possibly be renamed to AiRpgEntity if is generic enough.
    public class NPC : RPGEntity
    {
        double _lastAttack = 0;
        double _attackDelay = 1500;
        float _moveSpeed = 1.2f;
        float _attackDistance = 40f;
        int _agroDistance = 200;
        RPGEntity _target;

        List<Entity> _hitEntityList = new List<Entity>();

        public NPC()
        {
            this.Strength = 5;
        }

        public bool IsAttacking(GameTime gameTime)
        {
            return CurrentDrawableState.Contains("Slash");
        }

        public void OnAttack(GameTime gameTime)
        {
            if(!IsAttacking(gameTime) && gameTime.TotalGameTime.TotalMilliseconds - _lastAttack > _attackDelay)
            {
                _lastAttack = gameTime.TotalGameTime.TotalMilliseconds;

                CurrentDrawableState = "Slash_" + Direction;
                Drawables.ResetState(CurrentDrawableState, gameTime);
            }
        }

        public void Approach(Vector2 target)
        {
            Vector2 difference = target - this.Pos;
            difference.Normalize();

            this.Pos.X += _moveSpeed * difference.X;
            this.Pos.Y += _moveSpeed * difference.Y;
        }

        public void AggressiveAI(GameTime gameTime, TeeEngine engine)
        {
            Rectangle agroRegion = new Rectangle(
                (int)Math.Floor(Pos.X - _agroDistance),
                (int)Math.Floor(Pos.Y - _agroDistance),
                _agroDistance * 2,
                _agroDistance * 2
                );

            // DETECT NEARBY ENTITIES AND DETERMINE A TARGET.
            List<RPGEntity> nearbyEntities = engine.Collider.GetIntersectingEntities<RPGEntity>(agroRegion);
            float currDistance = float.MaxValue;
            int maxPriority = Int32.MinValue;

            foreach (RPGEntity entity in nearbyEntities)
            {
                if (entity.Faction != this.Faction && entity.HP > 0)
                {
                    float distance = Vector2.Distance(entity.Pos, Pos);

                    if (entity.AttackPriority > maxPriority ||
                        (entity.AttackPriority == maxPriority && distance < currDistance))
                    {
                        currDistance = distance;
                        maxPriority = entity.AttackPriority;
                        _target = entity;
                    }
                }
            }
            
            // If the NPC has been assigned a target.
            if(_target != null)
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

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            engine.UserPerformance.RestartTiming(string.Format("{0}-AI", Name));
            AggressiveAI(gameTime, engine);            
            engine.UserPerformance.StopTiming(string.Format("{0}-AI", Name));

            // TODO: THis should ALL be moved to RPGEntity
            if (IsAttacking(gameTime))
            {
                // Attack Complete Check.
                if (Drawables.IsStateFinished(CurrentDrawableState, gameTime))
                {
                    CurrentDrawableState = "Idle_" + Direction;
                    _hitEntityList.Clear();
                }
                else
                {
                    List<RPGEntity> intersectingEntities = engine.Collider.GetIntersectingEntities<RPGEntity>(CurrentBoundingBox);
                    foreach (RPGEntity entity in intersectingEntities)
                    {
                        if (this != entity && !_hitEntityList.Contains(entity) && entity.Faction != this.Faction)
                        {
                            _hitEntityList.Add(entity);
                            entity.OnHit(this, RollForDamage(), gameTime, engine);
                        }
                    }
                }
            }
            else
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
