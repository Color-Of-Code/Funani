/*
 * Copyright (c) 2008-2010, Jaap de Haan <jaap.dehaan@color-of-code.de>
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

	public class Database : IFileStorage
	{
		public Database()
		{
		}

		public void Start(String baseDirectory)
		{
			BaseDirectory = baseDirectory;
			Create(); // create if empty
			Connect();
		}

		public void Stop()
		{
			BaseDirectory = null;
		}

		public String StoreFile(FileInfo file)
		{
			string hash = Utils.ComputeHash.SHA1(file);
			if (FileExists(hash))
				return hash;

			string destination = GetDataPath(hash);
			File.Copy(file.FullName, destination);
			
			// verify the hash, paranoid, but would detect hardware issues
			string hashNew = Utils.ComputeHash.SHA1(new FileInfo(destination));
			if (hash != hashNew)
			{
				File.Delete(destination);
				throw new Exception("Copy not equal to original image");
			}
			File.SetAttributes(destination, FileAttributes.ReadOnly);
			return hashNew;
		}

		public void DeleteFile(string hash)
		{
			string source = GetDataPath(hash);
			if (File.Exists(source))
			{
				File.SetAttributes(source, FileAttributes.Normal);
				File.Delete(source);
			}
		}
		
		public Boolean FileExists(string hash)
		{
			string source = GetDataPath(hash);
			return File.Exists(source);
		}

		public byte[] LoadFile(string hash)
		{
			string source = GetDataPath(hash);
			return LoadFileNoException(source);
		}

		private Boolean Connect()
		{
			if (!Directory.Exists(DataPath))
				return false;
			return true;
		}

		#region Private

		private static byte[] LoadFileNoException(string source)
		{
			try
			{
				return File.ReadAllBytes(source);
			}
			catch
			{
				return null;
			}
		}

		private void Create()
		{
			if (Directory.GetFileSystemEntries(BaseDirectory).Count() != 0)
				throw new Exception("Directory is not empty!");

			CreateDataPaths();
		}

		private string BaseDirectory
		{ get; set; }

		private string DataPath
		{
			get
			{
				return BaseDirectory + @"\Data\";
			}
		}

		private String GetDataPath(String hash)
		{
			return DataPath + hash.Substring(0, 3) + @"\" + hash.Substring(3);
		}

		private void CreateDataPaths()
		{
			string baseDir = DataPath;
			Directory.CreateDirectory(baseDir);
			for (int i = 0; i < 4096; i++)
			{
				String dirName = baseDir + @"\" + i.ToString("X03");
				DirectoryInfo dir = new DirectoryInfo(dirName);
				dir.Create();
				dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
			}
		}

		#endregion
	}
}
