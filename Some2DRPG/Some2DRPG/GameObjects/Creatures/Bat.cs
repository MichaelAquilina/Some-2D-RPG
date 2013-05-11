using System;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Some2DRPG.GameObjects.Characters;
using GameEngine.Extensions;

namespace Some2DRPG.GameObjects.Creatures
{
    public class Bat : NPC
    {
        private static Random randomGenerator = new Random();

        private const int ATTACK_COUNTER_LIMIT = 40;
        private const double ATTACK_DISTANCE = 40;
        private const double AGRO_DISTANCE = 200;

        private int _attackCounter = 0;
        private Vector2 _attackHeight = Vector2.Zero;    
        private double _attackAngle = 0;
        private double _randomModifier;
        private float _attackSpeed = 5.4f;
        private float _moveSpeed = 1.8f;
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
            this.BaseRace = CREATURES_BAT;
            this.Faction = "Creatures";
            this.Pos = new Vector2(x, y);
            this.HP = 15;
            this._randomModifier = randomGenerator.NextDouble();
            this.Visible = true;
            this.EntityCollisionEnabled = false;
            this.TerrainCollisionEnabled = false;
        }

        public override void OnHit(Entity sender, int damageDealt, GameTime gameTime, TeeEngine engine)
        {
            base.OnHit(sender, damageDealt, gameTime, engine);

            if (HP > 0)
            {
                _hitColor = Color.Red;
                _hitPercentage = 0.0f;
            }
        }

        // TODO: Clean!!!!!!
        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            // Get the Hero player for interaction purposes.
            Hero player = (Hero)engine.GetEntity("Player");
            Vector2 prevPos = Pos;

            // Needs to be improved
            if (_hitPercentage != 1.0f)
            {
                _hitPercentage += 0.05f;
                _hitColor = ColorExtensions.Transition(Color.Red, Color.White, _hitPercentage);

                Drawables.SetGroupProperty("Body", "Color", _hitColor);
            }

            // Check if this Bat has died.
            if (HP <= 0)
            {
                this.Opacity -= 0.02f;
                this.Drawables.ResetState(CurrentDrawableState, gameTime);
                if (this.Opacity < 0) 
                    engine.RemoveEntity(this);
            }
            else
            {
                // ATTACKING LOGIC.
                if (AttackStance == AttackStance.Attacking)
                {
                    this.Pos.X -= (float) (Math.Cos(_attackAngle) * _attackSpeed);
                    this.Pos.Y -= (float) (Math.Sin(_attackAngle) * _attackSpeed);
                    this._attackHeight.Y += 30.0f / ATTACK_COUNTER_LIMIT;
                    this.Drawables.SetGroupProperty("Body", "Offset", _attackHeight);

                    if (Entity.IntersectsWith(this, "Shadow", player, "Shadow", gameTime))
                        player.OnHit(this, 4, gameTime, engine);

                    if (_attackCounter++ == ATTACK_COUNTER_LIMIT)
                        AttackStance = AttackStance.NotAttacking;
                }
                // ATTACK PREPERATION LOGIC.
                else if (AttackStance == AttackStance.Preparing)
                {
                    _attackHeight.Y -= 2;

                    if (_attackHeight.Y < -40)
                    {
                        _attackHeight.Y = -40;
                        _attackAngle = Math.Atan2(
                            this.Pos.Y - player.Pos.Y,
                            this.Pos.X - player.Pos.X
                            );
                        AttackStance = AttackStance.Attacking;
                        _attackCounter = 0;
                    }

                    Drawables.SetGroupProperty("Body", "Offset", _attackHeight);
                }
                // NON-ATTACKING LOGIC. PATROL AND APPROACH.
                else if (AttackStance == AttackStance.NotAttacking)
                {
                    double distance = Vector2.Distance(player.Pos, this.Pos);

                    if (distance < AGRO_DISTANCE)
                    {
                        // Move towards the player for an attack move.
                        double angle = Math.Atan2(
                            player.Pos.Y - this.Pos.Y,
                            player.Pos.X - this.Pos.X
                            );

                        // Approach Function.
                        double moveValue;
                        if (distance < ATTACK_DISTANCE)
                        {
                            AttackStance = AttackStance.Preparing;
                            moveValue = 0;
                        }
                        else
                            moveValue = _moveSpeed;

                        Pos.X += (float)(Math.Cos(angle) * moveValue);
                        Pos.Y += (float)(Math.Sin(angle) * moveValue);
                    }
                    else
                    {
                        // Perform a standard patrol action.
                        Pos.X += (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds - _randomModifier * 90) * 2);
                    }
                }

                // Determine the animation based on the change in position.
                if (Math.Abs(prevPos.X - Pos.X) > Math.Abs(prevPos.Y - Pos.Y))
                {
                    if (prevPos.X < Pos.X)
                        this.CurrentDrawableState = "Right";
                    if (prevPos.X > Pos.X)
                        this.CurrentDrawableState = "Left";
                }
                else
                {
                    if (prevPos.Y < Pos.Y)
                        this.CurrentDrawableState = "Down";
                    if (prevPos.Y > Pos.Y)
                        this.CurrentDrawableState = "Up";
                }
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

            CurrentDrawableState = "Left";
        }
    }
}
