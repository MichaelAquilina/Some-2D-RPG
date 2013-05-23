using Microsoft.Xna.Framework;

namespace GameEngine.Pathfinding
{
    public class ANode
    {
        public const int DIAGONAL_COST = 14;
        public const int NORMAL_COST = 10;

        public Vector2 TxPos { get; set; }
        public ANode Parent { get; private set; }

        public int Length { get; internal set; }

        public ANode(Vector2 txPos, ANode parent)
        {
            this.TxPos = txPos;
            this.Parent = parent;

            SetParent(parent);
        }

        public void SetParent(ANode parent)
        {
            if (parent == null)
                Length = 0;
            else
                if (parent.TxPos.X != TxPos.X && parent.TxPos.Y != TxPos.Y)
                    Length = parent.Length + DIAGONAL_COST;
                else
                    Length = parent.Length + NORMAL_COST;
        }

        public override string ToString()
        {
            return string.Format("ANode: TxPos={0}, Parent={1}, Length={2}", TxPos, Parent.TxPos, Length);
        }
    }
}
