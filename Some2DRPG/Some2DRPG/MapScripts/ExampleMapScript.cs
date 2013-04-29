using System;
using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Some2DRPG.GameObjects.Creatures;
using Some2DRPG.GameObjects.Misc;
using Some2DRPG.Shaders;

namespace Some2DRPG.MapScripts
{
    public class ExampleMapScript : Some2DRPGMasterScript
    {
        public override void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs mapEventArgs)
        {
            LightShader lightShader = (LightShader)engine.GetPostGameShader("LightShader");
            lightShader.Enabled = false;

            Random random = new Random();

            for (int i = 0; i < 50; i++)
            {
                int px = (int)Math.Ceiling(random.NextDouble() * engine.Map.pxWidth);
                int py = (int)Math.Ceiling(random.NextDouble() * engine.Map.pxHeight);

                Bat bat = new Bat(px, py);
                Coin coin = new Coin(px, py, 100, (CoinType)random.Next(3));

                // Switch between adding bats and coins to the map.
                if (i % 2 == 0)
                    engine.AddEntity(bat);
                else
                    engine.AddEntity(coin);
            }

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
