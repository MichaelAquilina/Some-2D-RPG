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
using Microsoft.Xna.Framework.Input;

namespace Some2DRPG.MapScripts
{
    public class ExampleMapScript : IMapScript
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

        public void LargeMapZone_MapZoneHit(MapZone sender, List<Entity> entitiesHit, TeeEngine engine, GameTime gameTime)
        {

        }

        public void CaveEntrance_MapZoneHit(MapZone sender, List<Entity> entitiesHit, TeeEngine engine, GameTime gameTime)
        {
            if( KeyboardExtensions.GetKeyDownState(Keyboard.GetState(), Keys.S, engine, true) && 
                entitiesHit.Contains(engine.GetEntity("Player")))
            {
                engine.ClearEntities();
                engine.LoadMap("Content/Maps/cave_example.tmx");
            }
        }

        #endregion
    }
}
