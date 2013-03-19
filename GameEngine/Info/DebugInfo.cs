using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GameEngine.Info
{
    /// <summary>
    /// Class that will contain debug information that can then be accessed by the user in a specific manner within the
    /// TeeEngine. This structure will be updated when necessary within the TeeEngine update logic.
    /// </summary>
    public class DebugInfo
    {
        public TimeSpan QuadTreeBuildTime { get; internal set; }
        public TimeSpan TileRenderingTime { get; internal set; }
        public TimeSpan EntityRenderingTime { get; internal set; }
        public TimeSpan EntityUpdateTime { get; internal set; }
        public TimeSpan GameShadersRenderTime { get; internal set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (PropertyInfo property in this.GetType().GetProperties())
                    builder.AppendLine(property.Name + "=" + property.GetValue(this, null));

            return builder.ToString();
        }
    }
}
