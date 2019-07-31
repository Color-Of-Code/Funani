﻿
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
        /// Initialize and start the file storage service.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the file storage service.
        /// </summary>
        void Stop();

        bool FileExists(string hash);

        void DeleteFile(string hash);

        string StoreFile(IFileInfo file);

        byte[] LoadFile(string hash);

        IFileInfo LoadThumbnail(string hash, string mime);

        IFileInfo GetFileInfo(string hash);
    }
}
