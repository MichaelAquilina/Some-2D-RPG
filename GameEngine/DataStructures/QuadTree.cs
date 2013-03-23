using System.Collections.Generic;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using System;

namespace GameEngine.DataStructures
{
    public class QuadTree
    {       
        public QuadTreeNode Root { get; private set; }
        public int EntityLimit { get; private set; }
        public int pxTileWidth { get; private set; }
        public int pxTileHeight { get; private set; }

        public bool UseNewUpdateAlgorithm = true;

        public uint LatestNodeIndex { get; private set; }

        public QuadTree(int txWidth, int txHeight, int pxTileWidth, int pxTileHeight, int EntityLimit=1)
        {
            Root = GetQuadTreeNode(0, 0, pxTileWidth * txWidth, pxTileHeight * txHeight, null);
            this.EntityLimit = EntityLimit;
            this.pxTileWidth = pxTileWidth;
            this.pxTileHeight = pxTileHeight;
            this.LatestNodeIndex = 0;
        }

        public void Update(Entity Entity)
        {
            if (UseNewUpdateAlgorithm)
            {
                //NEW UPDATE METHOD
                List<QuadTreeNode> updatedNodes = new List<QuadTreeNode>();
                List<QuadTreeNode> associatedNodes = new List<QuadTreeNode>();
                Root.GetAssociatedNodes(Entity, Entity.prevPxBoundingBox, ref associatedNodes);

                foreach (QuadTreeNode node in associatedNodes)
                {
                    bool update = true;

                    foreach (QuadTreeNode updatedNode in updatedNodes)
                        if (node.IsChildOf(updatedNode))
                            update = false;

                    if (update)
                    {
                        QuadTreeNode updatedNode = Reposition(Entity, node);
                        updatedNodes.Add(updatedNode);
                    }
                }
            }
            else
            {
                //OLD UPDATE METHOD
                Root.Remove(Entity, Entity.prevPxBoundingBox);
                Add(Entity);
            }
        }

        public void Remove(Entity Entity)
        {
            Root.Remove(Entity, null);
        }

        public void Add(Entity Entity)
        {
            Root.Add(Entity);
        }

        public void Rebuild(ICollection<Entity> Entities)
        {
            this.Root.Clear();
            this.LatestNodeIndex = 0;

            foreach (Entity entity in Entities)
                Add(entity);
        }

        /// <summary>
        /// TODO: May needs some revision
        /// Returns all intersecting Entities found in that region based on the
        /// CurrentPxBoundingBox property exposed by the Entity (which would have
        /// been last updated by the TeeEngine Update loop).
        /// </summary>
        /// <param name="pxRegion">Rectangle region to check in Pixels.</param>
        /// <returns>List of Entity objects intersecting the specified region.</returns>
        public List<Entity> GetIntersectingEntites(Rectangle pxRegion)
        {
            List<Entity> result = new List<Entity>();
            Root.GetEntities(pxRegion, ref result);

            return result;
        }

        //repsition the specified Entity in the given Node if it no longer contains it
        internal QuadTreeNode Reposition(Entity Entity, QuadTreeNode Node)
        {
            //if Node.Parent==null, then its the Root node and we have to do our best to add it
            if (Node.Parent != null && !Node.Contains(Entity.CurrentPxBoundingBox))
                return Reposition(Entity, Node.Parent);
            else
            {
                if (Node.Node1 != null)
                {
                    Node.Remove(Entity, Entity.prevPxBoundingBox);
                    Node.Add(Entity);
                }
                return Node;
            }
        }

        internal QuadTreeNode GetQuadTreeNode(int px, int py, int pxWidth, int pxHeight, QuadTreeNode Parent)
        {
            QuadTreeNode nodeResult = new QuadTreeNode();
            nodeResult.Clear();
            nodeResult.NodeID = LatestNodeIndex;
            nodeResult.pxBounds = new Rectangle(px, py, pxWidth, pxHeight);
            nodeResult.QuadTree = this;
            nodeResult.Parent = Parent;

            LatestNodeIndex++;

            return nodeResult;
        }

        public override string ToString()
        {
            return string.Format("QuadTree:pxWidth={1}, pxHeight={2}",
                Root.pxBounds.Width,
                Root.pxBounds.Height);
        }
    }
}
