using GameEngine;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Some2DRPG.Shaders;

namespace Some2DRPG.MapScripts
{
    public class CaveExampleMapScript : Some2DRPGMasterScript
    {
        public override void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs mapEventArgs)
        {
            LightShader lightShader = (LightShader)engine.GetPostGameShader("LightShader");
            lightShader.Enabled = true;

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
    }
}
