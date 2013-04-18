namespace GameEngine.Options
{
    public class DrawingOptions
    {
        /// <summary>
        /// Show Entity debug information when drawing the world view port to the screen.
        /// </summary>
        public bool ShowEntityDebugInfo { get; set; }

        /// <summary>
        /// Shows the QuadTrees bounding boxes when drawing the world viewport.
        /// </summary>
        public bool ShowQuadTree { get; set; }

        /// <summary>
        /// bool value specifying if the tile grid should be shown during render calls.
        /// </summary>
        public bool ShowTileGrid { get; set; }

        /// <summary>
        /// bool value specifying if the bounding boxes for entities should be shown during render calls.
        /// </summary>
        public bool ShowBoundingBoxes { get; set; }

        /// <summary>
        /// bool value specifying if the bounding boxes for each individual drawable component should be shown.
        /// </summary>
        public bool ShowDrawableComponents { get; set; }
    }
}
