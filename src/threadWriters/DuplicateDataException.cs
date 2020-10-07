using System;

namespace HashFiles.src.threadWriters
{
    public class DuplicateDataException : Exception
    {
        public DuplicateDataException(string message) : base(message) { }

        public DuplicateDataException(HashFunctionResult result)
            :base ($"Found duplicate: {result}")
        {
        }
    }
}
