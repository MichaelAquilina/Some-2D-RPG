using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GameObjects
{
    // TODO: Could probably be named something more appropriate.
    public class MapZone : Entity
    {
        public delegate void MapZoneEvent(MapZone sender, List<Entity> intersectingEntities, TeeEngine engine, GameTime gameTime);

        public int Width 
        {
            get { return _width; }
            set { _width = value; ResetImage();  }
        }
        public int Height 
        {
            get { return _height; }
            set { _height = value; ResetImage();  }
        }

        int _width = 0;
        int _height = 0;
        StaticImage _zoneImage;

        public event MapZoneEvent MapZoneHit;

        public MapZone()
        {
        }

        public MapZone(float x, float y, int width, int height)
            :base(x, y)
        {
            this._width = width;
            this._height = height;
        }

        void ResetImage()
        {
            if (_zoneImage != null)
                _zoneImage.SourceRectangle = new Rectangle(0, 0, _width, _height);
        }

        void OnMapZoneHit(List<Entity> intersectingEntities, TeeEngine engine, GameTime gameTime)
        {
            if (MapZoneHit != null)
                MapZoneHit(this, intersectingEntities, engine, gameTime);
        }

        public override void LoadContent(ContentManager content)
        {
            _zoneImage = new StaticImage(
                content.Load<Texture2D>("Misc/Zone"),
                new Rectangle(0, 0, Width, Height)
             );
            _zoneImage.Origin = new Vector2(0, 0);

            GameDrawableInstance instance = Drawables.Add("Standard", _zoneImage, "Body", 0);
            instance.Color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            CurrentDrawableState = "Standard";
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            List<Entity> intersectingEntities = engine.QuadTree.GetIntersectingEntites(CurrentBoundingBox);

            // If the same mapZone is not only thing in the intersection results - we have a hit.
            if (intersectingEntities.Count > 1)
                OnMapZoneHit(intersectingEntities, engine, gameTime);
        }

        public override string ToString()
        {
            return string.Format(
                "MapZone: Name={0}, Pos={1}, Width={2}, Height={3}",
                Name, Pos, Width, Height
                );
        }
    }
}
