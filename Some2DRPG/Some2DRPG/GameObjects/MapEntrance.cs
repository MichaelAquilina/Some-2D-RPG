using System.Collections.Generic;
using GameEngine;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameEngine.Tiled;

namespace Some2DRPG.GameObjects
{
    public class MapEntrance : MapZone
    {
        const Keys ACTIVATE_KEY = Keys.S;

        public string Destination { get; set; }     // Destination map in the form of the map file path.
        public string Target { get; set; }          // Target MapEntrance place the Player on.

        public MapEntrance()
        {
            this.MapZoneHit += MapEntrance_MapZoneHit;
        }

        void MapEntrance_MapZoneHit(MapZone sender, Entity entity, TeeEngine engine, GameTime gameTime)
        {
            if(KeyboardExtensions.GetKeyDownState(Keyboard.GetState(), ACTIVATE_KEY, engine, true) &&
               entity == engine.GetEntity("Player"))
            {
                MapEventArgs mapArgs = new MapEventArgs();
                mapArgs.SetProperty("Target", Target);

                engine.ClearEntities();
                engine.LoadMap(Destination, mapArgs);
            }
        }
    }
}
