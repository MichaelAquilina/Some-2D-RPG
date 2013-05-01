using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using System;

namespace GameEngine.Interfaces
{
    /// <summary>
    /// Defines an interface for which classes wishing to act as Map Scripts need to implement. Map Scripts
    /// can be associated with loaded maps through the properties defined in the loaded .tmx file.
    /// </summary>
    public interface IMapScript
    {
        void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs mapEventArgs);

        void Update(TeeEngine engine, GameTime gameTime);

        void MapUnloaded(TeeEngine engine, TiledMap map);
    }
}
