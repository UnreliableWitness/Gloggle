using Caliburn.Micro;
using LogMonitor.Services;

namespace LogMonitor.ViewModels
{
    public class ShellViewModel : Screen
    {
        private string _selectedFolder;
        private LogWatcher _logWatcher;

        public LogWatcher LogWatcher
        {
            get { return _logWatcher; }
            set
            {
                if (Equals(value, _logWatcher)) return;
                _logWatcher = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedFolder
        {
            get
            {
                return _selectedFolder; }
            set
            {
                _selectedFolder = value;
                Properties.Settings.Default.SelectedFolder = value;
                Properties.Settings.Default.Save();
                NotifyOfPropertyChange(()=> SelectedFolder);
            }
        }

        public ShellViewModel()
        {
            LogWatcher = new LogWatcher();
            SelectedFolder = Properties.Settings.Default.SelectedFolder;
            if(!string.IsNullOrEmpty(SelectedFolder))
                LogWatcher.StartWatching(SelectedFolder);
        }

        public void BrowseButton()
        {
            var dialog = new Avalon.Windows.Dialogs.FolderBrowserDialog();
            var hasResult = dialog.ShowDialog();
            if (hasResult.HasValue && hasResult.Value)
            {
                SelectedFolder = dialog.SelectedPath;
                LogWatcher.StartWatching(SelectedFolder);
            }
        }

        public void Start()
        {
            StartMonitoring();
        }

        private void StartMonitoring()
        {

        }



    }
}
