using HashFiles.src.options;
using System;

namespace HashFiles.src.threadWriters
{
    public class ConnectionWithConsole : ConnectionWith
    {
        private OptionsForConsole conOpt;

        public ConnectionWithConsole(OptionsForConsole conOpt)
        {
            this.conOpt = conOpt;
        }

        public override void Close()
        {
            // Nothing to close
        }

        public override void PrepareForWriting()
        {
            // No preparations
        }

        public override void SendHashData(HashFunctionResult res)
        {
            Console.WriteLine(res);
            // todo не работает консольный коннектор
        }
    }
}
