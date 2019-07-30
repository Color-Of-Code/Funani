
namespace Funani.Api
{
    using System;
    using System.IO.Abstractions;

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

        String StoreFile(IFileInfo file);

        byte[] LoadFile(String hash);
        
        IFileInfo LoadThumbnail(String hash, String mime);
        
        IFileInfo GetFileInfo(String hash);
    }
}
