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
        public TimeSpan ColliderUpdateTime { get; internal set; }
        public TimeSpan TileRenderingTime { get; internal set; }

        public TimeSpan TotalEntityUpdateBoundingBoxTime { get; internal set; }
        public TimeSpan TotalEntityRenderingTime { get; internal set; }
        public TimeSpan TotalEntityUpdateTime { get; internal set; }
        public TimeSpan TotalGameShaderRenderTime { get; internal set; }

        public TimeSpan TotalEntityAdditionTime { get; internal set; }
        public TimeSpan TotalEntityRemovalTime { get; internal set; }

        public Dictionary<string, TimeSpan> EntityRenderingTimes { get; internal set; }
        public Dictionary<string, TimeSpan> EntityUpdateTimes { get; internal set; }
        public Dictionary<string, TimeSpan> GameShaderRenderingTimes { get; internal set; }

        public DebugInfo()
        {
            EntityRenderingTimes = new Dictionary<string, TimeSpan>();
            GameShaderRenderingTimes = new Dictionary<string, TimeSpan>();
            EntityUpdateTimes = new Dictionary<string, TimeSpan>();
        }

        public void Reset()
        {
            TotalEntityRenderingTime = TimeSpan.FromMilliseconds(0);
            TotalEntityUpdateBoundingBoxTime = TimeSpan.FromMilliseconds(0);
            TotalEntityUpdateTime = TimeSpan.FromMilliseconds(0);
            TotalGameShaderRenderTime = TimeSpan.FromMilliseconds(0);
            TotalEntityAdditionTime = TimeSpan.FromMilliseconds(0);
            TotalEntityRemovalTime = TimeSpan.FromMilliseconds(0);

            EntityRenderingTimes.Clear();
            EntityUpdateTimes.Clear();
            GameShaderRenderingTimes.Clear();
        }

        /// <summary>
        /// Gets the largest number of specified key value pairs from the specified dictionary. This method
        /// can be used to retrieve the most important information from dictionary information available
        /// in this class.
        /// </summary>
        /// <param name="timeInfo">Dictionary containing a string item id as a key and a TimeSpan as a value.</param>
        /// <param name="top">integer value specifying how many of the top results should be returned.</param>
        /// <returns>Dictionary of the specified amount of largest key value pairs.</returns>
        public Dictionary<string, TimeSpan> GetTop(Dictionary<string, TimeSpan> timeInfo, int top)
        {
            Dictionary<string, TimeSpan> result = new Dictionary<string, TimeSpan>();
            List<KeyValuePair<string, TimeSpan>> sortList = new List<KeyValuePair<string, TimeSpan>>();

            foreach (string itemId in timeInfo.Keys)
                sortList.Add(new KeyValuePair<string,TimeSpan>(itemId, timeInfo[itemId]));

            sortList.Sort(CompareTimeSpans);
            for (int i = 0; i < top && i < sortList.Count; i++)
                result.Add(sortList[i].Key, sortList[i].Value);

            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Collider Update Time = " + ColliderUpdateTime);
            builder.AppendLine("Tile Rendering Time = " + TileRenderingTime);
            builder.AppendLine("Total Entity Rendering Time = " + TotalEntityRenderingTime);
            builder.AppendLine("Total Entity Update Time = " + TotalEntityUpdateTime);
            builder.AppendLine("Total Game Shader Time = " + TotalGameShaderRenderTime);
            builder.AppendLine("Total Entity Addition Time = " + TotalEntityAdditionTime);
            builder.AppendLine("Total Entity Remval Time = " + TotalEntityRemovalTime);
            builder.AppendLine("Total Entity Update Bounding Box Time = " + TotalEntityUpdateBoundingBoxTime);

            return builder.ToString();
        }

        /// <summary>
        /// Compares the two given KeyValuePairs by using their TimeSpan values. This function is used internally
        /// by this class to sort the topmost running timespans.
        /// </summary>
        /// <param name="timespan1">First KeyValuePair to compare.</param>
        /// <param name="timespan2">Second KeyValuePair to compare.</param>
        /// <returns>integer value specifying if timespan1 is larger, smaller or equal to timespan2</returns>
        private static int CompareTimeSpans(KeyValuePair<string, TimeSpan> timespan1, KeyValuePair<string, TimeSpan> timespan2)
        {
            return Convert.ToInt32(timespan2.Value.Ticks - timespan1.Value.Ticks);
        }
    }
}
