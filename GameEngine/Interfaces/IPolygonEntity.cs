using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GameEngine.Interfaces
{
    /// <summary>
    /// Defines an interface with which Entities loaded from a tiled map may inherit any polygon
    /// properties defined for that object within the map.
    /// </summary>
    public interface IPolygonEntity
    {
        List<Point> Points { get; set; }
    }
}
