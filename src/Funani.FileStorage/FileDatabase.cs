namespace Funani.FileStorage
{
    using System;
    using System.Diagnostics;
    using System.IO.Abstractions;
    using System.Linq;
    using Funani.Api;
    using Funani.Core.Hash;
    using Funani.Thumbnailer;

    public class FileDatabase : IFileStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDatabase"/> class.
        /// </summary>
        /// <param name="baseDirectory"></param>
        /// <param name="filesystem"></param>
        public FileDatabase(string baseDirectory, IFileSystem filesystem)
        {
            this.BaseDirectory = baseDirectory;
            this.filesystem = filesystem;
            this.Algorithms = new Algorithms(filesystem);
        }

        public void StartService()
        {
            this.Create(); // create if empty
            this.Connect();
        }

        public void StopService()
        {
            this.BaseDirectory = null;
        }

        public string StoreFile(IFileInfo file)
        {
            string hash = this.Algorithms.ComputeSha1(file);
            if (this.FileExists(hash))
            {
                return hash;
            }

            IFileInfo destination = this.GetFileInfo(hash);

            this.filesystem.Directory.CreateDirectory(destination.DirectoryName);
            this.filesystem.File.Copy(file.FullName, destination.FullName);

            // verify the hash, paranoid, but would detect some possible hardware issues
            string hashNew = this.Algorithms.ComputeSha1(destination);
            if (hash != hashNew)
            {
                destination.Delete();
                throw new Exception("Copy not equal to original image (SHA1 hash differs)");
            }

            destination.Attributes = destination.Attributes | System.IO.FileAttributes.ReadOnly;
            return hashNew;
        }

        public void DeleteFile(string hash)
        {
            IFileInfo source = this.GetFileInfo(hash);
            if (source.Exists)
            {
                source.Attributes = System.IO.FileAttributes.Normal;
                source.Delete();
            }
        }

        public bool FileExists(string hash)
        {
            IFileInfo source = this.GetFileInfo(hash);
            return source.Exists;
        }

        public IFileInfo GetFileInfo(string hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            // distribute the data into 2^16 directories in 2 levels and store the files
            // under their hash as filename
            string path = this.filesystem.Path.Combine(this.DataPath, hash.Substring(0, 2), hash.Substring(2, 2), hash);
            return this.filesystem.FileInfo.FromFileName(path);
        }

        public IFileInfo LoadThumbnail(string hash, string mime)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            if (mime == null)
            {
                throw new ArgumentNullException(nameof(mime));
            }

            IFileInfo source = this.GetThumbnailFileInfo(hash);
            if (!source.Exists)
            {
                if (mime.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase))
                {
                    IFileInfo originalImage = this.GetFileInfo(hash);
                    Thumbnail.Create(new Uri(originalImage.FullName), mime, 256, source);

                    // if the thumbnail was not created for some reason...
                    if (!source.Exists)
                    {
                        source = null;
                    }
                }
                else
                {
                    source = null;
                }
            }

            return source;
        }

        public byte[] LoadFile(string hash)
        {
            IFileInfo source = this.GetFileInfo(hash);
            return LoadFileNoException(source);
        }

        private IFileInfo GetThumbnailFileInfo(string hash)
        {
            string path = this.filesystem.Path.Combine(this.ThumbnailPath, hash.Substring(0, 2), hash.Substring(2, 2), hash + ".png");
            return this.filesystem.FileInfo.FromFileName(path);
        }

        private bool Connect()
        {
            return this.filesystem.Directory.Exists(this.DataPath);
        }

        private string BaseDirectory { get; set; }

        private Algorithms Algorithms { get; set; }

        private IFileSystem filesystem;

        private string DataPath
        {
            get { return this.filesystem.Path.Combine(this.BaseDirectory, "data"); }
        }

        private string ThumbnailPath
        {
            get { return this.filesystem.Path.Combine(this.BaseDirectory, "thumbs"); }
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
            return !this.filesystem.Directory.EnumerateFileSystemEntries(path).Any();
        }

        private void Create()
        {
            if (this.IsDirectoryEmpty(this.BaseDirectory) ||
                !this.filesystem.Directory.Exists(this.DataPath))
            {
                this.CreateDataPaths();
            }
        }

        private void CreateDataPaths()
        {
            var baseDir = this.filesystem.DirectoryInfo.FromDirectoryName(this.DataPath);
            baseDir.Create();
            baseDir.Attributes = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Hidden;

            // the other subdirectories are created once needed
        }
    }
}