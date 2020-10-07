using System;
using System.IO;
using System.Security.Cryptography;

namespace HashFiles
{
    static class HashFunction
    {
        public static HashFunctionResult ComputeMD5Checksum(string path)
        {
            var hashFuncResult = new HashFunctionResult() { filePath = path };
            hashFuncResult = TryCompute(hashFuncResult);
            return hashFuncResult;
        }

        private static HashFunctionResult TryCompute(HashFunctionResult hashFuncResult)
        {
            try
            {
                hashFuncResult.hashSum = HashFuncMD5(hashFuncResult.filePath);
                hashFuncResult.error = new HashFunctionException("Без ошибок");
            }
            catch (Exception e)
            {
                hashFuncResult.hashSum = String.Empty;
                hashFuncResult.error = new HashFunctionException("Сообщение ошибки: "+e.Message, e);
            }
            return hashFuncResult;
        }

        private static string HashFuncMD5(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] checkSum = md5.ComputeHash(fs);
                    string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                    return result;
                }
        }
    }
}
