using System;
using System.IO;
using System.Security.Cryptography;

namespace HashFiles
{
    static class HashFunc
    {
        public static string ComputeMD5Checksum(string path)
        {
            try
            {
                return ComputeMD5Checksum2(path);
            }
            catch (Exception e)
            {
                throw new HashFunc.HashFuncException(e.Message);
            }
        }

        private static string ComputeMD5Checksum1(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                return result;
            }
        }

        /// <summary>
        /// Хэш-сумма для больших файлов
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ComputeMD5Checksum2(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] checkSum = md5.ComputeHash(fs);
                    string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                    return result;
                }
        }

        public class HashFuncException : Exception
        {
            public HashFuncException(string message)
                : base(message)
            { }
        }
    }
}
