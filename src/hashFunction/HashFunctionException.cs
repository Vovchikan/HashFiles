using System;

namespace HashFiles
{
    public class HashFunctionException : Exception
    {
        public String ErrorMessage { get; }
        public HashFunctionException(String message, Exception exc=null)
            : base(message, exc)
        {
            ErrorMessage = message;
        }
    }
}
