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
            BitmapFrame frame = BitmapFrame.Create(uri, BitmapCreateOptions.None, BitmapCacheOption.None);
            BitmapMetadata meta = frame.Metadata as BitmapMetadata;
            if (meta == null)
                return null;
            var dictionary = new Dictionary<String, String>();
            dictionary.Add("DateTaken", meta.DateTaken);
            dictionary.Add("ApplicationName", meta.ApplicationName);
            dictionary.Add("Device", meta.CameraModel);
            return dictionary;
        }
    }
}
