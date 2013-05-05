using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameEngine.Info
{
    /// <summary>
    /// Class that allows for diagnsotics times to be stored and timed.
    /// </summary>
    public class DiagnosticInfo
    {
        public string Description { get; set; }

        private Dictionary<string, Stopwatch> _diagnostics = new Dictionary<string, Stopwatch>();

        public DiagnosticInfo(string description)
        {
            this.Description = description;
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

        public override string ToString()
        {
            return string.Format("DebugInfo: Description={0}, Timers={1}", Description, _diagnostics.Keys.Count);
        }
    }
}
