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

namespace Funani.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;

	using Funani.Api;
	using Funani.Api.Metadata;

	/// <summary>
	/// FileViewModel
	/// </summary>
	public class FileInformationViewModel : INotifyPropertyChanged
	{
		public FileInformationViewModel(FileInformation fileInformation)
		{
			FileInformation = fileInformation;
		}

        public override string ToString()
        {
            return String.Format("FileInfo: {0}", Hash);
        }

		public FileInformation FileInformation
		{
			get;
			private set;
		}

		public string Hash
		{
			get { return FileInformation.Id; }
		}

		public string Title
		{
			get { return FileInformation.Title; }
		}

		public long FileSize
		{
			get { return FileInformation.FileSize; }
		}

		public string DateTaken
		{
			get { return FileInformation.DateTaken.HasValue ? FileInformation.DateTaken.Value.ToString("yyyy-MM-dd HH:mm:ss") : null; }
		}

		public Int64 Width
		{
			get { return FileInformation.Width; }
		}

		public Int64 Height
		{
			get { return FileInformation.Height; }
		}

		public string Device
		{
			get { return FileInformation.Device; }
		}

		public string ApplicationName
		{
			get { return FileInformation.ApplicationName; }
		}

		public String MimeType
		{
			get { return FileInformation.MimeType; }
		}

        public int? Rating
        {
            get 
            {
                return FileInformation.Rating; 
            }
            set
            {
                if (FileInformation.Rating != value)
                {
                    FileInformation.Rating = value;
                    Funani.Gui.Engine.Funani.Save(FileInformation);
                }
            }
        }
        
        public IList<String> Paths
		{
			get
			{
				return FileInformation.Paths;
			}
		}
        
		public Stretch Stretch
		{
			get
			{
				return Stretch.Uniform;
			}
		}

		public BitmapSource Thumbnail
		{
			get
			{
				FileInfo thumbPath = Funani.Gui.Engine.Funani.GetThumbnail(
					FileInformation.Id, FileInformation.MimeType) as FileInfo;
				object value = thumbPath==null ? null : thumbPath.FullName;
				var bitmap = converter.Convert(value, typeof(BitmapSource), null, null) as BitmapSource;
                return bitmap;
			}
		}

        public BitmapSource Picture
        {
            get
            {
                if (FileInformation.MimeType.StartsWith("image/"))
                {
                    byte[] data = Funani.Gui.Engine.Funani.GetFileData(FileInformation.Id);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(data);
                    bi.EndInit();
                    return bi;
                }
                return null;
            }
        }

        public void RefreshMetadata()
		{
			TriggerPropertyChanged(null);
		}
		
		private const int MaxThumbnailSize = 256;
		private static readonly UriToThumbnailConverter converter = new UriToThumbnailConverter(MaxThumbnailSize);
		
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void TriggerPropertyChanged(string propertyName)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
