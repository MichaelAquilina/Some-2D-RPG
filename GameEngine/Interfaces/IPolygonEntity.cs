using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GameEngine.Interfaces
{
    public interface IPolygonEntity
    {
        List<Point> Points { get; set; }
    }
}
