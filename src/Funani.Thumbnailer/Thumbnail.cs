/*
 * Copyright (c) 2008-2013, Jaap de Haan <jaap.dehaan@color-of-code.de>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of the "Color-Of-Code" nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;

namespace Funani.Thumbnailer
{
    public static class Thumbnail
    {
        public static void Create(Uri uri, String mime, int thumbnailSize, FileInfo destination)
        {
            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new PngFormat { };
            Size size = new Size(thumbnailSize, thumbnailSize);
            using (var inStream = File.OpenRead(uri.LocalPath))
            {
                using (var outStream = File.Create(destination.FullName))
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData:true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                                    .Resize(size)
                                    .Format(format)
                                    .Save(outStream);
                    }
                    // Do something with the stream.
                }
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