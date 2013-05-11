using System.Collections.Generic;
using GameEngine;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Some2DRPG.GameObjects.Misc;
using Some2DRPG.Shaders;

namespace Some2DRPG.GameObjects.Characters
{
    public class Hero : NPC
    {
        const int INPUT_DELAY = 0;
        const float MOVEMENT_SPEED = 2.9f;

        public bool CollisionDetection { get; set; }

        public LightSource LightSource { get; set; }

        // List of Entities hit during an attack cycle.
        List<Entity> _hitEntityList = new List<Entity>();

        public Hero()
            : base(NPC.HUMAN_MALE)
        {
            Construct(0, 0);
        }

        public Hero(float x, float y) :
            base(x, y, NPC.HUMAN_MALE)
        {
            Construct(x, y);
        }

        private void Construct(float x, float y)
        {
            HP = 2000;
            XP = 0;

            Faction = "Allies";
            CollisionDetection = true;
            LightSource = new LightSource();
            LightSource.Width = 32 * 8;
            LightSource.Height = 32 * 8;
            LightSource.Pulse = 0.15f;
            LightSource.Color = Color.White;
            LightSource.PositionType = LightPositionType.Relative;

            QuickEquip("PlateHelmet");
            QuickEquip("RobeSkirt");
            QuickEquip("PlateGloves");
            QuickEquip("RobeShirt");
            QuickEquip("PlateBoots");
            QuickEquip("RustyDagger");
        }

        public override void PostCreate(GameTime gameTime, TeeEngine engine)
        {
            engine.AddEntity(LightSource);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            Vector2 movement = Vector2.Zero;
            float prevX = Pos.X;
            float prevY = Pos.Y;

            Tile prevTile = engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);
            float moveSpeedModifier = prevTile.GetProperty<float>("MoveSpeed", 1.0f);

            // TODO: Improve, we are retrieving this twice because it is called again in the CollidableEntity loop.
            List<Entity> intersectingEntities = engine.Collider.GetIntersectingEntites(CurrentBoundingBox);

            if (CurrentDrawableState.Contains("Slash") 
                && !Drawables.IsStateFinished(CurrentDrawableState, gameTime))
            {
                foreach (Entity entity in intersectingEntities)
                {
                    if (this != entity && entity is NPC && !_hitEntityList.Contains(entity))
                    {
                        NPC entityNPC = (NPC)entity;
                        if (entityNPC.Faction != this.Faction)
                        {
                            _hitEntityList.Add(entityNPC);

                            entityNPC.HP -= 10;
                            entityNPC.OnHit(this, gameTime, engine);
                        }
                    }
                }
            }
            else
            {
                _hitEntityList.Clear();

                if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.A, engine, true))
                {
                    CurrentDrawableState = "Slash_" + Direction;
                    Drawables.ResetState(CurrentDrawableState, gameTime);
                }
                else
                {
                    // Interaction
                    if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.S, engine, true))
                    {
                        foreach (Entity entity in intersectingEntities)
                        {
                            if (entity != this 
                                && entity is NPC
                                && Entity.IntersectsWith(this, null, entity, null, gameTime))
                            {
                                NPC entityNPC = (NPC)entity;
                                if (entityNPC.Faction == this.Faction)
                                    entityNPC.OnInteract(this, gameTime, engine);
                            }

                        }
                    }

                    // MOVEMENT BASED KEYBOARD EVENTS.
                    if (keyboardState.IsKeyDown(Keys.Up))
                    {
                        CurrentDrawableState = "Walk_Up";
                        Direction = Direction.Up;

                        movement.Y--;
                    }
                    if (keyboardState.IsKeyDown(Keys.Down))
                    {
                        CurrentDrawableState = "Walk_Down";
                        Direction = Direction.Down;

                        movement.Y++;
                    }
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        CurrentDrawableState = "Walk_Left";
                        Direction = Direction.Left;

                        movement.X--;
                    }
                    if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        CurrentDrawableState = "Walk_Right";
                        Direction = Direction.Right;

                        movement.X++;
                    }

                    // Set animation to idle of no movements where made.
                    if (movement.Length() == 0)
                        CurrentDrawableState = "Idle_" + Direction;
                    else
                    {
                        movement.Normalize();
                        Pos += movement * MOVEMENT_SPEED * moveSpeedModifier;
                    }

                    LightSource.Pos = this.Pos;
                }
            }

            base.Update(gameTime, engine);
        }
    }
}
