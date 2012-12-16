namespace Funani.Gui.Controls
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;

	[ValueConversion(typeof(string), typeof(ImageSource))]
	public class HeaderToImageConverter : IValueConverter
	{
		public static HeaderToImageConverter Instance = new HeaderToImageConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((value as string).Contains(@"\"))
			{
				Uri uri = new Uri("pack://application:,,,/Images/diskdrive.png");
				BitmapImage source = new BitmapImage(uri);
				return source;
			}
			else
			{
				Uri uri = new Uri("pack://application:,,,/Images/folder.png");
				BitmapImage source = new BitmapImage(uri);
				return source;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}