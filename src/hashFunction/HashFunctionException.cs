using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
