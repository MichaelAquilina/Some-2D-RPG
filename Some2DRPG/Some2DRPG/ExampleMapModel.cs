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
    public class ExampleMapModel : IMapScript
    {
        public void MapLoaded(TeeEngine engine, TiledMap map)
        {
            //// TODO.
        }

        public void Update(TeeEngine engine, GameTime gameTime)
        {
            // TODO.
        }

        public void MapUnloaded(TeeEngine engine, TiledMap map)
        {
            // TODO.
        }

        #region Event Handlers

        public void LargeMapZone_MapZoneHit(MapZone sender, List<Entity> entitiesHit, GameTime gameTime)
        {

        }

        #endregion
    }
}
