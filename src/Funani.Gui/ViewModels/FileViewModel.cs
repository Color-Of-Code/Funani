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
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Catel.Data;
using Catel.MVVM;
using Funani.Api;
using Funani.Gui.Converters;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     FileViewModel
    /// </summary>
    public class FileViewModel : ViewModelBase
    {
        private const int MaxThumbnailSize = 120;
        private static readonly UriToThumbnailConverter Converter = new UriToThumbnailConverter(MaxThumbnailSize);

        private BitmapSource _thumbnail;
        private readonly IEngine _engine;

        public FileViewModel(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
            _engine = GetService<IEngine>();
            UpdateInsideFunani();

            Store = new Command(OnStoreExecute);
            Remove = new Command(OnRemoveExecute);
        }

        public FileInfo FileInfo { get; private set; }

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
                return Thumbnail.PixelWidth;
            }
        }

        public double ThumbnailHeight
        {
            get
            {
                if (MaxThumbnailSize < Thumbnail.PixelHeight)
                    return double.NaN;
                return Thumbnail.PixelHeight;
            }
        }

        public Stretch Stretch
        {
            get { return Stretch.Uniform; }
        }

        public BitmapSource Thumbnail
        {
            get
            {
                return _thumbnail ??
                       (_thumbnail = Converter.Convert(FullName, typeof(BitmapSource), null, null) as BitmapSource);
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

        /// <summary>
        /// Is this file alread inside the storage area.
        /// </summary>
        public bool? IsStored
        {
            get { return GetValue<bool?>(IsStoredProperty); }
            set { SetValue(IsStoredProperty, value); }
        }

        /// <summary>
        /// Register the IsStored property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IsStoredProperty = RegisterProperty("IsStored", typeof(bool?), null);


        /// <summary>
        /// Gets the Store command.
        /// </summary>
        public Command Store { get; private set; }

        /// <summary>
        /// Store this file into the Funani database
        /// </summary>
        private void OnStoreExecute()
        {
            try
            {
                IsStored = null;
                _engine.AddFile(FileInfo);
            }
            finally
            {
                UpdateInsideFunani();
            }
        }

        /// <summary>
        /// Gets the Remove command.
        /// </summary>
        public Command Remove { get; private set; }

        /// <summary>
        /// Remove the file from the Funani database
        /// </summary>
        private void OnRemoveExecute()
        {
            try
            {
                IsStored = null;
                _engine.RemoveFile(FileInfo);
            }
            finally
            {
                UpdateInsideFunani();
            }
        }

        private void AddOrRemoveFile(bool value)
        {
            if (value)
                OnStoreExecute();
            else
                OnRemoveExecute();
        }

        private void UpdateInsideFunani()
        {
            IsStored = _engine.GetFileInformation(FileInfo) != null;
        }
    }
}