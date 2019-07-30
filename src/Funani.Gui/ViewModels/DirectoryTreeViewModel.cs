
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

using Catel.Data;
using Catel.MVVM;

using Funani.Api;
using Funani.Engine.Commands;
using Funani.Gui.Properties;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    /// The top level filesystem drives
    /// </summary>
    public class DirectoryTreeViewModel : ViewModelBase
    {
        private readonly IEngine _engine;
        private readonly ObservableCollection<DirectoryViewModel> _firstGeneration;

        public DirectoryTreeViewModel(IEngine engine)
        {
            _engine = engine;

            // register commands
            UploadAllInThisDirectory = new Command(OnUploadAllInThisDirectoryExecute, OnUploadAllInThisDirectoryCanExecute);
            UploadAllRecursively = new Command(OnUploadAllRecursivelyExecute, OnUploadAllRecursivelyCanExecute);

            IEnumerable<DirectoryInfo> rootDirectories = Directory.GetLogicalDrives().Select(x => new DirectoryInfo(x));
            _firstGeneration = new ObservableCollection<DirectoryViewModel>();
            foreach (DirectoryInfo model in rootDirectories)
            {
                try
                {
                    _firstGeneration.Add(new DirectoryViewModel(model));
                }
                catch (Exception ex)
                {
                    _firstGeneration.Add(new DirectoryViewModel(model, null, ex));
                }
            }

            Settings settings = Settings.Default;
            if (!String.IsNullOrWhiteSpace(settings.LastDirectoryExplorerSelectedPath))
            {
                ExpandAndSelect(new DirectoryInfo(settings.LastDirectoryExplorerSelectedPath));
            }
        }

        public ObservableCollection<DirectoryViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        #region Property: SelectedDirectory
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public DirectoryViewModel SelectedDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Called when the SelectedDirectory property has changed.
        /// </summary>
        private void OnSelectedDirectoryChanged()
        {
            DirectoryViewModel item = SelectedDirectory;
            if (item != null)
            {
                DirectoryInfo di = item.DirectoryInfo;
                if (di != null)
                {
                    ExpandAndSelect(di);
                    Settings settings = Settings.Default;
                    settings.LastDirectoryExplorerSelectedPath = di.FullName;
                    settings.Save();
                }
            }
        }
        #endregion

        #region Command: UploadAllInThisDirectory
        /// <summary>
        /// Gets the UploadAllInThisDirectory command.
        /// </summary>
        public Command UploadAllInThisDirectory { get; private set; }

        /// <summary>
        /// Method to check whether the UploadAllInThisDirectory command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnUploadAllInThisDirectoryCanExecute()
        {
            return SelectedDirectory != null;
        }

        /// <summary>
        /// Method to invoke when the UploadAllInThisDirectory command is executed.
        /// </summary>
        private void OnUploadAllInThisDirectoryExecute()
        {
            DirectoryViewModel dvm = SelectedDirectory;
            if (dvm == null)
                return;
            DirectoryInfo di = dvm.DirectoryInfo;
            var t = new Thread(() =>
                               AddFilesInDirectory(di, false));
            t.Start();
        }
        #endregion

        #region Command: UploadAllRecursively
        /// <summary>
        /// Gets the UploadAllRecursively command.
        /// </summary>
        public Command UploadAllRecursively { get; private set; }

        /// <summary>
        /// Method to check whether the UploadAllRecursively command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnUploadAllRecursivelyCanExecute()
        {
            return SelectedDirectory != null;
        }

        /// <summary>
        /// Method to invoke when the UploadAllRecursively command is executed.
        /// </summary>
        private void OnUploadAllRecursivelyExecute()
        {
            DirectoryViewModel dvm = SelectedDirectory;
            if (dvm == null)
                return;
            DirectoryInfo di = dvm.DirectoryInfo;
            var t = new Thread(() =>
                               AddFilesInDirectory(di, true));
            t.Start();
        }
        #endregion

        #region Helpers
        private DirectoryViewModel LookupRoot(DirectoryInfo path)
        {
            return FirstGeneration.FirstOrDefault(x => x.DirectoryInfo.Name == path.Root.Name);
        }

        private DirectoryViewModel Lookup(DirectoryInfo path)
        {
            DirectoryViewModel root = LookupRoot(path);
            if (root != null)
                return root.Lookup(path);
            return null;
        }

        private void AddFilesInDirectory(DirectoryInfo di, bool recurse)
        {
            foreach (FileInfo fi in di.EnumerateFiles())
            {
                _engine.CommandQueue.AddCommand(new AddFileCommand(_engine, fi));
            }
            if (recurse)
            {
                foreach (DirectoryInfo sdi in di.EnumerateDirectories())
                {
                    AddFilesInDirectory(sdi, true);
                }
            }
        }

        private void ExpandAndSelect(DirectoryInfo path)
        {
            DirectoryViewModel vm = Lookup(path);
            if (vm != null)
            {
                vm.IsExpanded = true;
                vm.IsSelected = true;
            }
        }
        #endregion
    }
}