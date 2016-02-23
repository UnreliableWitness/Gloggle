using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Text;

namespace LogMonitor.Models
{
    public class LogFile : PropertyChangedBase
    {
        private DateTime _lastWrite;
        private int _currentPosition;
        private FileSystemWatcher _watcher;

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

            _watcher = new FileSystemWatcher(Directory);
            _watcher.Filter = Name;

            _watcher.Changed += _watcher_Changed;
        }

        public void StartLooking()
        {
            if(!File.Exists(FullPath))
                return;

            _currentPosition = 0;
            LogEntries.Clear();

            ReadFile();

            _watcher.EnableRaisingEvents = true;
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            ReadFile();
        }

        private void ReadFile()
        {
            foreach (var line in ReadLines(FullPath).Skip(_currentPosition))
            {
                LogEntries.Add(new LogEntry(line));
                _currentPosition++;
            }
        }

        public void StopLooking()
        {
            _watcher.EnableRaisingEvents = false;
        }


        public static IEnumerable<string> ReadLines(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }

}