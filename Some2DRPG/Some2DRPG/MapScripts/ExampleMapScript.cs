using System.Collections.Generic;
using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Some2DRPG.GameObjects;
using Some2DRPG.Shaders;

namespace Some2DRPG.MapScripts
{
    public class ExampleMapScript : Some2DRPGMasterScript
    {
        public override void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs mapEventArgs)
        {
            LightShader lightShader = (LightShader)engine.GetPostGameShader("LightShader");
            lightShader.Enabled = false;

            base.MapLoaded(engine, map, mapEventArgs);
        }

        public override void Update(TeeEngine engine, GameTime gameTime)
        {
            base.Update(engine, gameTime);
        }

        public override void MapUnloaded(TeeEngine engine, TiledMap map)
        {
            base.MapUnloaded(engine, map);
        }

        #region Event Handlers

        public void LargeMapZone_MapZoneHit(MapZone sender, Entity entity, TeeEngine engine, GameTime gameTime)
        {

        }

        #endregion
    }
}
