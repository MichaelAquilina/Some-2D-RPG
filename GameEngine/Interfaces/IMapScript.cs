using GameEngine.Tiled;
using Microsoft.Xna.Framework;

namespace GameEngine.Interfaces
{
    public interface IMapScript
    {
        void MapLoaded(TeeEngine engine, TiledMap map);

        void Update(TeeEngine engine, GameTime gameTime);

        void MapUnloaded(TeeEngine engine, TiledMap map);
    }
}
