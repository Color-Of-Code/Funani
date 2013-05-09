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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Catel.Data;
using Catel.MVVM;
using Funani.Api;
using Funani.Api.Metadata;
using Funani.Gui.Controls;
using Funani.Gui.Converters;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     FileViewModel
    /// </summary>
    public class FileInformationViewModel : ViewModelBase
    {
        private const int MaxThumbnailSize = 256;
        private static readonly UriToThumbnailConverter Converter = new UriToThumbnailConverter(MaxThumbnailSize);

        public FileInformationViewModel(FileInformation fileInformation, IEngine engine)
        {
            FileInformation = fileInformation;
            _engine = engine;
        }

        private readonly IEngine _engine;

        /// <summary>
        /// FileInformation model.
        /// </summary>
        public FileInformation FileInformation
        {
            get;
            private set;
        }

        public string DateTaken
        {
            get
            {
                return FileInformation.DateTaken.HasValue
                           ? FileInformation.DateTaken.Value.ToString("yyyy-MM-dd HH:mm:ss")
                           : null;
            }
        }

        public int? Angle
        {
            get { return FileInformation.Angle; }
            set
            {
                if (FileInformation.Angle != value)
                {
                    FileInformation.Angle = value;
                    _engine.Save(FileInformation);
                }
            }
        }

        public int? Rating
        {
            get { return FileInformation.Rating; }
            set
            {
                if (FileInformation.Rating != value)
                {
                    FileInformation.Rating = value;
                    _engine.Save(FileInformation);
                }
            }
        }

        public bool IsDeleted
        {
            get { return FileInformation.IsDeleted; }
            set
            {
                if (FileInformation.IsDeleted != value)
                {
                    FileInformation.IsDeleted = value;
                    _engine.Save(FileInformation);
                }
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
                FileInfo thumbPath = _engine.GetThumbnail(
                    FileInformation.Id, FileInformation.MimeType);
                object value = thumbPath == null ? null : thumbPath.FullName;
                var bitmap = Converter.Convert(value, typeof(BitmapSource), null, null) as BitmapSource;
                return bitmap;
            }
        }

        public BitmapSource Picture
        {
            get
            {
                if (FileInformation.MimeType.StartsWith("image/"))
                {
                    byte[] data = _engine.GetFileData(FileInformation.Id);
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(data);
                    bi.EndInit();

                    if (Angle.HasValue)
                    {
                        int angle = Angle.Value;
                        if (angle != 0)
                        {
                            return new TransformedBitmap(bi.Clone(), new RotateTransform(angle));
                        }
                    }

                    return bi;
                }
                return null;
            }
        }

        public override string ToString()
        {
            return String.Format("FileInfo: {0}", FileInformation.Id);
        }

        public void RefreshMetadata()
        {
            _engine.RefreshMetadata(FileInformation);
            RaisePropertyChanged(null);
        }
    }
}