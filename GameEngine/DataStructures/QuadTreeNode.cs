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
        public int NodeID { get; set; }

        public bool IsLeafNode { get { return Node1 == null; } }

        public QuadTreeNode()
        {
            Entities = new List<Entity>();
            Clear();
        }

        /// <summary>
        /// Checks if this Node or any of its child nodes contains this Entity regardless of its
        /// BoundingBox state. This is useful for debugging purposes but is less effecient than
        /// one would expect since all the child nodes are traversed.
        /// </summary>
        /// <param name="Entity">Entity to search for.</param>
        /// <returns>bool value specifying if this Node or any of its children contains the Entity.</returns>
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

        /// <summary>
        /// Completely resets this QuadTreeNode for later re-use. Mainly used by the QuadTree class
        /// structure when the Node is pulled from its Node Pool to be used again in a new build
        /// iteration.
        /// </summary>
        public void Clear()
        {
            Entities.Clear();
            Node1 = null;
            Node2 = null;
            Node3 = null;
            Node4 = null;
            Parent = null;
        }

        public void Validate()
        {
            List<Entity> entities = new List<Entity>();
            GetEntities(ref entities);

            if (entities.Count <=  QuadTree.EntityLimit)
            {
                Node1 = null;
                Node2 = null;
                Node3 = null;
                Node4 = null;
                Entities = entities;

                if (Parent != null)
                    Parent.Validate();
            }
        }

        public void GetEntities(ref List<Entity> Result)
        {
            if (Node1 == null)
            {
                foreach (Entity entity in Entities)
                    if (!Result.Contains(entity)) Result.Add(entity);
            }
            else
            {
                Node1.GetEntities(ref Result);
                Node2.GetEntities(ref Result);
                Node3.GetEntities(ref Result);
                Node4.GetEntities(ref Result);                
            }   
        }
        
        /// <summary>
        /// Returns a list of all Entities intersecting this Node or its children
        /// in the specified region. This method makes use of a global buffer in the
        /// associated QuadTree to improve performance by avoiding over initialisation
        /// of new Lists at each recursive step.
        /// </summary>
        /// <param name="pxRegion">Intersecting Rectangular region to check in Pixels.</param>
        /// <param name="Result">List to place the Intersecting Entities in. Passed by reference.</param>
        /// //TODO: This can probably be built using GetAssociatedNodes because they are very similiar
        internal void GetIntersectingEntities(Rectangle pxRegion, ref List<Entity> Result)
        {
            if (Entities.Count > 0)
            {
                foreach (Entity entity in Entities)
                    if (entity.CurrentPxBoundingBox.Intersects(pxRegion) &&
                        !Result.Contains(entity))
                        Result.Add(entity);
            }
            else
            {
                if (IsLeafNode) return;

                if (Node1.Intersects(pxRegion)) Node1.GetIntersectingEntities(pxRegion, ref Result);
                if (Node2.Intersects(pxRegion)) Node2.GetIntersectingEntities(pxRegion, ref Result);
                if (Node3.Intersects(pxRegion)) Node3.GetIntersectingEntities(pxRegion, ref Result);
                if (Node4.Intersects(pxRegion)) Node4.GetIntersectingEntities(pxRegion, ref Result);
            }
        }

        /// <summary>
        /// Checks whether the specified Bounding Box region specified in Pixels intersects
        /// with the area specified in this QuadTreeNode. The function will return a true value
        /// in the case where it intersects and a false in the case where it does not.
        /// </summary>
        /// <param name="pxBoundingBox">Bounding Box in pixels to check for intersection.</param>
        /// <returns>bool value specifying the result of the intersection test.</returns>
        public bool Intersects(Rectangle pxBoundingBox)
        {
            return pxBounds.Intersects(pxBoundingBox);
        }

        //BUG: Not all the Associated nodes are retreived in some cases and this is due to a failed intersection test.
        //I doubt the problem is with the intersect function itself, but rather the Current state of the CurrentPxBoundingBox
        public void GetAssociatedNodes(Entity Entity, ref List<QuadTreeNode> Result)
        {
            if (Node1==null)
            {
                if(Entities.Contains(Entity)) Result.Add(this);
                return;
            }

            if (Node1.Intersects(Entity.CurrentPxBoundingBox)) Node1.GetAssociatedNodes(Entity, ref Result);
            if (Node2.Intersects(Entity.CurrentPxBoundingBox)) Node2.GetAssociatedNodes(Entity, ref Result);
            if (Node3.Intersects(Entity.CurrentPxBoundingBox)) Node3.GetAssociatedNodes(Entity, ref Result);
            if (Node4.Intersects(Entity.CurrentPxBoundingBox)) Node4.GetAssociatedNodes(Entity, ref Result);
        }

        /// <summary>
        /// Recursively adds the specified Entity to this QuadTreeNode and possibly any child QuadTreeNodes
        /// contained in the one specified in the parameters. This function will create any new Child tree
        /// nodes as necessary from the associated QuadTrees NodePool in order to be as cheap as possible
        /// on both memory and CPU.
        /// </summary>
        /// <param name="Entity">Entity to add to this QuadTreeNode.</param>
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
