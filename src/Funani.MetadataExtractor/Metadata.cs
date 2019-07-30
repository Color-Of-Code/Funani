
using System;
using System.Collections.Generic;
using MetadataExtractor;

namespace Funani.MetadataExtractor
{
    public static class Metadata
    {
        public static Dictionary<String, String> Extract(Uri uri, String mime)
        {
            var dictionary = new Dictionary<String, String>();
            if (mime.StartsWith("image/"))
            {
                var directories = ImageMetadataReader.ReadMetadata(uri.AbsolutePath);
                foreach (var directory in directories)
                    foreach (var tag in directory.Tags)
                        Console.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");
                throw new NotImplementedException();

                /* TODO: implement with an exif lib
                BitmapFrame frame = BitmapFrame.Create(uri, BitmapCreateOptions.None, BitmapCacheOption.None);
                dictionary.Add("Width", Convert.ToString(frame.PixelWidth));
                dictionary.Add("Height", Convert.ToString(frame.PixelHeight));
                var meta = frame.Metadata as BitmapMetadata;
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

                    object orientation = meta.GetQuery("/app1/ifd/{ushort=274}");
                    if (orientation != null)
                    {
                        String orientationName = orientation.ToString();
                        switch (orientationName)
                        {
                            case "3":
                                dictionary.Add("Angle", "180");
                                break;
                            case "6":
                                dictionary.Add("Angle", "90");
                                break;
                            case "8":
                                dictionary.Add("Angle", "-90");
                                break;
                        }
                    }
                }
                */
            }
            return dictionary;
        }
    }
}