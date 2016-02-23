using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using LogMonitor.Models;

namespace LogMonitor.Services
{
    public class LogWatcher : PropertyChangedBase
    {
        private readonly FileSystemWatcher _fileSystemWatcher;
        private string _folderFullPath;
        private LogFile _currentFile;
        public BindableCollection<LogFile> LogFiles { get; set; }
        

        public LogFile CurrentFile
        {
            get { return _currentFile; }
            set
            {
                if (Equals(value, _currentFile)) return;
                _currentFile = value;
                NotifyOfPropertyChange(() => CurrentFile);
            }
        }

        public LogWatcher()
        {
            LogFiles = new BindableCollection<LogFile>();

            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            _fileSystemWatcher.Filter = "*.txt";
            _fileSystemWatcher.Created += FileSystemWatcher_Created;
            _fileSystemWatcher.Changed += _fileSystemWatcher_Changed;
        }

        private void _fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            SortLogFiles();
        }

        private void SortLogFiles()
        {
            var key = CurrentFile?.FullPath;
            LogFiles.Clear();

            LogFiles.AddRange(GetAllLogs(_folderFullPath));
            if (key != null)
                CurrentFile = LogFiles.Single(l => l.FullPath.Equals(key));
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            LogFiles.Add(new LogFile(new FileInfo(e.FullPath)));
            SortLogFiles();
        }

        public void StartWatching(string folder)
        {
            if (folder == null) throw new ArgumentNullException(nameof(folder));
            if (!Directory.Exists(folder))
                return;

            _folderFullPath = folder;
            LogFiles.AddRange(GetAllLogs(folder));

            if (!string.IsNullOrEmpty(folder))
            {
                _fileSystemWatcher.Path = folder;
                _fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        private IEnumerable<LogFile> GetAllLogs(string folderPath)
        {
            var folder = new DirectoryInfo(folderPath);
            return folder.GetFiles("*.txt").Select(f=>new LogFile(f)).OrderByDescending(f=>f.LastWrite);
        }
    }
}
