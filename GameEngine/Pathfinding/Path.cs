using System;
using System.Collections.Generic;

namespace GameEngine.Pathfinding
{
    public class Path : Stack<ANode>
    {
        public Path() { }
        public Path(ANode[] nodes) : base(nodes) { }
        public Path(Path path) : base(new Stack<ANode>(path)) { }
    }
}
