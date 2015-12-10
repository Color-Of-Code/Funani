/*
 * Copyright (c) 2008-2016, Jaap de Haan <jaap.dehaan@color-of-code.de>
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

using System;
using System.Collections.Generic;
//using System.Windows.Media.Imaging;

namespace Funani.MetadataExtractor
{
    public static class Metadata
    {
        public static Dictionary<String, String> Extract(Uri uri, String mime)
        {
            var dictionary = new Dictionary<String, String>();
            if (mime.StartsWith("image/"))
            {
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