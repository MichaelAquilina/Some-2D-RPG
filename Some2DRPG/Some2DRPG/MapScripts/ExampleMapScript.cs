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
    public class ExampleMapScript : IMapScript
    {
        public void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs mapEventArgs)
        {
            LightShader lightShader = (LightShader)engine.GetPostGameShader("LightShader");
            lightShader.Enabled = false;

            if (mapEventArgs.HasProperty("Target"))
            {
                Hero player = new Hero();
                MapEntrance targetEntrance = (MapEntrance) engine.GetEntity(mapEventArgs.GetProperty("Target"));

                player.Pos = targetEntrance.Pos + new Vector2(targetEntrance.Width, targetEntrance.Height) / 2;

                engine.AddEntity("Player", player);
            }
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
