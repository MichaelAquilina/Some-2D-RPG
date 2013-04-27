using System.Collections.Generic;
using GameEngine;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Some2DRPG.GameObjects
{
    public class MapEntrance : MapZone
    {
        const Keys ACTIVATE_KEY = Keys.S;

        public string Destination { get; set; }

        public MapEntrance()
        {
            this.MapZoneHit += MapEntrance_MapZoneHit;
        }

        void MapEntrance_MapZoneHit(MapZone sender, List<Entity> entitiesHit, TeeEngine engine, GameTime gameTime)
        {
            if(KeyboardExtensions.GetKeyDownState(Keyboard.GetState(), ACTIVATE_KEY, engine, true) &&
               entitiesHit.Contains(engine.GetEntity("Player")))
            {
                engine.ClearEntities();
                engine.LoadMap(Destination);
            }
        }
    }
}
