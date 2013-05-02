using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameEngine.Pathfinding
{
    public class AStar
    {
        public static Path GeneratePath(ANode start, ANode target)
        {
            Path empty = new Path();

            List<ANode> openList = new List<ANode>();
            List<ANode> closedList = new List<ANode>();

            bool targetFound = false;
            ANode currentNode = start;

            // Setup the variables before we start the loop
            currentNode.Parent = null;
            openList.Add(currentNode);

            while (!targetFound)
            {
                // If there are no more nodes on the openlist list a path cannot be found
                if (openList.Count == 0)
                    return empty;

                float lowestF = 9999f;

                // Set current node to the node with the lowest F score
                foreach (ANode node in openList)
                {
                    if (node.F < lowestF)
                    {
                        lowestF = node.F;
                        currentNode = node;
                    }
                }

                // Check if the target is found
                if (target == currentNode)
                {
                    targetFound = true;

                    Path path = new Path();

                    while (currentNode.Parent != null)
                    {
                        path.Push(currentNode);
                        currentNode = currentNode.Parent;
                    }

                    return path;
                }

                // Move the current node from open to closed list
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (ANode neighbor in currentNode.Neighbors)
                {
                    if (neighbor.Passable)
                    {
                        if (!closedList.Contains(neighbor) && !openList.Contains(neighbor))
                        {
                            neighbor.Parent = currentNode;
                            neighbor.G = neighbor.Parent.G + Vector2.Distance(currentNode.Center, neighbor.Center);
                            neighbor.H = Vector2.Distance(currentNode.Center, target.Center);
                            neighbor.F = neighbor.G + neighbor.H;
                            openList.Add(neighbor);

                        }
                    }
                }
            }

            return empty;
        }
    }
}
