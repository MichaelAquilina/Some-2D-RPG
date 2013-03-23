using System;
using System.Collections.Generic;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;

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

        //no need for arrays!
        public QuadTreeNode Node1 { get; internal set; }
        public QuadTreeNode Node2 { get; internal set; }
        public QuadTreeNode Node3 { get; internal set; }
        public QuadTreeNode Node4 { get; internal set; }

        public QuadTree QuadTree { get; internal set; }
        public List<Entity> Entities { get; internal set; }

        public Rectangle pxBounds { get; internal set; }
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

        public bool HasEntity(Entity Entity)
        {
            if (Node1 == null)
                return Entities.Contains(Entity);
            else
            {
                if (Node1.HasEntity(Entity)) return true;
                if (Node2.HasEntity(Entity)) return true;
                if (Node3.HasEntity(Entity)) return true;
                if (Node4.HasEntity(Entity)) return true;
            }

            return false;
        }

        public bool HasNode(QuadTreeNode Node)
        {
            if (this == Node)
                return true;

            if (Node1 != null)
            {
                if (Node1.HasNode(Node)) return true;
                if (Node2.HasNode(Node)) return true;
                if (Node3.HasNode(Node)) return true;
                if (Node4.HasNode(Node)) return true;
            }
            
            return false;
        }

        public bool Contains(Rectangle pxBoundingBox)
        {
            return pxBounds.Contains(pxBoundingBox);
        }

        public bool Intersects(Rectangle pxBoundingBox)
        {
            return pxBounds.Intersects(pxBoundingBox);
        }

        public void Remove(Entity Entity, Rectangle? pxBoundingBox)
        {
            List<QuadTreeNode> associations = new List<QuadTreeNode>();
            GetAssociatedNodes(Entity, pxBoundingBox, ref associations);

            foreach (QuadTreeNode node in associations)
            {
                node.Entities.Remove(Entity);
                node.Validate(this);
            }
        }

        public void Add(Entity Entity)
        {
            int pxHalfWidth = (int)Math.Ceiling(pxBounds.Width / 2.0f);
            int pxHalfHeight = (int)Math.Ceiling(pxBounds.Height / 2.0f);

            if ((Node1 == null && Entities.Count < QuadTree.EntityLimit) || pxHalfWidth <= QuadTree.pxTileWidth || pxHalfHeight <= QuadTree.pxTileHeight)
            {
                Entities.Add(Entity);
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
                    pxBounds.Width - pxHalfWidth,
                    pxHalfHeight,
                    this);
                Node3 = QuadTree.GetQuadTreeNode(
                    pxBounds.X,
                    pxBounds.Y + pxHalfHeight,
                    pxHalfWidth,
                    pxBounds.Height - pxHalfHeight,
                    this);
                Node4 = QuadTree.GetQuadTreeNode(
                    pxBounds.X + pxHalfWidth,
                    pxBounds.Y + pxHalfHeight,
                    pxBounds.Width - pxHalfWidth,
                    pxBounds.Height - pxHalfHeight,
                    this);
            }

            //add the entity specified in the function paramaters
            if (Node1.Intersects(Entity.CurrentPxBoundingBox)) Node1.Add(Entity);
            if (Node2.Intersects(Entity.CurrentPxBoundingBox)) Node2.Add(Entity);
            if (Node3.Intersects(Entity.CurrentPxBoundingBox)) Node3.Add(Entity);
            if (Node4.Intersects(Entity.CurrentPxBoundingBox)) Node4.Add(Entity);

            //distrobute any entities found in this current Node
            foreach (Entity entityToAdd in this.Entities)
            {
                if (Node1.Intersects(entityToAdd.CurrentPxBoundingBox)) Node1.Add(entityToAdd);
                if (Node2.Intersects(entityToAdd.CurrentPxBoundingBox)) Node2.Add(entityToAdd);
                if (Node3.Intersects(entityToAdd.CurrentPxBoundingBox)) Node3.Add(entityToAdd);
                if (Node4.Intersects(entityToAdd.CurrentPxBoundingBox)) Node4.Add(entityToAdd);
            }

            Entities.Clear();
        }

        //Validates this nodes MaxEntity/MinEntity requirements
        internal void Validate(QuadTreeNode NodeLimit=null)
        {
            if (this == NodeLimit) return;

            List<Entity> entities = new List<Entity>();
            GetEntities(null, ref entities);

            if (entities.Count <= QuadTree.EntityLimit)
            {
                Node1 = null;
                Node2 = null;
                Node3 = null;
                Node4 = null;
                Entities = entities;

                if (Parent!=NodeLimit)
                    Parent.Validate();
            }
        }

        //if pxBoundingBox is null, it will search everywhere
        public void GetAssociatedNodes(Entity Entity, Rectangle? pxBoundingBox, ref List<QuadTreeNode> Result)
        {
            if (Node1 == null)
            {
                if (Entities.Contains(Entity)) Result.Add(this);
                return;
            }

            if (pxBoundingBox == null || Node1.Intersects(pxBoundingBox.Value)) Node1.GetAssociatedNodes(Entity, pxBoundingBox, ref Result);
            if (pxBoundingBox == null || Node2.Intersects(pxBoundingBox.Value)) Node2.GetAssociatedNodes(Entity, pxBoundingBox, ref Result);
            if (pxBoundingBox == null || Node3.Intersects(pxBoundingBox.Value)) Node3.GetAssociatedNodes(Entity, pxBoundingBox, ref Result);
            if (pxBoundingBox == null || Node4.Intersects(pxBoundingBox.Value)) Node4.GetAssociatedNodes(Entity, pxBoundingBox, ref Result);
        }

        internal void GetEntities(Rectangle? pxRegion, ref List<Entity> Result)
        {
            if (Entities.Count > 0)
            {
                foreach (Entity entity in Entities)
                    if (!Result.Contains(entity))
                        Result.Add(entity);
            }
            else
            {
                if (IsLeafNode) return;

                if (pxRegion == null || Node1.Intersects(pxRegion.Value)) Node1.GetEntities(pxRegion, ref Result);
                if (pxRegion == null || Node2.Intersects(pxRegion.Value)) Node2.GetEntities(pxRegion, ref Result);
                if (pxRegion == null || Node3.Intersects(pxRegion.Value)) Node3.GetEntities(pxRegion, ref Result);
                if (pxRegion == null || Node4.Intersects(pxRegion.Value)) Node4.GetEntities(pxRegion, ref Result);
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
