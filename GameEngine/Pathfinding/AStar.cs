using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameEngine.Tiled;

namespace GameEngine.Pathfinding
{
    public class AStar
    {
        /// <summary>
        /// The ANode's that make up the pathfinding network, each ANode is linked to others by its Neighbors list
        /// </summary>
        public ANode[,] Nodes { get; set; }

        public AStar()
        {
        }

        public AStar(TiledMap map)
        {
            Initialize(map);
        }

        /// <summary>
        /// Initialize builds the array of ANodes and their list of neighbors, must be called before a path is generated
        /// </summary>
        /// <param name="map">TiledMap that you wish to pathfind on</param>
        public void Initialize(TiledMap map)
        {
            // SETUP PATHFINDING NODES
            Nodes = new ANode[map.txWidth, map.txHeight];
            
            for (int y = 0; y < map.txHeight; y++)
            {
                for (int x = 0; x < map.txWidth; x++)
                {
                    ANode node = new ANode();
                    node.TX = x;
                    node.TY = y;
                    node.Center = new Vector2(map.TileWidth * x + map.TileWidth / 2, map.TileHeight * y + map.TileHeight / 2);
                    
                    Nodes[x, y] = node;
                }
            }

            // Build the neighbors
            RebuildNeighbors(map);
        }

        /// <summary>
        /// Rebuilds the List of neighbors for each node. Useful in case the map changes after it was originally loaded.
        /// </summary>
        /// <param name="map">TiledMap that you wish to pathfind on</param>
        public void RebuildNeighbors(TiledMap map)
        {
            // Remove any existing neighbors on each node
            for (int y = 0; y < map.txHeight; y++)
                for (int x = 0; x < map.txWidth; x++)
                    Nodes[x, y].Neighbors.Clear();


            // Setup new neighbors
            for (int y = 0; y < map.txHeight; y++)
            {
                for (int x = 0; x < map.txWidth; x++)
                {
                    Tile tile = map.GetTxTopMostTile(x, y);
                    Tile neighbor;

                    // If the tile at this location is impassable don't add any neighbors
                    if (tile == null || tile.HasProperty("Impassable"))
                        continue;

                    // Check each entry point to build a list of neighbors
                    List<string> entryPoints = new List<string>(tile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    bool left, right, up, down = false;

                    left = entryPoints.Contains("Left");
                    right = entryPoints.Contains("Right");
                    up = entryPoints.Contains("Top");
                    down = entryPoints.Contains("Bottom");

                    // Ortho neighbors
                    if (left)
                        if (ValidPos(map, x - 1, y))
                        {
                            // Check to see if the neighbor is a valid link
                            neighbor = map.GetTxTopMostTile(x - 1, y);

                            if (!neighbor.HasProperty("Impassable") && HasEntry(neighbor, "Right"))
                                Nodes[x, y].Neighbors.Add(Nodes[x - 1, y]);
                            else
                                left = false; // left is not a valid neighbor, this rules out some diagonals
                        }
                    if (right)
                        if (ValidPos(map, x + 1, y))
                        {
                            neighbor = map.GetTxTopMostTile(x + 1, y);

                            if (!neighbor.HasProperty("Impassable") && HasEntry(neighbor, "Left"))
                                Nodes[x, y].Neighbors.Add(Nodes[x + 1, y]);
                            else
                                right = false;
                        }
                    if (up)
                        if (ValidPos(map, x, y - 1))
                        {
                            neighbor = map.GetTxTopMostTile(x, y - 1);

                            if (!neighbor.HasProperty("Impassable") && HasEntry(neighbor, "Bottom"))
                                Nodes[x, y].Neighbors.Add(Nodes[x, y - 1]);
                            else
                                up = false;
                        }
                    if (down)
                        if (ValidPos(map, x, y + 1))
                        {
                            neighbor = map.GetTxTopMostTile(x, y + 1);

                            if (!neighbor.HasProperty("Impassable") && HasEntry(neighbor, "Top"))
                                Nodes[x, y].Neighbors.Add(Nodes[x, y + 1]);
                            else
                                down = false;
                        }

                    // Diagonal neighbors
                    if (left && up)
                        if (ValidPos(map, x - 1, y - 1))
                        {
                            neighbor = map.GetTxTopMostTile(x - 1, y - 1);

                            if (!neighbor.HasProperty("Impassable") && HasEntry(neighbor, "Bottom") && HasEntry(neighbor, "Right"))
                                Nodes[x, y].Neighbors.Add(Nodes[x - 1, y - 1]);
                        }
                    if (right && up)
                        if (ValidPos(map, x + 1, y - 1))
                        {
                            neighbor = map.GetTxTopMostTile(x + 1, y - 1);

                            if (!neighbor.HasProperty("Impassable") && HasEntry(neighbor, "Bottom") && HasEntry(neighbor, "Left"))
                                Nodes[x, y].Neighbors.Add(Nodes[x + 1, y - 1]);
                        }
                    if (right && down)
                        if (ValidPos(map, x + 1, y + 1))
                        {
                            neighbor = map.GetTxTopMostTile(x + 1, y + 1);

                            if (!neighbor.HasProperty("Impassable") && HasEntry(neighbor, "Top") && HasEntry(neighbor, "Left"))
                                Nodes[x, y].Neighbors.Add(Nodes[x + 1, y + 1]);
                        }
                    if (left && down)
                        if (ValidPos(map, x - 1, y + 1))
                        {
                            neighbor = map.GetTxTopMostTile(x - 1, y + 1);

                            if (!neighbor.HasProperty("Impassable") && HasEntry(neighbor, "Top") && HasEntry(neighbor, "Right"))
                                Nodes[x, y].Neighbors.Add(Nodes[x - 1, y + 1]);
                        }
                }
            }
        }

        /// <summary>
        /// Generates a Path from the start node to the target node using the AStar algorithm.
        /// </summary>
        /// <param name="start">ANode to start the path from.</param>
        /// <param name="target">ANode to end the path at.</param>
        /// <returns>Returns Path (stack<ANode>) that leads from the start to the target. Returns an empty Path if no path can be found.</returns>
        public Path GeneratePath(ANode start, ANode target)
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

        private bool ValidPos(TiledMap map, int x, int y)
        {
            return (x > 0 && x < map.txWidth && y > 0 && y < map.txHeight);
        }

        private bool HasEntry(Tile tile, string entry)
        {
            return new List<string>(tile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Contains(entry);
        }
    }
}
