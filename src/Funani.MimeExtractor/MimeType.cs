

using System;
using System.IO.Abstractions;
using Funani.MimeExtractor.Strategy;

namespace Funani.MimeExtractor
{
    public static class MimeType
    {
        private static readonly IMimeTypeExtractor FromExtension = new MimeTypeFromExtensionStrategy();
        private static readonly IMimeTypeExtractor FromData = new MimeTypeFromDataStrategy();

        /// <summary>
        ///     Extract the Mime type from the file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static String Extract(IFileInfo file)
        {
            String mime;

            // 1) analyze data
            mime = FromData.ExtractMimeType(file);
            
            // 2) fallback and use extension
            if (mime == null)
                mime = FromExtension.ExtractMimeType(file);
            
            return mime;
        }
    }
}