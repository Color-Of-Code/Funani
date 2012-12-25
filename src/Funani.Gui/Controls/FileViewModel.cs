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
	using System.ComponentModel;
	using System.IO;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;

	using Funani.Api;

	/// <summary>
	/// FileViewModel
	/// </summary>
	public class FileViewModel : INotifyPropertyChanged
	{
		public FileViewModel(String hash)
		{
			Hash = hash;
		}

		public FileViewModel(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
		}

		private FileInfo _fileInfo;
		public FileInfo FileInfo
		{
			get 
			{
				if (Hash != null)
				{
					if (_fileInfo == null)
					{
						_fileInfo = Engine.Funani.GetFileInfo(Hash);
					}
				}
				return _fileInfo;
			}
		}

		public String Hash
		{
			get;
			private set;
		}

		public string Name
		{
			get { return FileInfo.Name; }
		}

		public string FullName
		{
			get { return FileInfo.FullName; }
		}

		public long Length
		{
			get { return FileInfo.Length; }
		}

		public DateTime LastWriteTime
		{
			get { return FileInfo.LastWriteTime; }
		}

		public double ThumbnailWidth
		{
			get 
            {
                if (MaxThumbnailSize < Thumbnail.PixelWidth)
                    return double.NaN;
                else
                    return Thumbnail.PixelWidth;
            }
		}

		public double ThumbnailHeight
		{
			get 
            {
                if (MaxThumbnailSize < Thumbnail.PixelHeight)
                    return double.NaN;
                else
                    return Thumbnail.PixelHeight;
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
				if (_thumbnail == null)
					_thumbnail = converter.Convert(FullName, typeof(BitmapSource), null, null) as BitmapSource;
				return _thumbnail;
			}
		}

		public BitmapScalingMode ScalingMode
		{
			get
			{
				if (ThumbnailWidth < MaxThumbnailSize && ThumbnailHeight < MaxThumbnailSize)
					return BitmapScalingMode.Linear;
				return BitmapScalingMode.HighQuality;
			}
		}

		private Boolean? _insideFunani;
		public Boolean InsideFunani
		{
			get
			{
				if (!_insideFunani.HasValue)
				{
					if (Hash != null)
						_insideFunani = true;
					else
						_insideFunani = Engine.Funani.GetFileInformation(FileInfo) != null;
				}
				return (bool)_insideFunani;
			}
			set
			{
				if (InsideFunani != value)
				{
					_insideFunani = null; // trigger readback from metadata
					if (value)
					{
						// add
						Engine.Funani.AddFile(FileInfo);
					}
					else
					{
						// remove
						Engine.Funani.RemoveFile(FileInfo);
					}
					TriggerPropertyChanged("InsideFunani");
				}
			}
		}

		private BitmapSource _thumbnail;
		private const int MaxThumbnailSize = 120;
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
