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

namespace Funani.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Funani.Api;
    using Funani.Api.Metadata;
    using Funani.FileStorage;
    using Funani.Metadata;

    public class FunaniEngine : IEngine
    {
        public FunaniEngine()
        {
        }

        private IFileStorage _fileStorage;
        private Metadata.MetadataDatabase _metadata;

        public Boolean IsValidDatabase(String path)
        {
            if (String.IsNullOrWhiteSpace(path) ||
                !Directory.Exists(path) ||
                !Directory.Exists(Path.Combine(path, "metadata")) ||
                !Directory.Exists(Path.Combine(path, "data")))
                return false;
            return true;
        }

        public void OpenDatabase(String pathToMongod, String path, IConsoleRedirect listener)
        {
            // create the file database
            _fileStorage = new FileDatabase(path);
            _fileStorage.Start();

            // create the mongodb
            _metadata = new MetadataDatabase(pathToMongod, path);
            _metadata.Start(listener);
        }

        public void CloseDatabase()
        {
            _fileStorage.Stop();
            _metadata.Stop();
            _fileStorage = null;
            _metadata = null;
        }

        public FileInformation AddFile(FileInfo file)
        {
            var hash = _fileStorage.StoreFile(file);
            return _metadata.Retrieve(hash, file);
        }

        public void RemoveFile(FileInfo file)
        {
            _metadata.RemovePath(file);
        }

        public long GetFileCount()
        {
        	return _metadata.GetFileCount();
        }
        
		public IQueryable<FileInformation> FileInformation
		{
			get
			{
				return _metadata.FileInformation;
			}
		}

		public FileInformation GetFileInformation(FileInfo file)
        {
            return _metadata.Retrieve(file);
        }

        public byte[] GetFileData(String hash)
        {
        	return _fileStorage.LoadFile(hash);
        }
        
        public FileInfo GetFileInfo(String hash)
        {
        	return _fileStorage.GetFileInfo(hash);
        }

        public object MetadataDatabase
        {
        	get
        	{
        		return _metadata.Database;
        	}
        }
    }
}
