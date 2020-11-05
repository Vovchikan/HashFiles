using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashFiles
{
    public class HashFunctionResult
    {
        public string filePath;
        public string hashSum;
        public HashFunctionException error;

        public override string ToString()
        {
            return String.Format("файл: {0}, хэш-сумма:{1}, наличие ошибок:{2}", filePath, hashSum, error.Message);
        }

        public string ToStringShort()
        {
            string shortHash = GetShortHashSumOrEmptyString();
            FileInfo fi = new FileInfo(filePath);
            return $"{fi.Name} -> {shortHash}... , {error.Message}";
        }

        private string GetShortHashSumOrEmptyString()
        {
            int hashElementsCount = 6;
            string result = "";
            try
            {
                result = hashSum.Substring(0, hashElementsCount);
            }
            catch { }
            return result;
        }
    }
}
