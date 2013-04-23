using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Interfaces;
using GameEngine;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using GameEngine.GameObjects;
using GameEngine.Extensions;

namespace Some2DRPG
{
    public class ExampleMapModel : MapModel
    {
        public override void MapLoaded(TeeEngine engine, TiledMap map)
        {
            //// TODO.
            //foreach (TiledObjectLayer layer in map.TileObjectLayers)
            //{
            //    foreach (TiledObject tiledObject in layer.TiledObjects)
            //    {
            //        Entity entity = (Entity)Activator.CreateInstance(null, tiledObject.Type).Unwrap();
            //        foreach (string propertyKey in tiledObject.PropertyKeys)
            //        {
            //            ReflectionExtensions.SmartSetProperty(
            //                entity, propertyKey, tiledObject.GetProperty(propertyKey));
            //        }
            //    }
            //}
        }

        public override void Update(TeeEngine engine, GameTime gameTime)
        {
            // TODO.
        }

        public override void MapUnloaded(TeeEngine engine, TiledMap map)
        {
            // TODO.
        }

        #region Event Handlers

        public void LargeMapZone_OnMapZoneHit(MapZone sender, List<Entity> intersectingEntities, TeeEngine engine, GameTime gameTime)
        {

        }

        #endregion
    }
}
