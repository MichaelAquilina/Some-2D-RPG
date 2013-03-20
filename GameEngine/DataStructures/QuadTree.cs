using System.Collections.Generic;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;

namespace GameEngine.DataStructures
{
    public class QuadTree
    {       
        public QuadTreeNode Root { get; private set; }
        public List<QuadTreeNode> NodeList { get; private set; }
        public int EntityLimit { get; private set; }
        public int pxTileWidth { get; private set; }
        public int pxTileHeight { get; private set; }

        ////NODE POOL TO PREVENT INITIALISING NEW CLASSES EACH LOOP!
        //TODO: Remove this, this was needed when we were rebuilding at each update
        internal int _currentNodePoolIndex = 0;
        //internal List<QuadTreeNode> _nodePool = new List<QuadTreeNode>();

        public QuadTree(int txWidth, int txHeight, int pxTileWidth, int pxTileHeight, int EntityLimit=1)
        {
            NodeList = new List<QuadTreeNode>();
            Root = GetQuadTreeNode(0, 0, pxTileWidth * txWidth, pxTileHeight * txHeight, null);
            this.EntityLimit = EntityLimit;
            this.pxTileWidth = pxTileWidth;
            this.pxTileHeight = pxTileHeight;
        }

        public void Update(Entity Entity)
        {
            Rectangle dummyBox = Entity.CurrentPxBoundingBox;
            Entity.CurrentPxBoundingBox = Entity.prevPxBoundingBox;

            Remove(Entity);

            Entity.CurrentPxBoundingBox = dummyBox;

            Root.Add(Entity);
        }

        /// <summary>
        /// Completely Removes the specified Entity from the association with this QuadTree. The
        /// method will automatically clean up any un-needed nodes that may have been created from
        /// the Removal process.
        /// </summary>
        /// <param name="Entity"></param>
        public void Remove(Entity Entity)
        {
            List<QuadTreeNode> associations = new List<QuadTreeNode>();
            Root.GetAssociatedNodes(Entity, ref associations);          //TODO: GetAssociatedNodes is  QuadTree function not a QuadTreeNode

            foreach (QuadTreeNode node in associations)
            {
                node.Entities.Remove(Entity);
                node.Validate();
            }
        }

        /// <summary>
        /// Builds the QuadTree based on the input entities passed in the functions
        /// parameter. The function tries to be as cheap as possible by re-using previously
        /// insantiated objects in previous calls.
        /// </summary>
        /// <param name="Entities">Collection of Entities to build the QuadTree out of.</param>
        public void Build(ICollection<Entity> Entities)
        {
            _currentNodePoolIndex = 1;

            Root.Clear();
            NodeList.Clear();
            NodeList.Add(Root);

            foreach (Entity entity in Entities)
                Root.Add(entity);
        }

        /// <summary>
        /// Retrieves a QuadTreeNode from the current Pool of Nodes. If a free node is found in the
        /// pool, then it is cleared and then set with the specified parameters. If the pool does not
        /// contain a free node, then a new one is created and added to the pool before being set and
        /// returned to the user. Using this technique ensures that costly calls to initialise a new
        /// QuadTreeNode class is not done multiple times per second (probably 100s of times infact) -
        /// but when possible, a previously created object will be re-used. This function will also
        /// automatically set the NodeID of the QuadTreeNode for quick identification by the user
        /// during debugging.
        /// </summary>
        /// <param name="px">Top left location of the QuadTreeNode in pixels.</param>
        /// <param name="py">Top left location of the QuadTreeNode in pixels.</param>
        /// <param name="pxWidth">Width in pixels of the QuadTreeNode.</param>
        /// <param name="pxHeight">Height in pixels of the QuadTreeNode.</param>
        /// <param name="Parent">QuadTreeNode that will be the parent of the new node.</param>
        /// <returns>QuadTreeNode with the specified parameters.</returns>
        public QuadTreeNode GetQuadTreeNode(int px, int py, int pxWidth, int pxHeight, QuadTreeNode Parent)
        {
            QuadTreeNode nodeResult = new QuadTreeNode();
            nodeResult.Clear();
            nodeResult.NodeID = _currentNodePoolIndex;
            nodeResult.pxBounds = new Rectangle(px, py, pxWidth, pxHeight);
            nodeResult.QuadTree = this;
            nodeResult.Parent = Parent;

            _currentNodePoolIndex++;

            return nodeResult;
        }

        /// <summary>
        /// Returns all intersecting Entities found in that region based on the
        /// CurrentPxBoundingBox property exposed by the Entity (which would have
        /// been last updated by the TeeEngine Update loop). It is important to note
        /// that because this uses a form of result buffering to optimise performance,
        /// this method is NOT CONSIDERED THREAD SAFE in any scenario.
        /// </summary>
        /// <param name="pxRegion">Rectangle region to check in Pixels.</param>
        /// <returns>List of Entity objects intersecting the specified region.</returns>
        public List<Entity> GetIntersectingEntites(Rectangle pxRegion)
        {
            List<Entity> result = new List<Entity>();
            Root.GetIntersectingEntities(pxRegion, ref result);

            return result;
        }

        public override string ToString()
        {
            return string.Format("QuadTree:pxWidth={1}, pxHeight={2}",
                Root.pxBounds.Width,
                Root.pxBounds.Height);
        }
    }
}
