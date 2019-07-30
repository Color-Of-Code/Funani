using System;
using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace Funani.Core.Hash
{
    public class Algorithms
    {
        private readonly IFileSystem _fileSystem;

        public Algorithms(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        public Algorithms()
        {
            _fileSystem = new FileSystem();
        }

        public string ComputeSha1(IFileInfo file)
        {
            return Execute(file, new SHA1CryptoServiceProvider());
        }

        public string ComputeSha1(string path)
        {
            return ComputeSha1(_fileSystem.FileInfo.FromFileName(path));
        }

        public string ComputeSha256(IFileInfo file)
        {
            return Execute(file, new SHA256Managed());
        }

        public string ComputeSha256(string path)
        {
            return ComputeSha256(_fileSystem.FileInfo.FromFileName(path));
        }

        private String Execute(IFileInfo file, HashAlgorithm hashAlgorithm)
        {
            String hashCode;
            using (var fileStream = _fileSystem.FileStream.Create(file.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                hashAlgorithm.ComputeHash(fileStream);
                var buff = new StringBuilder();
                foreach (byte hashByte in hashAlgorithm.Hash)
                {
                    buff.Append(String.Format("{0:X2}", hashByte));
                }
                hashCode = buff.ToString();
            }
            return hashCode.ToLowerInvariant();
        }
    }
}
