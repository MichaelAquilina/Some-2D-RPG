using System;
using System.Collections.Generic;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;

namespace GameEngine.DataStructures
{
    /// <summary>
    /// Some Performance Notes:
    /// -Avoid Float point math operations where possible since these are more expensive than ints
    /// -Avoid creating new object instances when possible because creating a new class is a very costly process
    /// </summary>
    public class QuadTreeNode
    {
        public QuadTreeNode Parent { get; internal set; }

        // No need for arrays!
        public QuadTreeNode Node1 { get; internal set; }
        public QuadTreeNode Node2 { get; internal set; }
        public QuadTreeNode Node3 { get; internal set; }
        public QuadTreeNode Node4 { get; internal set; }

        public QuadTree QuadTree { get; internal set; }
        public List<Entity> Entities { get; internal set; }

        public FRectangle pxBounds { get; internal set; }
        public uint NodeID { get; set; }

        public bool IsLeafNode { get { return Node1 == null; } }

        public QuadTreeNode()
        {
            Entities = new List<Entity>();
            Reset();
        }

        public void Reset()
        {
            Entities.Clear();
            Node1 = null;
            Node2 = null;
            Node3 = null;
            Node4 = null;
            Parent = null;
        }

        public bool DebugHasEntity(Entity entity)
        {
            if (IsLeafNode)
                return Entities.Contains(entity);
            else
            {
                if (Node1.DebugHasEntity(entity)) return true;
                if (Node2.DebugHasEntity(entity)) return true;
                if (Node3.DebugHasEntity(entity)) return true;
                if (Node4.DebugHasEntity(entity)) return true;
            }

            return false;
        }

        public bool DebugHasNode(QuadTreeNode node)
        {
            if (this == node)
                return true;

            if (!IsLeafNode)
            {
                if (Node1.DebugHasNode(node)) return true;
                if (Node2.DebugHasNode(node)) return true;
                if (Node3.DebugHasNode(node)) return true;
                if (Node4.DebugHasNode(node)) return true;
            }
            
            return false;
        }

        public bool Contains(FRectangle pxBoundingBox)
        {
            return pxBounds.Contains(pxBoundingBox);
        }

        public bool Intersects(FRectangle pxBoundingBox)
        {
            return pxBounds.Intersects(pxBoundingBox);
        }

        public void Remove(Entity Entity, FRectangle? pxBoundingBox)
        {
            List<QuadTreeNode> associations = new List<QuadTreeNode>();
            GetAssociatedNodes(Entity, pxBoundingBox, ref associations);

            foreach (QuadTreeNode node in associations)
            {
                node.Entities.Remove(Entity);
                node.Validate(this);
            }
        }

        public void Add(Entity entity)
        {
            float pxHalfWidth = pxBounds.Width / 2.0f;
            float pxHalfHeight = pxBounds.Height / 2.0f;

            if ((IsLeafNode && Entities.Count < QuadTree.EntityLimit) || pxHalfWidth <= QuadTree.pxTileWidth || pxHalfHeight <= QuadTree.pxTileHeight)
            {
                if(!Entities.Contains(entity)) Entities.Add(entity);
                return;
            }

            if (IsLeafNode)
            {
                Node1 = QuadTree.GetQuadTreeNode(
                    pxBounds.X,
                    pxBounds.Y,
                    pxHalfWidth,
                    pxHalfHeight,
                    this);
                Node2 = QuadTree.GetQuadTreeNode(
                    pxBounds.X + pxHalfWidth,
                    pxBounds.Y,
                    pxHalfWidth,
                    pxHalfHeight,
                    this);
                Node3 = QuadTree.GetQuadTreeNode(
                    pxBounds.X,
                    pxBounds.Y + pxHalfHeight,
                    pxHalfWidth,
                    pxHalfHeight,
                    this);
                Node4 = QuadTree.GetQuadTreeNode(
                    pxBounds.X + pxHalfWidth,
                    pxBounds.Y + pxHalfHeight,
                    pxHalfWidth,
                    pxHalfHeight,
                    this);
            }

            // Add the entity specified in the function paramaters
            if (Node1.Intersects(entity.CurrentBoundingBox)) Node1.Add(entity);
            if (Node2.Intersects(entity.CurrentBoundingBox)) Node2.Add(entity);
            if (Node3.Intersects(entity.CurrentBoundingBox)) Node3.Add(entity);
            if (Node4.Intersects(entity.CurrentBoundingBox)) Node4.Add(entity);

            // Distrobute any entities found in this current Node
            foreach (Entity entityToAdd in this.Entities)
            {
                if (Node1.Intersects(entityToAdd.CurrentBoundingBox)) Node1.Add(entityToAdd);
                if (Node2.Intersects(entityToAdd.CurrentBoundingBox)) Node2.Add(entityToAdd);
                if (Node3.Intersects(entityToAdd.CurrentBoundingBox)) Node3.Add(entityToAdd);
                if (Node4.Intersects(entityToAdd.CurrentBoundingBox)) Node4.Add(entityToAdd);
            }

            Entities.Clear();
        }

