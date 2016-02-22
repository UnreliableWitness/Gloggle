using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace LogMonitor.Models
{
    public class LogFile : PropertyChangedBase
    {
        private DateTime _lastWrite;

        public string Directory { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }

        public DateTime LastWrite
        {
            get { return _lastWrite; }
            set
            {
                if (value.Equals(_lastWrite)) return;
                _lastWrite = value;
                NotifyOfPropertyChange(() => LastWrite);
            }
        }

        public BindableCollection<LogEntry> LogEntries { get; set; }

        public LogFile(FileInfo file)
        {
            LogEntries = new BindableCollection<LogEntry>();

            Name = file.Name;
            Directory = file.DirectoryName;
            FullPath = file.FullName;
            LastWrite = file.LastWriteTime;

            StartLooking();
        }

        private void StartLooking()
        {
            if(!File.Exists(FullPath))
                return;

            foreach (string line in File.ReadLines(FullPath))
            {
                LogEntries.Add(new LogEntry(line));
            }


            Task.Run(() =>
            {
                var wh = new AutoResetEvent(false);
                var fsw = new FileSystemWatcher(Directory);
                fsw.Filter = Name;
                fsw.Changed += (s, e) => wh.Set();
                using (var fileStream = File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var file = new StreamReader(fileStream))
                {
                    var s = "";
                    while (true)
                    {
                        s = file.ReadLine();
                        if (s != null)
                        {
                            LastWrite = DateTime.Now;
                            LogEntries.Add(new LogEntry(s));
                        }
                        else
                            wh.WaitOne(1000);
                    }
                }

                wh.Close();
            });

        }
    }
}