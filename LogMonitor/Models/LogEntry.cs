using Caliburn.Micro;
using System;

namespace LogMonitor.Models
{
    public class LogEntry : PropertyChangedBase
    {
        public string Line { get; set; }

        public LogEntry(string line)
        {
            Line = line;
        }
    }
}
