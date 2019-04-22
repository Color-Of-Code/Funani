
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catel.Data;
using Catel.MVVM;
using Funani.Gui.Converters;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     ThumbnailViewModel view model.
    /// </summary>
    public class ThumbnailViewModel : ViewModelBase
    {
        private const int MaxThumbnailSize = 256;
        private static readonly UriToThumbnailConverter Converter = new UriToThumbnailConverter(MaxThumbnailSize);

        private BitmapSource _thumbnail;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThumbnailViewModel" /> class.
        /// </summary>
        public ThumbnailViewModel(string fullName)
        {
            FullName = fullName;
        }

        /// <summary>
        ///     Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "ThumbnailViewModel"; }
        }

        #region Properties

        public String FullName { get; set; }

        public double ThumbnailWidth
        {
            get
            {
                if (MaxThumbnailSize > Thumbnail.PixelWidth)
                    return double.NaN;
                return Thumbnail.PixelWidth;
            }
        }

        public double ThumbnailHeight
        {
            get
            {
                if (MaxThumbnailSize > Thumbnail.PixelHeight)
                    return double.NaN;
                return Thumbnail.PixelHeight;
            }
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

        #endregion

        #region Commands

        #endregion
    }
}