using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameEngine.Pathfinding
{
    public class Path
    {
        public List<ANode> NodeList { get; internal set; }
        public Vector2 PxStart { get; internal set; }
        public Vector2 PxEnd { get; internal set; }

        public Path()
        {
            NodeList = new List<ANode>();
            PxStart = Vector2.Zero;
            PxEnd = Vector2.Zero;
        }

        public Path(ANode node, Vector2 pxStart, Vector2 pxEnd)
        {
            this.NodeList = new List<ANode>();
            this.PxStart = pxStart;
            this.PxEnd = pxEnd;

            ANode currNode = node;
            while (currNode != null)
            {
                NodeList.Add(currNode);
                currNode = currNode.Parent;
            }
        }
    }
}
