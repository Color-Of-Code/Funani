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

namespace Funani.Gui.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Funani.Api.Metadata;

    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class UriToThumbnailConverter : IValueConverter
    {
        public UriToThumbnailConverter(int thumbnailSize)
        {
            ThumbnailSize = thumbnailSize;
        }

        static UriToThumbnailConverter()
        {
            var packUri = new Uri("pack://application:,,,/Images/funani.png");
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = packUri;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.None;
            image.EndInit();

            if (image.CanFreeze)
                image.Freeze();

            _defaultThumbnail = image;
        }

        private static readonly BitmapSource _defaultThumbnail;

        public int ThumbnailSize
        {
            get;
            private set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
        	if (value == null)
        		return _defaultThumbnail;
            try
            {
                BitmapSource ret = Funani.Thumbnailer.Thumbnail.Extract(new Uri(value.ToString()),
                    "image/", ThumbnailSize
                    );
            	System.Diagnostics.Trace.TraceInformation("Generating thumbnail for '{0}'", value);
                return ret;
            }
            catch
            {
                return _defaultThumbnail;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
