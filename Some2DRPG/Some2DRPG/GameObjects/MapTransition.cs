using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameEngine;

namespace Some2DRPG.GameObjects
{
    // TODO: Document appropriately.
    public class MapTransition : Entity
    {
        int _width;
        int _height;
        string _targetMapPath;
        TeeEngine.LoadEntityHandler _callback;

        public MapTransition(
            float x, float y, 
            int width, int height, 
            string targetMapPath,
            TeeEngine.LoadEntityHandler callback
            )
            :base(x,y)
        {
            _width = width;
            _height = height;
            _targetMapPath = targetMapPath;
            _callback = callback;
        }

        public override void LoadContent(ContentManager content)
        {
            StaticImage image = new StaticImage(
                content.Load<Texture2D>("Misc/Zone"),
                new Rectangle(0, 0, _width, _height));
            image.Origin = new Vector2(0, 0);

            GameDrawableInstance instance = Drawables.Add("Standard", image, "Body", 0);
            instance.Color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            CurrentDrawableState = "Standard";
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            Hero player = (Hero)engine.GetEntity("Player");

            if (Entity.IntersectsWith(player, "Shadow", this, "Body", gameTime)
                && KeyboardExtensions.GetKeyDownState(Keyboard.GetState(), Keys.S, engine, true))
            {
                engine.ClearEntities();
                engine.LoadMap(_targetMapPath, _callback);
            }
        }
    }
}
