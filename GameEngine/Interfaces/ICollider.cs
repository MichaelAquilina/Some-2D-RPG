using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Info;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Interfaces
{
    /// <summary>
    /// Defines an interface for a class that is capable of performing quick collision/intersection
    /// checks through the use of some data structure. Any custom data structures that wish to be used
    /// within the engine for collision checks need to inherit from this interface.
    /// </summary>
    public interface ICollider
    {
        void Construct(int txWidth, int txHeight, int pxTileWidth, int pxTileHeight);

        void Update(Entity entity);

        void Remove(Entity entity);

        void Add(Entity entity);

        List<Entity> GetIntersectingEntites(FRectangle pxRegion);

        // TODO: This signature can be simplified.
        void DrawDebugInfo(ViewPortInfo viewPort,
            SpriteBatch spriteBatch,
            Rectangle destRectangle,
            SpriteFont spriteFont,
            float globalDispX, float globalDispY);
    }
}
