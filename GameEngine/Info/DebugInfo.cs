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
        public TimeSpan QuadTreeUpdateTime { get; internal set; }
        public TimeSpan TileRenderingTime { get; internal set; }
        public TimeSpan TotalEntityRenderingTime { get; internal set; }
        public TimeSpan TotalEntityUpdateTime { get; internal set; }
        public TimeSpan TotalGameShaderRenderTime { get; internal set; }

        public Dictionary<string, TimeSpan> EntityRenderingTime { get; internal set; }
        public Dictionary<string, TimeSpan> EntityUpdateTime { get; internal set; }
        public Dictionary<string, TimeSpan> GameShaderRenderingTime { get; internal set; }

        public DebugInfo()
        {
            EntityRenderingTime = new Dictionary<string, TimeSpan>();
            GameShaderRenderingTime = new Dictionary<string, TimeSpan>();
            EntityUpdateTime = new Dictionary<string, TimeSpan>();
        }

        /// <summary>
        /// Gets the largest number of specified key value pairs from the specified dictionary. This method
        /// can be used to retrieve the most important information from dictionary information available
        /// in this class.
        /// </summary>
        /// <param name="TimeInfo">Dictionary containing a string item id as a key and a TimeSpan as a value.</param>
        /// <param name="Top">integer value specifying how many of the top results should be returned.</param>
        /// <returns>Dictionary of the specified amount of largest key value pairs.</returns>
        public Dictionary<string, TimeSpan> GetTop(Dictionary<string, TimeSpan> TimeInfo, int Top)
        {
            Dictionary<string, TimeSpan> result = new Dictionary<string, TimeSpan>();
            List<KeyValuePair<string, TimeSpan>> sortList = new List<KeyValuePair<string, TimeSpan>>();

            foreach (string itemId in TimeInfo.Keys)
                sortList.Add(new KeyValuePair<string,TimeSpan>(itemId, TimeInfo[itemId]));

            sortList.Sort(CompareTimeSpans);
            for (int i = 0; i < Top && i < sortList.Count; i++)
                result.Add(sortList[i].Key, sortList[i].Value);

            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("QuadTree Update Time = " + QuadTreeUpdateTime);
            builder.AppendLine("Tile Rendering Time = " + TileRenderingTime);
            builder.AppendLine("Total Entity Rendering Time = " + TotalEntityRenderingTime);
            builder.AppendLine("Total Entity Update Time = " + TotalEntityUpdateTime);
            builder.AppendLine("Total Game Shader Time = " + TotalGameShaderRenderTime);

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
