
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catel.Logging;
using Funani.Thumbnailer;

namespace Funani.Gui.Converters
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class UriToThumbnailConverter : IValueConverter
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly BitmapSource DefaultThumbnail;

        static UriToThumbnailConverter()
        {
            var packUri = new Uri("pack://application:,,,/Images/funani.png");
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = packUri;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.DelayCreation;
            image.EndInit();

            if (image.CanFreeze)
                image.Freeze();

            DefaultThumbnail = image;
        }

        public UriToThumbnailConverter(int thumbnailSize)
        {
            ThumbnailSize = thumbnailSize;
        }

        public int ThumbnailSize { get; private set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DefaultThumbnail;
            try
            {
                throw new NotImplementedException();
                /*
                BitmapSource ret = Thumbnail.Extract(new Uri(value.ToString()),
                                                     "image/", ThumbnailSize);

                // TODO: why is this called twice?
                Trace.TraceInformation("Generating thumbnail for '{0}'", value);
                return ret;
                 */
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return DefaultThumbnail;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}