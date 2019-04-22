
namespace Funani.MimeExtractor.Strategy
{
    using System;
    using System.IO;

    public interface IMimeTypeExtractor
    {
        String ExtractMimeType(FileInfo file);
    }
}
