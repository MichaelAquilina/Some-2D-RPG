﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Tiled;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameEngine;

namespace GameEngine.GameObjects
{
    // TODO: Document appropriately.
    public class MapZone : Entity, ISizedEntity
    {
        public delegate void MapZoneHitEventHandler(MapZone sender, List<Entity> entitiesHit, GameTime gameTime);

        public event MapZoneHitEventHandler MapZoneHit;

        public int Width { get { return _width; } set { _width = value; } }
        public int Height { get { return _height; } set { _height = value; } }

        int _width;
        int _height;

        public MapZone()
        {
        }

        void OnMapZoneHit(List<Entity> entitiesHit, GameTime gameTime)
        {
            if (MapZoneHit != null)
                MapZoneHit(this, entitiesHit, gameTime);
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
            if (entitiesHit.Count > 1)
                OnMapZoneHit(entitiesHit, gameTime);
        }
    }
}