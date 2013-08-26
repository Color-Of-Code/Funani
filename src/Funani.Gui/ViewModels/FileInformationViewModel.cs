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
using Catel.IoC;
using Catel.MVVM;

using Funani.Api;
using Funani.Api.Metadata;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     ViewModel of a file stored inside the database
    /// </summary>
    public class FileInformationViewModel : ViewModelBase
    {
        private readonly IEngine _engine;

        public FileInformationViewModel(FileInformation fileInformation)
        {
            FileInformation = fileInformation;
            _engine = ServiceLocator.ResolveType<IEngine>();
            RefreshMetadata = new Command(OnRefreshMetadataExecute);
            Save = new Command(OnSaveExecute);
            Delete = new Command(OnDeleteExecute);
            RotateLeft = new Command(OnRotateLeftExecute);
            RotateRight = new Command(OnRotateRightExecute);
        }

        #region Model: FileInformation

        /// <summary>
        ///     Register the FileInformation property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FileInformationProperty
            = RegisterProperty("FileInformation", typeof(FileInformation));

        /// <summary>
        ///     FileInformation model.
        /// </summary>
        [Model]
        public FileInformation FileInformation
        {
            get { return GetValue<FileInformation>(FileInformationProperty); }
            private set { SetValue(FileInformationProperty, value); }
        }

        #endregion

        #region Property: Rating

        /// <summary>
        /// Rating
        /// </summary>
        [ViewModelToModel("FileInformation")]
        public int? Rating
        {
            get { return GetValue<int?>(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        /// <summary>
        /// Register the Rating property so it is known in the class.
        /// </summary>
        public static readonly PropertyData RatingProperty = RegisterProperty("Rating", typeof(int?));

        #endregion

        #region Property: Angle
        /// <summary>
        /// Angle of rotation.
        /// </summary>
        [ViewModelToModel("FileInformation")]
        public int? Angle
        {
            get { return GetValue<int?>(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// Register the Angle property so it is known in the class.
        /// </summary>
        public static readonly PropertyData AngleProperty = RegisterProperty("Angle", typeof(int?));
        #endregion

        #region Property: IsDeleted
        /// <summary>
        /// Is deleted.
        /// </summary>
        [ViewModelToModel("FileInformation")]
        public bool IsDeleted
        {
            get { return GetValue<bool>(IsDeletedProperty); }
            set { SetValue(IsDeletedProperty, value); }
        }

        /// <summary>
        /// Register the IsDeleted property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IsDeletedProperty = RegisterProperty("IsDeleted", typeof(bool));
        #endregion

        #region Property: Title
        /// <summary>
        /// Title.
        /// </summary>
        [ViewModelToModel("FileInformation")]
        public new String Title
        {
            get { return GetValue<String>(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Register the Title property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TitleProperty = RegisterProperty("Title", typeof(String));

        #endregion

        public Stretch Stretch
        {
            get { return Stretch.Uniform; }
        }

        public String ThumbnailPath
        {
            get
            {
                FileInfo thumbPath = _engine.GetThumbnail(FileInformation.Id, FileInformation.MimeType);
                return thumbPath == null ? null : thumbPath.FullName;
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

        #region Command: RefreshMetadata

        /// <summary>
        ///     Gets the RefreshMetadata command.
        /// </summary>
        public Command RefreshMetadata { get; private set; }

        /// <summary>
        ///     Refresh Metadata for this entry in the database
        /// </summary>
        private void OnRefreshMetadataExecute()
        {
            FileInformation.RefreshMetadata();
        }

        #endregion

        #region Command: Save
        /// <summary>
        /// Gets the Save command.
        /// </summary>
        public new Command Save { get; private set; }

        /// <summary>
        /// Method to invoke when the Save command is executed.
        /// </summary>
        private void OnSaveExecute()
        {
            FileInformation.Save();
        }
        #endregion

        #region Command: Delete
        /// <summary>
        /// Gets the Delete command.
        /// </summary>
        public Command Delete { get; private set; }

        /// <summary>
        /// Method to invoke when the Delete command is executed.
        /// </summary>
        private void OnDeleteExecute()
        {
            IsDeleted = true;
        }
        #endregion

        #region Command: RotateLeft
        /// <summary>
        /// Gets the RotateLeft command.
        /// </summary>
        public Command RotateLeft { get; private set; }

        /// <summary>
        /// Method to invoke when the RotateLeft command is executed.
        /// </summary>
        private void OnRotateLeftExecute()
        {
            int angle = Angle ?? 0;
            Angle = angle - 90;
        }
        #endregion

        #region Command: RotateRight
        /// <summary>
        /// Gets the RotateRight command.
        /// </summary>
        public Command RotateRight { get; private set; }

        /// <summary>
        /// Method to invoke when the RotateRight command is executed.
        /// </summary>
        private void OnRotateRightExecute()
        {
            int angle = Angle ?? 0;
            Angle = angle + 90;
        }
        #endregion
    }
}