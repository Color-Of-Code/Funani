namespace Funani.MetadataExtractor
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public static class Metadata
    {
        public static Dictionary<String, String> Extract(Uri uri, String mime)
        {
            var dictionary = new Dictionary<String, String>();
            if (mime.StartsWith("image/"))
            {
	            BitmapFrame frame = BitmapFrame.Create(uri, BitmapCreateOptions.None, BitmapCacheOption.None);
	            dictionary.Add("Width", Convert.ToString(frame.PixelWidth));
	            dictionary.Add("Height", Convert.ToString(frame.PixelHeight));
	            BitmapMetadata meta = frame.Metadata as BitmapMetadata;
	            if (meta != null && meta.Format != "png" && meta.Format != "gif")
	            {
	            	if (!String.IsNullOrWhiteSpace(meta.DateTaken))
			            dictionary.Add("DateTaken", meta.DateTaken);
		            dictionary.Add("ApplicationName", meta.ApplicationName);
		            if (meta.CameraModel != null)
		            {
			            if (meta.CameraModel.Contains(meta.CameraManufacturer))
				            dictionary.Add("Device", meta.CameraModel);
			            else
			            	dictionary.Add("Device", String.Format("{0} {1}", meta.CameraManufacturer, meta.CameraModel));
		            }
	            }
            }
            return dictionary;
        }
    }
}
