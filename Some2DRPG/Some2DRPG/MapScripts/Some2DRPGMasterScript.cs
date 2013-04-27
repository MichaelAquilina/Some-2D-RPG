using GameEngine;
using GameEngine.Interfaces;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Some2DRPG.GameObjects;

namespace Some2DRPG.MapScripts
{
    // All functionality that is common between maps should be placed here.
    public class Some2DRPGMasterScript : IMapScript
    {
        public virtual void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs mapEventArgs)
        {
            if (mapEventArgs.HasProperty("Target"))
            {
                Hero player = new Hero();
                MapEntrance targetEntrance = (MapEntrance)engine.GetEntity(mapEventArgs.GetProperty("Target"));

                player.Pos = targetEntrance.Pos + new Vector2(targetEntrance.Width, targetEntrance.Height) / 2;

                engine.AddEntity("Player", player);
            }
        }

        public virtual void Update(TeeEngine engine, GameTime gameTime)
        {
        }

        public virtual void MapUnloaded(TeeEngine engine, TiledMap map)
        {
        }
    }
}
