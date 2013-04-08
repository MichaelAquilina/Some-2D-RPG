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

        public void Update(Entity Entity, bool AddOnMissing=true)
        {
            List<QuadTreeNode> updatedNodes = new List<QuadTreeNode>();
            List<QuadTreeNode> associatedNodes = new List<QuadTreeNode>();
            Root.GetAssociatedNodes(Entity, Entity.prevBoundingBox, ref associatedNodes);

            if (associatedNodes.Count == 0 && AddOnMissing)
                Add(Entity);
            else
            {
                foreach (QuadTreeNode node in associatedNodes)
                {
                    bool update = true;

                    //if the one of the updated nodes already covers the current node
                    //then we dont need to bother repositioning this one
                    foreach (QuadTreeNode updatedNode in updatedNodes)
                        if (updatedNode.Contains(node.pxBounds))
                            update = false;

                    if (update) updatedNodes.Add(Reposition(Entity, node));
                }
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
            this.Root.Reset();
            this.LatestNodeIndex = 0;

            foreach (Entity entity in Entities)
                Add(entity);
        }

        public List<Entity> GetIntersectingEntites(FRectangle pxRegion)
        {
            List<Entity> result = new List<Entity>();
            Root.GetEntities(pxRegion, ref result);

            return result;
        }

        //reposition the specified Entity in the given Node if it no longer contains it
        internal QuadTreeNode Reposition(Entity Entity, QuadTreeNode Node)
        {
            //if Node.Parent==null, then its the Root node and we have to do our best to add it
            if (Node.Parent != null && !Node.Contains(Entity.CurrentBoundingBox))
                return Reposition(Entity, Node.Parent);
            else
            {
                if (!Node.IsLeafNode)
                {
                    Node.Remove(Entity, Entity.prevBoundingBox);
                    Node.Add(Entity);
                }
                return Node;
            }
        }

        internal QuadTreeNode GetQuadTreeNode(float px, float py, float pxWidth, float pxHeight, QuadTreeNode Parent)
        {
            QuadTreeNode nodeResult = new QuadTreeNode();
            nodeResult.Reset();
            nodeResult.NodeID = LatestNodeIndex;
            nodeResult.pxBounds = new FRectangle(px, py, pxWidth, pxHeight);
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
