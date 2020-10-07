using System;
using System.Collections.Generic;
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
    }
}
