namespace HashFiles.src.threadWriters
{
    public abstract class ConnectionWith
    {
        public abstract void SendHashData(HashFunctionResult res);
        public abstract void Open();
        public abstract void Close();
    }
}
