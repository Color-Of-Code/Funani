namespace Funani.Gui.Controls
{
	using System;
	using System.Windows.Data;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;

	[ValueConversion(typeof(string), typeof(ImageSource))]
	public class UriToThumbnailConverter : IValueConverter
	{
		public UriToThumbnailConverter()
		{
			ThumbnailWidth = 120;
		}
		
		public int ThumbnailWidth
		{
			get;set;
		}
		
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				BitmapImage bi = new BitmapImage();
				bi.BeginInit();
				bi.DecodePixelWidth = ThumbnailWidth;
				bi.CreateOptions = BitmapCreateOptions.DelayCreation;
				bi.CacheOption = BitmapCacheOption.OnLoad;
				bi.UriSource = new Uri(value.ToString());
				bi.EndInit();
				return bi;
			}
			catch
			{
				return null;
			}
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
