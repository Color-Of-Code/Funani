
namespace Funani.Api
{
    using System;
    using System.IO;

    /// <summary>
    /// Description of IFileStorage.
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Initialize and start the file storage service
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the file storage service
        /// </summary>
        void Stop();

        Boolean FileExists(String hash);

        void DeleteFile(String hash);

        String StoreFile(FileInfo file);

        byte[] LoadFile(String hash);
        
        FileInfo LoadThumbnail(String hash, String mime);
        
        FileInfo GetFileInfo(String hash);
    }
}
