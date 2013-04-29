using System;
using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Info;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.DataStructures
{
    public class QuadTree : ICollider
    {       
        public QuadTreeNode Root { get; private set; }
        public int EntityLimit { get; private set; }
        public int pxTileWidth { get; private set; }
        public int pxTileHeight { get; private set; }

        public uint LatestNodeIndex { get; private set; }

        public QuadTree(int entityLimit=1)
        {
            this.EntityLimit = entityLimit;
        }

        public void Construct(int txWidth, int txHeight, int pxTileWidth, int pxTileHeight)
        {
            Root = GetQuadTreeNode(0, 0, pxTileWidth * txWidth, pxTileHeight * txHeight, null);
            this.pxTileWidth = pxTileWidth;
            this.pxTileHeight = pxTileHeight;
            this.LatestNodeIndex = 0;
        }

        public void Update(Entity entity)
        {
            List<QuadTreeNode> updatedNodes = new List<QuadTreeNode>();
            List<QuadTreeNode> associatedNodes = new List<QuadTreeNode>();
            Root.GetAssociatedNodes(entity, entity.PreviousBoundingBox, ref associatedNodes);

            // Add if Missing.
            if (associatedNodes.Count == 0)
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

        public void DrawDebugInfo(
            ViewPortInfo viewPort,
            SpriteBatch spriteBatch,
            Rectangle destRectangle,
            SpriteFont spriteFont,
            float globalDispX, float globalDispY)
        {
            DrawQuadTreeNode(viewPort, spriteBatch, Root, destRectangle, spriteFont, globalDispX, globalDispY);
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
                    node.Remove(entity, entity.PreviousBoundingBox);
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

        internal void DrawQuadTreeNode(ViewPortInfo viewPort,
            SpriteBatch spriteBatch,
            QuadTreeNode node,
            Rectangle destRectangle,
            SpriteFont spriteFont,
            float globalDispX, float globalDispY)
        {
            if (node == null) return;

            int actualX = (int)Math.Ceiling(node.pxBounds.X * viewPort.ActualZoom - globalDispX);
            int actualY = (int)Math.Ceiling(node.pxBounds.Y * viewPort.ActualZoom - globalDispY);

            // We need to calculate the 'Actual' width and height otherwise drawing might be innacurate when zoomed.
            int actualWidth = (int)Math.Ceiling(node.pxBounds.Width * viewPort.ActualZoom);
            int actualHeight = (int)Math.Ceiling(node.pxBounds.Height * viewPort.ActualZoom);

            // Only draw leaf nodes which are within the viewport specified.
            if (node.ChildNode1 == null
                && new Rectangle(actualX, actualY, actualWidth, actualHeight).Intersects(destRectangle))
            {
                string nodeText = string.Format("{0}\n\r{1}", node.NodeID, node.Entities.Count);

                SpriteBatchExtensions.DrawRectangle(spriteBatch, new Rectangle(actualX, actualY, actualWidth, actualHeight), Color.Lime, 0);
                spriteBatch.DrawString(
                    spriteFont,
                    nodeText,
                    new Vector2(actualX + actualWidth / 2.0f, actualY + actualHeight / 2.0f) - spriteFont.MeasureString(nodeText) / 2,
                    Color.Lime
                );
            }

            DrawQuadTreeNode(viewPort, spriteBatch, node.ChildNode1, destRectangle, spriteFont, globalDispX, globalDispY);
            DrawQuadTreeNode(viewPort, spriteBatch, node.ChildNode2, destRectangle, spriteFont, globalDispX, globalDispY);
            DrawQuadTreeNode(viewPort, spriteBatch, node.ChildNode3, destRectangle, spriteFont, globalDispX, globalDispY);
            DrawQuadTreeNode(viewPort, spriteBatch, node.ChildNode4, destRectangle, spriteFont, globalDispX, globalDispY);
        }

        public override string ToString()
        {
            return string.Format("QuadTree: pxWidth={1}, pxHeight={2}",
                Root.pxBounds.Width,
                Root.pxBounds.Height);
        }
    }
}
