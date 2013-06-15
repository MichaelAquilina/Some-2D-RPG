using System;
using Microsoft.Xna.Framework;

namespace GameEngine.Pathfinding
{
    public class ANode
    {
        public const int DIAGONAL_COST = 14;
        public const int NORMAL_COST = 10;

        public Vector2 TxPos { get; set; }
        public ANode Parent { get; internal set; }
        public ANode Child { get; internal set; }

        public int Length { get; internal set; }

        public ANode(Vector2 txPos, ANode parent=null)
        {
            this.TxPos = txPos;
            this.Parent = parent;
            this.Child = null;

            SetParent(parent);
        }

        /// <summary>
        /// Returns a boolean value specifying if the current node is a diagonal
        /// neighbour to the node specified in the parameter.
        /// </summary>
        public bool IsDiagonalNeighbor(ANode node)
        {
            Vector2 diff = TxPos - node.TxPos;

            return Math.Abs(diff.X) == 1 && Math.Abs(diff.Y) == 1;
        }
        
        /// <summary>
        /// Sets the specified node as the current nodes Parent. The method will
        /// automatically recalculate the total Length of the current node as well
        /// as update the childe reference for the parent being set.
        /// </summary>
        public void SetParent(ANode parent)
        {
            if (parent == null)
                Length = 0;
            else
            {
                parent.Child = this;

                if (IsDiagonalNeighbor(Parent))
                    Length = parent.Length + DIAGONAL_COST;
                else
                    Length = parent.Length + NORMAL_COST;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "ANode: TxPos={0}, Parent={1}, Child={2}, Length={3}", 
                TxPos, 
                Parent==null? null : Parent.TxPos.ToString(), 
                Child==null? null: Child.TxPos.ToString(),
                Length
                );
        }
    }
}
