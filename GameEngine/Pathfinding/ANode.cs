using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Pathfinding
{
    public class ANode
    {
        public const int DIAGONAL_COST = 14;
        public const int NORMAL_COST = 10;

        public Vector2 Pos { get; set; }
        public ANode Parent { get; private set; }

        public int Length { get; internal set; }

        public ANode(Vector2 pos, ANode parent)
        {
            this.Pos = pos;
            this.Parent = parent;

            SetParent(parent);
        }

        public void SetParent(ANode parent)
        {
            if (parent == null)
                Length = 0;
            else
                if (parent.Pos.X != Pos.X && parent.Pos.Y != Pos.Y)
                    Length = parent.Length + DIAGONAL_COST;
                else
                    Length = parent.Length + NORMAL_COST;
        }

        public override bool Equals(object obj)
        {
            if (obj is ANode)
                return ((ANode)obj).Pos == this.Pos;
            
            return false;
        }

        public override string ToString()
        {
            return string.Format("ANode: Pos={0}, Parent={1}, Length={2}", Pos, Parent.Pos, Length);
        }
    }
}
