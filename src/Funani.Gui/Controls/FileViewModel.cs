namespace Funani.Gui.Controls
{
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// FileViewModel
    /// </summary>
    public class FileViewModel
    {
        public FileViewModel(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        public FileInfo FileInfo
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

        public BitmapSource Thumbnail
        {
            get 
            {
                return converter.Convert(FullName, typeof(BitmapSource), null, null) as BitmapSource;
            }
        }

        private static readonly UriToThumbnailConverter converter = new UriToThumbnailConverter();
    }
}
