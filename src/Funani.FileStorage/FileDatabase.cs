
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Funani.Api;
using Funani.Api.Utils;
using Funani.Thumbnailer;

namespace Funani.FileStorage
{
    public class FileDatabase : IFileStorage
    {
        public FileDatabase(String baseDirectory)
        {
            BaseDirectory = baseDirectory;
        }

        public void Start()
        {
            Create(); // create if empty
            Connect();
        }

        public void Stop()
        {
            BaseDirectory = null;
        }

        public String StoreFile(FileInfo file)
        {
            string hash = ComputeHash.SHA1(file);
            if (FileExists(hash))
                return hash;

            FileInfo destination = GetFileInfo(hash);
            Debug.Assert(destination != null, "destination filename is null");
            Directory.CreateDirectory(destination.DirectoryName);
            File.Copy(file.FullName, destination.FullName);

            // verify the hash, paranoid, but would detect some possible hardware issues
            string hashNew = ComputeHash.SHA1(destination);
            if (hash != hashNew)
            {
                destination.Delete();
                throw new Exception("Copy not equal to original image (SHA1 hash differs)");
            }
            destination.Attributes = destination.Attributes | FileAttributes.ReadOnly;
            return hashNew;
        }

        public void DeleteFile(String hash)
        {
            FileInfo source = GetFileInfo(hash);
            if (source.Exists)
            {
                source.Attributes = FileAttributes.Normal;
                source.Delete();
            }
        }

        public Boolean FileExists(String hash)
        {
            FileInfo source = GetFileInfo(hash);
            return source.Exists;
        }

        public FileInfo GetFileInfo(String hash)
        {
            // distribute the data into 2^16 directories in 2 levels and store the files
            // under their hash as filename
            String path = Path.Combine(DataPath, hash.Substring(0, 2), hash.Substring(2, 2), hash);
            return new FileInfo(path);
        }

        public FileInfo LoadThumbnail(String hash, String mime)
        {
            FileInfo source = GetThumbnailFileInfo(hash);
            if (!source.Exists)
            {
                if (mime.StartsWith("image/"))
                {
                    FileInfo originalImage = GetFileInfo(hash);
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
            FileInfo source = GetFileInfo(hash);
            return LoadFileNoException(source);
        }

        private FileInfo GetThumbnailFileInfo(String hash)
        {
            String path = Path.Combine(ThumbnailPath, hash.Substring(0, 2), hash.Substring(2, 2), hash + ".png");
            return new FileInfo(path);
        }

        private Boolean Connect()
        {
            if (!Directory.Exists(DataPath))
                return false;
            return true;
        }

        #region Private

        private string BaseDirectory { get; set; }

        private string DataPath
        {
            get { return Path.Combine(BaseDirectory, "data"); }
        }

        private string ThumbnailPath
        {
            get { return Path.Combine(BaseDirectory, "thumbs"); }
        }

        private static byte[] LoadFileNoException(FileInfo source)
        {
            try
            {
                return File.ReadAllBytes(source.FullName);
            }
            catch
            {
                return null;
            }
        }

        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private void Create()
        {
            if (IsDirectoryEmpty(BaseDirectory) || !Directory.Exists(DataPath))
                CreateDataPaths();
        }

        private void CreateDataPaths()
        {
            var baseDir = new DirectoryInfo(DataPath);
            baseDir.Create();
            baseDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            
            // the other subdirectories are created once needed
        }

        #endregion
    }
}