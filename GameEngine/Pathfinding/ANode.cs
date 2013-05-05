using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameEngine.Pathfinding
{
    public class ANode
    {
        // Parent node to this on the path
        public ANode Parent { get; set; }

        // Map index
        public int TX { get; set; }
        public int TY { get; set; }

        // Position of center
        public Vector2 Center { get; set; }

        public float F { get; set; }
        public float G { get; set; }
        public float H { get; set; }

        public List<ANode> Neighbors { get; set; }
        public bool Passable { get; set; }

        public ANode()
        {
            Construct(0, 0, Vector2.Zero, true);
        }

        public ANode(int tx, int ty, Vector2 center, bool passable = true)
        {
            Construct(tx, ty, center, passable);
        }

        private void Construct(int tx, int ty, Vector2 center, bool passable)
        {
            TX = tx;
            TY = ty;
            Center = center;
            Passable = passable;
            Neighbors = new List<ANode>();

            F = 0;
            G = 0;
            H = 0;
        }
    }
}
