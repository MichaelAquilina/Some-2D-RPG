using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameEngine.Info
{
    /// <summary>
    /// Class that allows for diagnostics to be recorded and organised through named values. Provides numerous
    /// methods that allow for each diagnostic timer to be started, stopped and reset in order to provide complete 
    /// control.
    /// </summary>
    public class DiagnosticInfo
    {
        public string Description { get; set; }

        private Dictionary<string, Stopwatch> _diagnostics = new Dictionary<string, Stopwatch>();

        public DiagnosticInfo(string description)
        {
            this.Description = description;
        }

        public void Clear()
        {
            _diagnostics.Clear();
        }

        public void ResetAll()
        {
            foreach (Stopwatch watch in _diagnostics.Values)
                watch.Reset();
        }

        public bool Reset(string name)
        {
            if (!_diagnostics.ContainsKey(name))
                return false;

            _diagnostics[name].Reset();
            return true;
        }

        public void StartTiming(string name)
        {
            if (!_diagnostics.ContainsKey(name))
                _diagnostics.Add(name, new Stopwatch());

            Stopwatch watch = _diagnostics[name];
            watch.Start();
        }

        public void RestartTiming(string name)
        {
            if (!_diagnostics.ContainsKey(name))
                _diagnostics.Add(name, new Stopwatch());

            Stopwatch watch = _diagnostics[name];
            watch.Restart();
        }

        public void StopTiming(string name)
        {
            if (!_diagnostics.ContainsKey(name))
                throw new ArgumentException("The specified diagnostic watch does not exist.");

            Stopwatch watch = _diagnostics[name];
            watch.Stop();
        }

        public TimeSpan GetTiming(string name)
        {
            if (!_diagnostics.ContainsKey(name))
                throw new ArgumentException("The specified diagnostic watch does not exist.");

            return _diagnostics[name].Elapsed;
        }

        public string ShowTop(int top, bool sort=true)
        {
            List<string> keyList = _diagnostics.Keys.ToList();

            if(sort) keyList.Sort(CompareWatches);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < top && i < keyList.Count; i++)
            {
                string key = keyList[i];

                builder.AppendFormat("{0}: {1}", key, GetTiming(key));
                builder.AppendLine();
            }

            return builder.ToString();
        }

        public string ShowAll(bool sort=true)
        {
            return ShowTop(_diagnostics.Keys.Count, sort);
        }

        public override string ToString()
        {
            return string.Format("DebugInfo: Description={0}, Timers={1}", Description, _diagnostics.Keys.Count);
        }

        private int CompareWatches(string key1, string key2)
        {
            return Convert.ToInt32(
                _diagnostics[key2].Elapsed.Ticks - _diagnostics[key1].Elapsed.Ticks
                );
        }
    }
}
