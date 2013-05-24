using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameEngine.Pathfinding
{
    public class AStar
    {
        private Dictionary<Vector2, ANode> _openList = new Dictionary<Vector2, ANode>();
        private Dictionary<Vector2, ANode> _closedList = new Dictionary<Vector2, ANode>();

        public delegate bool NodeValidationHandler(Vector2 txPos, TeeEngine engine, GameTime gameTime);

        /// <summary>
        /// Uses the AStar algorithm to generate a Path from pxStart to pxEnd of valid ANodes to pass through.
        /// What is considered to be a valid ANode is determined by the current delegate method assigned to
        /// the instance's Validator property. The method will return an empty path if the location is impossible
        /// to reach from the specified start location.
        /// </summary>
        /// <returns>Path object containing a path to the specified end location.</returns>
        public Path GeneratePath(Vector2 pxStart, Vector2 pxEnd, TeeEngine engine, GameTime gameTime, NodeValidationHandler validator)
        {
            // Prevent from creating a dictionary each time this method is called.
            _openList.Clear();
            _closedList.Clear();

            Vector2 TXEND = engine.Map.PxToTx(pxEnd);
            Vector2 TXSTART = engine.Map.PxToTx(pxStart);

            if(validator == null || !validator(TXEND, engine, gameTime)) return new Path();
            if(validator == null || !validator(TXSTART, engine, gameTime)) return new Path();

            // Working backwards allows us to follow the parent path.
            _openList.Add(TXEND, new ANode(TXEND, null));

            while (_openList.Count > 0)
            {
                int min = Int32.MaxValue;
                ANode selectedNode = null;

                // Select the most promising node from the open list.
                foreach (ANode node in _openList.Values)
                {
                    int G = node.Length;
                    int H = (int) Math.Ceiling(Vector2.Distance(node.TxPos, TXSTART)) * 10;
                    int length = G + H;
                    if (length < min)
                    {
                        min = length;
                        selectedNode = node;
                    }
                }

                _openList.Remove(selectedNode.TxPos);
                _closedList.Add(selectedNode.TxPos, selectedNode);

                if (selectedNode.TxPos == TXSTART)
                    return new Path(selectedNode, pxStart, pxEnd);

                // Iterate through the node's neighbors.
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (i == 0 && j == 0) continue;

                        ANode node = new ANode(selectedNode.TxPos + new Vector2(i, j), selectedNode);

                        // If a validator has been supplied, use it to check if the current node is valid.
                        if ((validator == null || validator(node.TxPos, engine, gameTime)) && !_closedList.ContainsKey(node.TxPos))
                        {
                            if (_openList.ContainsKey(node.TxPos))
                            {
                                if (_openList[node.TxPos].Length > node.Length)
                                    _openList[node.TxPos].SetParent(node.Parent);
                            }
                            else
                                _openList.Add(node.TxPos, node);
                        }
                    }
                }
            }

            return new Path();
        }
    }
}
