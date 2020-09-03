namespace Funani.Thumbnailer
{
    using System;
    using System.Drawing;
    using System.IO.Abstractions;
    using System.Net.Sockets;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;

    public static class Thumbnail
    {
        public static void Create(Uri uri, string mime, int thumbnailSize, IFileInfo destination)
        {
            // Format is automatically detected though can be changed.
            var size = new SixLabors.ImageSharp.Size(thumbnailSize, thumbnailSize);
            using (var image = Image.Load(uri.LocalPath))
            {
                image.Mutate(x => x.Resize(size.Width, size.Height));

                // Automatic encoder selected based on extension.
                image.Save(destination.FullName);
            }
        }

        /*
        /// <summary>
        ///     Create a thumbnail of the given max size for the resource specified by the uri
        ///     assuming the mime type is correctly specified too.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="mime"></param>
        /// <param name="thumbnailSize"></param>
        /// <returns></returns>
        public static BitmapSource Extract(Uri uri, String mime, int thumbnailSize)
        {
            return Extract(uri, mime, thumbnailSize, BitmapCreateOptions.DelayCreation);
        }

        public static BitmapSource Extract(Uri uri, String mime, int thumbnailSize,
                                           BitmapCreateOptions bitmapCreateOptions)
        {
            if (!mime.StartsWith("image/"))
                return null;

            // TODO: write thumbnailers depending on the mime type
            // this works for WPF supported image formats only
            var orientation = Orientation.Normal;
            BitmapFrame frame = BitmapFrame.Create(
                uri, bitmapCreateOptions, BitmapCacheOption.None);
            BitmapSource ret = null;
            var meta = frame.Metadata as BitmapMetadata;
            if (frame.PixelHeight < thumbnailSize && frame.PixelWidth < thumbnailSize)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = uri;
                image.CacheOption = BitmapCacheOption.None;
                image.CreateOptions = bitmapCreateOptions;
                image.EndInit();

                if (image.CanFreeze)
                    image.Freeze();

                ret = image;
            }
            else
            {
                if (frame.Thumbnail == null)
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = uri;
                    if (frame.PixelHeight >= frame.PixelWidth)
                        image.DecodePixelHeight = thumbnailSize;
                    else
                        image.DecodePixelWidth = thumbnailSize;
                    image.CacheOption = BitmapCacheOption.None;
                    image.CreateOptions = bitmapCreateOptions;
                    image.EndInit();

                    if (image.CanFreeze)
                        image.Freeze();

                    ret = image;
                }
                else
                {
                    ret = frame.Thumbnail;
                }
            }

            if ((meta != null) && (ret != null))
            {
                double angle = 0;
                if (meta.GetQuery("/app1/ifd/{ushort=274}") != null)
                {
                    orientation = (Orientation)Enum.Parse(typeof(Orientation),
                                                          meta.GetQuery("/app1/ifd/{ushort=274}").ToString());
                }

                switch (orientation)
                {
                    case Orientation.Rotate90:
                        angle = -90;
                        break;
                    case Orientation.Rotate180:
                        angle = 180;
                        break;
                    case Orientation.Rotate270:
                        angle = 90;
                        break;
                }

                if (angle != 0)
                {
                    ret = new TransformedBitmap(ret.Clone(), new RotateTransform(angle));
                    ret.Freeze();
                }
            }

            return ret;
        }
         */
    }
}