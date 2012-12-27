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
			MimeType = MimeExtractor.MimeType.Extract(file);
			ExtractMetadata(file);
			AddPath(file);
		}

		public void AddPath(FileInfo file)
		{
			if (!Paths.Contains(file.FullName))
				Paths.Add(file.FullName);
		}
		
		private void ExtractMetadata(FileInfo file)
		{
			var uri = new Uri(file.FullName);
            var metadata = MetadataExtractor.Metadata.Extract(uri, MimeType);
			if (metadata != null)
			{
				if (metadata.ContainsKey("Width"))
				    Width = Convert.ToInt64(metadata["Width"]);
				if (metadata.ContainsKey("Height"))
				    Height = Convert.ToInt64(metadata["Height"]);
				
				if (metadata.ContainsKey("Device"))
                    Device = metadata["Device"];
				if (metadata.ContainsKey("DateTaken"))
					DateTaken = DateTime.ParseExact(metadata["DateTaken"], "dd.MM.yyyy HH:mm:ss", null);
				if (metadata.ContainsKey("ApplicationName"))
                    ApplicationName = metadata["ApplicationName"];
			}
		}
		
		public String Id { get; private set; }
		public Int64  FileSize { get; private set; }
		public String MimeType { get; private set; }

        public IList<String> Paths { get; private set; }
        
        public Int64 Width { get; private set; }
        public Int64 Height { get; private set; }

        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        
        public String Title { get; set; }

		public DateTime? DateTaken { get; set; }    // start date for video
        public Int64 Duration { get; set; }         // for videos, sound

        public String Device { get; set; }          // digitalizing device
        public String ApplicationName { get; set; } // application used to process the data
        
        // 0 -> 5
        public int Rating { get; set; }

        // tagging
        public IList<String> EventTags { get; private set; }
        public IList<String> PeopleTags { get; private set; }
        public IList<String> LocationTags { get; private set; }
        public IList<String> ObjectTags { get; private set; }
        public IList<String> OtherTags { get; private set; }
    }
}
