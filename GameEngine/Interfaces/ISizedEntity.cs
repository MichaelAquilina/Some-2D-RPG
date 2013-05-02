namespace GameEngine.Interfaces
{
    /// <summary>
    /// Defines an interface with which any Entities loaded from a tiled map may inherit the width and height
    /// property defined within the tiled map.
    /// </summary>
    public interface ISizedEntity
    {
        int Width { get; set; }
        int Height { get; set; }
    }
}
