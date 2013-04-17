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

namespace Some2DRPG.GameObjects
{
    // TODO: Document appropriately.
    public class MapTransition : Entity
    {
        float _width;
        float _height;
        string _targetMapPath;

        public MapTransition(
            float x, float y, 
            float width, float height, 
            string targetMapPath)
            :base(x,y)
        {
            _width = width;
            _height = height;
            _targetMapPath = targetMapPath;
        }

        public override void LoadContent(ContentManager content)
        {
            // TODO: Replace with something more approrpiate than an image!!!!!
            // VERY, VERY temporary. This is just to provide a bounding area until a better api is added
            StaticImage image = new StaticImage(
                content.Load<Texture2D>("LPC/Terrain/buckets"),
                new Rectangle(0, 0, (int) _width, (int) _height));
            image.Origin = new Vector2(0, 0);

            Drawables.Add("Standard", image, "Body", 0);
            CurrentDrawableState = "Standard";
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            Hero player = (Hero)engine.GetEntity("Player");

            if (Entity.IntersectsWith(player, "Shadow", this, "Body", gameTime)
                && KeyboardExtensions.GetKeyDownState(Keyboard.GetState(), Keys.S, this, true))
            {
                // TODO: Clear previous entities? Reset Player position? Save entity states from unloaded map?
                // TODO: add an engine method. PersistEntityInfo() which saves entity information to disk and/or memory.
                // Data is saved in by name->entity format. This means that you can provided a method to LoadPersistedEntityInfo()
                // which loads the data as it was previously saved.
                engine.LoadMap(TiledMap.ReadTiledXml(_targetMapPath));
            }
        }
    }
}
