
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Catel.Data;
using Catel.IoC;
using Catel.MVVM;

using Funani.Api;
using Funani.Gui.Converters;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     View model for a file on the filesystem
    /// </summary>
    public class FileViewModel : ViewModelBase
    {
        private readonly IEngine _engine;

        public FileViewModel(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
            _engine = DependencyResolver.Resolve<IEngine>();
            UpdateInsideFunani();

            Store = new Command(OnStoreExecute);
            Remove = new Command(OnRemoveExecute);
        }

        public FileInfo FileInfo { get; private set; }

        public string Name
        {
            get { return FileInfo.Name; }
        }

        public string FullName
        {
            get { return FileInfo.FullName; }
        }

        public long Length
        {
            get { return FileInfo.Length; }
        }

        public DateTime LastWriteTime
        {
            get { return FileInfo.LastWriteTime; }
        }

        #region Property: IsStored

        /// <summary>
        /// Is this file alread inside the storage area.
        /// </summary>
        public bool? IsStored { get; set; }

        #endregion

        #region Command: Store

        /// <summary>
        /// Gets the Store command.
        /// </summary>
        public Command Store { get; private set; }

        /// <summary>
        /// Store this file into the Funani database
        /// </summary>
        private void OnStoreExecute()
        {
            try
            {
                IsStored = null;
                _engine.AddFile(FileInfo);
            }
            finally
            {
                UpdateInsideFunani();
            }
        }

        #endregion

        #region Command: Remove

        /// <summary>
        /// Gets the Remove command.
        /// </summary>
        public Command Remove { get; private set; }

        /// <summary>
        /// Remove the file from the Funani database
        /// </summary>
        private void OnRemoveExecute()
        {
            try
            {
                IsStored = null;
                _engine.RemoveFile(FileInfo);
            }
            finally
            {
                UpdateInsideFunani();
            }
        }

        #endregion

        #region Helpers

        private void AddOrRemoveFile(bool value)
        {
            if (value)
                OnStoreExecute();
            else
                OnRemoveExecute();
        }

        private void UpdateInsideFunani()
        {
            IsStored = _engine.GetFileInformation(FileInfo) != null;
        }

        #endregion
    }
}