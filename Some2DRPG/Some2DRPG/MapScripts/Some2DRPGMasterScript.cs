using GameEngine;
using GameEngine.GameObjects;
using GameEngine.Interfaces;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Some2DRPG.GameObjects;
using Some2DRPG.GameObjects.Characters;

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

        #region EventHandlers

        public void staticObject_UpdateEvent(Entity caller, GameTime gameTime, TeeEngine engine)
        {
            caller.Opacity = 1.0f;

            foreach (RPGEntity entity in engine.Collider.GetIntersectingEntites<RPGEntity>(caller.CurrentBoundingBox))
            {
                if (entity != caller
                    && caller.Pos.Y > entity.Pos.Y
                    && entity.CurrentBoundingBox.Intersects(caller.CurrentBoundingBox))
                {
                    caller.Opacity = 0.5f;
                    return;
                }
            }
        }

        #endregion
    }
}
