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

namespace Funani.Api.Metadata
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Windows.Media.Imaging;

	public class FileInformation
	{
		public FileInformation()
		{
			Paths = new List<String>();
		}

		public FileInformation(FileInfo file)
			: this()
		{
			Id = Utils.ComputeHash.SHA1(file);
			FileSize = file.Length;
			Title = file.Name;
			DetectMimeType(file);
			ExtractMetadata(file);
			AddPath(file);
		}

		public void AddPath(FileInfo file)
		{
			if (!Paths.Contains(file.FullName))
				Paths.Add(file.FullName);
		}
		
		private void DetectMimeType(FileInfo file)
		{
			String mime;
			String extension = file.Extension.ToLowerInvariant();
			switch (extension)
			{
				case ".gif":
					mime = "image/gif";
					break;
				case ".jpe":
				case ".jpeg":
				case ".jpg":
					mime = "image/jpeg";
					break;
				case ".png":
					mime = "image/png";
					break;
				case ".tiff":
					mime = "image/tiff";
					break;
				default:
					throw new NotSupportedException("MIME type not recognized for file " + file.Name);
			}
			MimeType = mime;
		}

		private void ExtractMetadata(FileInfo file)
		{
			try
			{
				var uri = new Uri(file.FullName);
				BitmapFrame frame = BitmapFrame.Create(uri, BitmapCreateOptions.None, BitmapCacheOption.None);
				BitmapMetadata meta = frame.Metadata as BitmapMetadata;
				if (meta != null)
				{
					DateTaken = DateTime.ParseExact(meta.DateTaken, "dd.MM.yyyy HH:mm:ss", null);
					ApplicationName = meta.ApplicationName;
				}
			}
			catch
			{
				// ignore silently
			}
		}
		
		public String Id { get; private set; }
		public Int64  FileSize { get; private set; }
		public String MimeType { get; private set; }
		
		public String Title { get; set; }
		public IList<String> Paths { get; private set; }

		public DateTime? DateTaken { get; private set; }
		public String ApplicationName { get; private set; }

	}
}
