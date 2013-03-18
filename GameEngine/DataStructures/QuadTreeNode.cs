using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;

namespace GameEngine.DataStructures
{
    public class QuadTreeNode
    {
        //incremental counter to assign IDs during build

        //no need for arrays!
        public QuadTreeNode Node1 { get; private set; }
        public QuadTreeNode Node2 { get; private set; }
        public QuadTreeNode Node3 { get; private set; }
        public QuadTreeNode Node4 { get; private set; }

        public List<Entity> Entities { get; set; }

        public float txWidth { get; set; }
        public float txHeight { get; set; }
        public float TX { get; set; }
        public float TY { get; set; }

        public int NodeID { get; set; }

        public QuadTreeNode()
        {
            this.Entities = new List<Entity>();
            Clear();
        }

        public void Clear()
        {
            Entities.Clear();
            Node1 = null;
            Node2 = null;
            Node3 = null;
            Node4 = null;
        }

        public bool Contains(float tx, float ty)
        {
            return
                tx >= this.TX && tx < this.TX + this.txWidth &&
                ty >= this.TY && ty < this.TY + this.txHeight;
        }

        //recursive
        public void Add(Entity entity, int entityLimit, QuadTree QuadTree)
        {
            if ((Node1 == null && Entities.Count < entityLimit) || txWidth <= 1 || txHeight <= 1)
            {
                Entities.Add(entity);
                return;
            }

            if (Node1 == null)
            {
                float txHalfWidth = this.txWidth / 2.0f;
                float txHalfHeight = this.txHeight / 2.0f;

                Node1 = QuadTree.GetQuadTreeNode(
                    this.TX,
                    this.TY,
                    txHalfWidth,
                    txHalfHeight);
                Node2 = QuadTree.GetQuadTreeNode(
                    this.TX + txHalfWidth,
                    TY,
                    txHalfWidth,
                    txHalfHeight);
                Node3 = QuadTree.GetQuadTreeNode(
                    this.TX,
                    this.TY + txHalfHeight,
                    txHalfWidth,
                    txHalfHeight);
                Node4 = QuadTree.GetQuadTreeNode(
                    this.TX + txHalfWidth,
                    this.TY + txHalfHeight,
                    txHalfWidth,
                    txHalfHeight);

                QuadTree.NodeList.Add(Node1);
                QuadTree.NodeList.Add(Node2);
                QuadTree.NodeList.Add(Node3);
                QuadTree.NodeList.Add(Node4);
            }

            List<Entity> toAdd = new List<Entity>();
            toAdd.Add(entity);
            toAdd.AddRange(this.Entities);

            this.Entities.Clear();

            foreach (Entity entityToAdd in toAdd)
            {
                if (Node1.Contains(entityToAdd.TX, entityToAdd.TY)) Node1.Add(entityToAdd, entityLimit, QuadTree);
                else if (Node2.Contains(entityToAdd.TX, entityToAdd.TY)) Node2.Add(entityToAdd, entityLimit, QuadTree);
                else if (Node3.Contains(entityToAdd.TX, entityToAdd.TY)) Node3.Add(entityToAdd, entityLimit, QuadTree);
                else if (Node4.Contains(entityToAdd.TX, entityToAdd.TY)) Node4.Add(entityToAdd, entityLimit, QuadTree);
            }
        }

        public override string ToString()
        {
            return string.Format("QuadTreeNode: NodeId={0}; (TX,TY)=({1},{2}), txWidth={3}, txHeight={4}, Leaf={5}",
                NodeID,
                TX, TY, txWidth, txHeight,
                Node1 == null);
        }
    }
}
