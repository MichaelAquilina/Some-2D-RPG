using System.Collections.Generic;
using GameEngine.Drawing;
using GameEngine.GameObjects;

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
        public QuadTreeNode ChildNode1 { get; internal set; }
        public QuadTreeNode ChildNode2 { get; internal set; }
        public QuadTreeNode ChildNode3 { get; internal set; }
        public QuadTreeNode ChildNode4 { get; internal set; }

        public QuadTree QuadTree { get; internal set; }
        public List<Entity> Entities { get; internal set; }

        public FRectangle pxBounds { get; internal set; }
        public uint NodeID { get; set; }

        public bool IsLeafNode { get { return ChildNode1 == null; } }

        public QuadTreeNode()
        {
            Entities = new List<Entity>();
            Reset();
        }

        public void Reset()
        {
            Entities.Clear();
            ChildNode1 = null;
            ChildNode2 = null;
            ChildNode3 = null;
            ChildNode4 = null;
            Parent = null;
        }

        public bool DebugHasEntity(Entity entity)
        {
            if (IsLeafNode)
                return Entities.Contains(entity);
            else
            {
                if (ChildNode1.DebugHasEntity(entity)) return true;
                if (ChildNode2.DebugHasEntity(entity)) return true;
                if (ChildNode3.DebugHasEntity(entity)) return true;
                if (ChildNode4.DebugHasEntity(entity)) return true;
            }

            return false;
        }

        public bool DebugHasNode(QuadTreeNode node)
        {
            if (this == node)
                return true;

            if (!IsLeafNode)
            {
                if (ChildNode1.DebugHasNode(node)) return true;
                if (ChildNode2.DebugHasNode(node)) return true;
                if (ChildNode3.DebugHasNode(node)) return true;
                if (ChildNode4.DebugHasNode(node)) return true;
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
                ChildNode1 = QuadTree.GetQuadTreeNode(
                    pxBounds.X,
                    pxBounds.Y,
                    pxHalfWidth,
                    pxHalfHeight,
                    this);
                ChildNode2 = QuadTree.GetQuadTreeNode(
                    pxBounds.X + pxHalfWidth,
                    pxBounds.Y,
                    pxHalfWidth,
                    pxHalfHeight,
                    this);
                ChildNode3 = QuadTree.GetQuadTreeNode(
                    pxBounds.X,
                    pxBounds.Y + pxHalfHeight,
                    pxHalfWidth,
                    pxHalfHeight,
                    this);
                ChildNode4 = QuadTree.GetQuadTreeNode(
                    pxBounds.X + pxHalfWidth,
                    pxBounds.Y + pxHalfHeight,
                    pxHalfWidth,
                    pxHalfHeight,
                    this);
            }

            // Add the entity specified in the function paramaters
            if (ChildNode1.Intersects(entity.CurrentBoundingBox)) ChildNode1.Add(entity);
            if (ChildNode2.Intersects(entity.CurrentBoundingBox)) ChildNode2.Add(entity);
            if (ChildNode3.Intersects(entity.CurrentBoundingBox)) ChildNode3.Add(entity);
            if (ChildNode4.Intersects(entity.CurrentBoundingBox)) ChildNode4.Add(entity);

            // Distrobute any entities found in this current Node
            foreach (Entity entityToAdd in this.Entities)
            {
                if (ChildNode1.Intersects(entityToAdd.CurrentBoundingBox)) ChildNode1.Add(entityToAdd);
                if (ChildNode2.Intersects(entityToAdd.CurrentBoundingBox)) ChildNode2.Add(entityToAdd);
                if (ChildNode3.Intersects(entityToAdd.CurrentBoundingBox)) ChildNode3.Add(entityToAdd);
                if (ChildNode4.Intersects(entityToAdd.CurrentBoundingBox)) ChildNode4.Add(entityToAdd);
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
                ChildNode1 = null;
                ChildNode2 = null;
                ChildNode3 = null;
                ChildNode4 = null;
                Entities = entities;

                if (Parent!=nodeLimit)
                    Parent.Validate(nodeLimit);
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

            if (pxBoundingBox == null || ChildNode1.Intersects(pxBoundingBox.Value)) ChildNode1.GetAssociatedNodes(entity, pxBoundingBox, ref result);
            if (pxBoundingBox == null || ChildNode2.Intersects(pxBoundingBox.Value)) ChildNode2.GetAssociatedNodes(entity, pxBoundingBox, ref result);
            if (pxBoundingBox == null || ChildNode3.Intersects(pxBoundingBox.Value)) ChildNode3.GetAssociatedNodes(entity, pxBoundingBox, ref result);
            if (pxBoundingBox == null || ChildNode4.Intersects(pxBoundingBox.Value)) ChildNode4.GetAssociatedNodes(entity, pxBoundingBox, ref result);
        }

        internal void GetEntities(FRectangle? pxRegion, ref List<Entity> result)
        {
            if (Entities.Count > 0)
            {
                foreach (Entity entity in Entities)
                    if (!result.Contains(entity))           // TODO: This needs to be removed. It is slow and should not be here
                        result.Add(entity);
            }
            else
            {
                if (IsLeafNode) return;

                if (pxRegion == null || ChildNode1.Intersects(pxRegion.Value)) ChildNode1.GetEntities(pxRegion, ref result);
                if (pxRegion == null || ChildNode2.Intersects(pxRegion.Value)) ChildNode2.GetEntities(pxRegion, ref result);
                if (pxRegion == null || ChildNode3.Intersects(pxRegion.Value)) ChildNode3.GetEntities(pxRegion, ref result);
                if (pxRegion == null || ChildNode4.Intersects(pxRegion.Value)) ChildNode4.GetEntities(pxRegion, ref result);
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
