using System;
using System.IO;
using System.Windows;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Funani.Api;
using Funani.Gui.Properties;
using System.Threading.Tasks;

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
        private readonly ISelectDirectoryService _selectDirectoryService;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel(IEngine engine, ISelectDirectoryService selectDirectoryService)
        {
            _engine = engine;
            _selectDirectoryService = selectDirectoryService;


            ApplicationExit = new Command(OnApplicationExitExecute);
            BrowseToFunanidb = new Command(OnBrowseToFunanidbExecute);
            BrowseToMongod = new Command(OnBrowseToMongodExecute);

            Settings = Settings.Default;
            Settings.Upgrade();
        }

        protected override Task InitializeAsync()
        {
            EnsureMongodbPathIsValid();
            EnsureFunanidbPathIsValid();
            _engine.OpenDatabase(MongodbPath, Settings.LastFunaniDatabase);
            return base.InitializeAsync();
        }

        protected override Task OnClosedAsync(bool? result)
        {
            _engine.CloseDatabase();
            return base.OnClosedAsync(result);
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
            _selectDirectoryService.Title = "Browse to a valid Funani DB or empty directory";
            _selectDirectoryService.ShowNewFolderButton = true;
            if (_selectDirectoryService.DetermineDirectory())
            {
                var di = new DirectoryInfo(_selectDirectoryService.DirectoryName);
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
                OnBrowseToFunanidbExecute();
                OnApplicationExitExecute();
            }
        }

        #endregion
    }
}