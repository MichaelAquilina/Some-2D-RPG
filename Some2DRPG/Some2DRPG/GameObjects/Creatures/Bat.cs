using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Some2DRPG.GameObjects.Characters;

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
            this.HP = 15;
            this._randomModifier = randomGenerator.NextDouble();
            this._moveSpeed = 1.8f;
            this.Visible = true;
            this.EntityCollisionEnabled = false;
            this.TerrainCollisionEnabled = false;
            this.IdlePrefix = "Fly";
            this.MovePrefix = "Fly";
        }

        public override void OnHit(Entity sender, int damageDealt, GameTime gameTime, TeeEngine engine)
        {
            if (HP > 0)
            {
                _hitColor = Color.Red;
                _hitPercentage = 0.0f;

                base.OnHit(sender, damageDealt, gameTime, engine);
            }
        }

        public override bool IsAttacking(GameTime gameTime)
        {
            return AttackStance == AttackStance.Attacking;
        }

        public override bool IsFinishedAttacking(GameTime gameTime)
        {
            return AttackStance == AttackStance.NotAttacking;
        }

        public void AggressiveBatAI(GameTime gameTime, TeeEngine engine)
        {
            _target = GetHighestPriorityTarget(gameTime, engine, _agroDistance);

            if (_target != null)
            {
                // ATTACKING LOGIC.
                if (AttackStance == AttackStance.Attacking)
                {
                    this.Pos.X -= (float)(Math.Cos(_attackAngle) * _attackSpeed);
                    this.Pos.Y -= (float)(Math.Sin(_attackAngle) * _attackSpeed);
                    this._attackHeight.Y += 30.0f / ATTACK_COUNTER_LIMIT;
                    this.Drawables.SetGroupProperty("Body", "Offset", _attackHeight);

                    if (_attackCounter++ == ATTACK_COUNTER_LIMIT)
                    {
                        _hitEntityList.Clear();
                        AttackStance = AttackStance.NotAttacking;
                    }
                }
                // ATTACK PREPERATION LOGIC.
                else if (AttackStance == AttackStance.Preparing)
                {
                    _attackHeight.Y -= 2;

                    if (_attackHeight.Y < -40)
                    {
                        _attackHeight.Y = -40;
                        _attackAngle = Math.Atan2(
                            this.Pos.Y - _target.Pos.Y,
                            this.Pos.X - _target.Pos.X
                            );
                        AttackStance = AttackStance.Attacking;
                        _attackCounter = 0;
                    }

                    Drawables.SetGroupProperty("Body", "Offset", _attackHeight);
                }
                // NON-ATTACKING LOGIC. PATROL AND APPROACH.
                else if (AttackStance == AttackStance.NotAttacking)
                {
                    double distance = Vector2.Distance(_target.Pos, this.Pos);

                    if (distance < _attackDistance)
                        Approach(_target.Pos);
                    else
                        AttackStance = AttackStance.Preparing;
                }
            }
            else
            {
                // Perform a standard patrol action.
                Pos.X += (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds - _randomModifier * 90) * 2);
            }
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (this.HP > 0)
            {
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
                "Animations/Monsters/bat.anim", 
                content, startTimeMS
                );

            CurrentDrawableState = "Fly_Left";
        }
    }
}
