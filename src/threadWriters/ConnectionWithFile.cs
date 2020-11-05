using HashFiles.src.options;
using System;

namespace HashFiles.src.threadWriters
{
    public class ConnectionWithFile : ConnectionWith
    {
        private OptionsForFile fileOpt;

        public ConnectionWithFile(OptionsForFile fileOpt)
        {
            this.fileOpt = fileOpt;
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void PrepareForWriting()
        {
            throw new NotImplementedException();
        }

        public override void SendHashData(HashFunctionResult res)
        {
            throw new NotImplementedException();
        }
    }
}
