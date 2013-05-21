using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameEngine.Tiled;

namespace GameEngine.Pathfinding
{
    // TODO: Allow setting of IsValid from external Games.
    public class AStar
    {
        public Path GeneratePath(Vector2 pxStart, Vector2 pxEnd, TeeEngine engine)
        {
            Dictionary<Vector2, ANode> openList = new Dictionary<Vector2, ANode>();
            Dictionary<Vector2, ANode> closedList = new Dictionary<Vector2, ANode>();

            Vector2 TXEND = engine.Map.PxToTx(pxEnd);
            Vector2 TXSTART = engine.Map.PxToTx(pxStart);

            if(!IsValid(engine.Map.GetTxTopMostTile((int)TXEND.X, (int)TXEND.Y))) return new Path();
            if(!IsValid(engine.Map.GetTxTopMostTile((int)TXSTART.X, (int)TXSTART.Y))) return new Path();

            openList.Add(TXEND, new ANode(TXEND, null));

            while (openList.Count > 0)
            {
                int min = Int32.MaxValue;
                ANode selectedNode = null;

                // Select the most promising node from the open list.
                foreach (ANode node in openList.Values)
                {
                    int G = node.Length;
                    int H = (int) Math.Ceiling(Vector2.Distance(node.Pos, TXSTART)) * 10;
                    int length = G + H;
                    if (length < min)
                    {
                        min = length;
                        selectedNode = node;
                    }
                }

                openList.Remove(selectedNode.Pos);
                closedList.Add(selectedNode.Pos, selectedNode);

                if (selectedNode.Pos == TXSTART)
                    return new Path(selectedNode, pxStart, pxEnd);

                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (i == 0 && j == 0) continue;

                        ANode node = new ANode(selectedNode.Pos + new Vector2(i, j), selectedNode);
                        Tile tile = engine.Map.GetTxTopMostTile((int)node.Pos.X, (int)node.Pos.Y);

                        if (IsValid(tile) && !closedList.ContainsKey(node.Pos))
                        {
                            if (openList.ContainsKey(node.Pos))
                            {
                                if (openList[node.Pos].Length > node.Length)
                                    openList[node.Pos].SetParent(node.Parent);
                            }
                            else
                                openList.Add(node.Pos, node);
                        }
                    }
                }
            }

            return null;
        }

        // TODO: Checking if a tile is impassble should be placed in the user space.
        private bool IsValid(Tile tile)
        {
            return (tile != null && !tile.HasProperty("Impassable"));
        }
    }
}
