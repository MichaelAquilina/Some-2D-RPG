using GameEngine;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Some2DRPG.GameObjects
{
    public class Chest : Entity
    {
        public Chest(float X, float Y)
            :base(X,Y)
        {
        }

        public override void Update(GameTime GameTime, TeeEngine Engine)
        {
            Hero player = (Hero) Engine.GetEntity("Player");
            KeyboardState keyboardState = Keyboard.GetState();

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.S, true)
                && this.CurrentBoundingBox.Intersects(player.CurrentBoundingBox))
            {
                if (CurrentDrawableState != "Open")
                {
                    CurrentDrawableState = "Open";
                    Drawables.ResetState("Open", GameTime);
                }
            }
        }

        public override void LoadContent(ContentManager Content)
        {
            Animation.LoadAnimationXML(Drawables, "Animations/Misc/chests.anim", Content);
            CurrentDrawableState = "Closed";
        }
    }
}
