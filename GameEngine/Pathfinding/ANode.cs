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

        public void SetParent(ANode parent)
        {
            if (parent == null)
                Length = 0;
            else
            {
                parent.Child = this;

                if (parent.TxPos.X != TxPos.X && parent.TxPos.Y != TxPos.Y)
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
