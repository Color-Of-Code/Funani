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

namespace Funani.FileStorage
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Linq;
	using System.Drawing;
	using System.Text;

	using Funani.Api;
    using Funani.Api.Utils;

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
			Directory.CreateDirectory(destination.DirectoryName);
			File.Copy(file.FullName, destination.FullName);
			
			// verify the hash, paranoid, but would detect hardware issues
			string hashNew = ComputeHash.SHA1(destination);
			if (hash != hashNew)
			{
				destination.Delete();
				throw new Exception("Copy not equal to original image");
			}
			destination.Attributes = destination.Attributes | FileAttributes.ReadOnly;
			return hashNew;
		}

		public void DeleteFile(string hash)
		{
			FileInfo source = GetFileInfo(hash);
			if (source.Exists)
			{
				source.Attributes = FileAttributes.Normal;
				source.Delete();
			}
		}
		
		public Boolean FileExists(string hash)
		{
			FileInfo source = GetFileInfo(hash);
			return source.Exists;
		}

		public FileInfo GetFileInfo(string hash)
		{
			// distribute the data into 2^16 directories in 2 levels and store the files
			// under their hash as filename
			String path = Path.Combine(DataPath, hash.Substring(0, 2), hash.Substring(2, 2), hash);
			return new FileInfo(path);
		}
		
		public byte[] LoadFile(string hash)
		{
			FileInfo source = GetFileInfo(hash);
			return LoadFileNoException(source);
		}

		private Boolean Connect()
		{
			if (!Directory.Exists(DataPath))
				return false;
			return true;
		}

		#region Private

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

		private string BaseDirectory
		{ get; set; }

		private string DataPath
		{
			get
			{
				return Path.Combine(BaseDirectory, "data");
			}
		}

		private void CreateDataPaths()
		{
			DirectoryInfo baseDir = new DirectoryInfo(DataPath);
			baseDir.Create();
			baseDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
			// the other subdirectories are created once needed
		}

		#endregion
	}
}
