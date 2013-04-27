using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using System;

namespace GameEngine.Interfaces
{
    public interface IMapScript
    {
        void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs mapEventArgs);

        void Update(TeeEngine engine, GameTime gameTime);

        void MapUnloaded(TeeEngine engine, TiledMap map);
    }
}
