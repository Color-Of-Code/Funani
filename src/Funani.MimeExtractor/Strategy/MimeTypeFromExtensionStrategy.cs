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
                case ".cab":
                    mime = "application/vnd.ms-cab-compressed";
                    break;
                case ".css":
                    mime = " text/css";
                    break;
                case ".doc":
                    mime = "application/msword";
                    break;
                case ".exe":
                    mime = "application/exe";
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
                case ".htm":
                case ".html":
                    mime = "text/html";
                    break;
                case ".xml":
                    mime = "text/xml";
                    break;
                case ".iso":
                    mime = "application/iso-image";
                    break;
                case ".jpe":
                case ".jpeg":
                case ".jpg":
                    mime = "image/jpeg";
                    break;
                case ".lnk":
                    mime = "application/x-ms-shortcut";
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
                case ".php":
                    mime = "text/x-php";
                    break;
                case ".odg":
                    mime = "application/vnd.oasis.opendocument.graphics";
                    break;
                case ".ods":
                    mime = "application/vnd.oasis.opendocument.spreadsheet";
                    break;
                case ".odt":
                    mime = "application/vnd.oasis.opendocument.text";
                    break;
                case ".png":
                    mime = "image/png";
                    break;
                case ".pps":
                case ".ppt":
                    mime = "application/vnd.ms-powerpoint";
                    break;
                case ".rtf":
                    mime = "application/rtf";
                    break;
                case ".tif":
                case ".tiff":
                    mime = "image/tiff";
                    break;
                case ".wps":
                    mime = "application/vnd.ms-works";
                    break;
                case ".xcf":
                    mime = "application/gimp";
                    break;
                case ".xls":
                    mime = "application/msexcel";
                    break;
                case ".gz":
                    mime = "application/gzip";
                    break;
                case ".zip":
                    mime = "application/zip";
                    break;
                case "":
                case ".db":
                case ".indexarrays":
                case ".indexgroups":
                case ".shadowindexgroups":
                case ".indexhead":
                case ".shadowindexhead":
                case ".indexids":
                case ".indexdirectory":
                case ".shadowindexdirectory":
                case ".indexpostings":
                case ".indexpositions":
                case ".indexpositiontable":
                case ".indexcompactdirectory":
                case ".plist": // MAC OS X trash folder
                case ".trashes": // MAC OS X trash folder
                case ".tag":
                case ".hdr":
                case ".dat":
                case ".bin":
                case ".dll":
                    mime = "application/octet-stream";
                    break;
                default:
                    mime = "application/octet-stream";
                    System.Diagnostics.Trace.TraceWarning("MIME type not recognized for file '{0}'", file.Name);
                    break;
            }
            return mime;
        }
    }
}
