using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;

// Specifies a means for providing logic to a map. Uses the View-Model paradigm often used
// in server side web pages.
// 'Event' Handlers should also be placed here for objects. Examples include 'EntityEntered',
// 'EntityExited' for Entity objects like zones. This way, scripts for specific events can occur.

// Not sure if this is the appropriate namespace to put this in.
namespace GameEngine.Interfaces
{
    // 'Assembly' and 'Model' property should be specified in the TiledMap properties which should point
    // to the extended MapModel class.

    // MapModel vs MapScript? What seems more appropriate?
    public abstract class MapModel
    {
        // Not sure if NotImplementedException should be thrown because it could be we want default functionality
        // for some simpler maps.
        public virtual void MapLoaded(TeeEngine engine, TiledMap map)
        {
            // Load Map Entities from reading TiledObjects
            // Setup the map based on certain properties?
            // I am not sure if this should happen here or from external event. Loading of map entities
            // and loading map properties should be something globally shared. it should not really change
            // from map to map. Only place individual map logic in these sections.
            throw new NotImplementedException();
        }

        public virtual void Update(TeeEngine engine, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public virtual void MapUnloaded(TeeEngine engine, TiledMap map)
        {
            throw new NotImplementedException();
        }
    }
}
