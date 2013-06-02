using System;
using GameEngine;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Some2DRPG.GameObjects.Misc;
using Some2DRPG.Shaders;

namespace Some2DRPG.GameObjects.Characters
{
    public class Hero : RPGEntity
    {
        const int INPUT_DELAY = 0;

        public LightSource LightSource { get; set; }

        public Hero()
            : base(RPGEntity.HUMAN_MALE)
        {
            Construct(0, 0);
        }

        public Hero(float x, float y) :
            base(x, y, RPGEntity.HUMAN_MALE)
        {
            Construct(x, y);
        }

        private void Construct(float x, float y)
        {
            _moveSpeed = 2.0f;

            MaxHP = 2000;
            HP = MaxHP;

            XP = 0;
            Strength = 10;
            AttackPriority = 5;

            Faction = "Allies";
            CollisionGroup = "Shadow";
            LightSource = new LightSource();
            LightSource.Width = 32 * 8;
            LightSource.Height = 32 * 8;
            LightSource.Pulse = 0.15f;
            LightSource.Color = Color.White;
            LightSource.PositionType = LightPositionType.Relative;

            QuickEquip("PlateHelmet");
            QuickEquip("PlatePants");
            QuickEquip("PlateGloves");
            QuickEquip("PlateChest");
            QuickEquip("PlateBoots");
            QuickEquip("Sabre");
        }

        public override void PostCreate(GameTime gameTime, TeeEngine engine)
        {
            engine.AddEntity(LightSource);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
        }

        public void PerformInteraction(GameTime gameTime, TeeEngine engine)
        {
            foreach (CollidableEntity entity in IntersectingEntities)
            {
                if (entity != this
                    && entity is RPGEntity
                    && Entity.IntersectsWith(this, null, entity, null, gameTime))
                {
                    RPGEntity rpgEntity = (RPGEntity)entity;
                    if (rpgEntity.Faction == this.Faction)
                        rpgEntity.OnInteract(this, gameTime, engine);
                }

            }
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Vector2 movement = Vector2.Zero;

            if (CurrentState == EntityStates.Attacking)
            {
                if (Drawables.IsStateFinished(CurrentDrawableState, gameTime))
                    CurrentState = EntityStates.Alert;
                else
                    PerformHitCheck(gameTime, engine);
            }
            else
            {
                // PERFORM ATTACK MOVE.
                if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.A, engine, true))
                {
                    CurrentState = EntityStates.Attacking;
                    CurrentDrawableState = "Slash_" + Direction;
                    ClearHitList();

                    Drawables.ResetState(CurrentDrawableState, gameTime);
                }
                else
                {
                    // PERFORM INTERACTION
                    if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.S, engine, true))
                        PerformInteraction(gameTime, engine);

                    // MOVEMENT BASED KEYBOARD EVENTS.
                    if (keyboardState.IsKeyDown(Keys.Up))
                        movement.Y--;

                    if (keyboardState.IsKeyDown(Keys.Down))
                        movement.Y++;

                    if (keyboardState.IsKeyDown(Keys.Left))
                        movement.X--;

                    if (keyboardState.IsKeyDown(Keys.Right))
                        movement.X++;

                    if (movement.Length() > 0)
                    {
                        movement.Normalize();
                        Pos += movement * _moveSpeed;
                    }

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

            base.Update(gameTime, engine);

            // Update Light Source based on latest position.
            LightSource.Pos = this.Pos;
        }
    }
}
