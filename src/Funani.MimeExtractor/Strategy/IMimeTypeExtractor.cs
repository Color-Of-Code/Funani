
namespace Funani.MimeExtractor.Strategy
{
    using System;
    using System.IO.Abstractions;

    public interface IMimeTypeExtractor
    {
        String ExtractMimeType(IFileInfo file);
    }
}
