/*
 * Copyright (c) 2012-2013, Jaap de Haan <jaap.dehaan@color-of-code.de>
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
namespace Funani.MimeExtractor.Strategy
{
    using System;
    using System.IO;

    internal class MimeTypeFromExtensionStrategy : IMimeTypeExtractor
    {
        public string ExtractMimeType(FileInfo file)
        {
        	if (file.Name.ToLowerInvariant() == "thumbs.db")
        		return "application/x-msthumbnails";
        	
            String mime;
            String extension = file.Extension.ToLowerInvariant();
            switch (extension)
            {
                case ".avd":
                    mime = "application/x-magixmoviedata";
                    break;
                case ".hdp":
                    mime = "application/x-magixaudiodata";
                    break;
                case ".mxc2":
                    mime = "application/x-magixmediapreview";
                    break;
                    
                case ".avi":
                    mime = "video/x-msvideo";
                    break;
                case ".bmp":
                    mime = "image/bmp";
                    break;
                case ".doc":
                    mime = "application/msword";
                    break;
                case ".gif":
                    mime = "image/gif";
                    break;
                case ".hps-metadata":
                    mime = "application/xml";
                    break;
                case ".ini":
                case ".txt":
                    mime = "text/plain";
                    break;
                case ".jpe":
                case ".jpeg":
                case ".jpg":
                    mime = "image/jpeg";
                    break;
                case ".mov":
                case ".qt":
                    mime = "video/quicktime";
                    break;
                case ".mpe":
                case ".mpeg":
                case ".mpg":
                    mime = "video/mpeg";
                    break;
                case ".pdf":
                    mime = "application/pdf";
                    break;
                case ".png":
                    mime = "image/png";
                    break;
                case ".tif":
                case ".tiff":
                    mime = "image/tiff";
                    break;
                case ".xcf":
                    mime = "application/gimp";
                    break;
                case ".xls":
                    mime = "application/msexcel";
                    break;
                case ".zip":
                    mime = "application/zip";
                    break;
                default:
                    throw new NotSupportedException("MIME type not recognized for file " + file.Name);
            }
            return mime;
        }
    }
}
