
namespace HashFiles.src.threadWriters
{
    public abstract class ConnectionWith
    {
        public abstract void SendHashData(HashFunctionResult res);
        public abstract void Close();
        public abstract void PrepareForWriting();

    }
}
