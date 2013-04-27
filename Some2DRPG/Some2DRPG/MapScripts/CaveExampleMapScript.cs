using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine;
using GameEngine.Interfaces;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Some2DRPG.GameObjects;
using Some2DRPG.Shaders;

namespace Some2DRPG.MapScripts
{
    public class CaveExampleMapScript : Some2DRPGMasterScript
    {
        public virtual void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs mapEventArgs)
        {
            LightShader lightShader = (LightShader)engine.GetPostGameShader("LightShader");
            lightShader.Enabled = true;

            base.MapLoaded(engine, map, mapEventArgs);
        }

        public virtual void Update(TeeEngine engine, GameTime gameTime)
        {
            base.Update(engine, gameTime);
        }

        public virtual void MapUnloaded(TeeEngine engine, TiledMap map)
        {
            base.MapUnloaded(engine, map);
        }
    }
}
