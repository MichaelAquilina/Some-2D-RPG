using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Info;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Interfaces
{
    // Defines a common interface for collision data structures to adhere to. This allows quick swapping of
    // different collision data structures such as HashLists and QuadTrees.
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
