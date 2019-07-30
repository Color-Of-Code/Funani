

using System;
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
            _engine = DependencyResolver.Resolve<IEngine>();
            RefreshMetadata = new Command(OnRefreshMetadataExecute);
            Save = new Command(OnSaveExecute);
            Delete = new Command(OnDeleteExecute);
            RotateLeft = new Command(OnRotateLeftExecute);
            RotateRight = new Command(OnRotateRightExecute);
        }

        #region Model: FileInformation

        /// <summary>
        ///     FileInformation model.
        /// </summary>
        [Model]
        public FileInformation FileInformation
        {
            get;
            private set;
        }

        #endregion

        #region Property: Rating

        /// <summary>
        /// Rating
        /// </summary>
        [ViewModelToModel("FileInformation")]
        public int? Rating
        {
            get;
            set;
        }

        #endregion

        #region Property: Angle
        /// <summary>
        /// Angle of rotation.
        /// </summary>
        [ViewModelToModel("FileInformation")]
        public int? Angle
        {
            get;
            set;
        }

        #endregion

        #region Property: IsDeleted
        /// <summary>
        /// Is deleted.
        /// </summary>
        [ViewModelToModel("FileInformation")]
        public bool IsDeleted
        {
            get;
            set;
        }

        #endregion

        #region Property: Title
        /// <summary>
        /// Title.
        /// </summary>
        [ViewModelToModel("FileInformation")]
        public new String Title
        {
            get;
            set;
        }

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