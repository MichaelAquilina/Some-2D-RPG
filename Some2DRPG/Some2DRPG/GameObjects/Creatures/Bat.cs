using System;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameEngine.Extensions;

namespace Some2DRPG.GameObjects.Creatures
{
    public class Bat : RPGEntity
    {
        private static Random randomGenerator = new Random();

        private const int ATTACK_COUNTER_LIMIT = 40;
        private const double _attackDistance = 40;

        private float _agroDistance = 200;
        private RPGEntity _target = null;
        private int _attackCounter = 0;
        private Vector2 _attackHeight = Vector2.Zero;    
        private double _attackAngle = 0;
        private double _randomModifier;
        private float _attackSpeed = 5.4f;
        private Color _hitColor = Color.White;
        private float _hitPercentage = 1.0f;
        
        public Bat()
        {
            Construct(0, 0);
        }

        public Bat(float x, float y)
        {
            Construct(x, y);
        }

        void Construct(float x, float y)
        {
            this.Strength = 4;
            this.AttackPriority = 3;
            this.BaseRace = CREATURES_BAT;
            this.Faction = "Creatures";
            this.Pos = new Vector2(x, y);
            this.MaxHP = 15;
            this.HP = this.MaxHP;
            this._randomModifier = randomGenerator.NextDouble();
            this._moveSpeed = 1.8f;
            this.EntityCollisionEnabled = false;
            this.TerrainCollisionEnabled = false;
            this.CurrentState = EntityStates.Idle;
            this.Hit += Bat_Hit;
        }

        private void Bat_Hit(RPGEntity sender, Entity invoker, GameTime gameTime, TeeEngine engine)
        {
            if (HP > 0)
            {
                _hitColor = Color.Red;
                _hitPercentage = 0.0f;
            }
        }

        public void AggressiveBatAI(GameTime gameTime, TeeEngine engine)
        {
            _target = GetHighestPriorityTarget(gameTime, engine, _agroDistance);

            if (CurrentState == EntityStates.Idle)
            {
                if (_target != null) CurrentState = EntityStates.Alert;

                Pos.X += (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds - _randomModifier * 90) * 2);
            }
            else if (CurrentState == EntityStates.Alert)
            {
                if (_target == null) CurrentState = EntityStates.Idle;
                else
                {
                    if (Vector2.Distance(this.Pos, _target.Pos) < _attackDistance)
                        CurrentState = EntityStates.PrepareAttack;
                    else 
                        Approach(_target.Pos);
                }
            }
            else if (CurrentState == EntityStates.PrepareAttack)
            {
                if (_target == null) CurrentState = EntityStates.Alert;
                else
                {
                    _attackHeight.Y -= 2;

                    if (_attackHeight.Y < -40)
                    {
                        _attackHeight.Y = -40;
                        _attackAngle = Math.Atan2(
                            this.Pos.Y - _target.Pos.Y,
                            this.Pos.X - _target.Pos.X
                            );
                        CurrentState = EntityStates.Attacking;
                        ClearHitList();
                        _attackCounter = 0;
                    }

                    Drawables.SetGroupProperty("Body", "Offset", _attackHeight);
                }
            }
            else if (CurrentState == EntityStates.Attacking)
            {
                this.Pos.X -= (float)(Math.Cos(_attackAngle) * _attackSpeed);
                this.Pos.Y -= (float)(Math.Sin(_attackAngle) * _attackSpeed);
                this._attackHeight.Y += 30.0f / ATTACK_COUNTER_LIMIT;
                this.Drawables.SetGroupProperty("Body", "Offset", _attackHeight);

                PerformHitCheck(gameTime, engine);

                if (_attackCounter++ == ATTACK_COUNTER_LIMIT)
                    CurrentState = EntityStates.Alert;
            }

            if (PrevPos != Pos)
            {
                Vector2 difference = Pos - PrevPos;
                if (Math.Abs(difference.X) > Math.Abs(difference.Y))
                    Direction = (difference.X > 0) ? Direction.Right : Direction.Left;
                else
                    Direction = (difference.Y > 0) ? Direction.Down : Direction.Up;

                CurrentDrawableState = "Fly_" + Direction;
            }
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (this.HP > 0)
            {
                if (_hitColor != Color.White)
                {
                    _hitPercentage += 0.06f;
                    _hitColor = ColorExtensions.Transition(Color.Red, Color.White, _hitPercentage, false);
                    Drawables.SetGroupProperty("Body", "Color", _hitColor);
                }

                AggressiveBatAI(gameTime, engine);

                base.Update(gameTime, engine);
            }
            else
            {
                this.Opacity -= 0.02f;
                this.Drawables.ResetState(CurrentDrawableState, gameTime);
                if (this.Opacity < 0)
                    engine.RemoveEntity(this);
            }
        }

        public override void LoadContent(ContentManager content)
        {
            double startTimeMS = randomGenerator.NextDouble() * 4000;

            DrawableSet.LoadDrawableSetXml(
                Drawables, 
                "Animations/Monsters/bat.draw", 
                content, startTimeMS
                );

            CurrentDrawableState = "Fly_Left";
        }
    }
}
