namespace Funani.Engine
{
    using System;
    using System.IO.Abstractions;
    using System.Linq;

    using Catel.Data;
    using Catel.IoC;
    using Catel.Runtime.Serialization;

    using Funani.Api;
    using Funani.Api.Metadata;
    using Funani.FileStorage;
    using Funani.Metadata;

    public class FunaniEngine : ObservableObject, IEngine
    {
        private IFileStorage _fileStorage;
        private IFileSystem _filesystem;
        private DatabaseInfo _info;
        private MetadataDatabase _metadata;
        private String _rootPath;

        public FunaniEngine()
        {
            _filesystem = new FileSystem();
            CommandQueue = ServiceLocator.Default.ResolveType<ICommandQueue>();
        }

        private string DatabaseInfoPath
        {
            get { return _filesystem.Path.Combine(_rootPath, "funani.info"); }
        }

        public bool IsValidDatabase(string path)
        {
            if (string.IsNullOrWhiteSpace(path) ||
                !_filesystem.Directory.Exists(path))
                return false;
            if (!_filesystem.File.Exists(_filesystem.Path.Combine(path, "funani.info")))
                return false;
            if (!_filesystem.Directory.Exists(_filesystem.Path.Combine(path, "metadata")) ||
                !_filesystem.Directory.Exists(_filesystem.Path.Combine(path, "data")))
                return false;
            return true;
        }

        public ICommandQueue CommandQueue { get; private set; }

        public DatabaseInfo DatabaseInfo
        {
            get { return _info; }
        }

        public void OpenDatabase(string pathToMongod, string path)
        {
            if (_filesystem.Directory.EnumerateFileSystemEntries(path).Any())
            {
                if (!IsValidDatabase(path))
                    throw new Exception("Invalid Funani Database");
                _rootPath = path;
                CommonCreationOpening(pathToMongod, path);
            }
            else
            {
                CreateDatabase(path);
                CommonCreationOpening(pathToMongod, path);
            }
            if (!IsValidDatabase(path))
                throw new Exception("Invalid Funani Database");
        }

        public void CloseDatabase()
        {
            DatabaseInfo.Save(DatabaseInfoPath, SerializationFactory.GetXmlSerializer());

            _fileStorage.StopService();
            _metadata.Stop();
            _fileStorage = null;
            _metadata = null;
            _rootPath = null;
            _info = null;

            RaisePropertyChanged(string.Empty);
        }

        public void Backup()
        {
            _metadata.Backup();
        }

        public FileInformation AddFile(IFileInfo file)
        {
            string hash = _fileStorage.StoreFile(file);
            FileInformation fileInformation = _metadata.Retrieve(hash, file);
            RaisePropertyChanged("TotalFileCount");
            return fileInformation;
        }

        public void RemoveFile(IFileInfo file)
        {
            _metadata.RemovePath(file);
        }

        public long TotalFileCount
        {
            get
            {
                if (_metadata == null)
                    return 0;
                return _metadata.FileInformation.Count();
            }
        }

        public IQueryable<FileInformation> FileInformation
        {
            get
            {
                if (_metadata == null)
                    return null;
                return _metadata.FileInformation;
            }
        }

        public void Save(FileInformation fileinfo)
        {
            _metadata.Save(fileinfo);
        }

        public FileInformation GetFileInformation(IFileInfo file)
        {
            return _metadata.Retrieve(file);
        }

        public IFileInfo GetThumbnail(String hash, String mime)
        {
            return _fileStorage.LoadThumbnail(hash, mime);
        }

        public byte[] GetFileData(String hash)
        {
            return _fileStorage.LoadFile(hash);
        }

        public void RefreshMetadata(FileInformation fileinfo)
        {
            IFileInfo fi = _fileStorage.GetFileInfo(fileinfo.Id);
            _metadata.RefreshMetadata(fileinfo, fi);
        }

        public object MetadataDatabase
        {
            get
            {
                if (_metadata == null)
                    return null;
                return _metadata.Database;
            }
        }

        public string DatabasePath
        {
            get { return _rootPath; }
        }

        private void CreateDatabase(String path)
        {
            _rootPath = path;
            _info = new DatabaseInfo
            {
                Title = "<Give a title here>",
                Description = "<Write a description here>"
            };
            DatabaseInfo.Save(DatabaseInfoPath, SerializationFactory.GetXmlSerializer());
        }

        private void CommonCreationOpening(String pathToMongod, String path)
        {
            _info = DatabaseInfo.Load(_filesystem.File.OpenRead(DatabaseInfoPath), SerializationFactory.GetXmlSerializer());

            // create the file database
            _fileStorage = new FileDatabase(path, new FileSystem());
            _fileStorage.StartService();

            // create the mongodb
            _metadata = new MetadataDatabase(pathToMongod, path);
            _metadata.Start();

            RaisePropertyChanged(String.Empty);
        }
    }
}