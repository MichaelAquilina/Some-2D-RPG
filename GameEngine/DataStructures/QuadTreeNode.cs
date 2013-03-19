using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;

namespace GameEngine.DataStructures
{
    public class QuadTreeNode
    {
        //no need for arrays!
        public QuadTreeNode Node1 { get; private set; }
        public QuadTreeNode Node2 { get; private set; }
        public QuadTreeNode Node3 { get; private set; }
        public QuadTreeNode Node4 { get; private set; }

        public QuadTree QuadTree { get; internal set; }
        public List<Entity> Entities { get; internal set; }

        public int PX { get; internal set; }
        public int PY { get; internal set; }
        public int pxWidth { get; internal set; }
        public int pxHeight { get; internal set; }

        public int NodeID { get; set; }

        public QuadTreeNode()
        {
            this.Entities = new List<Entity>();
            Clear();
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
            return new Rectangle(PX,PY,pxWidth,pxHeight).Intersects(pxBoundingBox);
        }

        /// <summary>
        /// Recursively adds the specified Entity to this QuadTreeNode and possibly any child QuadTreeNodes
        /// contained in the one specified in the parameters. This function will create any new Child tree
        /// nodes as necessary from the associated QuadTrees NodePool in order to be as cheap as possible
        /// on both memory and CPU.
        /// </summary>
        /// <param name="Entity">Entity to add to this QuadTreeNode.</param>
        /// <param name="EntityLimit">(optional) integer value specifying the max number of entities to store in each QuadTreeNode.</param>
        public void Add(Entity Entity, int EntityLimit)
        {
            int pxHalfWidth = (int)Math.Ceiling(pxWidth / 2.0f);
            int pxHalfHeight = (int)Math.Ceiling(pxHeight / 2.0f);

            if ((Node1 == null && Entities.Count < EntityLimit) || pxHalfWidth <= QuadTree.pxTileWidth || pxHalfHeight <= QuadTree.pxTileHeight)
            {
                Entities.Add(Entity);
                return;
            }

            if (Node1 == null)
            {
                Node1 = QuadTree.GetQuadTreeNode(
                    PX,
                    PY,
                    pxHalfWidth,
                    pxHalfHeight);
                Node2 = QuadTree.GetQuadTreeNode(
                    PX + pxHalfWidth - 1,
                    PY,
                    pxHalfWidth,
                    pxHalfHeight);
                Node3 = QuadTree.GetQuadTreeNode(
                    PX,
                    PY + pxHalfHeight - 1,
                    pxHalfWidth,
                    pxHalfHeight);
                Node4 = QuadTree.GetQuadTreeNode(
                    PX + pxHalfWidth - 1,
                    PY + pxHalfHeight - 1,
                    pxHalfWidth,
                    pxHalfHeight);

                QuadTree.NodeList.Add(Node1);
                QuadTree.NodeList.Add(Node2);
                QuadTree.NodeList.Add(Node3);
                QuadTree.NodeList.Add(Node4);
            }

            //determine which entities need to be re-added
            List<Entity> toAdd = new List<Entity>();
            toAdd.Add(Entity);
            toAdd.AddRange(this.Entities);
            this.Entities.Clear();

            foreach (Entity entityToAdd in toAdd)
            {
                if (Node1.Intersects(entityToAdd.CurrentPxBoundingBox)) Node1.Add(entityToAdd, EntityLimit);
                if (Node2.Intersects(entityToAdd.CurrentPxBoundingBox)) Node2.Add(entityToAdd, EntityLimit);
                if (Node3.Intersects(entityToAdd.CurrentPxBoundingBox)) Node3.Add(entityToAdd, EntityLimit);
                if (Node4.Intersects(entityToAdd.CurrentPxBoundingBox)) Node4.Add(entityToAdd, EntityLimit);
            }
        }

        public override string ToString()
        {
            return string.Format("QuadTreeNode: NodeId={0}; (PX,PY)=({1},{2}), pxWidth={3}, pxHeight={4}, IsLeafNode={5}",
                NodeID,
                PX, PY, pxWidth, pxHeight,
                Node1 == null);
        }
    }
}
