namespace Funani.Core.Hash
{
    using System;
    using System.Globalization;
    using System.IO.Abstractions;
    using System.Security.Cryptography;
    using System.Text;

    public class Algorithms
    {
        private readonly IFileSystem filesystem;

        public Algorithms(IFileSystem fileSystem)
        {
            this.filesystem = fileSystem;
        }

        public Algorithms()
        {
            this.filesystem = new FileSystem();
        }

        public string ComputeSha1(IFileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using (var hasher = new SHA1CryptoServiceProvider())
            {
                return this.Execute(file, hasher);
            }
        }

        public string ComputeSha1(string path)
        {
            return this.ComputeSha1(this.filesystem.FileInfo.FromFileName(path));
        }

        public string ComputeSha256(IFileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using (var hasher = new SHA256Managed())
            {
                return this.Execute(file, hasher);
            }
        }

        public string ComputeSha256(string path)
        {
            return this.ComputeSha256(this.filesystem.FileInfo.FromFileName(path));
        }

        private string Execute(IFileInfo file, HashAlgorithm hashAlgorithm)
        {
            string hashCode;
            using (var fileStream = this.filesystem.FileStream.Create(file.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                hashAlgorithm.ComputeHash(fileStream);
                var buff = new StringBuilder();
                foreach (byte hashByte in hashAlgorithm.Hash)
                {
                    buff.Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", hashByte));
                }

                hashCode = buff.ToString();
            }

            return hashCode.ToLowerInvariant();
        }
    }
}
