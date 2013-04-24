using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine;
using GameEngine.Interfaces;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;

namespace Some2DRPG.MapScripts
{
    public class CaveExampleMapScript : IMapScript
    {
        public void MapLoaded(TeeEngine engine, TiledMap map)
        {
            engine.GetPostGameShader("LightShader").Enabled = true;
        }

        public void Update(TeeEngine engine, GameTime gameTime)
        {
        }

        public void MapUnloaded(TeeEngine engine, TiledMap map)
        {
        }
    }
}
