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

namespace Funani.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Reflection;
    using System.Threading;
    using System.Diagnostics;

    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using Funani.Api.Metadata;

    public class MetadataDatabase
    {
        public MetadataDatabase(String pathToMongod, String path)
        {
            _pathToMongod = pathToMongod;
            _pathToDatabase = path;
        }

        public MongoDatabase Funani
        { get { return _funani; } }

        public FileInformation Retrieve(String hash, FileInfo file)
        {
            var files = Funani.GetCollection("fileinfo");
            var info = files.FindOneAs<FileInformation>(Query.EQ("_id", hash));
            if (info == null)
            {
                info = new FileInformation(file);
                files.Insert<FileInformation>(info);
            }
            return info;
        }

        public void Start()
        {
            Stop();

            if (!Directory.Exists(DatabasePath))
                Directory.CreateDirectory(DatabasePath);
            if (!Directory.Exists(JournalPath))
                Directory.CreateDirectory(JournalPath);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(_pathToMongod, "mongod");
            psi.Arguments = String.Format(" --journal --dbpath \"{0}\" --port {1}",
                                          DatabasePath, 27017);
            _process = Process.Start(psi);

            String connectionString = "mongodb://localhost:27017";

            _mongoClient = new MongoClient(connectionString);
            _mongoServer = _mongoClient.GetServer();
            _funani = _mongoServer.GetDatabase("Funani");
        }

        public void Stop()
        {
            if (_process != null)
            {
                if (_mongoServer != null)
                {
                    try
                    {
                        // according to the documentation there should not be a need to call disconnect
                        _mongoServer.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                _mongoServer = null;
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


        private string DatabasePath
        {
            get
            {
                return Path.Combine(_pathToDatabase, "metadata");
            }
        }

        private string JournalPath
        {
            get
            {
                return Path.Combine(DatabasePath, "journal");
            }
        }


        #region Private
        private Process _process = null;
        private String _pathToDatabase;
        private String _pathToMongod;
        private MongoClient _mongoClient = null;
        private MongoServer _mongoServer = null;
        private MongoDatabase _funani = null;
        #endregion

    }
}
