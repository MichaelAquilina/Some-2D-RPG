using System.Collections.Generic;
using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;

namespace Some2DRPG.MapScripts
{
    public class ExampleMapScript : IMapScript
    {
        public void MapLoaded(TeeEngine engine, TiledMap map)
        {
            engine.GetPostGameShader("LightShader").Enabled = false;
        }

        public void Update(TeeEngine engine, GameTime gameTime)
        {
        }

        public void MapUnloaded(TeeEngine engine, TiledMap map)
        {
        }

        #region Event Handlers

        public void LargeMapZone_MapZoneHit(MapZone sender, List<Entity> entitiesHit, TeeEngine engine, GameTime gameTime)
        {

        }

        #endregion
    }
}
