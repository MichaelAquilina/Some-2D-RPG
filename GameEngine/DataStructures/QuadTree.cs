using System;
using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;

namespace GameEngine.DataStructures
{
    public class QuadTree
    {       
        public QuadTreeNode Root { get; private set; }
        public int EntityLimit { get; private set; }
        public int pxTileWidth { get; private set; }
        public int pxTileHeight { get; private set; }

        public uint LatestNodeIndex { get; private set; }

        public QuadTree(int txWidth, int txHeight, int pxTileWidth, int pxTileHeight, int EntityLimit=1)
        {
            Root = GetQuadTreeNode(0, 0, pxTileWidth * txWidth, pxTileHeight * txHeight, null);
            this.EntityLimit = EntityLimit;
            this.pxTileWidth = pxTileWidth;
            this.pxTileHeight = pxTileHeight;
            this.LatestNodeIndex = 0;
        }

        public void Update(Entity entity, bool addOnMissing=true)
        {
            List<QuadTreeNode> updatedNodes = new List<QuadTreeNode>();
            List<QuadTreeNode> associatedNodes = new List<QuadTreeNode>();
            Root.GetAssociatedNodes(entity, entity.prevBoundingBox, ref associatedNodes);

            if (associatedNodes.Count == 0 && addOnMissing)
                Add(entity);
            else
            {
                foreach (QuadTreeNode node in associatedNodes)
                {
                    bool update = true;

                    // If the one of the updated nodes already covers the current node
                    // then we dont need to bother repositioning this one.
                    foreach (QuadTreeNode updatedNode in updatedNodes)
                        if (updatedNode.Contains(node.pxBounds))
                            update = false;

                    if (update) updatedNodes.Add(Reposition(entity, node));
                }
            }
        }

        public void Remove(Entity entity)
        {
            Root.Remove(entity, null);
        }

        public void Add(Entity entity)
        {
            Root.Add(entity);
        }

        public void Rebuild(ICollection<Entity> entities)
        {
            this.Root.Reset();
            this.LatestNodeIndex = 0;

            foreach (Entity entity in entities)
                Add(entity);
        }

        public List<Entity> GetIntersectingEntites(FRectangle pxRegion)
        {
            List<Entity> result = new List<Entity>();
            Root.GetEntities(pxRegion, ref result);

            return result;
        }

        // Reposition the specified Entity in the given Node if it no longer contains it.
        internal QuadTreeNode Reposition(Entity entity, QuadTreeNode node)
        {
            // If Node.Parent==null, then its the Root node and we have to do our best to add it
            if (node.Parent != null && !node.Contains(entity.CurrentBoundingBox))
                return Reposition(entity, node.Parent);
            else
            {
                if (!node.IsLeafNode)
                {
                    node.Remove(entity, entity.prevBoundingBox);
                    node.Add(entity);
                }
                return node;
            }
        }

        internal QuadTreeNode GetQuadTreeNode(float x, float y, float width, float height, QuadTreeNode parentNode)
        {
            QuadTreeNode nodeResult = new QuadTreeNode();
            nodeResult.Reset();
            nodeResult.NodeID = LatestNodeIndex;
            nodeResult.pxBounds = new FRectangle(x, y, width, height);
            nodeResult.QuadTree = this;
            nodeResult.Parent = parentNode;

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
