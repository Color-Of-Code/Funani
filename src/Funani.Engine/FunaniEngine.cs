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
using System.Xml;
using System.Xml.Serialization;
using Funani.Api;
using Funani.Api.Metadata;
using Funani.FileStorage;
using Funani.Metadata;

namespace Funani.Engine
{
    public class FunaniEngine : IEngine
    {
        private IFileStorage _fileStorage;
        private DatabaseInfo _info;
        private MetadataDatabase _metadata;
        private String _rootPath;

        public FunaniEngine()
        {
            CommandQueue = new FunaniCommandQueue();
        }

        private String DatabaseInfoPath
        {
            get { return Path.Combine(_rootPath, "funani.info"); }
        }

        public Boolean IsValidDatabase(String path)
        {
            if (String.IsNullOrWhiteSpace(path) ||
                !Directory.Exists(path))
                return false;
            if (!File.Exists(Path.Combine(path, "funani.info")))
                return false;
            if (!Directory.Exists(Path.Combine(path, "metadata")) ||
                !Directory.Exists(Path.Combine(path, "data")))
                return false;
            return true;
        }

        public ICommandQueue CommandQueue { get; private set; }

        public DatabaseInfo DatabaseInfo
        {
            get { return _info; }
        }

        public void OpenDatabase(String pathToMongod, String path, IConsoleRedirect listener)
        {
            if (Directory.EnumerateFileSystemEntries(path).Any())
            {
                if (!IsValidDatabase(path))
                    throw new Exception("Invalid Funani Database");
                _rootPath = path;
                CommonCreationOpening(pathToMongod, path, listener);
            }
            else
            {
                CreateDatabase(path);
                CommonCreationOpening(pathToMongod, path, listener);
            }
            if (!IsValidDatabase(path))
                throw new Exception("Invalid Funani Database");
        }

        public void CloseDatabase()
        {
            SaveDatabaseInfo();

            _fileStorage.Stop();
            _metadata.Stop();
            _fileStorage = null;
            _metadata = null;
            _rootPath = null;
            _info = null;

            TriggerPropertyChanged(null);
        }

        public void Backup()
        {
            _metadata.Backup();
        }

        public FileInformation AddFile(FileInfo file)
        {
            string hash = _fileStorage.StoreFile(file);
            FileInformation fileInformation = _metadata.Retrieve(hash, file);
            TriggerPropertyChanged("TotalFileCount");
            return fileInformation;
        }

        public void RemoveFile(FileInfo file)
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

        public FileInformation GetFileInformation(FileInfo file)
        {
            return _metadata.Retrieve(file);
        }

        public FileInfo GetThumbnail(String hash, String mime)
        {
            return _fileStorage.LoadThumbnail(hash, mime);
        }

        public byte[] GetFileData(String hash)
        {
            return _fileStorage.LoadFile(hash);
        }

        public void RefreshMetadata(FileInformation fileinfo)
        {
            FileInfo fi = _fileStorage.GetFileInfo(fileinfo.Id);
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void TriggerPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void CreateDatabase(String path)
        {
            _rootPath = path;
            _info = new DatabaseInfo
                {
                    Title = "<Give a title here>",
                    Description = "<Write a description here>"
                };
            SaveDatabaseInfo();
        }

        private void CommonCreationOpening(String pathToMongod, String path, IConsoleRedirect listener)
        {
            var s = new XmlSerializer(typeof (DatabaseInfo));
            using (XmlReader reader = XmlReader.Create(DatabaseInfoPath))
            {
                _info = s.Deserialize(reader) as DatabaseInfo;
            }

            // create the file database
            _fileStorage = new FileDatabase(path);
            _fileStorage.Start();

            // create the mongodb
            _metadata = new MetadataDatabase(pathToMongod, path);
            _metadata.Start(listener);

            TriggerPropertyChanged(null);
        }

        private void SaveDatabaseInfo()
        {
            var s = new XmlSerializer(typeof (DatabaseInfo));
            using (XmlWriter writer = XmlWriter.Create(DatabaseInfoPath))
            {
                s.Serialize(writer, _info);
            }
        }
    }
}