using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Drawing;
using GameEngine.GameObjects;

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
    }
}
