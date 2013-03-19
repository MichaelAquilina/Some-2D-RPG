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

        //NODE POOL TO PREVENT INITIALISING NEW CLASSES EACH LOOP!
        internal int _currentNodePoolIndex = 0;
        internal List<QuadTreeNode> _nodePool = new List<QuadTreeNode>();

        public QuadTree(int txWidth, int txHeight, int pxTileWidth, int pxTileHeight, int EntityLimit=1)
        {
            NodeList = new List<QuadTreeNode>();
            Root = GetQuadTreeNode(0, 0, pxTileWidth * txWidth, pxTileHeight * txHeight);
            this.EntityLimit = EntityLimit;
            this.pxTileWidth = pxTileWidth;
            this.pxTileHeight = pxTileHeight;
        }

        /// <summary>
        /// Builds the QuadTree based on the input entities passed in the functions
        /// parameter. The function tries to be as cheap as possible by re-using previously
        /// insantiated objects in previous calls.
        /// </summary>
        /// <param name="Entities">List of Entities to build the QuadTree out of.</param>
        public void Build(ICollection<Entity> Entities)
        {
            _currentNodePoolIndex = 1;
            Root.Clear();
            NodeList.Clear();
            NodeList.Add(Root);

            foreach (Entity entity in Entities)
                Root.Add(entity, EntityLimit);
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
        /// <returns>QuadTreeNode with the specified parameters.</returns>
        public QuadTreeNode GetQuadTreeNode(int px, int py, int pxWidth, int pxHeight)
        {
            if (_currentNodePoolIndex == _nodePool.Count)
                _nodePool.Add(new QuadTreeNode());

            QuadTreeNode nodeResult = _nodePool[_currentNodePoolIndex];
            nodeResult.Clear();
            nodeResult.NodeID = _currentNodePoolIndex;
            nodeResult.pxBounds = new Rectangle(px, py, pxWidth, pxHeight);
            nodeResult.QuadTree = this;

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

            return Root.GetIntersectingEntities(pxRegion, result);
        }

        public override string ToString()
        {
            return string.Format("QuadTree: NodeCount={0}, pxWidth={1}, pxHeight={2}",
                NodeList.Count,
                Root.pxBounds.Width,
                Root.pxBounds.Height);
        }
    }
}
