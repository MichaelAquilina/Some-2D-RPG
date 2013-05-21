using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Pathfinding
{
    public class Path
    {
        public List<ANode> NodeList { get; internal set; }
        public Vector2 Start { get; internal set; }
        public Vector2 End { get; internal set; }

        public Path()
        {
            NodeList = new List<ANode>();
            Start = Vector2.Zero;
            End = Vector2.Zero;
        }

        public Path(ANode node, Vector2 start, Vector2 end)
        {
            this.NodeList = new List<ANode>();
            this.Start = start;
            this.End = end;

            ANode currNode = node;
            while (currNode != null)
            {
                NodeList.Add(currNode);
                currNode = currNode.Parent;
            }
        }
    }
}
