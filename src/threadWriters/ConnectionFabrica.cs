using HashFiles.src.options;
using System;

namespace HashFiles.src.threadWriters
{
    public class ConnectionFabrica
    {
        public ConnectionWith CreateConnection(Options options)
        {
            if (options is OptionsForSqlDb bdOpt)
            {
                return new ConnectionWithSqlDb(bdOpt);
            }
            if (options is OptionsForConsole conOpt)
            {
                return new ConnectionWithConsole(conOpt);
            }
            if (options is OptionsForFile fileOpt)
            {
                return new ConnectionWithFile(fileOpt);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
