
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Funani.Api.Utils
{
    public class ComputeHash
    {
        public static String SHA1(FileInfo file)
        {
            return Execute(file, new SHA1CryptoServiceProvider());
        }

        public static String SHA256(FileInfo file)
        {
            return Execute(file, new SHA256Managed());
        }

        public static String MD5(FileInfo file)
        {
            return Execute(file, new MD5CryptoServiceProvider());
        }

        private static String Execute(FileInfo file, HashAlgorithm hashAlgorithm)
        {
            String hashCode;
            using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                hashAlgorithm.ComputeHash(fileStream);
                var buff = new StringBuilder();
                foreach (byte hashByte in hashAlgorithm.Hash)
                {
                    buff.Append(String.Format("{0:X2}", hashByte));
                }
                hashCode = buff.ToString();
            }
            return hashCode;
        }
    }
}