/*
 * Copyright (c) 2008-2013, Jaap de Haan <jaap.dehaan@color-of-code.de>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of the "Color-Of-Code" nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Funani.Api.Metadata;

namespace Funani.Api
{
    /// <summary>
    ///     Unification of the metadata and file databases inside this interface.
    ///     Hides the complexity of handling the file storage and querying the
    ///     metadata database.
    ///     TODO: maybe split into a connection information and the engine
    /// </summary>
    public interface IEngine : INotifyPropertyChanged
    {
        /// <summary>
        ///     Return the command queue associated with this engine for operations
        ///     performed on a separate worker thread.
        /// </summary>
        ICommandQueue CommandQueue { get; }

        /// <summary>
        ///     Get the database info for the currently open database
        /// </summary>
        DatabaseInfo DatabaseInfo { get; }

        /// <summary>
        ///     Path to the currently open database
        /// </summary>
        String DatabasePath { get; }

        /// <summary>
        ///     Number of files in the database
        /// </summary>
        long TotalFileCount { get; }

        /// <summary>
        ///     Return the file information as a queryable
        /// </summary>
        IQueryable<FileInformation> FileInformation { get; }

        /// <summary>
        ///     Return the metadata database for direct manipulation
        /// </summary>
        object MetadataDatabase { get; }

        /// <summary>
        ///     Allows to check if the provided path points to a valid funani db structure
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Boolean IsValidDatabase(String path);

        /// <summary>
        ///     Opens a funani database initializing the path if it is initially empty
        /// </summary>
        /// <param name="pathToMongod">Path to the directory containing the mongo server executable</param>
        /// <param name="path">Path to the directory containing the funani data</param>
        void OpenDatabase(String pathToMongod, String path);

        /// <summary>
        ///     Closes the funani database and flushes any pending operation to disk
        /// </summary>
        void CloseDatabase();

        /// <summary>
        ///     Dump the database to a file
        /// </summary>
        void Backup();

        /// <summary>
        ///     Add a file to the database (if not already present) and get its info back
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        FileInformation AddFile(FileInfo file);

        /// <summary>
        ///     Remove a file path from the metadata but leaves the file inside the storage area.
        ///     A file gets physically removed only by purging the database.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        void RemoveFile(FileInfo file);

        /// <summary>
        ///     Persist the fileinfo to the metadata database
        /// </summary>
        /// <param name="fileinfo"></param>
        void Save(FileInformation fileinfo);

        /// <summary>
        ///     Get the info associated in mongodb for the path of the specified file.
        ///     If no info could be found, null is returned. This method is intended for
        ///     frequent use and does not make any other check than the path/size/timestamp
        ///     check
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        FileInformation GetFileInformation(FileInfo file);

        /// <summary>
        ///     Return a bitmap source for the thumbnail of this file
        /// </summary>
        /// <param name="hash">Hash of the file to get a thumbnail for</param>
        /// <param name="mime">Mime-type of the file</param>
        /// <returns></returns>
        FileInfo GetThumbnail(String hash, String mime);

        /// <summary>
        ///     Returns the file data of the file with the given hash
        /// </summary>
        /// <param name="hash">SHA1 hash of the file to retrieve</param>
        /// <returns>The file contents as a byte array</returns>
        byte[] GetFileData(String hash);

        /// <summary>
        ///     Refresh the metadata information from the file into the Funani
        ///     database. Useful if the algorithms to extract metadata were
        ///     improved and the database content needs an update.
        /// </summary>
        /// <param name="fileinfo">The file to refresh</param>
        void RefreshMetadata(FileInformation fileinfo);
    }
}