
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;

using Catel.IoC;

using Funani.Api;
using Funani.Api.Metadata;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Funani.Metadata
{
    public class MetadataDatabase : IDisposable
    {
        public MetadataDatabase(String pathToMongod, String path)
        {
            _filesystem = new FileSystem();
            _pathToMongod = pathToMongod;
            _pathToDatabase = path;
            _listener = ServiceLocator.Default.ResolveType<IConsoleRedirect>();
        }

        public IMongoDatabase Funani
        {
            get { return _funani; }
        }

        public object Database
        {
            get { return _funani; }
        }

        private readonly IFileSystem _filesystem;

        public IQueryable<FileInformation> FileInformation
        {
            get
            {
                return Funani.GetCollection<FileInformation>("fileinfo")
                    .AsQueryable<FileInformation>();
            }
        }

        public IEnumerable<Funani.Api.Metadata.Tag> Tag
        {
            get
            {
                return Funani.GetCollection<Funani.Api.Metadata.Tag>("tag")
                    .AsQueryable<Funani.Api.Metadata.Tag>();
            }
        }

        private string DatabasePath
        {
            get { return _filesystem.Path.Combine(_pathToDatabase, "metadata"); }
        }

        private string JournalPath
        {
            get { return _filesystem.Path.Combine(DatabasePath, "journal"); }
        }

        #region Private

        private readonly String _pathToDatabase;
        private readonly String _pathToMongod;
        private IMongoDatabase _funani;
        private IMongoClient _mongoClient;
        private Process _process;

        #endregion

        #region Console I/O to listener redirection

        private readonly IConsoleRedirect _listener;

        private void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            IConsoleRedirect l = _listener;
            if (l != null)
                l.OnErrorDataReceived(e.Data);
        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            IConsoleRedirect l = _listener;
            if (l != null)
                l.OnOutputDataReceived(e.Data);
        }

        #endregion

        public FileInformation Retrieve(String hash, IFileInfo file)
        {
            var files = Funani.GetCollection<FileInformation>("fileinfo");
            FileInformation info = null; //TODO: repair: files.FindAsync<FileInformation>(hash).Result;
            if (info == null)
            {
                info = new FileInformation(file);
                files.InsertOne(info);
            }
            else
            {
                info.AddPath(file);
                // TODO: files.Save(info);
            }
            return info;
        }

        public void RefreshMetadata(FileInformation fileinfo, IFileInfo file)
        {
            var files = Funani.GetCollection<FileInformation>("fileinfo");
            fileinfo.RefreshMetadata(file);
            // TODO: files.Save(fileinfo);
        }

        public IEnumerable<String> GetCollectionNames()
        {
            return Funani.ListCollections().ToList().Select(x => x["name"].AsString);
        }

        public DatabaseStatsResult GetStats()
        {
            return null; //TODO: Funani.GetStats();
        }

        public FileInformation Retrieve(IFileInfo file)
        {
            var files = Funani.GetCollection<FileInformation>("fileinfo");
            string path = file.FullName;
            var result = files.AsQueryable<FileInformation>()
                              .Where(x => (x.FileSize == file.Length) && x.Paths.Contains(path));

            // TODO: Check why some have more than one!
            // return result.SingleOrDefault();
            return result.FirstOrDefault();
        }

        public void RemovePath(IFileInfo file)
        {
            var files = Funani.GetCollection<FileInformation>("fileinfo");
            string path = file.FullName;
            var list = new[] { path };
            var queryBuilder = new QueryBuilder<FileInformation>();
            var query = queryBuilder.In(x => x.Paths, list);
            FileInformation info = null; //TODO: files.FindOneAs<FileInformation>(query);
            if (info != null)
            {
                info.Paths.Remove(file.FullName);
                //TODO: files.Save(info);
            }
        }

        public void Save(FileInformation fileinfo)
        {
            if (fileinfo != null)
            {
                var files = Funani.GetCollection<FileInformation>("fileinfo");
                //TODO: files.Save(fileinfo);
            }
        }

        public void Backup()
        {
            var psi = new ProcessStartInfo();
            psi.FileName = _filesystem.Path.Combine(_pathToMongod, "mongodump.exe");
            psi.Arguments = String.Format(" --db Funani --port {0}", 27017);
            psi.WorkingDirectory = _pathToMongod;
            Process.Start(psi).WaitForExit();
        }

        public void Start()
        {
            Stop();

            if (!_filesystem.Directory.Exists(DatabasePath))
                _filesystem.Directory.CreateDirectory(DatabasePath);
            if (!_filesystem.Directory.Exists(JournalPath))
                _filesystem.Directory.CreateDirectory(JournalPath);

            var psi = new ProcessStartInfo();
            psi.FileName = _filesystem.Path.Combine(_pathToMongod, "mongod");
            psi.Arguments = String.Format(" --journal --dbpath \"{0}\" --port {1}",
                                          DatabasePath, 27017);
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            _process = new Process();
            _process.StartInfo = psi;
            _process.OutputDataReceived += _process_OutputDataReceived;
            _process.ErrorDataReceived += _process_ErrorDataReceived;
            _process.EnableRaisingEvents = true;
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            String connectionString = "mongodb://localhost:27017";

            _mongoClient = new MongoClient(connectionString);
            _funani = _mongoClient.GetDatabase("Funani");
        }

        public void Stop()
        {
            if (_process != null)
            {
                //TODO: stop server via command line
                //_mongoClient.Cluster.Stop();
                _mongoClient = null;

                try
                {
                    _process.CloseMainWindow();
                    _process.WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                _process = null;
            }
        }

        #region IDisposable

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}