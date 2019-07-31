
using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using Funani.Api;
using Funani.Core.Hash;
using Funani.Thumbnailer;

namespace Funani.FileStorage
{
    public class FileDatabase : IFileStorage
    {
        public FileDatabase(String baseDirectory, IFileSystem filesystem)
        {
            BaseDirectory = baseDirectory;
            _filesystem = filesystem;
            Algorithms = new Algorithms(filesystem);
        }

        public void StartService()
        {
            Create(); // create if empty
            Connect();
        }

        public void StopService()
        {
            BaseDirectory = null;
        }

        public String StoreFile(IFileInfo file)
        {
            string hash = Algorithms.ComputeSha1(file);
            if (FileExists(hash))
                return hash;

            IFileInfo destination = GetFileInfo(hash);

            _filesystem.Directory.CreateDirectory(destination.DirectoryName);
            _filesystem.File.Copy(file.FullName, destination.FullName);

            // verify the hash, paranoid, but would detect some possible hardware issues
            string hashNew = Algorithms.ComputeSha1(destination);
            if (hash != hashNew)
            {
                destination.Delete();
                throw new Exception("Copy not equal to original image (SHA1 hash differs)");
            }
            destination.Attributes = destination.Attributes | System.IO.FileAttributes.ReadOnly;
            return hashNew;
        }

        public void DeleteFile(String hash)
        {
            IFileInfo source = GetFileInfo(hash);
            if (source.Exists)
            {
                source.Attributes = System.IO.FileAttributes.Normal;
                source.Delete();
            }
        }

        public Boolean FileExists(String hash)
        {
            IFileInfo source = GetFileInfo(hash);
            return source.Exists;
        }

        public IFileInfo GetFileInfo(String hash)
        {
            // distribute the data into 2^16 directories in 2 levels and store the files
            // under their hash as filename
            String path = _filesystem.Path.Combine(DataPath, hash.Substring(0, 2), hash.Substring(2, 2), hash);
            return _filesystem.FileInfo.FromFileName(path);
        }

        public IFileInfo LoadThumbnail(String hash, String mime)
        {
            IFileInfo source = GetThumbnailFileInfo(hash);
            if (!source.Exists)
            {
                if (mime.StartsWith("image/"))
                {
                    IFileInfo originalImage = GetFileInfo(hash);
                    Thumbnail.Create(new Uri(originalImage.FullName), mime, 256, source);

                    // if the thumbnail was not created for some reason...
                    if (!source.Exists)
                        source = null;
                }
                else
                    source = null;
            }
            return source;
        }

        public byte[] LoadFile(String hash)
        {
            IFileInfo source = GetFileInfo(hash);
            return LoadFileNoException(source);
        }

        private IFileInfo GetThumbnailFileInfo(String hash)
        {
            String path = _filesystem.Path.Combine(ThumbnailPath, hash.Substring(0, 2), hash.Substring(2, 2), hash + ".png");
            return _filesystem.FileInfo.FromFileName(path);
        }

        private Boolean Connect()
        {
            if (!_filesystem.Directory.Exists(DataPath))
                return false;
            return true;
        }

        #region Private

        private string BaseDirectory { get; set; }
        private Algorithms Algorithms { get; set; }
        private IFileSystem _filesystem;
        
        private string DataPath
        {
            get { return _filesystem.Path.Combine(BaseDirectory, "data"); }
        }

        private string ThumbnailPath
        {
            get { return _filesystem.Path.Combine(BaseDirectory, "thumbs"); }
        }

        private static byte[] LoadFileNoException(IFileInfo source)
        {
            try
            {
                return source.FileSystem.File.ReadAllBytes(source.FullName);
            }
            catch
            {
                return null;
            }
        }

        private bool IsDirectoryEmpty(string path)
        {
            return !_filesystem.Directory.EnumerateFileSystemEntries(path).Any();
        }

        private void Create()
        {
            if (IsDirectoryEmpty(BaseDirectory) || !_filesystem.Directory.Exists(DataPath))
                CreateDataPaths();
        }

        private void CreateDataPaths()
        {
            var baseDir = _filesystem.DirectoryInfo.FromDirectoryName(DataPath);
            baseDir.Create();
            baseDir.Attributes = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Hidden;

            // the other subdirectories are created once needed
        }

        #endregion
    }
}