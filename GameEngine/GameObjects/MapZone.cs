using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GameObjects
{
    // Repesents an Interactable Zone on the Map that can have a number of Scripted events assigned to it.
    public class MapZone : Entity, ISizedEntity
    {
        public delegate void MapZoneHitEventHandler(MapZone sender, Entity entity, TeeEngine engine, GameTime gameTime);

        public event MapZoneHitEventHandler MapZoneHit;

        public int Width { get { return _width; } set { _width = value; } }
        public int Height { get { return _height; } set { _height = value; } }

        int _width;
        int _height;

        public MapZone()
        {
            Visible = false;
        }

        void OnMapZoneHit(Entity entity, TeeEngine engine, GameTime gameTime)
        {
            if (MapZoneHit != null)
                MapZoneHit(this, entity, engine, gameTime);
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

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            List<Entity> entitiesHit = engine.QuadTree.GetIntersectingEntites(this.CurrentBoundingBox);

            foreach (Entity entity in entitiesHit)
            {
                if (entity != this && Entity.IntersectsWith(this, null, entity, null, gameTime))
                    OnMapZoneHit(entity, engine, gameTime);
            }
        }
    }
}