        // Validates this nodes MaxEntity/MinEntity requirements
        internal void Validate(QuadTreeNode nodeLimit=null)
        {
            if (this == nodeLimit) return;

            List<Entity> entities = new List<Entity>();
            GetEntities(null, ref entities);

            if (entities.Count <= QuadTree.EntityLimit)
            {
                Node1 = null;
                Node2 = null;
                Node3 = null;
                Node4 = null;
                Entities = entities;

                if (Parent!=nodeLimit)
                    Parent.Validate();
            }
        }

        // If pxBoundingBox is null, it will search everywhere
        public void GetAssociatedNodes(Entity entity, FRectangle? pxBoundingBox, ref List<QuadTreeNode> result)
        {
            if (IsLeafNode)
            {
                if (Entities.Contains(entity)) result.Add(this);
                return;
            }

            if (pxBoundingBox == null || Node1.Intersects(pxBoundingBox.Value)) Node1.GetAssociatedNodes(entity, pxBoundingBox, ref result);
            if (pxBoundingBox == null || Node2.Intersects(pxBoundingBox.Value)) Node2.GetAssociatedNodes(entity, pxBoundingBox, ref result);
            if (pxBoundingBox == null || Node3.Intersects(pxBoundingBox.Value)) Node3.GetAssociatedNodes(entity, pxBoundingBox, ref result);
            if (pxBoundingBox == null || Node4.Intersects(pxBoundingBox.Value)) Node4.GetAssociatedNodes(entity, pxBoundingBox, ref result);
        }

        internal void GetEntities(FRectangle? pxRegion, ref List<Entity> result)
        {
            if (Entities.Count > 0)
            {
                foreach (Entity entity in Entities)
                    if (!result.Contains(entity))
                        result.Add(entity);
            }
            else
            {
                if (IsLeafNode) return;

                if (pxRegion == null || Node1.Intersects(pxRegion.Value)) Node1.GetEntities(pxRegion, ref result);
                if (pxRegion == null || Node2.Intersects(pxRegion.Value)) Node2.GetEntities(pxRegion, ref result);
                if (pxRegion == null || Node3.Intersects(pxRegion.Value)) Node3.GetEntities(pxRegion, ref result);
                if (pxRegion == null || Node4.Intersects(pxRegion.Value)) Node4.GetEntities(pxRegion, ref result);
            }
        }

        public override string ToString()
        {
            return string.Format("QuadTreeNode: NodeId={0}; ParentNodeId={1}, (PX,PY)=({2},{3}), pxWidth={4}, pxHeight={5}, IsLeafNode={6}, Entities={7}",
                NodeID, Parent.NodeID,
                pxBounds.X, pxBounds.Y, 
                pxBounds.Width, pxBounds.Height,
                IsLeafNode, Entities.Count);
        }
    }
}
