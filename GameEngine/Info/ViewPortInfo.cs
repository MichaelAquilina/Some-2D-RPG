using Microsoft.Xna.Framework;

namespace GameEngine.Info
{
    /// <summary>
    /// Representation of the data currently found in the ViewPort being rendered by the TileEngine.
    /// This structure will be passed to any GameShaders that need to be applied to the world and will be 
    /// returned by the DrawWorldViewPort method in order to allow external code to place and detect coordinates
    /// correctly.
    /// </summary>
    public class ViewPortInfo
    {
        /// <summary>
        /// The rectangular region of the game world being shown by the viewport in world coordinates.
        /// </summary>
        public Rectangle pxViewPortBounds { get; set; }
        
        /// <summary>
        /// The actual width of the tiles (in pixels) when they are drawn to the viewport.
        /// </summary>
        public float pxTileWidth { get; set; }            

        /// <summary>
        /// The actual heigth of the tiles (in pixels) when they are drawn to the viewport.
        /// </summary>
        public float pxTileHeight { get; set; }

        /// <summary>
        /// The x value for the top left corner of the drawn viewport in world coordinates.
        /// </summary>
        public float pxTopLeftX { get; set; } 

        /// <summary>
        /// The y value for the top left corner of the drawn viewport in world coordinates.
        /// </summary>
        public float pxTopLeftY { get; set; }

        /// <summary>
        /// The width of the world currently being shown in the viewport - in pixels.
        /// </summary>
        public float pxWidth { get; set; }

        /// <summary>
        /// The height of the world currently being shown in the viewport - in pixels.
        /// </summary>
        public float pxHeight { get; set; }
        
        /// <summary>
        /// The x-width amount of tiles being shown in the view. This value includes any partially showing tiles
        /// thay may be showing at the edges of the screen.
        /// </summary>
        public int TileCountX { get; set; }

        /// <summary>
        /// The y-height amount of tiles being shown in the view. This value includes any partially showing tiles
        /// thay may be showing at the edges of the screen.
        /// </summary>
        public int TileCountY { get; set; }
        
        /// <summary>
        /// The displacement in X that occurs during drawing because a Tile is being partially shown in the top
        /// left corner of the screen. This value allows for smooth scrolling of the screen when placing Entities.
        /// The value returned by this property never be more than the TileWidth of a loaded map.
        /// </summary>
        public float pxDispX { get; set; }

        /// <summary>
        /// The displacement in Y that occurs during drawing because a Tile is being partially shown in the top
        /// left corner of the screen. This value allows for smooth scrolling of the screen when placing Entities.
        /// The value returned by this property never be more than the TileHeight of a loaded map.
        /// </summary>
        public float pxDispY { get; set; }

        /// <summary>
        /// The Actual amount of Zoom that occured during the draw call. It is most likely that any calculations
        /// you may want to perform that are affected by zoom should be calculated using this value rather than the
        /// one specified in the parameters for DrawWorldViewPort. The Actual Zoom value differs from the zoom value
        /// parameter specified in the DrawWorldViewPort because there is a loss of data between the conversion 
        /// from Map.pxTileWidth * Zoom -> (int). The *actual* level of zoom that was applied to the tiles is therefore
        /// used instead during draw calls to Entities in order to ensure they are drawn at the correct position on the screen.
        /// </summary>
        public float ActualZoom { get; set; }

        /// <summary>
        /// Returns the translated world coordinates of the specified position on the viewport in pixels. 
        /// </summary>
        /// <param name="position">Point value representing the pixel position on viewport.</param>
        /// <returns>Vector2 coordinate representing the location of the specified position in world coordinates.</returns>
        public Vector2 GetWorldCoordinates(Point position)
        {
            return new Vector2(
                position.X / ActualZoom + pxTopLeftX,
                position.Y / ActualZoom + pxTopLeftY
                );
        }

        public override string ToString()
        {
            return string.Format(
                "ViewPort: pxTopLeft=({0},{1}), pxWidth={2}, pxHeight={3}, pxDisp=({4},{5})",
                pxTopLeftX, pxTopLeftY, pxWidth, pxHeight, pxDispX, pxDispY);
        }
    }
}
