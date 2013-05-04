using GameEngine;
using GameEngine.Extensions;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Some2DRPG.GameObjects.Misc;
using Some2DRPG.Items;
using Some2DRPG.Shaders;

namespace Some2DRPG.GameObjects.Characters
{
    public class Hero : NPC
    {
        const int INPUT_DELAY = 0;
        const float MOVEMENT_SPEED = 2.9f;

        public bool CollisionDetection { get; set; }

        public LightSource LightSource { get; set; }

        public Hero()
            :base(NPC.MALE_HUMAN)
        {
            Construct(0, 0);
        }

        public Hero(float x, float y) :
            base(x, y, NPC.MALE_HUMAN)
        {
            Construct(x, y);
        }

        private void Construct(float x, float y)
        {
            HP = 2000;
            XP = 0;

            CollisionDetection = true;
            LightSource = new LightSource();
            LightSource.Width = 32 * 8;
            LightSource.Height = 32 * 8;
            LightSource.Pulse = 0.15f;
            LightSource.Color = Color.White;
            LightSource.PositionType = LightPositionType.Relative;

            Equip(ItemRepository.GameItems["RobeHood"]);
            Equip(ItemRepository.GameItems["RobeSkirt"]);
            Equip(ItemRepository.GameItems["PlateGloves"]);
            Equip(ItemRepository.GameItems["RobeShirt"]);
            Equip(ItemRepository.GameItems["PlateBoots"]);
        }

        public override void PostInitialize(GameTime gameTime, TeeEngine engine)
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

            // ATTACK KEY.
            if (keyboardState.IsKeyDown(Keys.A))
            {
                bool reset = !CurrentDrawableState.StartsWith("Slash");
                CurrentDrawableState = "Slash_" + Direction;

                if (reset) Drawables.ResetState(CurrentDrawableState, gameTime);
            }
            else
            {
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

                if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.T, this, true))
                    Unequip(ItemType.Vest);

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

            base.Update(gameTime, engine);
        }
    }
}
