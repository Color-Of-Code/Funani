using System;
using System.IO;
using System.Windows;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.MVVM.Services;
using Funani.Api;
using Funani.Gui.Properties;
using SWF = System.Windows.Forms;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private static readonly String MongodFileFilter = "Mongo Server (mongod.exe)|mongod.exe";
        private readonly IEngine _engine;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel(IEngine engine)
        {
            _engine = engine;

            ApplicationExit = new Command(OnApplicationExitExecute);
            BrowseToFunanidb = new Command(OnBrowseToFunanidbExecute);
            BrowseToMongod = new Command(OnBrowseToMongodExecute);

            Settings = Settings.Default;
            Settings.Upgrade();
        }

        protected override void Initialize()
        {
            EnsureMongodbPathIsValid();
            EnsureFunanidbPathIsValid();
            _engine.OpenDatabase(MongodbPath, Settings.LastFunaniDatabase);
        }

        protected override void OnClosed(bool? result)
        {
            _engine.CloseDatabase();
            base.OnClosed(result);
        }

        #endregion

        #region Properties

        #region Property: Title

        public override string Title
        {
            get { return "MainWindow"; }
        }

        #endregion

        [Model]
        public Settings Settings { get; set; }


        [ViewModelToModel("Settings")]
        public String MongodbPath { get; set; }

        private void OnMongodbPathChanged()
        {
            Settings.Save();
            IsMongodbPathValid = GetIsMongodbPathValid(MongodbPath);
        }

        public bool IsMongodbPathValid { get; set; }

        private static bool GetIsMongodbPathValid(String path)
        {
            if (String.IsNullOrWhiteSpace(path) ||
                !Directory.Exists(path) ||
                !File.Exists(Path.Combine(path, "mongod.exe")))
                return false;
            return true;
        }

        #endregion

        #region Commands

        #region Command: ApplicationExit

        public Command ApplicationExit { get; private set; }

        private void OnApplicationExitExecute()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Command: BrowseToMongod

        public Command BrowseToMongod { get; private set; }

        private void OnBrowseToMongodExecute()
        {
            var openFileService = DependencyResolver.Resolve<IOpenFileService>();
            openFileService.Title = "Browse to the mongoDB executable";
            openFileService.Filter = MongodFileFilter;
            if (openFileService.DetermineFile())
            {
                var fi = new FileInfo(openFileService.FileName);
                if (GetIsMongodbPathValid(fi.DirectoryName))
                {
                    MongodbPath = fi.DirectoryName;
                }
            }
        }

        private void EnsureMongodbPathIsValid()
        {
            if (!IsMongodbPathValid)
            {
                OnBrowseToMongodExecute();
                if (!IsMongodbPathValid)
                    OnApplicationExitExecute();
            }
        }

        #endregion

        #region Command: BrowseToMongod

        public Command BrowseToFunanidb { get; private set; }

        private void OnBrowseToFunanidbExecute()
        {
            var ofd = new SWF.FolderBrowserDialog();
            ofd.Description = "Browse to a valid Funani DB or empty directory";
            ofd.ShowNewFolderButton = true;
            if (ofd.ShowDialog() == SWF.DialogResult.OK)
            {
                var di = new DirectoryInfo(ofd.SelectedPath);
                Settings.LastFunaniDatabase = di.FullName;
                Settings.Save();
            }
        }

        #endregion

        #endregion

        #region Methods

        private void EnsureFunanidbPathIsValid()
        {
            String funanidbPath = Settings.LastFunaniDatabase;
            if (!_engine.IsValidDatabase(funanidbPath))
            {
                var ofd = new SWF.FolderBrowserDialog();
                ofd.Description = "Browse to a valid Funani DB or empty directory";
                ofd.ShowNewFolderButton = true;
                if (ofd.ShowDialog() == SWF.DialogResult.OK)
                {
                    var di = new DirectoryInfo(ofd.SelectedPath);
                    Settings.LastFunaniDatabase = di.FullName;
                    Settings.Save();
                    return;
                }
                OnApplicationExitExecute();
            }
        }

        #endregion
    }
}