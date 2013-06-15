using System;
using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Pathfinding;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Some2DRPG.GameObjects.Misc;

namespace Some2DRPG.GameObjects.Characters
{
    // Could possibly be renamed to AiRpgEntity if is generic enough.
    public class NPC : RPGEntity
    {
        double _randomDelay = 0;
        double _attackDelay = 800;
        double _lastAttack = 0;

        bool _startedDeathAnim = false;
        float _attackDistance = 40f;
        float _agroDistance = 200;
        RPGEntity _target;

        public NPC()
        {
            this.CurrentState = EntityStates.Idle;
            this.AttackPriority = 6;
            this.MaxHP = 50;
            this.HP = this.MaxHP;
            this.Strength = 5;
            this.CollisionGroup = "Shadow";
            this.Interact += NPC_Interact;
            this._randomDelay = RPGRandomGenerator.Next(1000);
        }

        private void NPC_Interact(RPGEntity sender, Entity invoker, GameTime gameTime, TeeEngine engine)
        {
            SpeechBubble speech = new SpeechBubble(this, "Hello there Adventurer! Whats you're name?");

            engine.AddEntity(speech);

            Vector2 distance = this.Pos - invoker.Pos;
            if (distance.X > distance.Y)
                Direction = (distance.X > 0) ? Direction.Left : Direction.Right;
            else
                Direction = (distance.Y > 0) ? Direction.Up : Direction.Down;
        }

        private bool PathfindingValidator(ANode current, ANode target, TeeEngine engine, GameTime gameTime)
        {
            Tile tile = engine.Map.GetTxTopMostTile((int)target.TxPos.X, (int)target.TxPos.Y);

            // Diagonal to the current tile
            if (current != null && Vector2.Distance(current.TxPos, target.TxPos) > 1 )
            {
                Tile diagonal1;
                Tile diagonal2;

                return false;
            }
            else return tile != null && !tile.HasProperty("Impassable");
        }

        private void AggressiveNpcAI(GameTime gameTime, TeeEngine engine)
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
                        if (gameTime.TotalGameTime.TotalMilliseconds - _lastAttack > _attackDelay + _randomDelay)
                        {
                            _lastAttack = gameTime.TotalGameTime.TotalMilliseconds;
                            _randomDelay = RPGRandomGenerator.Next(1000);

                            CurrentDrawableState = "Slash_" + Direction;
                            CurrentState = EntityStates.Attacking;

                            ClearHitList();
                            Drawables.ResetState(CurrentDrawableState, gameTime);
                        }
                    }
                    else
                    {
                        Path path = engine.Pathfinding.GeneratePath(this.Pos, _target.Pos, engine, gameTime, PathfindingValidator);
                        FollowPath(path, engine);
                    }

                    Vector2 difference = this.Pos - _target.Pos;
                    if (Math.Abs(difference.X) > Math.Abs(difference.Y))
                        Direction = (difference.X > 0) ? Direction.Left : Direction.Right;
                    else
                        Direction = (difference.Y > 0) ? Direction.Up : Direction.Down;
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
                if (PrevPos != Pos)
                {
                    Vector2 difference = Pos - PrevPos;
                    if (Math.Abs(difference.X) > Math.Abs(difference.Y))
                        Direction = (difference.X > 0) ? Direction.Right : Direction.Left;
                    else
                        Direction = (difference.Y > 0) ? Direction.Down : Direction.Up;

                    CurrentDrawableState = "Walk_" + Direction;
                }
                else CurrentDrawableState = "Idle_" + Direction;
            }
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (HP > 0)
            {
                AggressiveNpcAI(gameTime, engine);
                base.Update(gameTime, engine);
            }
            else
            {
                this.EntityCollisionEnabled = false;
                this.TerrainCollisionEnabled = false;

                if (!_startedDeathAnim)
                {
                    _startedDeathAnim = true;
                    CurrentDrawableState = "Death";
                    Drawables.ResetState(CurrentDrawableState, gameTime);
                }
            }

        }
    }
}
